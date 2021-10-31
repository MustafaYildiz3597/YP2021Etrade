using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nero2021.BLL.Models
{
    public class MessageModels
    {
    }

    public enum MessageTypes
    {
        [Description("DetectionMessage")]
        DetectionMessage = 1,
        [Description("Contact")]
        Contact = 2,
        [Description("AdminMessage")]
        AdminMessage = 3,
    }

    public class MessageAddModel
    {
        public int TypeID { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public int FromMemberID { get; set; }
        public int ToMemberID { get; set; }
        public int? DetectionID { get; set; }
    }

    public class MessageListByMemberModel
    {
        public int? MemberID { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

    public class DeleteMessageModel
    {
        // public MessageIDListModel MessageIDList { get; set; }
        public int MessageID { get; set; }
        public int MemberID { get; set; }
    }

    public class ListAllModel
    {
        public int MessageTypeID { get; set; }
    }

    public class SendUsernameModel
    {
        public string ToUsername { get; set; }
        public string FromUsername { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
    }

    public class SendMessageToMemberIdModel
    {
        public int MemberID { get; set; }
        public string FromUsername { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
    }
}
