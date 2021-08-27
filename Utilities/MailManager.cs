using mFFramework.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;

namespace mFFramework.Utilities
{


    /// <summary>
    /// 
    /// </summary>
    public class mBody
    {

        /// <summary>
        /// 
        /// </summary>
        public string Subject;

        /// <summary>
        /// 
        /// </summary>
        public string Message;

        /// <summary>
        /// 
        /// </summary>
        public bool IsHtml;

    }


    /// <summary>
    /// 
    /// </summary>
    public class mAddress
    { 
    
        /// <summary>
        /// 
        /// </summary>
        public string From;

        /// <summary>
        /// 
        /// </summary>
        public string To;

        /// <summary>
        /// 
        /// </summary>
        public string CC;

        /// <summary>
        /// 
        /// </summary>
        public List<string> BCC;
    }


    /// <summary>
    /// 
    /// </summary>
    public class mParameters
    {

        /// <summary>
        /// 
        /// </summary>
        public string SmtpServer;

        /// <summary>
        /// 
        /// </summary>
        public int Port;




    }


    /// <summary>
    /// 
    /// </summary>
    public class mAccount
    {

        /// <summary>
        /// 
        /// </summary>
        public string Username;

        /// <summary>
        /// 
        /// </summary>
        public string Password;
    }


    /// <summary>
    /// 
    /// </summary>
    public class mAttachments
    {
        
