using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Caching;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;

using Microsoft.AspNet.Identity.Owin;
 
using System.Configuration;
using System.IO;
 
using Nero2021.Data;
using Nero2021.BLL.Repository;

namespace Nero2021
{
    public sealed class AppHelper
    {
        public static List<GetUserPermissions_Result> GetUserPermissions()
        {
           return new UserLevelsRepo().GetUserPermissions();
        }

        //public static readonly UserManager userManager = UserManager.Create();

        //private static readonly AppHelper instance = new AppHelper();
        //public static AppHelper Instance
        //{
        //    get
        //    {
        //        return instance;
        //    }
        //}

        //public string MemberInfo
        //{
        //    get
        //    {
        //        return GetMemberInfo(HttpContext.Current.User.Identity.GetUserId());
        //    }
        //}

        //public void ResetMemberInfo()
        //{
        //    string userID = HttpContext.Current.User.Identity.GetUserId();

        //    if (!String.IsNullOrEmpty(userID))
        //    {
        //        if (HttpRuntime.Cache["MemberInfo"] != null)
        //        {
        //            Dictionary<string, string> cachedUsers = (Dictionary<string, string>)HttpRuntime.Cache["MemberInfo"];

        //            if (cachedUsers.ContainsKey(userID))
        //                cachedUsers.Remove(userID);
        //        }

        //        if (HttpRuntime.Cache["Account"] != null)
        //        {
        //            Dictionary<string, AccountModel> cachedUsers = (Dictionary<string, AccountModel>)HttpRuntime.Cache["Account"];

        //            if (cachedUsers.ContainsKey(userID))
        //            {
        //                cachedUsers.Remove(userID);
        //                // GetAccount(userID);
        //            }
        //        }

        //    }
        //}


        //public static string GetMemberInfo(string userID)
        //{
        //    if (HttpRuntime.Cache["MemberInfo"] == null)
        //    {
        //        // UserList is available until midnight...
        //        HttpRuntime.Cache.Insert("MemberInfo",
        //            new Dictionary<string, string>(),
        //            null,
        //            DateTime.UtcNow.AddDays(1),
        //            Cache.NoSlidingExpiration);
        //    }

        //    Dictionary<string, string> cachedUsers = (Dictionary<string, string>)HttpRuntime.Cache["MemberInfo"];
        //    string memberInfo = String.Empty;

        //    if (cachedUsers.ContainsKey(userID))
        //    {
        //        memberInfo = cachedUsers[userID];
        //    }
        //    else
        //    {
        //        var db = new TalentSpotDBEntities();
        //        if (!string.IsNullOrEmpty(userID))
        //        {
        //            var member = db.Member.Where(q => q.UserID == userID).FirstOrDefault();
        //            if (member != null)
        //            {
        //                var memberreport = db.MemberBirkmanImapReport.Where(q => q.MemberID == member.ID).OrderByDescending(o => o.ID).FirstOrDefault();
        //                string reportID = (memberreport == null) ? null : memberreport.ID.ToString();

        //                memberInfo = member.FirstName + "|" + member.LastName + "|" + member.AvatarPath + "|" + member.ID.ToString() + "|" + reportID;
        //            }
        //            else
        //                memberInfo = "|||";

        //            try
        //            {
        //                cachedUsers.Add(userID, memberInfo);
        //            }
        //            catch (Exception)
        //            {
        //                memberInfo = cachedUsers[userID];
        //            }
        //        }
        //    }

        //    return memberInfo;
        //}

        //public AccountModel Account
        //{
        //    get
        //    {
        //        return GetAccount(HttpContext.Current.User.Identity.GetUserId());
        //    }
        //}

        //public static void SetMemberInfo(string userID)
        //{
        //    if (HttpRuntime.Cache["MemberInfo"] != null)
        //    {
        //        Dictionary<string, string> cachedUsers = (Dictionary<string, string>)HttpRuntime.Cache["MemberInfo"];
        //        string memberInfo = String.Empty;

