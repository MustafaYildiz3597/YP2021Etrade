using Nero2021.BLL.Repository;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Nero2021.Controllers
{

    public class SiparisController : BaseController
    {
        [Authorize]
        // GET: siparis
        public ActionResult Index()
        {
            string device = string.Empty;
            bool ismobile = base.isMobileBrowser(out device);
            ViewBag.Device = device;

            //ViewBag.Title = "Kinder Çekiliş";
            //return View("NotStarted");
            ViewBag.Title = "NERO - Sipariş Sorgu";

            return RedirectToAction("Sorgu");
        }

        [Authorize]
        // GET: siparis/sorgu
        public ActionResult Sorgu()
        {
            #region check authorize
            var action = new ActionRepo().GetByFilter(q => q.Key == "siparissorgulama" && q.IsEnabled == true);
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
            ViewBag.Title = "NERO - Sipariş Sorgu";

            return View();
        }

        [Authorize]
        // GET: siparis/ExceldenYukle
        public ActionResult ExceldenYukle()
        {
            #region check authorize
            var action = new ActionRepo().GetByFilter(q => q.Key == "siparisexceldenyukle" && q.IsEnabled == true);
            if (action == null)
            {
                ViewBag.Message = "Sayfa aktif olmadığından giriş yapılamadı!";
                return View("~/Views/Error/Page404.cshtml");
            }

            var userpermissions = AppHelper.GetUserPermissions().Where(q => q.ID == action.ID).FirstOrDefault();
            if (userpermissions == null)
                return new HttpForbiddenResult();

            if (userpermissions.ExecutePermission != 1)
                return new HttpForbiddenResult();
            #endregion

            string device = string.Empty;
            bool ismobile = base.isMobileBrowser(out device);
            ViewBag.Device = device;

            //string IdentityUserID = User.Identity.GetUserId();
            //ViewBag.FirmID = new MemberRepo().GetByID(IdentityUserID)?.FirmID.ToString();

            ViewBag.Title = "NERO - Sipariş / Excelden Yükle";

            return View("OrderExcelImport");
        }



        [AllowAnonymous]
        public ActionResult GoruntuleXls(int ID)
        {
            var FileBytesArray = CreateOrderXls(ID);
            return File(FileBytesArray, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BU_Sipariş_" + ID.ToString() + ".xlsx");
        }

        public byte[] CreateOrderXls(int ID)
        {
            try
            {
                byte[] fileData = null;

                var order = new OrderRepo().GetByID(ID);
                if (order == null)
                    return fileData;

                using (var stream = new MemoryStream())
                {
                    using (var package = new ExcelPackage(stream))
                    {
                        var sheet = package.Workbook.Worksheets.Add("Sipariş");

                        sheet.DefaultRowHeight = 12;
                        sheet.TabColor = System.Drawing.Color.Black;
                        sheet.Cells.Style.Font.Name = "Verdana";
                        sheet.Cells.Style.Font.Size = 11;

                        //Header of table  
                        //  
                        // sheet.Cells[1, 1, 8, 1].Style.Font.Bold = true;
                        sheet.Cells[1, 1].Value = "Sipariş Kodu";
                        sheet.Cells[1, 2].Value = order.OrderNo; // model.Siparis.Kodu;

                        sheet.Cells[2, 1].Value = "Firma";
                        sheet.Cells[2, 2].Value = order.MUSTERILER?.FIRMA_ADI; // model.Firma == null ? model.Siparis.Firma_Adi : model.Firma.Adi;

                        sheet.Cells[3, 1].Value = "Yetkili";
                        sheet.Cells[3, 2].Value = order.YETKILI_KISILER?.ADI + " " + order.YETKILI_KISILER?.SOYADI; // "Levent Gürkaya"; //model.Yetkili == null ? model.Siparis.Sorumlu_AdSoyad : model.Yetkili.FirstName + " " + model.Yetkili.LastName;

                        sheet.Cells[4, 1].Value = "Sipariş Tarihi";
                        sheet.Cells[4, 2].Value = order.OrderDate?.ToString("dd/MM/yyyy");  // model.Yetkili == null ? model.Siparis.Sorumlu_Eposta : model.Yetkili.Email;

                        sheet.Cells[5, 1].Value = "Eklenme Tarihi";
                        sheet.Cells[5, 2].Value = order.CreatedOn?.ToString("dd/MM/yyyy");  // model.Yetkili == null ? model.Siparis.Sorumlu_Eposta : model.Yetkili.Email;

                        sheet.Cells[6, 1].Value = "Ekleyen";
                        sheet.Cells[6, 2].Value = order.Member?.FirstName + " " + order.Member?.LastName;  // model.Yetkili == null ? model.Siparis.Sorumlu_Eposta : model.Yetkili.Email;

                        sheet.Cells[7, 1].Value = "Güncellenme Tarihi";
                        sheet.Cells[7, 2].Value = order.UpdatedOn?.ToString("dd/MM/yyyy");  // model.Yetkili == null ? model.Siparis.Sorumlu_Eposta : model.Yetkili.Email;

                        sheet.Cells[8, 1].Value = "Güncelleyen";
                        sheet.Cells[8, 2].Value = order.Member1?.FirstName + " " + order.Member1?.LastName;  // model.Yetkili == null ? model.Siparis.Sorumlu_Eposta : model.Yetkili.Email;

                        int currentrow = 10;

                        sheet.Row(currentrow).Style.Font.Bold = true;
                        sheet.Cells[currentrow, 1, currentrow, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet.Cells[currentrow, 1, currentrow, 13].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.Gray);
                        sheet.Cells[currentrow, 1].Value = "Sıra No";
                        sheet.Cells[currentrow, 2].Value = "Müşteri Ürün Kodu";
                        sheet.Cells[currentrow, 3].Value = "BU Numarası";
                        sheet.Cells[currentrow, 4].Value = "BU Eski";
                        sheet.Cells[currentrow, 5].Value = "NAME_EN";
                        sheet.Cells[currentrow, 6].Value = "NAME_DE";
                        sheet.Cells[currentrow, 7].Value = "Birim Fiyat";
                        sheet.Cells[currentrow, 8].Value = "P.B.";
                        sheet.Cells[currentrow, 9].Value = "Adet";
                        sheet.Cells[currentrow, 10].Value = "Toplam";
                        sheet.Cells[currentrow, 11].Value = "P.B.";
                        sheet.Cells[currentrow, 12].Value = "Güncelleyen";
                        sheet.Cells[currentrow, 13].Value = "Güncellenme Tarihi";

                        currentrow++;
                        int lineno = 1;
                        int adetToplam = 0;
                        decimal EURToplam = 0;
                        decimal USDToplam = 0;
                        decimal TLToplam = 0;

                        foreach (var item in order.ODetay.AsQueryable())
                        {
                            sheet.Cells[currentrow, 1].Value = (lineno).ToString();
                            sheet.Cells[currentrow, 2].Value = item.CustomerCode;
                            sheet.Cells[currentrow, 3].Value = item.BuCode;
                            sheet.Cells[currentrow, 4].Value = item.BUESKI;
                            sheet.Cells[currentrow, 5].Value = item.NAME_EN;
                            sheet.Cells[currentrow, 6].Value = item.NAME_DE;

                            sheet.Cells[currentrow, 7].Style.Numberformat.Format = "#,##0.00";
                            sheet.Cells[currentrow, 7].Value = item.UnitPrice;

                            sheet.Cells[currentrow, 8].Value = item.Currency?.Code;

                            sheet.Cells[currentrow, 9].Style.Numberformat.Format = "#,##0.00";
                            sheet.Cells[currentrow, 9].Value = item.Quantity;
                            adetToplam += item.Quantity ?? 0;

                            sheet.Cells[currentrow, 10].Style.Numberformat.Format = "#,##0.00";
                            sheet.Cells[currentrow, 10].Value = item.Quantity * item.UnitPrice;

                            sheet.Cells[currentrow, 11].Value = item.Currency.Code;
                            sheet.Cells[currentrow, 12].Value = item.Member1?.FirstName + " " + item.Member1?.LastName;
                            sheet.Cells[currentrow, 13].Value = item.UpdatedOn?.ToString("dd/MM/yyyy");

                            if (item.Currency.Code == "EURO")
                                EURToplam += (item.UnitPrice ?? 0) * (item.Quantity ?? 0);
                            else if (item.Currency.Code == "USD")
                                USDToplam += (item.UnitPrice ?? 0) * (item.Quantity ?? 0);
                            else if (item.Currency.Code == "TL")
                                TLToplam += (item.UnitPrice ?? 0) * (item.Quantity ?? 0);

                            lineno++;
                            currentrow++;
                        }

                        sheet.Cells[currentrow, 8].Style.Numberformat.Format = "#,##0";
                        sheet.Cells[currentrow, 8].Value = adetToplam;
                        //sheet.Cells[currentrow + 1, 8].Value = "Toplam USD";
                        //sheet.Cells[currentrow + 2, 8].Value = "Toplam TL";

                        sheet.Cells[currentrow, 9].Style.Numberformat.Format = "#,##0.00";
                        sheet.Cells[currentrow, 9].Value = EURToplam;
                        sheet.Cells[currentrow, 10].Value = "EURO";

                        sheet.Cells[currentrow + 1, 9].Style.Numberformat.Format = "#,##0.00";
                        sheet.Cells[currentrow + 1, 9].Value = USDToplam;
                        sheet.Cells[currentrow + 1, 10].Value = "USD";

                        sheet.Cells[currentrow + 2, 9].Style.Numberformat.Format = "#,##0.00";
                        sheet.Cells[currentrow + 2, 9].Value = TLToplam;
                        sheet.Cells[currentrow + 2, 10].Value = "TL";

                        //var genel_toplam = 555; // model.Urunler.Sum(x => x.AraToplam);

                        //sheet.Cells[recordIndex, 6].Value = "Alt Toplam";
                        //sheet.Cells[recordIndex, 6].Style.Font.Bold = true;
                        //sheet.Cells[recordIndex, 7].Value = genel_toplam;
                        //sheet.Cells[recordIndex, 7].Style.Numberformat.Format = "#,##0.00";
                        //recordIndex++;

                        //sheet.Cells[recordIndex, 6].Value = "KDV";
                        //sheet.Cells[recordIndex, 6].Style.Font.Bold = true;
                        //sheet.Cells[recordIndex, 7].Value = genel_toplam * (decimal)0.18;
                        //sheet.Cells[recordIndex, 7].Style.Numberformat.Format = "#,##0.00";
                        //recordIndex++;

                        //sheet.Cells[recordIndex, 6].Value = "GENEL TOPLAM";
                        //sheet.Cells[recordIndex, 6].Style.Font.Bold = true;
                        //sheet.Cells[recordIndex, 7].Value = genel_toplam * (decimal)1.18;
                        //sheet.Cells[recordIndex, 7].Style.Numberformat.Format = "#,##0.00";
                        //recordIndex++;


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
                        sheet.Column(11).AutoFit();
                        sheet.Column(12).AutoFit();
                        sheet.Column(13).AutoFit();

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

        public string BuildPDFPage(int orderID)
        {
            string retval = string.Empty;

            var order = new OrderRepo().GetByID(orderID);
            if (order == null)
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
            sb.Append("		<td>Sipariş Kodu</td>");
            sb.Append("		<td>" + order.OrderNo + "</td>");
            sb.Append("	</tr>");
            sb.Append("	<tr>");
            sb.Append("		<td>Firma</td>");
            sb.Append("		<td>" + order.MUSTERILER?.FIRMA_ADI + "</td>");
            sb.Append("	</tr>");
            sb.Append("	<tr>");
            sb.Append("		<td>Yetkili</td>");
            sb.Append("		<td>" + order.YETKILI_KISILER?.ADI + " " + order.YETKILI_KISILER?.SOYADI + "</td>");
            sb.Append("	</tr>");
            sb.Append("	<tr>");
            sb.Append("		<td>Sipariş Tarihi</td>");
            sb.Append("		<td>" + order.OrderDate?.ToString("dd/MM/yyyy") + "</td>");
            sb.Append("	</tr>");
            sb.Append("	<tr>");
            sb.Append("		<td>Eklenme Tarihi</td>");
            sb.Append("		<td>" + order.CreatedOn?.ToString("dd/MM/yyyy") + "</td>");
            sb.Append("	</tr>");
            sb.Append("	<tr>");
            sb.Append("		<td>Ekleyen</td>");
            sb.Append("		<td>" + order.Member?.FirstName + " " + order.Member?.LastName + "</td>");
            sb.Append("	</tr>");
            sb.Append("<tr>");
            sb.Append("		<td>Güncellenme Tarihi</td>");
            sb.Append("		<td>" + order.UpdatedOn?.ToString("dd/MM/yyyy") + "</td>");
            sb.Append("	</tr>");
            sb.Append("<tr>");
            sb.Append("		<td>Güncelleyen</td>");
            sb.Append("		<td>" + order.Member1?.FirstName + " " + order.Member1?.LastName + "</td>");
            sb.Append("	</tr>");
            sb.Append("</table>");

            sb.Append("<table cellpadding=\"2\" cellspacing=\"0\" border=\"1\">");
            sb.Append("	 <thead>");
            sb.Append("		<tr>");
            sb.Append("			<th>Sıra No</th>");
            sb.Append("			<th style=\"width:100px\">Müşteri Ürün Kodu</th>");
            sb.Append("			<th style=\"width:100px\">BU Numarası</th>");
            //sb.Append("			<th>BU Eski</th>");
            sb.Append("			<th style=\"width:150px\">Product Name</th>");
            //sb.Append("			<th>NAME_DE</th>");
            sb.Append("			<th>Birim Fiyat</th>");
            //sb.Append("			<th>P.B.</th>");
            sb.Append("			<th>Adet</th>");
            sb.Append("			<th>Toplam</th>");
            sb.Append("			<th>P.B.</th>");
            //sb.Append("			<th>Güncelleyen</th>");
            //sb.Append("			<th>Güncellenme Tarihi</th>");
            sb.Append("		</tr>");
            sb.Append("	</thead>");
            sb.Append("	<tbody>");

            int lineno = 1;
            int adetToplam = 0;
            decimal EURToplam = 0;
            decimal USDToplam = 0;
            decimal TLToplam = 0;
            foreach (var item in order.ODetay.AsQueryable())
            {
                sb.Append("		<tr>");
                sb.Append("			<td>" + (lineno).ToString() + "</td>");
                sb.Append("			<td>" + item.CustomerCode + "</td>");
                sb.Append("			<td>" + item.BuCode + "</td>");
                //sb.Append("			<td>" + item.BUESKI + "</td>");
                sb.Append("			<td>" + (string.IsNullOrWhiteSpace(item.NAME_EN) ? item.NAME_EN : item.NAME_DE) + "</td>");
                // sb.Append("			<td>" + item.NAME_DE + "</td>");
                sb.Append("			<td style=\"text-align:right\">" + item.UnitPrice?.ToString("N2") + "</td>");
                //sb.Append("			<td>" + item.Currency?.Code + "</td>");
                sb.Append("			<td style=\"text-align:right\">" + item.Quantity.ToString() + "</td>");
                sb.Append("			<td style=\"text-align:right\">" + (item.Quantity * item.UnitPrice)?.ToString("N2") + "</td>");
                sb.Append("			<td>" + item.Currency?.Code + "</td>");
                //sb.Append("			<td>" + item.Member1?.FirstName + " " + item.Member1?.LastName + "</td>");
                //sb.Append("			<td>" + item.UpdatedOn?.ToString("dd/MM/yyyy") + "</td>");
                sb.Append("		</tr>");

                adetToplam += item.Quantity ?? 0;

                if (item.Currency.Code == "EURO")
                    EURToplam += (item.UnitPrice ?? 0) * (item.Quantity ?? 0);
                else if (item.Currency.Code == "USD")
                    USDToplam += (item.UnitPrice ?? 0) * (item.Quantity ?? 0);
                else if (item.Currency.Code == "TL")
                    TLToplam += (item.UnitPrice ?? 0) * (item.Quantity ?? 0);

                lineno++;
            }

            sb.Append("		<tr>");
            sb.Append("			<td colspan\"6\">&nbsp;</td>");
            sb.Append("			<td>" + adetToplam.ToString("N0") + "</td>");
            sb.Append("			<td>" + EURToplam.ToString("N2") + " EURO</td>");
            sb.Append("		</tr>");
            sb.Append("		<tr>");
            sb.Append("			<td colspan\"6\">&nbsp;</td>");
            sb.Append("			<td></td>");
            sb.Append("			<td>" + USDToplam.ToString("N2") + " USD</td>");
            sb.Append("		</tr>");
            sb.Append("		<tr>");
            sb.Append("			<td colspan\"6\">&nbsp;</td>");
            sb.Append("			<td></td>");
            sb.Append("			<td>" + TLToplam.ToString("N2") + " TL</td>");
            sb.Append("		</tr>");

            sb.Append("	</tbody>");
            sb.Append("</table>");

            //sb.Append("</body>");
            //sb.Append("</html>");

            #endregion

            retval = sb.ToString();

            return retval;
        }


        //public ActionResult pdftest()
        //{
        //    string html1 = BuildPDFPage(2728);

        //    html1 = @"<head><style> .test { background-color: linen; font-size:10px; color: maroon; }</style></head><body class=""test"">" + html1 + "</body>";

        //    return File(HtmlToPdfConvert(html1), "application/pdf");
        //}


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
