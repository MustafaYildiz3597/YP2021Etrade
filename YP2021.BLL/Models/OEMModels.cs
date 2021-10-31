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
    public class OEMModels
    {
    }

    public class OEMImportExcelUploadDataDTO
    {
        public List<OEMImportExcelUploadingDataDTO> Data { get; set; }
    }

    public class OEMImportExcelUploadingDataDTO
    {
        public string BUKOD { get; set; }
        public string OEMNR { get; set; }
        public string SUPID { get; set; }
    }


}
