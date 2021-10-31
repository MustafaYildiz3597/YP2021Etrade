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
    public class AnaKategorilerModels
    {
    }

    public class AnaKategoriListDTDTO
    {
        public int? SECID { get; set; }
    }

    public class AnaKategoriDetailDTO
    {
        public int MCATID { get; set; }
    }

    public class AnaKategoriDeleteDTO
    {
        public int MCATID { get; set; }
    }

    public class AnaKategoriSaveDTO
    {
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