        //        if (cachedUsers.ContainsKey(userID))
        //        {
        //            var db = new TalentSpotDBEntities();
        //            var member = db.Member.Where(q => q.UserID == userID).FirstOrDefault();
        //            if (member != null)
        //            {
        //                var memberreport = db.MemberBirkmanImapReport.Where(q => q.MemberID == member.ID).OrderByDescending(o => o.ID).FirstOrDefault();
        //                string reportID = (memberreport == null) ? null : memberreport.ID.ToString();

        //                memberInfo = member.FirstName + "|" + member.LastName + "|" + member.AvatarPath + "|" + member.ID.ToString() + "|" + reportID;
        //            }
        //            else
        //                memberInfo = "|||";

        //            cachedUsers[userID] = memberInfo;
        //        }
        //    }
        //}

        //public static void SetTeam(int projectID, int teamID)
        //{
        //    AccountModel model = new AccountModel();

        //    if (HttpRuntime.Cache["Account"] == null)
        //        HttpRuntime.Cache.Insert("Account", new Dictionary<string, AccountModel>(), null, DateTime.UtcNow.AddDays(1), Cache.NoSlidingExpiration);

        //    using (TalentSpotDBEntities db = new TalentSpotDBEntities())
        //    {
        //        string userID = HttpContext.Current.User.Identity.GetUserId();
        //        int? teamcoachID = db.WorkTeam.Where(q => q.ID == teamID).FirstOrDefault()?.CoachMemberID;

        //        Dictionary<string, AccountModel> cachedUsers = (Dictionary<string, AccountModel>)HttpRuntime.Cache["Account"];
        //        if (cachedUsers.ContainsKey(userID))
        //        {
        //            cachedUsers[userID].ProjectID = projectID;
        //            cachedUsers[userID].TeamID = teamID;
        //            cachedUsers[userID].TeamCoachID = teamcoachID;
        //            cachedUsers[userID].ProjectID = projectID;
        //        }
        //        else
        //        {
        //            int memberID = new MemberRepo().GetIDByUSerID(userID);
        //            model.TenantID = new MemberRepo().GetTanentIDByMemberID(memberID);
        //            model.MemberID = memberID;
        //            model.TeamID = teamID;
        //            model.TeamCoachID = teamcoachID;
        //            model.ProjectID = projectID;

        //            cachedUsers.Add(userID, model);
        //        }
        //    }
        //}

        //public static AccountModel GetAccount(string userID)
        //{
        //    if (String.IsNullOrEmpty(userID))
        //        return null;

        //    AccountModel model = new AccountModel();

        //    if (HttpRuntime.Cache["Account"] == null)
        //        HttpRuntime.Cache.Insert("Account", new Dictionary<string, AccountModel>(), null, DateTime.UtcNow.AddDays(1), Cache.NoSlidingExpiration);

        //    Dictionary<string, AccountModel> cachedUsers = (Dictionary<string, AccountModel>)HttpRuntime.Cache["Account"];
        //    if (cachedUsers.ContainsKey(userID))
        //    {
        //        model = cachedUsers[userID];
        //    }
        //    else
        //    {
        //        // var member =  new MemberRepo().GetAll(q => q.UserID == userID).Select(s => new { s.ID, s.TenantID }).FirstOrDefault();
        //        using (TalentSpotDBEntities db = new TalentSpotDBEntities())
        //        {
        //            var member = (from m in db.Member.Where(q => q.UserID == userID)
        //                          select new
        //                          {
        //                              m.ID,
        //                              m.TenantID
        //                          }).FirstOrDefault();

        //            if (member == null)
        //                return null;

        //            // int memberID = ; //new MemberRepo().GetIDByUSerID(HttpContext.Current.User.Identity.GetUserId());
        //            model.MemberID = member.ID;
        //            model.TenantID = member.TenantID; // new MemberRepo().GetTanentIDByMemberID(memberID);

        //            if (HttpContext.Current.User.IsInRole("Participant"))
        //            {
        //                var workteammember = (from m in db.WorkTeamMember.Where(q => q.MemberID == member.ID && q.IsParticipant == true)
        //                                      select new
        //                                      {
        //                                          m.WorkTeamID
        //                                      }).FirstOrDefault();
        //                if(workteammember != null)
        //                {
        //                    var workteam = (from t in db.WorkTeam.Where(q => q.ID == workteammember.WorkTeamID)
        //                                    select new
        //                                    {
        //                                        t.ID,
        //                                        t.ProjectID,
        //                                        t.CoachMemberID
        //                                    }).FirstOrDefault();

