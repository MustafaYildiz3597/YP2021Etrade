using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;
using System.Data;
using System.Configuration;

using System.Net.Mail;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Nero2021.BLL.Repository;
using System.Web.Script.Serialization;
using System.Web;
using SmsApi.Types;
using System.Data.SqlClient;

namespace Nero2021.BLL.Utilities
{
    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;
        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }
        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }
        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;

            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }

            return base.VisitParameter(p);
        }
    }
    public static class Utility
    {
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }


        public static bool SendSMS(string gsmnumber, string smscontent, SmsApi.Messenger request)
        {
            bool retval = false;

            try
            {
                var smsByMsisdnInfo = new SmsByMsisdnInfo { Msisdn = Convert.ToInt64(gsmnumber) };

                var submitInstantlyRequest = new InstantSms();
                submitInstantlyRequest.SenderName = "iLEKAAKADMi";
                submitInstantlyRequest.DataCoding = DataCoding.Default;
                submitInstantlyRequest.SmsContent = smscontent;
                submitInstantlyRequest.ToMsisdns = new SmsByMsisdnInfo[] { smsByMsisdnInfo };

                long messageID = 0;
                var submitInstantlyResponse = request.SubmitInstantly(submitInstantlyRequest);

                Console.WriteLine("client.SubmitInstantly() ->");
                Console.WriteLine("{0}, {1}", submitInstantlyResponse.Status.Code, submitInstantlyResponse.Status.Description);
                if (submitInstantlyResponse.Status.Code == 200)
                {
                    messageID = submitInstantlyResponse.MessageID;
                    Console.WriteLine("MessageID:{0}", submitInstantlyResponse.MessageID);
                    // loglama yapılacak.

                    retval = true;
                }
                else
                {
                    Console.WriteLine("{0}, {1}", submitInstantlyResponse.Status.Message, submitInstantlyResponse.Status.Description);
                    // loglama yapılacak.
                }

            }
            catch (Exception ex)
            {
                //TODO: add to log file
            }

            return retval;
        }

        public static bool SendMail(string mailTo, string mailcc, string msgSubject, string msgBody)
        {
            bool retval = false;

            var smtphost = ConfigurationManager.AppSettings["smtphost"];
            var smtpport = ConfigurationManager.AppSettings["smtpport"];

            var credentialUserName = ConfigurationManager.AppSettings["mailfrom"];
            var mailfrom = ConfigurationManager.AppSettings["mailfrom"];
            var mailpass = ConfigurationManager.AppSettings["mailpass"];
            //mailTo = ConfigurationManager.AppSettings["mailto"];
            var mailCC = ConfigurationManager.AppSettings["mailcc"];

            try
            {
                SmtpClient sc = new SmtpClient();
                sc.Port = Convert.ToInt32(smtpport);
                sc.Host = smtphost; // "smtp.gmail.com";
                sc.EnableSsl = true;

                sc.Credentials = new NetworkCredential(mailfrom, mailpass);

                MailMessage mail = new MailMessage();

                mail.From = new MailAddress(mailfrom, mailfrom);

                mail.To.Add(mailTo);
                // mail.To.Add("alici2@mail.com");

                //mail.CC.Add(mailCC);
                // mail.CC.Add("alici4@mail.com");

                mail.Subject = msgSubject;
                mail.IsBodyHtml = true;
                mail.Body = msgBody;

                //mail.Attachments.Add(new Attachment(@"C:\Rapor.xlsx"));
                //mail.Attachments.Add(new Attachment(@"C:\Sonuc.pptx"));

                sc.Send(mail);

                retval = true;
                return retval;
            }
            catch (Exception ex)
            {
                return retval;
            }

            //try
            //{
            //    var credentialUserName = ConfigurationManager.AppSettings["mailfrom"];
            //    var mailfrom = ConfigurationManager.AppSettings["mailfrom"];
            //    var pwd = ConfigurationManager.AppSettings["mailpass"];
            //    mailTo = ConfigurationManager.AppSettings["mailto"];

            //    // Configure the client:
            //    //System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("mail.gmail.com");
            //    System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(smtphost);

            //    client.Port = 587;
            //    client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            //    client.UseDefaultCredentials = false;

            //    // Create the credentials:
            //    System.Net.NetworkCredential credentials =
            //        new System.Net.NetworkCredential(credentialUserName, pwd);

            //    client.EnableSsl = true;
            //    client.Credentials = credentials;

            //    // Create the message:
            //    var mail = new System.Net.Mail.MailMessage(mailfrom, mailTo);

            //    if (!string.IsNullOrEmpty(mailcc))
            //        mail.CC.Add(mailcc);

            //    mail.IsBodyHtml = true;
            //    mail.Subject = msgSubject;
            //    mail.Body = msgBody;

            //    // Send:
            //    client.Send(mail);

            //    //retval.Message = result;
            //    //retval.Result = true;

            //    //return retval;
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}

        }

        public static string GenerateOTP(int passwordlength)
        {
            string alphabets = ""; // "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string small_alphabets = "";  // "abcdefghijklmnopqrstuvwxyz";
            string numbers = "1234567890";

            string characters = numbers;
            //if (rbType.SelectedItem.Value == "1")
            //{
            characters += alphabets + small_alphabets + numbers;
            //}
            int length = passwordlength;
            string otp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }
            return otp;
        }

        public static string GeneratePassword(int passwordlength)
        {
            string alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string small_alphabets = "abcdefghijklmnopqrstuvwxyz";
            string numbers = "1234567890";

            string characters = numbers;
            //if (rbType.SelectedItem.Value == "1")
            //{
            characters += alphabets + small_alphabets + numbers;
            //}
            int length = passwordlength;
            string otp = string.Empty;
            for (int i = 0; i < length; i++)
            {
                string character = string.Empty;
                do
                {
                    int index = new Random().Next(0, characters.Length);
                    character = characters.ToCharArray()[index].ToString();
                } while (otp.IndexOf(character) != -1);
                otp += character;
            }
            return otp;
        }


        public static string pushGonderAndroid(string deviceID, string Smessage, int tip)
        {
            try
            {
                string SERVER_API_KEY = "AAAA6cmc6VI:APA91bHLhouvLlWKUN539OEiknOEa9VUbZUWXq4KnEjnHn86ZlY_bXnX6FZ6GinrcSqX_uNqueNZejv00pLsFhm2TK1EAoI21zhFHACJe3CSF6wrH5SS28Fn-Eib4tjEtoGT9o-66ie8";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));
                var x = new
                {
                    to = deviceID,
                    data = new
                    {
                        message = Smessage,
                        tip = tip.ToString()
                    }
                };

                string postData = JsonConvert.SerializeObject(x);
                Byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
                tRequest.ContentLength = byteArray.Length;
                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse tResponse = tRequest.GetResponse();
                dataStream = tResponse.GetResponseStream();
                StreamReader tReader = new StreamReader(dataStream);
                String sResponseFromServer = tReader.ReadToEnd();
                tReader.Close();
                dataStream.Close();
                tResponse.Close();
                return sResponseFromServer;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static void pushGonderIOS(string deviceID, string title, string Smessage)
        {
            try
            {
                string SERVER_API_KEY = "AAAANqMcz-8:APA91bFrljK5pLRxtFPgvaUrh48HAxC9-VBXzorLRIlRxV31ZVATjvg_V67BCVnXeHCXjlC6SopF1E-MBdNvRE_mP2ezWZ1IcyWZykBJ5AimQeFXjdOYewiXvax44g3CtDDpfscKg1iN";
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));

                var x = new
                {
                    to = deviceID,
                    notification = new
                    {
                        title = title,
                        text = Smessage //+ "_" + tip.ToString()
                    },
                };

                string postData = JsonConvert.SerializeObject(x);
                Byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(postData);
                tRequest.ContentLength = byteArray.Length;
                Stream dataStream = tRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse tResponse = tRequest.GetResponse();
                dataStream = tResponse.GetResponseStream();
                StreamReader tReader = new StreamReader(dataStream);
                String sResponseFromServer = tReader.ReadToEnd();
                tReader.Close();
                dataStream.Close();
                tResponse.Close();
                // return sResponseFromServer;
            }
            catch (Exception ex)
            {
                // return ex.Message;
            }

        }

        public static void AddToApiRequestLog(object model)
        {
            #region apirequestlog
            var json = new JavaScriptSerializer().Serialize(model);

            new ApiRequestLogRepo().Insert(new Nero2021.Data.ApiRequestLog
            {
                RequestURI = HttpContext.Current.Request.Url.ToString(),
                JSONData = json.ToString(),
                CreatedOn = DateTime.Now
            });
            #endregion
        }

        public static void AddToApiRequestLog(object model, Exception ex)
        {
            #region apirequestlog
            var json = new JavaScriptSerializer().Serialize(model);

            new ApiRequestLogRepo().Insert(new Nero2021.Data.ApiRequestLog
            {
                RequestURI = HttpContext.Current.Request.Url.ToString(),
                JSONData = json.ToString(),
                CreatedOn = DateTime.Now,
                ResultText = ex != null ? ex.GetBaseException().Message : null
            });
            #endregion
        }

        public static string CreateEmailBody(string filename)
        {
            try
            {
                string body = string.Empty;
                //using streamreader for reading my htmltemplate   

                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/mailing/" + filename)))
                {
                    body = reader.ReadToEnd();
                }

                //body = body.Replace("{UserName}", userName); //replacing the required things  
                //body = body.Replace("{Title}", title);
                //body = body.Replace("{message}", message);

                return body;
            }
            catch (Exception ex)
            {
                WriteToLog("CreateEmailBody ERR: GetBaseException: " + ex.GetBaseException().Message);
                WriteToLog("CreateEmailBody ERR: InnerException: " + ex.InnerException.ToString());

                throw;

            }
        }

        public static void WriteToLog(string messagetext)
        {
            string sqlconnstr = "";
#if DEBUG
            sqlconnstr = @"Server=DESKTOP-QPGN981\SS17;Database=TurkBelgeKEPDB;User Id=tbkepusr;Password=\[q#U7t5m5K2zdJA;";
#else
            sqlconnstr = "Server=test.kep-ik.com;Database=ilekacom_kepiktestdb;User Id=ilekacom_kepiktestuser;Password=0Am!d96d;";
#endif
            //"data source=217.195.203.126;initial catalog=DuelsKingsDB;user id=duelskingsusr;password=bjeW!Wvy?w4L9fSD"
            using (SqlConnection openCon = new SqlConnection(sqlconnstr))
            {
                string saveStaff = "INSERT into Tmp_Log ([ResultText]) VALUES (@ResultText)";

                using (SqlCommand querySaveStaff = new SqlCommand(saveStaff))
                {
                    querySaveStaff.Connection = openCon;
                    querySaveStaff.Parameters.Add("@ResultText", SqlDbType.NVarChar, 8000).Value = messagetext;
                    //querySaveStaff.Parameters.Add("@Text1", SqlDbType.NVarChar, 4000).Value = model.Message;
                    //querySaveStaff.Parameters.Add("@Text2", SqlDbType.NVarChar, 4000).Value = model.Type;
                    //querySaveStaff.Parameters.Add("@fbuid", SqlDbType.NVarChar, 64).Value = model.fbuid;

                    openCon.Open();

                    querySaveStaff.ExecuteNonQuery();
                }
            }
        }
    }

}
