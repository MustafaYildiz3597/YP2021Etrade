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
    public class OEMSUPNAMEModels
    {
    }

    public class OEMSUPNAMEDetailDTO
    {
        public int SUPID { get; set; }
    }

    public class OEMSUPNAMEDeleteDTO
    {
        public int SUPID { get; set; }
    }

    public class OEMSUPNAMESaveDTO
    {
        public int? SUPID { get; set; }
        public string SUPNAME { get; set; }
    }
     
}
