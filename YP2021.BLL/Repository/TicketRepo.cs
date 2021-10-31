using Microsoft.AspNet.Identity;
using Nero2021.BLL.Models;
using Nero2021.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nero2021.BLL.Repository
{
    public class TicketRepo : RepositoryBase<Ticket, int>
    {
        private NeroDBEntities db = new NeroDBEntities();

        public IQueryable ListDT(TicketListDTO model)
        {
            try
            {
                string userId = HttpContext.Current.User.Identity.GetUserId();

                return (from t in db.Ticket.Where(q => (q.IsDeleted ?? false) == false &&                        
                        (q.TypeID == model.TypeID || model.TypeID == null) &&
                        (q.PriorityID == model.PriorityID || model.PriorityID == null) &&
                        (q.StatusID == model.StatusID || model.StatusID == null) 
                        )
                        join tm in db.TicketMember.Where(q => q.MemberID == userId && (q.IsDeleted ?? false) == false && 
                            (q.Direction == (model.Direction ?? 1) ) &&
                            ( (q.IsArchived ?? false) == (model.IsArchived ?? false)) 
                        ) on t.ID equals tm.TicketID
                        select new {
                            t.ID,
                            t.MusteriID,
                            FIRMA_ADI = t.MUSTERILER == null ? "" : t.MUSTERILER.FIRMA_ADI,
                            t.StatusID,
                            Status = t.TicketStatus == null ? "" : t.TicketStatus.Name,
                            t.PriorityID,
                            Priority = t.TicketPriority == null ? "" : t.TicketPriority.Name,
                            t.TypeID,
                            Type = t.TicketType == null ? "" : t.TicketType.Name,
                            t.Title,
                            t.CreatedOn,
                            CreatedBy = t.Member1 == null ? "" : t.Member1.FirstName + " " + t.Member1.LastName,
                            LastReplayedMember = (t.Member2 == null ? "" : t.Member2.FirstName + " " + t.Member2.LastName),
                            LastReplayedOn = t.LastRepliedOn,
                            tm.IsArchived,
                            tm.IsRead,
                            UnreadCount = tm.UnreadCount ?? 0
                        }).AsQueryable();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Object FindByID(int id)
        {
            try
            {
                return (from t in db.Ticket.Where(q => q.ID == id && (q.IsDeleted ?? false) == false)
                        select new {
                            t.ID,
                            CreatedBy = t.Member1 == null ? "" : (t.Member1.FirstName + " " + t.Member1.LastName),
                            t.CreatedOn,
                            t.DetailText,
                            t.LastRepliedBy,
                            t.LastRepliedOn,
                            FIRMA_ADI = t.MUSTERILER == null ? "" : t.MUSTERILER.FIRMA_ADI,
                            PriorityName = t.TicketPriority == null ? "" : t.TicketPriority.Name,
                            StatusName = t.TicketStatus == null ? "" : t.TicketStatus.Name,
                            TypeName = t.TicketType == null ? "" : t.TicketType.Name,
                            t.Title,
                            Members = t.TicketMember.Where(q => q.TicketID == t.ID && q.Direction == 2).Select(s => new { FullName = s.Member.FirstName + " " + s.Member.LastName }),
                            TicketReplies = t.TicketReply.Where(q => q.TicketID == t.ID).Select(s => new { s.Message, s.FilePath, s.FileType, s.FileName, FullName = (s.Member == null ? "" : s.Member.FirstName + " " + s.Member.LastName), s.CreatedOn })   
                        }).FirstOrDefault();
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
