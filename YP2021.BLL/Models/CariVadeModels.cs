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
    public class CariVadeModels
    {
    }

    public class CariVadeDetailDTO
    {
        public int ID { get; set; }
    }

    public class CariVadeDeleteDTO
    {
        public int ID { get; set; }
    }

    public class CariVadeSaveDTO
    {
        public int? ID { get; set; }
        public string Name { get; set; }
        public int? RankNumber { get; set; }
        public bool? IsActive { get; set; }
    }
     
}
