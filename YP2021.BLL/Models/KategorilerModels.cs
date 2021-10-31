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
    public class KategorilerModels
    {
    }

    public class KategoriListDTDTO
    {
        public int? SECID { get; set; }
        public int? MCATID { get; set; }
    }

    public class KategoriDetailDTO
    {
        public int CATID { get; set; }
    }

    public class KategoriDeleteDTO
    {
        public int CATID { get; set; }
    }

    public class KategoriSaveDTO
    {
        public int? CATID { get; set; }    
        public int? MCATID { get; set; }
        public int? SECID { get; set; }
        public string NAME { get; set; }
        public string NAME_EN { get; set; }
        public string NAME_DE { get; set; }
        public string NAME_FR { get; set; }
        public bool? ENABLED { get; set; }
        public int? SORT { get; set; }
    }

}
