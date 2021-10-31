using Nero2021.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nero2021.BLL.Models
{
    public class KepikModels
    {
    }

    public class StartBulkPDFParsingDTO
    {
        public HttpPostedFileBase Attachment { get; set; }
        public HttpPostedFileBase Attachment2 { get; set; }
        public HttpPostedFileBase Attachment3 { get; set; }
        public HttpPostedFileBase Attachment4 { get; set; }
        public HttpPostedFileBase Attachment5 { get; set; }
        public int? SigningType { get; set; }
        public string FirmID { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string PdfFileData { get; set; }
        public string PdfFileName { get; set; }
    }

    public class StartBordroSendingsDTO
    {
        public string sid { get; set; }
        public string tokenid { get; set; }
    }
}
