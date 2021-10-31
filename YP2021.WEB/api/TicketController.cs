using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Nero2021.BLL.Models;
using Nero2021.BLL.Repository;
using Nero2021.BLL.Utilities;
using Nero2021.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Web;
using System.Web.Http;

namespace Nero2021.Web.api
{
    //[Authorize]
    public class TicketController : ApiController
    {
        [HttpPost]
        public IHttpActionResult ListDT(TicketListDTO model)
        {
            try
            {
                return Ok(new TicketRepo().ListDT(model));
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult FillAllCmb()
        {
            try
            {
                var tickettypes = new TicketTypeRepo().GetAll().Select(s => new { s.ID, value = s.Name, label = s.Name }).OrderBy(o => o.ID).AsQueryable();
                var ticketstatus = new TicketStatusRepo().GetAll().Select(s => new { s.ID, s.Name }).OrderBy(o => o.ID).AsQueryable();
                var ticketpriorities = new TicketPriorityRepo().GetAll().Select(s => new { s.ID, s.Name }).OrderBy(o => o.ID).AsQueryable();
                var members = new MemberRepo().GetAll(q => (q.IsDeleted ?? false) == false).Select(s => new { s.ID, s.FirstName, s.LastName, s.IsDeleted }).AsQueryable();
                var firmalar = new MusterilerRepo().GetAll(q => (q.IsDeleted ?? false) == false).Select(s => new { s.ID, s.FIRMA_ADI, s.FIRMA_TIPI, s.IsDeleted }).OrderBy(o => o.FIRMA_ADI).AsQueryable();

                return Ok(new
                {
                    TicketTypes = tickettypes,
                    TicketStatus = ticketstatus,
                    TicketPriorities = ticketpriorities,
                    Members = members,
                    Firmalar = firmalar
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }


        [HttpPost]
        public IHttpActionResult Detail(TicketDetailDTO model)
        {
            try
            {
                string userID = HttpContext.Current.User.Identity.GetUserId();

                var ticketmember = new TicketMemberRepo().GetByFilter(q => q.MemberID == userID && (q.IsDeleted ?? false) == false);
                if (ticketmember == null)
                    return BadRequest("Geçersiz/Yetkisiz işlem!");

                var ticket = new TicketRepo().FindByID(model.ID ?? 0);

                return Ok(new
                {
                    Ticket = ticket
                });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Save(SaveTicketDTO model)
        {
            string returnmessage = String.Empty;

            #region check params
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            if (model.SelectedFirm == null)
                return BadRequest("Müşteri bilgisi eksik!");

            if (model.SelectedFirm.ID == null)
                return BadRequest("Müşteri bilgisi eksik!");

            if (model.SelectedMembers == null)
                return BadRequest("Kullanıcı bilgisi eksik!");

            if (model.SelectedMembers.Count() == 0)
                return BadRequest("Kullanıcı bilgisi eksik!");
            #endregion


            try
            {
                string userID = HttpContext.Current.User.Identity.GetUserId();

                if (model.ID == null)
                {
                    returnmessage = "Ticket oluşturuldu!";

                    var newticket = new Ticket()
                    {
                        CreatedBy = userID,
                        CreatedOn = DateTime.Now,
                        DetailText = model.DetailText,
                        IsDeleted = false,
                        MusteriID = model.SelectedFirm.ID,
                        PriorityID = model.PriorityID,
                        StatusID = model.StatusID,
                        TypeID = model.TypeID,
                        Title = model.Title
                    };

                    new TicketRepo().Insert(newticket);
                    model.ID = newticket.ID;

                    #region for creator
                    new TicketMemberRepo().Insert(new TicketMember()
                    {
                        CreatedBy = userID,
                        CreatedOn = DateTime.Now,
                        IsArchived = false,
                        IsAssignedTo = false,
                        IsDeleted = false,
                        IsEnrollment = true,
                        IsRead = true,
                        MemberID = userID,
                        TicketID = model.ID,
                        UnreadCount = 0,
                        Direction = 1
                    });
                    #endregion

                    foreach (var item in model.SelectedMembers)
                    {
                        if (item.ID != userID) // ticket açan hariç. kayda alınmaz.
                        {
                            new TicketMemberRepo().Insert(new TicketMember()
                            {
                                CreatedBy = userID,
                                CreatedOn = DateTime.Now,
                                IsArchived = false,
                                IsAssignedTo = true,
                                IsDeleted = false,
                                IsEnrollment = false,
                                IsRead = false,
                                MemberID = item.ID,
                                TicketID = model.ID,
                                UnreadCount = 1,
                                Direction = 2
                            });
                        }

                    }
                }
                else
                {
                    returnmessage = "Ticket güncellendi!";

                    var ticket = new TicketRepo().GetByFilter(q => q.ID == (model.ID ?? 0) && (q.IsDeleted ?? false) == false);
                    if (ticket == null)
                        return BadRequest("Ticket bulunamadı!");

                    ticket.UpdatedBy = userID;
                    ticket.UpdatedOn = DateTime.Now;
                    ticket.DetailText = model.DetailText;
                    ticket.MusteriID = model.SelectedFirm.ID;
                    ticket.PriorityID = model.PriorityID;
                    ticket.StatusID = model.StatusID;
                    ticket.TypeID = model.TypeID;
                    ticket.Title = model.Title;
                    new TicketRepo().Update();

                    foreach (var item in model.SelectedMembers)
                    {
                        var ticketmember = new TicketMemberRepo().GetByFilter(q => q.IsAssignedTo == true && q.TicketID == model.ID && q.MemberID == item.ID && (q.IsDeleted ?? false) == false);
                        if (ticketmember != null)
                        {
                            ticketmember.IsArchived = false;
                            ticketmember.IsRead = false;
                            ticketmember.UnreadCount += 1;
                            new TicketMemberRepo().Update();
                        }
                    }

                    var ticketmembers = new TicketMemberRepo().GetAll(q => q.TicketID == model.ID);

                    if (model.SelectedMembers != null)
                    {
                        foreach (var item in model.SelectedMembers)
                        {
                            if (!ticketmembers.Any(q => q.MemberID == item.ID))
                            {
                                new TicketMemberRepo().Insert(new TicketMember()
                                {
                                    CreatedBy = userID,
                                    CreatedOn = DateTime.Now,
                                    IsArchived = false,
                                    IsAssignedTo = true,
                                    IsDeleted = false,
                                    IsEnrollment = false,
                                    IsRead = false,
                                    MemberID = item.ID,
                                    TicketID = model.ID,
                                    UnreadCount = 0
                                });
                            }
                        }
                    }

                    if (ticketmembers != null)
                    {
                        foreach (var item in ticketmembers)
                        {
                            if (!model.SelectedMembers.Any(q => q.ID == item.MemberID))
                            {
                                var deleted = new TicketMemberRepo().GetByID(item.ID);
                                if (deleted != null)
                                {
                                    new TicketMemberRepo().Delete(deleted);
                                }
                            }
                        }
                    }

                }

                return Ok(new { ID = model.ID, Message = returnmessage });
            }
            catch (Exception ex)
            {
                //TODO: loga ekle
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }

        }

        [HttpPost]
        public IHttpActionResult SaveReply(TicketSaveReplyDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            if (model.TicketID == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            try
            {
                string userID = HttpContext.Current.User.Identity.GetUserId();

                var ticketmember = new TicketMemberRepo().GetByFilter(q => q.MemberID == userID && (q.IsDeleted ?? false) == false);
                if (ticketmember == null)
                    return BadRequest("Bu ticket'a ekli olmadığınız için cevap gönderemezsiniz!");

                Guid guid = Guid.NewGuid();
                string filename = model.FileName;
                string filepath = String.Empty;

                if (!String.IsNullOrWhiteSpace(model.DocumentData))
                {
                    string folder = "/upload/ticketfiles/" + guid.ToString(); // model.ItemType == 1 ? "/upload/pimages/" : "/upload/trimages/";
                    string filemappath = Path.Combine(HttpContext.Current.Server.MapPath(folder), filename);
                    filepath = folder + "/" + filename; // Path.Combine(folder, filename);

                    #region save document 

                    if (!Directory.Exists(HttpContext.Current.Server.MapPath(folder)))
                        Directory.CreateDirectory(HttpContext.Current.Server.MapPath(folder));

                    using (FileStream fs = new FileStream(filemappath, FileMode.Create))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            byte[] data = Convert.FromBase64String(model.DocumentData);
                            bw.Write(data);
                            bw.Close();
                        }
                    }
                    #endregion

                    if (!System.IO.File.Exists(filemappath))
                        return BadRequest("Dosya ekleme işleminde bir hata oluştu.");
                }

                var ticketreply = new TicketReply()
                {
                    CreatedOn = DateTime.Now,
                    FileName = model.FileName,
                    FilePath = filepath,
                    FileType = model.DocumentType,
                    MemberID = userID,
                    Message = model.Message,
                    TicketID = model.TicketID,
                };
                new TicketReplyRepo().Insert(ticketreply);

                var ticketmemberlist = new TicketMemberRepo().GetAll(q => q.TicketID == model.TicketID);
                foreach (var item in ticketmemberlist)
                {
                    item.IsRead = false;
                    item.UnreadCount = item.UnreadCount + 1;
                }
                new TicketMemberRepo().Update();


                foreach (var item in ticketmemberlist)
                {
                    //mail atılacak.
                    Utility.SendMail(item.Member.Email, "", "Ticket: Mesajınız var.", item.Ticket.Title + "konulu ticketa ait bir mesajınız var. Gönderen: " + item.Member.FirstName + " " + item.Member.LastName + ". Kontrol ediniz!");

                    // notification tablosu oluştur ve ekle.
                    new NotificationRepo().Insert(new Notification()
                    {
                        ID = Guid.NewGuid(),
                        CreatedOn = DateTime.Now,
                        IsDeleted = false,
                        IsRead = false,
                        Message = "Ticket mesajınız var.",
                        LinkUrl = "/ticket/detail/" + item.TicketID.ToString()
                    });

                }


                var ticketReplies = new TicketReplyRepo().GetAll(q => q.TicketID == model.TicketID).Select(s => new { s.Message, s.FilePath, s.FileType, s.FileName, FullName = (s.Member == null ? "" : s.Member.FirstName + " " + s.Member.LastName), s.CreatedOn });

                return Ok(new { ID = ticketreply.ID, Message = "Cevabınız gönderildi.", TicketReplies = ticketReplies });
            }
            catch (Exception ex)
            {
                //TODO: loga ekle
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }


        [HttpPost]
        public IHttpActionResult Delete(MProductDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Müşteri Ürün silindi.";

                var mproduct = new MProductsRepo().GetByID(model.ID ?? 0);
                if (mproduct == null)
                    return BadRequest("Silme işlemi yapılırken kayıt bilgilerine ulaşılamadı. Lütfen tekrar deneyiniz.");

                //mproduct.IsDeleted = true;
                //mproduct.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                //mproduct.DeletedOn = DateTime.Now;
                new MProductsRepo().Delete(mproduct);

                new MProductsLogRepo().Insert(new MProductsLog()
                {
                    ADDED = mproduct.ADDED,
                    BUKOD = mproduct.BUKOD,
                    CreatedBy = mproduct.CreatedBy,
                    CreatedOn = mproduct.CreatedOn,
                    CURRENCY = mproduct.CURRENCY,
                    ID = Guid.NewGuid(),
                    IsDeleted = mproduct.IsDeleted,
                    MPID = mproduct.MPID,
                    MusteriID = mproduct.MusteriID,
                    NAME = mproduct.NAME,
                    NAME_DE = mproduct.NAME_DE,
                    NAME_EN = mproduct.NAME_EN,
                    OEM = mproduct.OEM,
                    LogType = "D",
                    ProductID = mproduct.ProductID,
                    PRICE = mproduct.PRICE,
                    XPSNO = mproduct.XPSNO,
                    XPSUP = mproduct.XPSUP
                });

                return Ok(new { ID = model.ID, Message = successMessage });
            }
            catch (Exception ex)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }


        [HttpPost]
        public IHttpActionResult SendToArchive(SendTicketMemberToArchiveDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            if (model.TicketID == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            try
            {
                string userID = HttpContext.Current.User.Identity.GetUserId();

                var ticketmember = new TicketMemberRepo().GetByFilter(q => q.TicketID == model.TicketID && q.MemberID == userID);

                if (ticketmember == null)
                    return BadRequest("Kayda ulaşılamadı. Listenizi yenileyerek devam edebilirsiniz!");

                if (ticketmember.IsDeleted == true)
                    return BadRequest("Kayda ulaşılamadı. Listenizi yenileyerek devam edebilirsiniz!");

                if (ticketmember.IsArchived == true)
                    return BadRequest("Ticket zaten arşivde bulunmaktadır. Listenizi yenileyerek devam edebilirsiniz!");

                ticketmember.IsArchived = true;
                ticketmember.LastArchivedOn = DateTime.Now;
                new TicketMemberRepo().Update();

                return Ok(new { Message = "Ticket arşive gönderildi!" });
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public IHttpActionResult TakeBackTicketMemberFromArchive(TakeBackTicketMemberFromArchiveDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            if (model.TicketID == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            try
            {
                string userID = HttpContext.Current.User.Identity.GetUserId();

                var ticketmember = new TicketMemberRepo().GetByFilter(q => q.TicketID == model.TicketID && q.MemberID == userID);

                if (ticketmember == null)
                    return BadRequest("Kayda ulaşılamadı. Listenizi yenileyerek devam edebilirsiniz!");

                if (ticketmember.IsDeleted == true)
                    return BadRequest("Kayda ulaşılamadı. Listenizi yenileyerek devam edebilirsiniz!");

                if (ticketmember.IsArchived == false)
                    return BadRequest("Ticket zaten kullanımda. Listenizi yenileyerek devam edebilirsiniz!");

                ticketmember.IsArchived = false;
                ticketmember.LastArchivedOn = null;
                new TicketMemberRepo().Update();

                return Ok(new { Message = "Ticket arşivden çıkartıldı." });
            }
            catch (Exception)
            {

                throw;
            }
        }




    }
}