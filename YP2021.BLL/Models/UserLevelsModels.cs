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
    public class UserLevelsModels
    {
    }

    public class UserLevelDetailDTO
    {
        public int ID { get; set; }
    }


    public class UserLevelDeleteDTO
    {
        public int ID { get; set; }
    }

    public class UserLevelSaveDTO
    {
        public int? UserLevelID { get; set; }
        public string UserLevelName { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UserLevelGetPermissionsDTO
    {
        public int? UserLevelID { get; set; }
    }

    public class UserLevelSetPermissionsDTO
    {
        public int? UserLevelID { get; set; }
        public List<UserLevelPermissionDTO> UserLevelPermissions { get; set; }
    }

    public class UserLevelPermissionDTO
    {
        public int? ID { get; set; }
        public bool? ViewPermission { get; set; }
        public bool? AddPermission { get; set; }
        public bool? UpdatePermission { get; set; }
        public bool? DeletePermission { get; set; }
        public bool? SearchPermission { get; set; }
        public bool? ExecutePermission { get; set; }
    }
}
