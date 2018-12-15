using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Install
{
    public partial class Install : Form
    {
        class InternetConnection
        {
            [DllImport("wininet.dll")]
            private extern static bool InternetGetConnectedState(out int description, int reservedValuine);
            public static bool IsConnectedToInternet()
            {
                int desc;
                return InternetGetConnectedState(out desc, 0);
            }
        }
        string nguon = @"C:\\nguon";                //tạo thư mục nguồn lưu tất cả chương trình và cập nhật
        string updatetxt = @"C:\\nguon\update.txt";    //tạo file.txt ghi mã hiệu bản cập nhật mới
        string currenttxt = @"C:\\nguon\current.txt";  //tạo file.txt ghi mã hiệu bản hiện tại đang dùng.
        string apppath = @"C:\\nguon\app";          //Tạo thư mục lưu chương trình sẽ giải nén của bạn.
        string apprar = @"C:\\nguon\VeDoThi.rar";       //tên file.rar chương trình của bạn sẽ được tải về từ google drive
        string rarpath = @"C:\\nguon\rarpath";      //tạo thư mục lưu file cài đặt winrar  
        string rarexe = @"C:\\nguon\rarpath\zip\WinRaR.exe"; //file khởi động winrar
        string updateeexe = @"C:\\nguon\app\VeDoThi.exe"; //file khởi động chương trình của bạn.
        string applink = "https://docs.google.com/uc?export=download&id=1yRwAF4uCKaBqXCTZZz7GVRtUxzKh0dIZ"; //duong dan app.rar
        string rarsave = @"C:\\nguon\zip.zip";
        string winrar = @"C:\\Program Files\WinRAR\RAR.exe";
        string rarlink = "https://docs.google.com/uc?export=download&id=1b--gtoCQ-d0GNM5hRaUDpkXLSRPlUcqn"; //duong dan zip.zip
        public Install()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            progressBar.Maximum = 100;
            progressBar.Minimum = 0;
            progressBar.Step = 1;
            //kiem tra su ton tai cua 2 file update va current 
            if (File.Exists(currenttxt) && File.Exists(updatetxt))
            {
                if (InternetConnection.IsConnectedToInternet())
                {
                    //kiem tra da co winrar cai măc định trong ổ C chưa. nếu chưa cài mà vẫn sử dụng đường dẫn sẽ báo lỗi
                    if (File.Exists(winrar))
                    {
                        //kiem tra phien ban
                        kiemtra();
                    }
                    else
                    {
                        if (Directory.Exists(rarpath))
                        { }
                        else
                            Directory.CreateDirectory(rarpath);
                        //tai winrar để cài đăt
                        taiwinrar();
                    }

                }
                else
                {
                    Application.Exit();
                    MessageBox.Show("Cần kết nối mạng để cập nhật");

                }
            }
            else
            {
                //thoat va bao loi

                MessageBox.Show("Lỗi. cần khởi chạy chương trình update trước");
                Application.Exit();
            }
        }

        private void taiwinrar()
        {
            label1.Text = "Đang tạo môi trường cài đặt";
            WebClient rar = new WebClient();
            rar.DownloadFileCompleted += new AsyncCompletedEventHandler(rarcom);
            Uri wr = new Uri(rarlink);
            rar.DownloadFileAsync(wr, apprar);
        }
        private void rarcom(object sender, AsyncCompletedEventArgs e)
        {
            //giải nen winrar
            ZipFile.ExtractToDirectory(rarsave, rarpath);
            kiemtra();
        }
        //kiem tra phien ban
        private void kiemtra()
        {
            //doc file current
            StreamReader st = new StreamReader(currenttxt);
            label4.Text = st.ReadLine();
            st.Close();
            //doc file update
            StreamReader re = new StreamReader(updatetxt);
            label5.Text = re.ReadLine();
            re.Close();
            // so sanh
            if (label4.Text == label5.Text)
            {
                MessageBox.Show("Không có bản cập nhât");
                Application.Exit();
            }
            else
            {
                taicapnhat();
            }

        }

        private void taicapnhat()
        {
            //xoa thu muc cu
            if (Directory.Exists(apppath))
            {
                DirectoryInfo directory = new DirectoryInfo(apppath);

                //delete files:
                directory.GetFiles().ToList().ForEach(f => f.Delete());

                //delete folders inside choosen folder
                directory.GetDirectories().ToList().ForEach(d => d.Delete(true));
            }
            //thong tin tien trinh
            label2.Text = "Đang tải về bản cập nhật";
            progressBar.Value = 40;
            label3.Text = progressBar.Value.ToString() + "%";
            //tai ve file chuong trinh .rar
            WebClient exe = new WebClient();
            exe.DownloadFileCompleted += new AsyncCompletedEventHandler(appcom);
            Uri app = new Uri(applink);
            exe.DownloadFileAsync(app, apprar);
        }

        private void appcom(object sender, AsyncCompletedEventArgs e)
        {
            //giai nen file.rar
            label2.Text = "Đang giải nén bản cài đặt";
            progressBar.Value = 70;
            label3.Text = progressBar.Value.ToString() + "%";
            // neu co winrar trong c thì xài winrar khong co thì xài zip moi tai ve
            if (File.Exists(@"C:\\Program Files\WinRAR\RAR.exe"))
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = @"C:\\Program Files\WinRAR\RAR.exe";
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.EnableRaisingEvents = false;
                process.StartInfo.Arguments = string.Format("x -o+ \"{0}\" \"{1}\"", apprar, apppath);
                process.Start();
                //khoi dong
                timerdongbo.Enabled = true;
            }
            else
            {
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = rarexe;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                process.EnableRaisingEvents = false;
                process.StartInfo.Arguments = string.Format("x -o+ \"{0}\" \"{1}\"", apprar, apppath);
                process.Start();
                //dong bo
                timerdongbo.Enabled = true;

            }
        }

        private void timerdongbo_Tick(object sender, EventArgs e)
        {
            timerkhoidong.Stop();
            //dong dong phien ban moi
            if (File.Exists(updateeexe))
                Process.Start(updateeexe);
            Application.Exit();
        }

        private void timerkhoidong_Tick(object sender, EventArgs e)
        {
            timerdongbo.Stop();
            timerkhoidong.Enabled = true;
            label2.Text = "đang đồng bộ bản cập nhật";
            progressBar.Value = 100;
            label3.Text = progressBar.Value.ToString() + "%";
            //xoa file app.rar vua tai
            if (File.Exists(apprar))
                File.Delete(apprar);
            //xoa file zip.zip (winrar bên thứ 3 vừa tải)
            if (File.Exists(rarsave))
                File.Delete(rarsave);
        }
    }
}
