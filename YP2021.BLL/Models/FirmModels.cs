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
    public class FirmModels
    {
    }
      
    public class DetailRQDTO
    {
        public string ID { get; set; }
    }

    public class SaveDTO
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ContactName { get; set; }
        public string ContactGSMNo { get; set; }
        public string ContactEmail { get; set; }
        public string KEPEmail { get; set; }
        public string PhoneNo { get; set; }
        public string TaxNumber { get; set; }
        public string TaxOffice { get; set; }
        public int KEPMemberMaxLimit { get; set; }
        public DateTime KEPExpirationDT2 { get; set; }
        public bool? KEPStatus { get; set; }
        public string Password { get; set; }
    }
}
