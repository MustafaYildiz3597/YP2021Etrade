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
    public class EmployeeModels
    {
    }
      
    public class EmployeeDetailRQDTO
    {
        public string ID { get; set; }
    }

    public class EmployeeSaveDTO
    {
        public string ID { get; set; }
        public string FirmID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TCIdentityNo { get; set; }
        public string KEPAddress { get; set; }
        public string ContactEmail { get; set; }
        public string KEPEmail { get; set; }
        public bool? KEPStatus { get; set; }
        public bool? ApprovalStatus { get; set; }
        public string SEPAddress { get; set; }
        public string GSMNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class TransferEmployeeFromExcelDTO
    {
        public string FirmID { get; set; }
        public List<TransferEmployeeFromExcelRow> ExcelRows { get; set; }
        public string FileName { get; set; }
    }

    public class TransferEmployeeFromExcelRow
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string TCIdentityNo { get; set; }
        public string KEPAddress { get; set; }
        public string GSMNumber { get; set; }
        public string Birim { get; set; }
        public string EMailAddress { get; set; }
        public string BeginDate { get; set; }
    }

    public class BulkPayrollListDTO
    {
        public Guid EmployeeID { get; set; }
        public string TCKno { get; set; }

        public Guid BordroEmployeeID { get; set; }
        public bool? BordroEmployeeIsActive { get; set; }
        public bool? BordroEmployeeIsDeleted { get; set; }

        //public Guid NoMatchedBordroEmployeeID { get; set; }
        //public string NoMatchedParsedTCIdentityNo { get; set; }
    }

    public class NoMatchedPayrollList
    {
        public Guid BordroEmployeeID { get; set; }
        public string ParsedTCKno { get; set; }
    }
}
