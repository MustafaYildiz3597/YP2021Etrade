using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Nero2021.BLL.Models;
using Nero2021.BLL.Repository;
using Nero2021.BLL.Utilities;
using Nero2021.Data;
//using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Text;
using System.Web;
using System.Web.Http;
using Nero2021;
//using static TurkBelge.Controllers.PersonelController;

namespace Nero.Web.api
{
    //[Authorize]
    public class MeetingController : ApiController
    {
        [HttpPost]
        public IHttpActionResult DTList(MeetingDTListDTO model)
        {
            try
            {
                return Ok(new ToplantilarRepo().ListDT(model));
            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
            }
        }

        [HttpPost]
        public IHttpActionResult FillAllCmb()
        {
            try
            {
                var firmalar = new MusterilerRepo().GetAll()  //&& ((q.FIRMA_TIPI == (int)FirmaTipleri.Müşteri) || (q.FIRMA_TIPI == (int)FirmaTipleri.MüşteriVeTedarikçi)) 
                 .Select(s => new { s.ID, s.FIRMA_ADI, s.FIRMA_TIPI, s.IsDeleted })
                 .OrderBy(o => o.FIRMA_ADI).AsQueryable();
                //var currencies = new CurrencyRepo().GetAll().Select(s => new { s.ID, s.Code }).OrderBy(o => o.ID).AsQueryable();
                var yetkilikisiler = new YetkiliKisilerRepo().GetAll().Select(s => new { s.ID, s.ADI, s.SOYADI, s.FIRMA_ID, s.IsDeleted }).AsQueryable();
                var members = new MemberRepo().GetAll().Select(s => new { s.ID, s.FirstName, s.LastName, s.IsDeleted }).AsQueryable();
                var toplantisebepler = new ToplantiSebepRepo().GetAll().Select(s => new { s.ID, s.Title }).AsQueryable();

                return Ok(new
                {
                    Firmalar = firmalar,
                    //Currencies = currencies,
                    YetkiliKisiler = yetkilikisiler,
                    Members = members,
                    ToplantiSebepler = toplantisebepler
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }


        [HttpPost]
        public IHttpActionResult Detail(MeetingIDDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                //    return Content(HttpStatusCode.Unauthorized, "My error message");

                var detail = new ToplantilarRepo().FindByID(model.ID ?? 0);

                return Ok(new
                {
                    ToplantiDetail = detail,
                });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(MeetingIDDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string userID = HttpContext.Current.User.Identity.GetUserId();

                var item = new ToplantilarRepo().GetByID(model.ID ?? 0);
                if (item == null)
                    return BadRequest("Toplantı kaydına ulaşılamadı.");

                item.IsDeleted = true;
                item.DeletedBy = userID;
                item.DeletedOn = DateTime.Now;

                new ToplantilarRepo().Update();

                return Ok("Toplantı silindi");

            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Save(MeetingSaveDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            string userID = HttpContext.Current.User.Identity.GetUserId();

            string successMessage = String.Empty;

            try
            {
                if (model.ID == null)
                {
                    #region new item       

                    successMessage = "Toplantı eklendi.";

                    var newmeeting = new TOPLANTILAR()
                    {
                        FIRMID = model.FIRMID,
                        SEBEP = model.SEBEP ?? 0,
                        TITLE = model.TITLE,
                        ICERIK = model.ICERIK,
                        TARIH = DateTime.Now,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userID,
                        IsDeleted = false
                    };
                    new ToplantilarRepo().Insert(newmeeting);

                    model.ID = newmeeting.ID;

                    foreach (var item in model.ToplantiMemberList)
                    {
                        var newtoplantimember = new ToplantiMember()
                        {
                            AssignedBy = userID,
                            AssignedOn = DateTime.Now,
                            MemberID = item.ID, //.MemberID,
                            ToplantiID = model.ID
                        };
                        new ToplantiMemberRepo().Insert(newtoplantimember);
                    }

                    foreach (var item in model.ToplantiYetkiliKisilerLlist)
                    {
                        var newtoplantiyetkilikisi = new ToplantiYetkiliKisi()
                        {
                            AssignedBy = userID,
                            AssignedOn = DateTime.Now,
                            YetkiliKisiID = item.ID, //.YetkiliKisiID,
                            ToplantiID = model.ID
                        };
                        new ToplantiYetkiliKisiRepo().Insert(newtoplantiyetkilikisi);
                    }
                    #endregion
                }
                else
                {
                    #region update item

                    successMessage = "Toplantı güncellendi.";

                    var toplantiitem = new ToplantilarRepo().GetByID(model.ID ?? 0);
                    if (toplantiitem == null)
                        return BadRequest("Toplantı kaydına ulaşılamadı.");

                    toplantiitem.FIRMID = model.FIRMID;
                    toplantiitem.SEBEP = model.SEBEP ?? 0;
                    toplantiitem.TITLE = model.TITLE;
                    toplantiitem.ICERIK = model.ICERIK;
                    toplantiitem.UpdatedBy = userID;
                    toplantiitem.UpdatedOn = DateTime.Now;
                    new ToplantilarRepo().Update();

                    var toplantiMemberData = new ToplantiMemberRepo().GetAll(q => q.ToplantiID == model.ID).Select(s => new { s.ID, s.MemberID }).ToList();

                    if (model.ToplantiMemberList != null)
                    {
                        foreach (var item in model.ToplantiMemberList)
                        {
                            if (!toplantiMemberData.Any(q => q.MemberID == item.ID)) //MemberID))
                            {
                                var newtoplantimember = new ToplantiMember()
                                {
                                    AssignedBy = userID,
                                    AssignedOn = DateTime.Now,
                                    MemberID = item.ID, //.MemberID,
                                    ToplantiID = model.ID
                                };
                                new ToplantiMemberRepo().Insert(newtoplantimember);
                            }
                        }
                    }

                    if (toplantiMemberData != null)
                    {
                        foreach (var item in toplantiMemberData)
                        {
                            if (!model.ToplantiMemberList.Any(q => q.ID == item.MemberID))
                            {
                                var deletetoplantiMember = new ToplantiMemberRepo().GetByID(item.ID);
                                if (deletetoplantiMember != null)
                                {
                                    new ToplantiMemberRepo().Delete(deletetoplantiMember);
                                }
                            }
                        }
                    }

                    var toplantiYetkiliKisiData = new ToplantiYetkiliKisiRepo().GetAll(q => q.ToplantiID == model.ID).Select(s => new { s.ID, s.YetkiliKisiID }).ToList();

                    if (model.ToplantiYetkiliKisilerLlist != null)
                    {
                        foreach (var item in model.ToplantiYetkiliKisilerLlist)
                        {
                            if (!toplantiYetkiliKisiData.Any(q => q.YetkiliKisiID == item.ID)) //.YetkiliKisiID))
                            {
                                var newtoplantiyetkilikisi = new ToplantiYetkiliKisi()
                                {
                                    AssignedBy = userID,
                                    AssignedOn = DateTime.Now,
                                    YetkiliKisiID = item.ID, //.YetkiliKisiID,
                                    ToplantiID = model.ID
                                };
                                new ToplantiYetkiliKisiRepo().Insert(newtoplantiyetkilikisi);
                            }
                        }
                    }

                    if (toplantiYetkiliKisiData != null)
                    {
                        foreach (var item in toplantiYetkiliKisiData)
                        {
                            if (!model.ToplantiYetkiliKisilerLlist.Any(q => q.ID == item.YetkiliKisiID))
                            {
                                var deletetoplantiYetkiliKisi = new ToplantiYetkiliKisiRepo().GetByID(item.ID);
                                if (deletetoplantiYetkiliKisi != null)
                                {
                                    new ToplantiYetkiliKisiRepo().Delete(deletetoplantiYetkiliKisi);
                                }
                            }
                        }
                    }
                    #endregion
                }

                var data = new ToplantilarRepo().GetAll(q => q.ID == model.ID).Select(s => new
                {
                    s.ID,
                    s.FIRMID,
                    s.MUSTERILER?.FIRMA_ADI,
                    ToplantiMembers = s.ToplantiMember.Select(s1 => new { FullName = s1.Member?.FirstName + " " + s1.Member?.LastName }),
                    ToplantiYetkiliKisiler = s.ToplantiYetkiliKisi.Select(s2 => new { s2.ID, FullName = s2.YETKILI_KISILER?.ADI + " " + s2.YETKILI_KISILER?.SOYADI }),
                    s.TARIH,
                    s.TITLE,
                    s.SEBEP,
                    //s.ICERIK,
                    ToplantiSebep = s.ToplantiSebep?.Title,
                    s.CreatedOn
                }).ToList();

                return Ok(new { ID = model.ID, Data = data, Message = successMessage });
            }
            catch (Exception ex)
            {
                //todo: add to log
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }




    }
}