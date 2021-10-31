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
    public enum FirmaTipleri
    {
        [Description("Müşteri")]
        Müşteri = 1,
        [Description("Potansiyel Müşteri")]
        PotansiyelMüşteri = 2,
        [Description("Tedarikçi")]
        Tedarikçi = 3,
        [Description("Potansiyel Tedarikçi")]
        PotansiyelTedarikçi = 4,
        [Description("Müşteri ve Tedarikçi")]
        MüşteriVeTedarikçi = 5,
    }
     
    public class FIRMA_TIPLERIModels
    {
    }
     
}
