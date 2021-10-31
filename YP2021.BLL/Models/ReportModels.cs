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
    public class ReportModels
    {
    }

    public class RangeDateTimeModel
    {
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
    }


    public class TespitCozumAdetModel
    {
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
    }

    public class SubeTespitAdetModel
    {
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
    }

    public class SubeCozumAdetModel
    {
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
    }

    public class UyeBazliModel
    {
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
        public int? MemberID { get; set; }
    }


    public class DashboardModel
    {
        public string FirmCode { get; set; }
        public string PeriodCode { get; set; }
        public int? SalesmanRef { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class NagatifStokModel
    {
        public string FirmCode { get; set; }
        public string PeriodCode { get; set; }
    }

    public class KDVListModel
    {
        public string FirmCode { get; set; }
        public string PeriodCode { get; set; }
        public int? SalesmanRef { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class KarlilikListModel
    {
        public string FirmCode { get; set; }
        public string PeriodCode { get; set; }
        public int? SalesmanRef { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class IndirimYuzdeleriModel
    {
        public string FirmCode { get; set; }
        public string PeriodCode { get; set; }
        public int? SalesmanRef { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class AcikKalanFaturalarListModel
    {
        public string FirmCode { get; set; }
        public string PeriodCode { get; set; }
        public int? SalesmanRef { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? KacgungectiMin { get; set; }
        public int? KacgungectiMax { get; set; }
    }


    public class KapanmisFaturalarListModel
    {
        public string FirmCode { get; set; }
        public string PeriodCode { get; set; }
        public int? SalesmanRef { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? KacgungectiMin { get; set; }
        public int? KacgungectiMax { get; set; }
    }


    public class OdemesiGecenlerListModel
    {
        public string FirmCode { get; set; }
        public string PeriodCode { get; set; }
        public int? SalesmanRef { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }


    public class UrunSatisAdetModel
    {
        public string FirmCode { get; set; }
        public string PeriodCode { get; set; }
        public int? SalesmanRef { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UrunSatisTutarModel
    {
        public string FirmCode { get; set; }
        public string PeriodCode { get; set; }
        public int? SalesmanRef { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class STAylikSatislarModel
    {
        public string FirmCode { get; set; }
        public string PeriodCode { get; set; }
        public int? SalesmanRef { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class Top10UrunSatisAdetModel
    {
        public string FirmCode { get; set; }
        public string PeriodCode { get; set; }
        public int? SalesmanRef { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class Top10UrunSatisTutarModel
    {
        public string FirmCode { get; set; }
        public string PeriodCode { get; set; }
        public int? SalesmanRef { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class FaturaListesiModel
    {
        public string FirmCode { get; set; }
        public string PeriodCode { get; set; }
        public int? SalesmanRef { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
    }


}
