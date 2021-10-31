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
    public class UlkeModels
    {
    }

    public class UlkeDetailDTO
    {
        public int UID { get; set; }
    }

    public class UlkeDeleteDTO
    {
        public int UID { get; set; }
    }

    public class UlkeSaveDTO
    {
        public int? UID { get; set; }
        public string UNAME { get; set; }
    }
     
}
