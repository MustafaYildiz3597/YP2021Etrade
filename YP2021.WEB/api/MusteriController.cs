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
    public class MusteriController : ApiController
    {
        //[HttpPost]
        //public IHttpActionResult SorguPageDTList()
        //{
        //    try
        //    {
        //        return Ok(new OrderRepo().SorguPageDTList());
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
        //    }

        //}

        [HttpPost]
        public IHttpActionResult CarilerPageDTList(CarilerPageDTListDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası!");

            try
            {
                return Ok(new MusterilerRepo().CarilerPageDTList(model));
            }
            catch (Exception ex)
            {
                return BadRequest("Listeleme işlemi yapılırken bir hata oluştu.");
            }

        }

        [HttpPost]
        public IHttpActionResult Detail(MusteriIDDTO model)
        {
            try
            {
                //if (!User.Identity.IsAuthenticated)
                //    return Unauthorized();
                //    return Content(HttpStatusCode.Unauthorized, "My error message");

                var musteridetail = new MusterilerRepo().FindByID(model.ID ?? 0);
                var musteriDocuments = new MusteriDocumentRepo().GetAll(q => q.MusteriID == model.ID && (q.IsDeleted ?? false) == false)
                    .Select(s => new { s.ID, s.Title, s.FileName, CreatedBy = s.Member?.FirstName + " " + s.Member?.LastName, s.CreatedOn, s.Description, FilePath = s.FilePath + "?t=" + DateTime.Now.Ticks.ToString(), s.MusteriID, s.DocumentType, s.RankNumber, s.ThumbnailPath });

                return Ok(new
                {
                    MusteriDetail = musteridetail,
                    MusteriDocumentList = musteriDocuments
                });
            }
            catch (Exception ex)
            {
                return BadRequest("İşlem hatası." + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Delete(MusteriIDDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string userID = HttpContext.Current.User.Identity.GetUserId();

                var item = new MusterilerRepo().GetByID(model.ID ?? 0);
                if (item == null)
                    return BadRequest("Sipariş kaydına ulaşılamadı.");

                item.IsDeleted = true;
                item.DeletedBy = userID;
                item.DeletedOn = DateTime.Now;

                new MusterilerRepo().Update();

                return Ok("Cari silindi");
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult Save(MusteriSaveDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            //if (model.TDURUMU == null)
            //    return BadRequest("Teklif durumu belirtilmemiş. Lütfen kontrol ediniz.");

            string userID = HttpContext.Current.User.Identity.GetUserId();

            string successMessage = String.Empty;

            try
            {
                if (model.ID == null)
                {
                    #region new item       

                    successMessage = "Cari eklendi.";

                    var newmusteri = new MUSTERILER()
                    {
                        FIRMA_TIPI = model.FIRMA_TIPI,
                        FIRMA_ADI = model.FIRMA_ADI,
                        ADRES = model.ADRES,
                        POSTA_KODU = model.POSTA_KODU,
                        VERGI_DAIRESI = model.VERGI_DAIRESI,
                        VERGI_NUMARASI = model.VERGI_NUMARASI,
                        MARKA = model.MARKA,
                        MAIL_ADRESI = model.MAIL_ADRESI,
                        WEB_STESI = model.WEB_STESI,
                        GSM = model.GSM,
                        TEL_1 = model.TEL_1,
                        TEL_2 = model.TEL_2,
                        FAX = model.FAX,
                        NOT = model.NOT,
                        TARIH = DateTime.Now,
                        ULKE = model.ULKE,
                        SEHIR = model.SEHIR,
                        CariVadeAltRakamID = model.CariVadeAltRakamID,
                        CariVadeID = model.CariVadeID,
                        CariOdemeSekliID = model.CariOdemeSekliID,
                        CariTeslimatSekliID = model.CariTeslimatSekliID,
                        CariNakliyeOdemesiID = model.CariNakliyeOdemesiID,
                        FIRMNICK = model.FIRMNICK,
                        IsB2BDealer = model.IsB2BDealer,
                        B2BDealerTypeID = model.B2BDealerTypeID,
                        CreatedOn = DateTime.Now,
                        CreatedBy = userID,
                        IsDeleted = false
                    };
                    new MusterilerRepo().Insert(newmusteri);

                    model.ID = newmusteri.ID;

                  
                    #endregion
                }
                else
                {
                    #region update item

                    successMessage = "Cari güncellendi.";

                    var item = new MusterilerRepo().GetByID(model.ID ?? 0);
                    if (item == null)
                        return BadRequest("Cari kaydına ulaşılamadı.");

                    item.FIRMA_TIPI = model.FIRMA_TIPI;
                    item.FIRMA_ADI = model.FIRMA_ADI;
                    item.ADRES = model.ADRES;
                    item.POSTA_KODU = model.POSTA_KODU;
                    item.VERGI_DAIRESI = model.VERGI_DAIRESI;
                    item.VERGI_NUMARASI = model.VERGI_NUMARASI;
                    item.MARKA = model.MARKA;
                    item.MAIL_ADRESI = model.MAIL_ADRESI;
                    item.WEB_STESI = model.WEB_STESI;
                    item.GSM = model.GSM;
                    item.TEL_1 = model.TEL_1;
                    item.TEL_2 = model.TEL_2;
                    item.FAX = model.FAX;
                    item.NOT = model.NOT;
                    item.ULKE = model.ULKE;
                    item.SEHIR = model.SEHIR;
                    item.CariVadeAltRakamID = model.CariVadeAltRakamID;
                    item.CariVadeID = model.CariVadeID;
                    item.CariOdemeSekliID = model.CariOdemeSekliID;
                    item.CariTeslimatSekliID = model.CariTeslimatSekliID;
                    item.CariNakliyeOdemesiID = model.CariNakliyeOdemesiID;
                    item.FIRMNICK = model.FIRMNICK;
                    item.IsB2BDealer = model.IsB2BDealer;
                    item.B2BDealerTypeID = model.B2BDealerTypeID;
                    item.UpdatedBy = userID;
                    item.UpdatedOn = DateTime.Now;
                    
                    new MusterilerRepo().Update();

                    #endregion
                }

                var data = new MusterilerRepo().GetAll(q => q.ID == model.ID).Select(s => new
                {
                    s.ID,
                    FirmaTipi = s.FIRMA_TIPLERI?.TITLE,
                    s.FIRMA_ADI,
                    Ulke = s.ULKELER?.UNAME,
                    Sehir = s.SEHIRLER?.NAME,
                    s.WEB_STESI,
                    s.FIRMNICK
                }).ToList();

                return Ok(new { ID = model.ID, Data = data, Message = successMessage });
            }
            catch (Exception ex)
            {
                //todo: add to log
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }

        [HttpPost]
        public IHttpActionResult CarilerPageFillAllCmb()
        {
            try
            {
                var firmatipleri = new FirmaTipleriRepo().GetAll().Select(s => new { s.ID, s.TITLE }).OrderBy(o => o.ID).AsQueryable();
                var ulkeler = new UlkelerRepo().GetAll().Select(s => new { s.UID, s.UNAME }).OrderBy(o => o.UID).AsQueryable();
                var sehirler = new SehirlerRepo().GetAll().Select(s => new { s.ID, s.NAME, s.UID }).OrderBy(o => o.ID).AsQueryable();
                var contactTitles = new ContactTitleRepo().GetAll().Select(s => new { s.ID, s.Title }).OrderBy(o => o.Title).AsQueryable();
                var adrestipleri = new AdresTipiRepo().GetAll().Select(s => new { s.ID, s.Name }).OrderBy(o => o.ID).AsQueryable();
                var b2bdealertypes = new B2BDealerTypeRepo().GetAll(q => (q.IsDeleted ?? false) == false).Select(s => new { s.ID, s.Name }).AsQueryable();
                var carivadealtrakamlist = new CariVadeAltRakamRepo().GetAll().OrderBy(o => o.RankNumber).Select(s => new { s.ID, s.Name }).AsQueryable();
                var carivadelist = new CariVadeRepo().GetAll().OrderBy(o => o.RankNumber).Select(s => new { s.ID, s.Name }).AsQueryable();
                var cariodemeseklilist = new CariOdemeSekliRepo().GetAll().OrderBy(o => o.RankNumber).Select(s => new { s.ID, s.Name }).AsQueryable();
                var cariteslimatseklilist = new CariTeslimatSekliRepo().GetAll().OrderBy(o => o.RankNumber).Select(s => new { s.ID, s.Name }).AsQueryable();
                var carinakliyeodemesilist = new CariNakliyeOdemesiRepo().GetAll().OrderBy(o => o.RankNumber).Select(s => new { s.ID, s.Name }).AsQueryable();

                return Ok(new
                {
                    FirmaTipleri = firmatipleri,
                    Ulkeler = ulkeler,
                    Sehirler = sehirler,
                    ContactTitles = contactTitles,
                    AdresTipleri = adrestipleri,
                    CariVadeAltRakamList = carivadealtrakamlist,
                    CariVadeList = carivadelist,
                    CariOdemeSekliList = cariodemeseklilist,
                    CariTeslimatSekliList = cariteslimatseklilist,
                    CariNakliyeOdemesiList = carinakliyeodemesilist,
                    B2bDealerTypes = b2bdealertypes
                });
            }
            catch (Exception ex)
            {
                return BadRequest("Bir hata oluştu. " + ex.Message);
            }
        }

        [HttpPost]
        public IHttpActionResult ContactList(MusteriIDDTO model)
        {
            try
            {
                var list = new YetkiliKisilerRepo().GetAll(q => q.FIRMA_ID == model.ID && (q.IsDeleted ?? false) == false)
                    .Select(s => new
                    {
                        s.ID,
                        s.FIRMA_ID,
                        s.ADI,
                        s.SOYADI,
                        FullName = s.ADI + " " + s.SOYADI,
                        s.GOREV,
                        s.ContactTitleID,
                        ContactTitle = s.ContactTitle?.Title,
                        s.GSM,
                        s.MAIL_ADRESI,
                        s.TEL,
                        s.TARIH,
                        s.B2bPassword,
                        s.B2bUsername,
                        s.IsB2bUser,
                        s.CreatedOn,
                        CreatedBy = s.Member?.FirstName + " " + s.Member?.LastName,
                        s.UpdatedOn,
                        UpdatedBy = s.Member1?.FirstName + " " + s.Member1?.LastName,

                    });

                return Ok(new { YetkiliKisiler = list });
            }
            catch (Exception)
            {
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult SaveContact(MusteriContactSaveDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string userID = HttpContext.Current.User.Identity.GetUserId();

                string successMessage = String.Empty;

                if (model.ID == null)
                {
                    #region new item       
                    successMessage = "Yetkili Kişi eklendi.";

                    var item = new YETKILI_KISILER()
                    {
                        FIRMA_ID = model.FIRMA_ID,
                        ADI = model.ADI,
                        ContactTitleID = model.ContactTitleID,
                        GSM = model.GSM,
                        IsDeleted = false,
                        MAIL_ADRESI = model.MAIL_ADRESI,
                        SOYADI = model.SOYADI,
                        TARIH = DateTime.Now,
                        TEL = model.TEL,
                         IsB2bUser = model.IsB2bUser,
                          B2bUsername = model.B2bUsername,
                           B2bPassword = model.B2bPassword,
                        CreatedBy = userID,
                        CreatedOn = DateTime.Now
                    };
                    new YetkiliKisilerRepo().Insert(item);

                    model.ID = item.ID;
                    #endregion
                }
                else
                {
                    successMessage = "Yetkili Kişi güncellendi.";

                    var yetkilikisi = new YetkiliKisilerRepo().GetByID(model.ID ?? 0);

                    if (yetkilikisi == null)
                        return BadRequest("Kayıt yapılırken yetkili kişi Kaydına erişilemedi. Lütfen tekrar deneyiniz.");

                    yetkilikisi.ADI = model.ADI;
                    yetkilikisi.ContactTitleID = model.ContactTitleID;
                    yetkilikisi.GSM = model.GSM;
                    yetkilikisi.MAIL_ADRESI = model.MAIL_ADRESI;
                    yetkilikisi.SOYADI = model.SOYADI;
                    yetkilikisi.TEL = model.TEL;
                    yetkilikisi.IsB2bUser = model.IsB2bUser;
                    yetkilikisi.B2bPassword = model.B2bPassword;
                    yetkilikisi.B2bUsername = model.B2bUsername;
                    yetkilikisi.UpdatedBy = userID;
                    yetkilikisi.UpdatedOn = DateTime.Now;

                    new YetkiliKisilerRepo().Update();
                }

                var owner = new MemberRepo().GetByID(userID);

                return Ok(new { ID = model.ID, Owner = owner.FirstName + " " + owner.LastName, Message = successMessage, Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteContact(MusteriDeleteContactDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Yetkili Kişi silindi.";

                var yetkilikisi = new YetkiliKisilerRepo().GetByID(model.ID ?? 0);
                if (yetkilikisi == null)
                    return BadRequest("Silme işlemi yapılırken kişi kaydına erişilemedi. Lütfen tekrar deneyiniz.");

                yetkilikisi.IsDeleted = true;
                yetkilikisi.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                yetkilikisi.DeletedOn = DateTime.Now;
                new YetkiliKisilerRepo().Update();

                return Ok(new { ID = model.ID, Message = successMessage });
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }


        [HttpPost]
        public IHttpActionResult SaveDocument(MusteriDocumentSaveDTO model)
        {
            if (model == null)
                return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

            if (model.MusteriID == null)
                return BadRequest("Cari kaydına ulaşılamadı.");

            var musteri = new MusterilerRepo().GetByID(model.MusteriID ?? 0);

            if (musteri == null)
                return BadRequest("Cari kaydına ulaşılamadı.");

            string successMessage = String.Empty;

            string filename = model.FileName;

            try
            {
                if (model.ID == null)
                {
                    if (String.IsNullOrWhiteSpace(model.MusteriDocumentData))
                        return BadRequest("Döküman yüklenemedi.");

                    int newitemindex = (new MusteriDocumentRepo().GetAll(q => q.MusteriID == model.MusteriID).OrderByDescending(o => o.ItemIndex).FirstOrDefault()?.ItemIndex ?? 0) + 1;
                    string folder = "/upload/caridokuman/" + model.MusteriID; // model.ItemType == 1 ? "/upload/pimages/" : "/upload/trimages/";
                    string filemappath = Path.Combine(HttpContext.Current.Server.MapPath(folder), filename);
                    string filepath = folder + "/" + filename; // Path.Combine(folder, filename);

                    if (!String.IsNullOrWhiteSpace(model.MusteriDocumentData))
                    {
                        //filename = "p" + newitemindex.ToString() + "_" + reg.Replace(product.BUKOD + ".png", string.Empty);

                        #region save document 
                        // /upload/caridokuman/493/kduelslogs1.xlsx
                        

                        if (!Directory.Exists(HttpContext.Current.Server.MapPath(folder)))
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(folder));

                        using (FileStream fs = new FileStream(filemappath, FileMode.Create))
                        {
                            using (BinaryWriter bw = new BinaryWriter(fs))
                            {
                                byte[] data = Convert.FromBase64String(model.MusteriDocumentData);
                                bw.Write(data);
                                bw.Close();
                            }
                        }
                        #endregion

                        if (!System.IO.File.Exists(filemappath))
                            return BadRequest("Döküman ekleme işleminde bir hata oluştu.");
                    }

                    #region new item       
                    successMessage = "Döküman ve döküman bilgileri eklendi.";

                    var item = new MusteriDocument()
                    {
                        DocumentType = model.DocumentType,
                        FilePath = filepath,
                        FileName = filename,
                        IsDeleted = false,
                        ItemIndex = newitemindex,
                        //ItemType = model.ItemType,
                        RankNumber = model.RankNumber,
                        ThumbnailPath = "",
                        Title = model.Title,
                        Description = model.Description,
                        MusteriID = model.MusteriID,
                        CreatedOn = DateTime.Now,
                        CreatedBy = HttpContext.Current.User.Identity.GetUserId()
                    };
                    new MusteriDocumentRepo().Insert(item);

                    model.ID = item.ID;
                    #endregion
                }
                else
                {
                    successMessage = "Döküman bilgileri güncellendi.";

                    var musteriDocument = new MusteriDocumentRepo().GetByID(model.ID ?? 0);

                    if (musteriDocument == null)
                        return BadRequest("Ürün-resim kaydına erişilemedi. Lütfen tekrar deneyiniz.");

                    /* kayıt güncellemeye gerek yok. silsin baştan eklesin.*/
                    //filename = musteriDocument.FilePath;

                    //if (!String.IsNullOrWhiteSpace(model.MusteriDocumentData))
                    //{
                    //    #region save image 
                    //    string folder = model.ItemType == 1 ? "/upload/pimages/" : "/upload/trimages/";
                    //    string filemappath = HttpContext.Current.Server.MapPath(filename);
                    //    filename = Path.Combine(folder, filename);

                    //    using (FileStream fs = new FileStream(filemappath, FileMode.Truncate))
                    //    {
                    //        using (BinaryWriter bw = new BinaryWriter(fs))
                    //        {
                    //            byte[] data = Convert.FromBase64String(model.MusteriDocumentData);
                    //            bw.Write(data);
                    //            bw.Close();
                    //        }
                    //    }
                    //    #endregion

                    //    if (!System.IO.File.Exists(filemappath))
                    //        return BadRequest("Resim kaydedilirken bir hata oluştu.");
                    //}

                    musteriDocument.RankNumber = model.RankNumber;
                    musteriDocument.Title = model.Title;
                    musteriDocument.Description = model.Description;
                    musteriDocument.UpdatedOn = DateTime.Now;
                    musteriDocument.UpdatedBy = HttpContext.Current.User.Identity.GetUserId();

                    new MusteriDocumentRepo().Update();
                }

                var musteriDocumentlist = new MusteriDocumentRepo().GetAll(q => q.MusteriID == model.MusteriID && (q.IsDeleted ?? false) == false)
                    .Select(s => new { s.ID, s.DocumentType, s.Title, s.FileName, CreatedBy = s.Member?.FirstName + " " + s.Member?.LastName, s.CreatedOn, s.Description, FilePath = s.FilePath + "?t=" + DateTime.Now.Ticks.ToString(), s.MusteriID, s.RankNumber, s.ThumbnailPath });
                
                return Ok(new
                {
                    ID = model.ID ?? 0,
                    Message = successMessage,
                    MusteriDocumentList = musteriDocumentlist
                });
            }
            catch (Exception ex)
            {
                // todo: add to log 
                return BadRequest("İşlem yapılırken bir hata oluştu.");
            }
        }
        [HttpPost]
        public IHttpActionResult DeleteDocument(MusteriDocumentDeleteDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Döküman silindi.";

                var document = new MusteriDocumentRepo().GetByID(model.ID ?? 0);
                if (document == null)
                    return BadRequest("Silme işlemi yapılırken döküman kaydına erişilemedi. Lütfen tekrar deneyiniz.");

                document.IsDeleted = true;
                document.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                document.DeletedOn = DateTime.Now;
                new MusteriDocumentRepo().Update();

                return Ok(new { ID = model.ID, Message = successMessage });
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }



        [HttpPost]
        public IHttpActionResult AddressList(MusteriIDDTO model)
        {
            try
            {
                var list = new AdreslerRepo().GetAll(q => q.FIRMA_ID == model.ID && (q.IsDeleted ?? false) == false)
                    .Select(s => new
                    {
                        s.ID,
                        s.FIRMA_ID,
                        s.TIP,
                        AdresTipi = s.AdresTipi?.Name,
                        Ulke = s.ULKE,
                        UlkeAd = s.ULKELER?.UNAME,
                        Sehir = s.SEHIR,
                        SehirAd = s.SEHIRLER?.NAME,
                        s.POSTA_KODU,
                        s.TARIH,
                        s.ADRES,
                        s.CreatedOn,
                        CreatedBy = s.Member?.FirstName + " " + s.Member?.LastName,
                        s.UpdatedOn,
                        UpdatedBy = s.Member1?.FirstName + " " + s.Member1?.LastName
                    });

                return Ok(new { Addresses = list });
            }
            catch (Exception)
            {
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult SaveAddress(MusteriAddressSaveDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");


                string successMessage = String.Empty;

                if (model.ID == null)
                {
                    #region new item       
                    successMessage = "Adres eklendi.";

                    var item = new ADRESLER()
                    {
                        ADRES = model.ADRES,
                        FIRMA_ID = model.FIRMA_ID,
                        TIP = model.TIP,
                        ULKE = model.ULKE,
                        SEHIR = model.SEHIR,
                        POSTA_KODU = model.POSTA_KODU,
                        IsDeleted = false,
                        TARIH = DateTime.Now,
                        CreatedBy = HttpContext.Current.User.Identity.GetUserId(),
                        CreatedOn = DateTime.Now
                    };
                    new AdreslerRepo().Insert(item);

                    model.ID = item.ID;
                    #endregion
                }
                else
                {
                    successMessage = "Adres güncellendi.";

                    var adres = new AdreslerRepo().GetByID(model.ID ?? 0);

                    if (adres == null)
                        return BadRequest("Kayıt yapılırken yetkili adres kaydına erişilemedi. Lütfen tekrar deneyiniz.");

                    adres.ADRES = model.ADRES;
                    adres.TIP = model.TIP;
                    adres.ULKE = model.ULKE;
                    adres.SEHIR = model.SEHIR;
                    adres.POSTA_KODU = model.POSTA_KODU;
                    adres.UpdatedBy = HttpContext.Current.User.Identity.GetUserId();
                    adres.UpdatedOn = DateTime.Now;

                    new AdreslerRepo().Update();
                }

                return Ok(new { ID = model.ID, Message = successMessage, Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

        [HttpPost]
        public IHttpActionResult DeleteAddress(MusteriDeleteAddressDTO model)
        {
            try
            {
                if (model == null)
                    return BadRequest("Parametre hatası. Lütfen tekrar deneyiniz.");

                string successMessage = "Adres silindi.";

                var adres = new AdreslerRepo().GetByID(model.ID ?? 0);
                if (adres == null)
                    return BadRequest("Silme işlemi yapılırken adres kaydına erişilemedi. Lütfen tekrar deneyiniz.");

                adres.IsDeleted = true;
                adres.DeletedBy = HttpContext.Current.User.Identity.GetUserId();
                adres.DeletedOn = DateTime.Now;
                new AdreslerRepo().Update();

                return Ok(new { ID = model.ID, Message = successMessage });
            }
            catch (Exception)
            {
                //todo: a to log
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }


        [HttpPost]
        public IHttpActionResult MeetingList(MusteriIDDTO model)
        {
            try
            {
                var list = new ToplantilarRepo().GetAll(q => q.FIRMID == model.ID && (q.IsDeleted ?? false) == false)
                    .Select(s => new
                    {
                        s.ID,
                        s.FIRMID,
                        ToplantiMembers = s.ToplantiMember.Select(s1 => new { s1.ID, FullName = s1.Member.FirstName + " " + s1.Member.LastName }),
                        ToplantiYetkiliKisiler = s.ToplantiYetkiliKisi.Select(s2 => new { s2.ID, FullName = s2.YETKILI_KISILER.ADI + " " + s2.YETKILI_KISILER.SOYADI }),
                        s.TARIH,
                        s.TITLE,
                        s.SEBEP,
                        s.ICERIK,
                        ToplantiSebep = s.ToplantiSebep?.Title,
                        s.CreatedOn
                    });

                return Ok(new { Meetings = list });
            }
            catch (Exception)
            {
                return BadRequest("Bir hata oluştu. Lütfen tekrar deneyiniz.");
            }
        }

    }
}