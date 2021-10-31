using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nero2021.BLL.Models
{
    public class NotificationModels
    {
    }

    public class NotificationAddModel
    {
        public string ToMemberIDs { get; set; }
        public int? ToMemberID { get; set; }
        public int MemberID { get; set; }
        public string Message { get; set; }
        public string PushMessage { get; set; }
        public string RedirectLink { get; set; }
        public int? RedirectRefID { get; set; }
        public DateTime? SendingDate { get; set; }
    }

    public class NotificationListByMemberModel
    {
        public int? MemberID { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
}
