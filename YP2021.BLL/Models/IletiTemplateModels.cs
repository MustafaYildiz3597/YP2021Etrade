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

    public class SelectionGetDTO
    {
        public Guid ID { get; set; }  
    }

    public class IletiTemplateDetailDTO
    {
        public string ID { get; set; }
    }

    public class IletiTemplateSaveDTO
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}
