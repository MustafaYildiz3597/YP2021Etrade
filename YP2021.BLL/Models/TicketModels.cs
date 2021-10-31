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
    public class TicketModels
    {
    }

    public class TicketListDTO
    {
        public int? Direction { get; set; }
        public bool? IsArchived { get; set; }
        public int? TypeID { get; set; }
        public int? PriorityID { get; set; }
        public int? StatusID { get; set; }
    }

    public class SaveTicketDTO
    {
        public int? ID { get; set; }
        public int? TypeID { get; set; }
        //public int? MusteriID { get; set; }
        public string Title { get; set; }
        public string DetailText { get; set; }
        public int? PriorityID { get; set; }
        public int? StatusID { get; set; }
        public SaveTicketSelectedFirm SelectedFirm { get; set; }
        public List<SaveTicketSelectedMember> SelectedMembers { get; set; }
    }

    public class TicketDetailDTO
    {
        public int? ID { get; set; }
    }

    public class SaveTicketSelectedFirm
    {
        public int? ID { get; set; }
    }

    public class SaveTicketSelectedMember
    {
        public string ID { get; set; }
    }

    public class SendTicketMemberToArchiveDTO
    {
        public int? TicketID { get; set; }
    }
    public class TakeBackTicketMemberFromArchiveDTO
    {
        public int? TicketID { get; set; }
    }

    public class TicketSaveReplyDTO
    {
        public int? TicketID { get; set; }
        //public string FilePath { get; set; }
        public string FileName { get; set; }
        public string DocumentData { get; set; }
        public string DocumentType { get; set; }
        public string Message { get; set; }
    }

}
