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
    public class MeetingModels
    {
    }

    public class MeetingDTListDTO
    {
        public int? FirmID { get; set; }
    }

    public class MeetingIDDTO
    {
        public int? ID { get; set; }
    }


    public class MeetingSaveDTO
    {
        public int? ID { get; set; }
        public int FIRMID { get; set; }
        public string TITLE { get; set; }
        public int? SEBEP { get; set; }
        public string ICERIK { get; set; }
        public List<ToplantiMembersItemDTO> ToplantiMemberList { get; set; }
        public List<ToplantiYetkiliKisilerItemDTO> ToplantiYetkiliKisilerLlist { get; set; }
    }

    public class ToplantiMembersItemDTO
    {
        public string ID { get; set; }
        //public string MemberID { get; set; }
    }

    public class ToplantiYetkiliKisilerItemDTO
    {
        public int? ID { get; set; }
        //public int YetkiliKisiID { get; set; }
    }

}
