using Nero.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nero.BLL.Models
{
    public class DetectionModels
    {
    }

    public class DetectionFollowingModel
    {
        public int? MemberID { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }

    public class DetectionListModel
    {
        public int? MemberID { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
    public class DetectionListByMemberModel
    {
        public int? MemberID { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
    }
    public class DetectionListByKeywordModel
    {
        public String Keyword { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public int? MemberID { get; set; }
    }

    public class DetectionGetModel
    {
        public int DetectionID { get; set; }
        public int? MemberID { get; set; }
    }
    public class SaveDetectionModel
    {
        // public Detection Detection { get; set; }
        public int? ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? SubjectID { get; set; }
        public int? MemberID { get; set; }
        //public byte[] ImageFile { get; set; }
        //public byte[] VideoFile { get; set; }

        public string ImagePath { get; set; }
        public string VideoPath { get; set; }
        //public HttpPostedFileBase ImageFile { get; set; }
        //public HttpPostedFileBase VideoFile { get; set; }
    }
    public class DeleteDetectionModel
    {
        public int DetectionID { get; set; }
        public int MemberID { get; set; }
    }
    public class DetectionLikeModel
    {
        public int DetectionID { get; set; }
        public int MemberID { get; set; }
    }
    
    public class UploadFilesModel
    {
        public string ImageFile { get; set; }
    }

    public class Admin_DetectionSaveModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? SubjectID { get; set; }
        public int? ExtraPoint { get; set; }
        public bool? IsActive { get; set; }
    }

}
