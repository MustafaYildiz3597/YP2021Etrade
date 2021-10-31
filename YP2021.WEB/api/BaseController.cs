using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace Nero2021.api
{
    public class BaseController : ApiController
    {
       

        //public static void WriteToLog(string messagetext)
        //{

        //    string sqlconnstr = "Server=test.kep-ik.com;Database=ilekacom_kepiktestdb;User Id=ilekacom_kepiktestuser;Password=0Am!d96d;";
        //    //"data source=217.195.203.126;initial catalog=DuelsKingsDB;user id=duelskingsusr;password=bjeW!Wvy?w4L9fSD"
        //    using (SqlConnection openCon = new SqlConnection(sqlconnstr))
        //    {
        //        string saveStaff = "INSERT into Tmp_Log ([ResultText]) VALUES (@ResultText)";

        //        using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
        //        {
        //            querySaveStaff.Connection = openCon;
        //            querySaveStaff.Parameters.Add("@ResultText", SqlDbType.NVarChar, 8000).Value = messagetext;
        //            //querySaveStaff.Parameters.Add("@Text1", SqlDbType.NVarChar, 4000).Value = model.Message;
        //            //querySaveStaff.Parameters.Add("@Text2", SqlDbType.NVarChar, 4000).Value = model.Type;
        //            //querySaveStaff.Parameters.Add("@fbuid", SqlDbType.NVarChar, 64).Value = model.fbuid;

        //            openCon.Open();

        //            querySaveStaff.ExecuteNonQuery();
        //        }
        //    }
        //}
    }
}