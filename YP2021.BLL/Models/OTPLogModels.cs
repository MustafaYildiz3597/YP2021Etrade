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
    public class OTPLogModels
    {
    }

    public enum OTPLogTypes
    {
        [Description("İleti Employee")]
        IletiEmployee = 1,
        [Description("Bordro Employee")]
        BordroEmployee = 2,
        [Description("Remember Password")]
        RememberPassword = 3,

    }
}