        //                    if (workteam != null)
        //                    {
        //                        model.TeamID = workteam.ID;
        //                        model.TeamCoachID = workteam.CoachMemberID;
        //                        model.ProjectID = workteam.ProjectID;
        //                    }
        //                }
        //            }
        //            else if (HttpContext.Current.User.IsInRole("TeamCoach"))
        //            {
        //                #region teamID  

        //                // WorkTeam workteam = new WorkTeamRepo().GetAll(q => q.CoachMemberID == model.MemberID)?.FirstOrDefault();

        //                var workteam = (from t in db.WorkTeam.Where(q => q.CoachMemberID == member.ID)
        //                                select new
        //                                {
        //                                    t.ID,
        //                                    t.ProjectID,
        //                                    t.CoachMemberID
        //                                }).FirstOrDefault();
        //                if (workteam != null)
        //                {
        //                    model.TeamID = workteam.ID;
        //                    model.TeamCoachID = workteam.CoachMemberID;
        //                    model.ProjectID = workteam.ProjectID;
        //                }
        //                #endregion
        //            }
        //            //else if (HttpContext.Current.User.IsInRole("Observer"))
        //            //{
        //            //    var project = (from pm in db.ProjectMember.Where(q => q.MemberID == member.ID && q.Role == "OBS")
        //            //                   select new
        //            //                   {
        //            //                       pm.ProjectID
        //            //                   }).FirstOrDefault();



        //            //}

        //        }

        //        cachedUsers.Add(userID, model);

        //    }

        //    return model;
        //}

        //public class AccountModel
        //{
        //    public int MemberID { get; set; }
        //    public int? ProjectID { get; set; }
        //    public int? TeamID { get; set; }
        //    public int? TeamCoachID { get; set; }
        //    public int TenantID { get; set; }
        //}

        ////public List<Project> Project
        ////{
        ////    get
        ////    {
        ////        return GetProjectList(HttpContext.Current.User.Identity.GetUserId());
        ////    } 
        ////}

        ////public static List<Project> GetProjectList(string userID)
        ////{
        ////    int memberID 


        ////    //ApplicationUserManager UserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
        ////    //var roles = UserManager.GetRoles(userID);
        ////    var db = new TalentSpotDBEntities();
        ////    var list = from p in db.ProjectMember.Where(q => q.MemberID == memberID);


        ////}

        //public string Firm
        //{
        //    get
        //    {
        //        return GetFirm(HttpContext.Current.User.Identity.GetUserId());
        //    }
        //}
        //public static string GetFirm(string userId)
        //{
        //    string firmName = String.Empty;

        //    if (!string.IsNullOrEmpty(userId))
        //    {
        //        using (TalentSpotDBEntities db = new TalentSpotDBEntities())
        //        {
        //            try
        //            {
        //                //var firmakullanici = (from f in db.Firma
        //                //                      join fk in db.FirmaKullanici.Where(q => q.KullaniciID == userId) on f.FirmaID equals fk.FirmaID
        //                //                      select new {
        //                //                          f.FirmaAck
        //                //                      }).FirstOrDefault();

        //                //if(firmakullanici!=null)
        //                //{
        //                //    firmName = firmakullanici.FirmaAck;
        //                //}

        //                //var user = db.AspNetUsers.Where(u => u.UserName == userName).FirstOrDefault();

        //                //if (user != null)
        //                //{
        //                //    var firm = user.FirmaKullanici.FirstOrDefault();
        //                //    firmName = (firm == null) ? null : user.FirmaKullanici.FirstOrDefault().Firma;
        //                //}
        //            }
        //            catch (Exception ex)
        //            {
        //                Console.Write(ex.Message);

        //            }
        //        }
        //    }



        //    //if (HttpRuntime.Cache["FirmList"] == null)
        //    //{
        //    //    // UserList is available until midnight...
        //    //    HttpRuntime.Cache.Insert("FirmList",
        //    //        new Dictionary<string, string>(),
        //    //        null,
        //    //        DateTime.UtcNow.AddDays(1),
        //    //        Cache.NoSlidingExpiration);
        //    //}

