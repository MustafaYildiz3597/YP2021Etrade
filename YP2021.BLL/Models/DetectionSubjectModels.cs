using Nero.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nero.BLL.Models
{
    public class DetectionSubjectModels
    {
    }

    public class DetectionSubjectSaveModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public bool? IsActive { get; set; }
        public int? SubjectIDToBeTransferred { get; set; }
    }
    public class DetectionSubjectDeleteModel
    {
        public int ID { get; set; }
        public int? SubjectIDToBeTransferred { get; set; }
    }
}
