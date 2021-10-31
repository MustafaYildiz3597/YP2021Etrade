using Nero2021.BLL.Repository;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Nero2021.Controllers
{
   
    public class TeklifController : BaseController
    {
        [Authorize]
        // GET: teklif
        public ActionResult Index()
        {
           
            string device = string.Empty;
            bool ismobile = base.isMobileBrowser(out device);
            ViewBag.Device = device;

            //ViewBag.Title = "Kinder Çekiliş";
            //return View("NotStarted");
            ViewBag.Title = "NERO - Teklif Sorgu";

            return View("Sorgu");
        }

        [Authorize]
        // GET: teklif/sorgu
        public ActionResult Sorgu()
        {
            #region check authorize
            var action = new ActionRepo().GetByFilter(q => q.Key == "teklifsorgulama" && q.IsEnabled == true);
            if (action == null)
            {
                ViewBag.Message = "Sayfa aktif olmadığından giriş yapılamadı!";
                return View("~/Views/Error/Page404.cshtml");
            }

            var userpermissions = AppHelper.GetUserPermissions().Where(q => q.ID == action.ID).FirstOrDefault();
            if (userpermissions == null)
                return new HttpForbiddenResult();

            if (userpermissions.ViewPermission != 1)
                return new HttpForbiddenResult();
            #endregion

            string device = string.Empty;
            bool ismobile = base.isMobileBrowser(out device);
            ViewBag.Device = device;

            //ViewBag.Title = "Kinder Çekiliş";
            //return View("NotStarted");
            ViewBag.Title = "NERO - Teklif Sorgu";

            return View();
        }

        [AllowAnonymous]
        public ActionResult GoruntuleXls(int ID)
        {
            var FileBytesArray = CreateOfferXls(ID);
            return File(FileBytesArray, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BU_Teklif_" + ID.ToString() + ".xlsx");
        }

        public byte[] CreateOfferXls(int ID)
        {
            try
            {
                byte[] fileData = null;

                var offer = new OfferRepo().GetByID(ID);
                if (offer == null)
                    return fileData;

                using (var stream = new MemoryStream())
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var sheet = package.Workbook.Worksheets.Add("Teklif");

                        sheet.DefaultRowHeight = 12;
                        sheet.TabColor = System.Drawing.Color.Black;
                        sheet.Cells.Style.Font.Name = "Verdana";
                        sheet.Cells.Style.Font.Size = 11;

                        //Header of table  
                        //  
                        // sheet.Cells[1, 1, 8, 1].Style.Font.Bold = true;
                        sheet.Cells[1, 1].Value = "Teklif NO";
                        sheet.Cells[1, 2].Value = offer.TEKLIFNO;

                        sheet.Cells[1, 1].Value = "Teklif Tipi";
                        sheet.Cells[1, 2].Value = offer.TeklifTipi?.Name; 

                        sheet.Cells[2, 1].Value = "Firma";
                        sheet.Cells[2, 2].Value = offer.MUSTERILER?.FIRMA_ADI;

                        sheet.Cells[3, 1].Value = "Yetkili Kişi";
                        sheet.Cells[3, 2].Value = offer.YETKILI_KISILER?.ADI + " " + offer.YETKILI_KISILER?.SOYADI;

                        sheet.Cells[4, 1].Value = "Konu";
                        sheet.Cells[4, 2].Value = offer.TITLE;

                        sheet.Cells[5, 1].Value = "iÇERİK";
                        sheet.Cells[5, 2].Value = Server.HtmlDecode(offer.ICERIK);

                        //sheet.Cells[6, 1].Value = "Müşteri Temsilcisi";
                        //sheet.Cells[6, 2].Value = offer.Member.FirstName + " " + offer.Member.LastName;

                        sheet.Cells[6, 1].Value = "Eklenme Tarihi";
                        sheet.Cells[6, 2].Value = offer.CreatedOn?.ToString("dd/MM/yyyy");  // model.Yetkili == null ? model.Siparis.Sorumlu_Eposta : model.Yetkili.Email;

                        sheet.Cells[7, 1].Value = "Ekleyen";
                        sheet.Cells[7, 2].Value = offer.Member?.FirstName + " " + offer.Member?.LastName;  // model.Yetkili == null ? model.Siparis.Sorumlu_Eposta : model.Yetkili.Email;

                        sheet.Cells[8, 1].Value = "Güncellenme Tarihi";
                        sheet.Cells[8, 2].Value = offer.UpdatedOn?.ToString("dd/MM/yyyy");  // model.Yetkili == null ? model.Siparis.Sorumlu_Eposta : model.Yetkili.Email;

                        sheet.Cells[9, 1].Value = "Güncelleyen";
                        sheet.Cells[9, 2].Value = offer.Member1?.FirstName + " " + offer.Member1?.LastName;  // model.Yetkili == null ? model.Siparis.Sorumlu_Eposta : model.Yetkili.Email;

                        sheet.Cells[10, 1].Value = "Teklif Durumu";
                        sheet.Cells[10, 2].Value = offer.TeklifDurumu?.Name;


                        int currentrow = 8;

                        sheet.Row(currentrow).Style.Font.Bold = true;
                        sheet.Cells[currentrow, 1, currentrow, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet.Cells[currentrow, 1, currentrow, 13].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                        sheet.Cells[currentrow, 1].Value = "Sıra No";
                        sheet.Cells[currentrow, 2].Value = "Müşteri/Tedarikçi Kodu";
                        sheet.Cells[currentrow, 3].Value = "BU Numarası";
                        sheet.Cells[currentrow, 4].Value = "OEM";
                        sheet.Cells[currentrow, 5].Value = "OEM 1";
                        sheet.Cells[currentrow, 6].Value = "Birim Fiyat";
                        sheet.Cells[currentrow, 7].Value = "P.B.";
                        sheet.Cells[currentrow, 8].Value = "Adet";
                        sheet.Cells[currentrow, 9].Value = "Toplam";
                        sheet.Cells[currentrow, 10].Value = "P.B.";
                        //sheet.Cells[currentrow, 12].Value = "Güncelleyen";
                        //sheet.Cells[currentrow, 13].Value = "Güncellenme Tarihi";

                        currentrow++;
                        int lineno = 1;
                        decimal EURToplam = 0;
                        decimal USDToplam = 0;
                        decimal TLToplam = 0;

                        foreach (var item in offer.TEKLIFLER_DETAY.AsQueryable())
                        {
                            sheet.Cells[currentrow, 1].Value = (lineno).ToString();
                            sheet.Cells[currentrow, 2].Value = item.CustomerCode;
                            sheet.Cells[currentrow, 3].Value = item.BuCode;
                            sheet.Cells[currentrow, 4].Value = item.Oem;
                            sheet.Cells[currentrow, 5].Value = item.Oem1;

                            sheet.Cells[currentrow, 6].Style.Numberformat.Format = "#,##0.00";
                            sheet.Cells[currentrow, 6].Value = item.UnitPrice;

                            sheet.Cells[currentrow, 7].Value = item.Currency1?.Code;

                            sheet.Cells[currentrow, 8].Style.Numberformat.Format = "#,##0.00";
                            sheet.Cells[currentrow, 8].Value = item.Quantity;

                            sheet.Cells[currentrow, 9].Style.Numberformat.Format = "#,##0.00";
                            sheet.Cells[currentrow, 9].Value = item.Quantity * item.UnitPrice;

                            sheet.Cells[currentrow, 10].Value = item.Currency1?.Code;
                            //sheet.Cells[currentrow, 12].Value = item.Member1.FirstName + " " + item.Member1.LastName;
                            //sheet.Cells[currentrow, 13].Value = item.UpdatedOn?.ToString("dd/MM/yyyy");

                            if (item.Currency1?.Code == "EURO")
                                EURToplam += (item.UnitPrice ?? 0) * (item.Quantity ?? 0);
                            else if (item.Currency1?.Code == "USD")
                                USDToplam += (item.UnitPrice ?? 0) * (item.Quantity ?? 0);
                            else if (item.Currency1?.Code == "TL")
                                TLToplam += (item.UnitPrice ?? 0) * (item.Quantity ?? 0);

                            lineno++;
                            currentrow++;
                        }

                        sheet.Cells[currentrow, 8].Value = "Toplam EUR";
                        sheet.Cells[currentrow + 1, 8].Value = "Toplam USD";
                        sheet.Cells[currentrow + 2, 8].Value = "Toplam TL";

                        sheet.Cells[currentrow, 9].Style.Numberformat.Format = "#,##0.00";
                        sheet.Cells[currentrow, 9].Value = EURToplam;
                        sheet.Cells[currentrow + 1, 9].Style.Numberformat.Format = "#,##0.00";
                        sheet.Cells[currentrow + 1, 9].Value = USDToplam;
                        sheet.Cells[currentrow + 2, 9].Style.Numberformat.Format = "#,##0.00";
                        sheet.Cells[currentrow + 2, 9].Value = TLToplam;
                         

                        sheet.Column(1).AutoFit();
                        sheet.Column(2).AutoFit();
                        sheet.Column(3).AutoFit();
                        sheet.Column(4).AutoFit();
                        sheet.Column(5).AutoFit();
                        sheet.Column(6).AutoFit();
                        sheet.Column(7).AutoFit();
                        sheet.Column(8).AutoFit();
                        sheet.Column(9).AutoFit();
                        sheet.Column(10).AutoFit();
                        //sheet.Column(11).AutoFit();
                        //sheet.Column(12).AutoFit();
                        //sheet.Column(13).AutoFit();

                        package.SaveAs(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        fileData = stream.ToArray();

                        return fileData;
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [AllowAnonymous]
        public ActionResult GoruntulePdf(int ID)
        {
            string html1 = BuildPDFPage(ID);
            return File(HtmlToPdfConvert(html1), "application/pdf");
        }

        public string BuildPDFPage(int offerID)
        {
            string retval = string.Empty;

            var offer = new OfferRepo().GetByID(offerID);
            if (offer == null)
                return "";

            #region build Page in html 
            StringBuilder sb = new StringBuilder();


            //sb.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
            //sb.Append("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            //sb.Append("<head>");
            //sb.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");


            //sb.Append("<title>Untitled Document</title>");
            //sb.Append("</head>");
            //sb.Append("<body>");

            sb.Append("<table cellpadding=\"0\" cellpadding=\"0\" border=\"0\">");
            sb.Append("	<tr>");
            sb.Append("		<td>Teklif NO</td>");
            sb.Append("		<td>" + offer.TEKLIFNO + "</td>");
            sb.Append("	</tr>");
            sb.Append("	<tr>");
            sb.Append("		<td>Teklif Tipi</td>");
            sb.Append("		<td>" + offer.TeklifTipi?.Name + "</td>");
            sb.Append("	</tr>");
            sb.Append("	<tr>");
            sb.Append("		<td>Firma</td>");
            sb.Append("		<td>" + offer.MUSTERILER?.FIRMA_ADI + "</td>");
            sb.Append("	</tr>");
            sb.Append("	<tr>");
            sb.Append("		<td>Yetkili Kişi</td>");
            sb.Append("		<td>" + offer.YETKILI_KISILER?.ADI + " " + offer.YETKILI_KISILER?.SOYADI + "</td>");
            sb.Append("	</tr>");
            sb.Append("	<tr>");
            sb.Append("		<td>Konu</td>");
            sb.Append("		<td>" + offer.TITLE + "</td>");
            sb.Append("	</tr>");
            sb.Append("	<tr>");
            sb.Append("		<td>iÇERİK</td>");
            sb.Append("		<td>" + Server.HtmlDecode(offer.ICERIK) + "</td>");
            sb.Append("	</tr>");
 
            sb.Append("	<tr>");
            sb.Append("		<td>Eklenme Tarihi</td>");
            sb.Append("		<td>" + offer.CreatedOn?.ToString("dd/MM/yyyy") + "</td>");
            sb.Append("	</tr>");
            sb.Append("	<tr>");
            sb.Append("		<td>Ekleyen</td>");
            sb.Append("		<td>" + offer.Member?.FirstName + " " + offer.Member?.LastName + "</td>");
            sb.Append("	</tr>");
            sb.Append("<tr>");
            sb.Append("		<td>Güncellenme Tarihi</td>");
            sb.Append("		<td>" + offer.UpdatedOn?.ToString("dd/MM/yyyy") + "</td>");
            sb.Append("	</tr>");
            sb.Append("<tr>");
            sb.Append("		<td>Güncelleyen</td>");
            sb.Append("		<td>" + offer.Member1?.FirstName + " " + offer.Member1?.LastName + "</td>");
            sb.Append("	</tr>");
            sb.Append("<tr>");
            sb.Append("		<td>Teklif Durumu</td>");
            sb.Append("		<td>" + offer.TeklifDurumu?.Name + "</td>");
            sb.Append("	</tr>");
            sb.Append("</table>");

            sb.Append("<table cellpadding=\"2\" cellspacing=\"0\" border=\"1\">");
            sb.Append("	 <thead>");
            sb.Append("		<tr>");
            sb.Append("			<th>Sıra No</th>");
            sb.Append("			<th style=\"width:100px\">Müşteri/Tedarikçi Kodu</th>");
            sb.Append("			<th style=\"width:100px\">BU Numarası</th>");
            //sb.Append("			<th>BU Eski</th>");
            sb.Append("			<th>OEM</th>");
            sb.Append("			<th>OEM1</th>");
            sb.Append("			<th>Birim Fiyat</th>");
            sb.Append("			<th>Adet</th>");
            sb.Append("			<th>Toplam</th>");
            sb.Append("			<th>P.B.</th>");
            //sb.Append("			<th>Güncelleyen</th>");
            //sb.Append("			<th>Güncellenme Tarihi</th>");
            sb.Append("		</tr>");
            sb.Append("	</thead>");
            sb.Append("	<tbody>");

            int lineno = 1;
            decimal EURToplam = 0;
            decimal USDToplam = 0;
            decimal TLToplam = 0;
            foreach (var item in offer.TEKLIFLER_DETAY.AsQueryable())
            {
                sb.Append("		<tr>");
                sb.Append("			<td>" + (lineno).ToString() + "</td>");
                sb.Append("			<td>" + item.CustomerCode + "</td>");
                sb.Append("			<td>" + item.BuCode + "</td>");
                sb.Append("			<td>" + item.Oem + "</td>");
                sb.Append("			<td>" + item.Oem1 + "</td>");

                //sb.Append("			<td>" + item.BUESKI + "</td>");
                //sb.Append("			<td>" + (string.IsNullOrWhiteSpace(item.NAME_EN) ? item.NAME_EN : item.NAME_DE) + "</td>");
                // sb.Append("			<td>" + item.NAME_DE + "</td>");
                sb.Append("			<td style=\"text-align:right\">" + item.UnitPrice?.ToString("N2") + "</td>");
                //sb.Append("			<td>" + item.Currency?.Code + "</td>");
                sb.Append("			<td style=\"text-align:right\">" + item.Quantity.ToString() + "</td>");
                sb.Append("			<td style=\"text-align:right\">" + (item.Quantity * item.UnitPrice)?.ToString("N2") + "</td>");
                sb.Append("			<td>" + item.Currency1?.Code + "</td>");
                //sb.Append("			<td>" + item.Member1?.FirstName + " " + item.Member1?.LastName + "</td>");
                //sb.Append("			<td>" + item.UpdatedOn?.ToString("dd/MM/yyyy") + "</td>");
                sb.Append("		</tr>");

                if (item.Currency1.Code == "EURO")
                    EURToplam += (item.UnitPrice ?? 0) * (item.Quantity ?? 0);
                else if (item.Currency1.Code == "USD")
                    USDToplam += (item.UnitPrice ?? 0) * (item.Quantity ?? 0);
                else if (item.Currency1.Code == "TL")
                    TLToplam += (item.UnitPrice ?? 0) * (item.Quantity ?? 0);

                lineno++;
            }

            sb.Append("		<tr>");
            sb.Append("			<td colspan\"6\">&nbsp;</td>");
            sb.Append("			<td>Toplam EUR</td>");
            sb.Append("			<td>" + EURToplam.ToString("N2") + "</td>");
            sb.Append("		</tr>");
            sb.Append("		<tr>");
            sb.Append("			<td colspan\"6\">&nbsp;</td>");
            sb.Append("			<td>Toplam USD</td>");
            sb.Append("			<td>" + USDToplam.ToString("N2") + "</td>");
            sb.Append("		</tr>");
            sb.Append("		<tr>");
            sb.Append("			<td colspan\"6\">&nbsp;</td>");
            sb.Append("			<td>Toplam TL</td>");
            sb.Append("			<td>" + TLToplam.ToString("N2") + "</td>");
            sb.Append("		</tr>");


            sb.Append("	</tbody>");
            sb.Append("</table>");



            //sb.Append("</body>");
            //sb.Append("</html>");

            #endregion

            retval = sb.ToString();

            return retval;
        }

        public static Byte[] HtmlToPdfConvert(String html)
        {
            Byte[] res = null;
            using (MemoryStream ms = new MemoryStream())
            {
                var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(html, PdfSharp.PageSize.A4);
                pdf.Save(ms);
                res = ms.ToArray();
            }
            return res;
        }
    }
}