        //    //Dictionary<string, string> cachedUsers = (Dictionary<string, string>)HttpRuntime.Cache["FirmList"];
        //    //string firmName = String.Empty;

        //    //if (cachedUsers.ContainsKey(userName))
        //    //{
        //    //    firmName = cachedUsers[userName];
        //    //}
        //    //else
        //    //{
        //    //    var db = new RWLojistikDBEntities();
        //    //    if (!string.IsNullOrEmpty(userName))
        //    //    {
        //    //        var user = db.AspNetUsers.Where(u => u.UserName == userName).FirstOrDefault();

        //    //        if (user != null)
        //    //        {
        //    //            var firm = user.FirmaKullanici.FirstOrDefault();
        //    //            firmName = (firm == null) ? null : user.FirmaKullanici.FirstOrDefault().Firma;
        //    //        }

        //    //        // firmName = db.FirmaKullanicis.Where(f => f.KullaniciID == userId).FirstOrDefault().Firma; 
        //    //        cachedUsers.Add(userName, firmName);
        //    //    }
        //    //}

        //    return firmName;
        //}

        //public string Role
        //{
        //    get
        //    {
        //        return GetRole(HttpContext.Current.User.Identity.Name);
        //    }
        //}

        //public static string GetRole(string userName)
        //{
        //    if (HttpRuntime.Cache["RoleList"] == null)
        //    {
        //        // UserList is available until midnight...
        //        HttpRuntime.Cache.Insert("RoleList",
        //            new Dictionary<string, string>(),
        //            null,
        //            DateTime.UtcNow.AddDays(1),
        //            Cache.NoSlidingExpiration);
        //    }

        //    Dictionary<string, string> cachedUsers = (Dictionary<string, string>)HttpRuntime.Cache["RoleList"];
        //    string roleName = "";

        //    if (cachedUsers.ContainsKey(userName))
        //    {
        //        roleName = cachedUsers[userName];
        //    }
        //    else
        //    {
        //        var db = new TalentSpotDBEntities();
        //        if (!string.IsNullOrEmpty(userName))
        //        {
        //            // var user = db.AspNetUsers.Where(u => u.UserName == userName).FirstOrDefault();

        //            string userID = HttpContext.Current.User.Identity.GetUserId();

        //            //var user2 = userManager.FindByEmail(userName);

        //            ApplicationUserManager UserManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();


        //            var roles = UserManager.GetRoles(userID);
        //            //if (roles.Contains("SysAdmin"))
        //            //{

        //            //}

        //            if (roles.Count() > 0)
        //                roleName = roles[0];

        //            //if (user != null)
        //            //{
        //            //    var userroles = user.AspNetUserRoles.FirstOrDefault();

        //            //    if (userroles != null)
        //            //    {
        //            //        roleName = userroles.AspNetRole.Name;
        //            //    }
        //            //}

        //            cachedUsers.Add(userName, roleName);
        //        }
        //    }

        //    if (string.IsNullOrEmpty(roleName))
        //    {
        //        roleName = ""; // userName;
        //    }

        //    return roleName;
        //}


        ////public static string appVersion = ReadSetting("VersionNumber");
        //public static string ReadSetting(string key)
        //{
        //    try
        //    {
        //        var appSettings = ConfigurationManager.AppSettings;
        //        string result = appSettings[key] ?? "Not Found";
        //        // Console.WriteLine(result);
        //        return result;
        //    }
        //    catch (ConfigurationErrorsException)
        //    {
        //        return "";
        //        //Console.WriteLine("Error reading app settings");
        //    }

        //}

        //public static string GetBase64String(string path)
        //{
        //    try
        //    {

        //        var server_path = HttpContext.Current.Server.MapPath(path);
        //        var plainTextBytes = File.ReadAllBytes(server_path);
        //        return System.Convert.ToBase64String(plainTextBytes);
        //    }
        //    catch (ConfigurationErrorsException)
        //    {
        //        return "";

        //    }

        //}

    }



}