using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nero2021.BLL.Models
{
    public class RegistrationModels
    {
    }

    public class CheckUserModel
    {
        public string RegisterNumber { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class SetUserNameModel
    {
        public string RegisterNumber { get; set; }
        public string Username { get; set; }
    }
}
