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
    public class SehirModels
    {
    }

    public class SehirListDTDTO
    {
        public int? UID { get; set; }
    }

    public class SehirDetailDTO
    {
        public int ID { get; set; }
    }

    public class SehirDeleteDTO
    {
        public int ID { get; set; }
    }

    public class SehirSaveDTO
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public int? UID { get; set; }
    }

}