        /// <summary>
        /// 
        /// </summary>
        public List<Attachment> Attachments { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public mAttachments()
        {

            Attachments = new List<Attachment>();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameFile"></param>
        public void Add(string nameFile)
        {

            Attachments.Add(new Attachment(nameFile));
        }



    }




    /// <summary>
    /// 
    /// </summary>
    public class MailManager
    {

        private MailMessage mail;
        private NetworkCredential credentials;
        private SmtpClient smtpclient;

        private string applicationPath;
        private string filename;
        private string mailTest;

        private FileStream fs;
        private StreamWriter sw;

        private Process process;

     
        /// <summary>
        /// 
        /// </summary>
        public int codeResultInternetConnection;

        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int Description, int ReservedValue);

   
      


        /// <summary>
        /// 
        /// </summary>
        public mAddress Address { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public mAccount Account { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public mParameters Parameters { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public mBody Body { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public mAttachments Attachments {get;set;}



        private string resultSend;
        /// <summary>
        /// Descrizione dell'esito dell'invio mail
        /// </summary>
        public string ResultSend { get { return resultSend; } }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public TypeSendMail TrySend()
        {

            try
            {


                // imposta le credenziali dell'account per invio mail
                credentials = new NetworkCredential(Account.Username, Account.Password);

                // imposta la mail con il mittente, il destinatario, la copia, la copia nascosta, il soggetto e il messaggio;
                // l'eventuale destinatario in copia e la priorità della mail
                mail = new MailMessage(Address.From, Address.To, Body.Subject, Body.Message);
                if (!Address.CC.IsVoid())
                    mail.CC.Add(Address.CC);
                if (!Address.BCC.IsVoid())
                    Address.BCC.ForEach(bcc => mail.Bcc.Add(bcc));

                mail.Priority = MailPriority.High;
                mail.IsBodyHtml = Body.IsHtml;

                Attachments.Attachments
                           .ForEach(a => mail.Attachments.Add(a));
               
                // imposta il server di invio, la porta, le credenziali e il metodo di consegna
                smtpclient = new SmtpClient();
                smtpclient.Host = Parameters.SmtpServer;
                smtpclient.Port = Parameters.Port;
                smtpclient.UseDefaultCredentials = false;
                smtpclient.Credentials = credentials;
                smtpclient.DeliveryMethod = SmtpDeliveryMethod.Network;

                // verifica che ci sia una connessione internet attiva
                if (!InternetGetConnectedState(out codeResultInternetConnection, 0))
                {
                    resultSend = TypeSendMail.NoNetworkConnection.GetDescription();
                    return TypeSendMail.NoNetworkConnection;
                }


                // invio
                smtpclient.Send(mail);


                resultSend = TypeSendMail.SendOK.GetDescription();
                return TypeSendMail.SendOK;

            }
            catch (SmtpException sex)
            {
                resultSend = sex.Message;
                return TypeSendMail.SendKO;
            }
            catch (Exception ex)
            {
                resultSend = ex.Message;
                return TypeSendMail.SendKO;

            }


        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pageCaller"></param>
        /// <returns></returns>
        public TypeSendMail TrySendForTest(Page pageCaller)
        {

            try
            {

                
                // percorso principale applicazione web
                applicationPath = pageCaller.Server.MapPath(@"\");
                // nome completo file test mail
                filename = applicationPath + "__Temp_TestMail.html";
                // corpo mail test
                mailTest = "<html><body><br/>" + Body.Subject + "<br/><hr/><br/>" + Body.Message + "<br/></body></html>";

                // crea la pagina html della mail di test
                if (File.Exists(filename))
                    File.Delete(filename);
                fs = new FileStream(filename, FileMode.Create);
                sw = new StreamWriter(fs);
                sw.Write(mailTest);
                sw.Flush();
                sw.Close();
                fs.Close();

                //HttpContext.Current.Response.Redirect(pp + "?" )
                // apre il processo della pagina web di test
                process = new Process();
                process.StartInfo.FileName = filename;
                process.Exited += process_Exited;
                process.Start();
                process.WaitForExit(10000);
                

            }
            catch (Exception ex)
            {
                if (pageCaller != null)
                {
                    // cancella il file di test
                    //File.Delete(filename);
                    // invia la segnalazione di test
                    resultSend = TypeSendMail.SendTest.GetDescription() + Environment.NewLine + ex.Message;
                    return TypeSendMail.SendTest;
                }

                resultSend = ex.Message;
                return TypeSendMail.SendTest;

            }

            resultSend = TypeSendMail.SendTest.GetDescription();
            return TypeSendMail.SendTest;
        }



        //private static void Redirect(this HttpResponse response, string url, string target, string windowFeatures)
        //{
        //    if ((target.IsVoid() || target.Equals("_self", StringComparison.OrdinalIgnoreCase)) && windowFeatures.IsVoid())
        //        response.Redirect(url);
        //     else
        //    {
        //        Page page = (Page)HttpContext.Current.Handler;

        //        if (page == null)
        //        {
        //            throw new InvalidOperationException("Error redirecting, please try again.");
        //        }
        //        url = page.ResolveClientUrl(url);

        //        string script;
        //        if (!String.IsNullOrEmpty(windowFeatures))
        //        {
        //            script = @"window.open(""{0}"", ""{1}"", ""{2}"");";
        //        }
        //        else
        //        {
        //            script = @"window.open(""{0}"", ""{1}"");";
        //        }
        //        script = String.Format(script, url, target, windowFeatures);
        //        ScriptManager.RegisterStartupScript(page, typeof(Page), "Redirect", script, true);
        //    }
        //}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sc"></param>
        /// <param name="urlMailTester"></param>
        /// <returns></returns>
        public TypeSendMail TrySendForTest2(ClientScriptManager sc, string urlMailTester)
        {

            try
            {


                HttpContext.Current.Session["Subject_MailTester"] = Body.Subject;
                HttpContext.Current.Session["Message_MailTester"] = Body.Message;

                string scriptWindow = "window.open('" + urlMailTester + "','_blank','status=no,resizable=yes,menubar=no,toolbar=no,scrollbars=yes,location=no');";
                sc.RegisterClientScriptBlock(typeof(string), "OPEN_WINDOW", scriptWindow, true);
              //  HttpContext.Current.Response.Redirect(urlMailTester);
               
                resultSend = TypeSendMail.SendTest.GetDescription();
                return TypeSendMail.SendTest;

            }
            catch (Exception ex)
            {
               

                resultSend = ex.Message;
                return TypeSendMail.SendTest;

            }


        }

        private void process_Exited(object sender, EventArgs e)
        {
            // cancella il file di test
            File.Delete(filename);
        }


        /// <summary>
        /// 
        /// </summary>
        public MailManager()
        {


            Address = new mAddress();
            Parameters = new mParameters();
            Account = new mAccount();
            Body = new mBody();
            Attachments = new mAttachments();
        
        }

    }
}
