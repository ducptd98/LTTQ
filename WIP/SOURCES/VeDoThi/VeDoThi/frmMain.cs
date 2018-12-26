using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using EB.Math;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Net;
using System.Diagnostics;

namespace VeDoThi
{
    public partial class frmMain : Form
    {
        int max_x, max_y;
        Graphics g;
        Function fn = new Function();
        ArrayList arr;
        double x, dx, fx1, fx2;
        int max, min, x1, y1, x2, y2, x0, y0, k = 30, delay = 0;
        int m_x, m_y;
        bool stop = false, repaint = false ;
        int positionY = 5;
        int colorPositon = 0;
        Color[] listColor = { Color.Blue, Color.Red, Color.Green, Color.Yellow };
        List<string> listFunction;
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
        public frmMain()
        {
            InitializeComponent();
            listFunction = new List<string>();
        }

        string nguon = @"C:\\nguon";                //tạo thư mục nguồn lưu tất cả chương trình và cập nhật
        string updatetxt = @"C:\\nguon\update.txt";    //tạo file.txt ghi mã hiệu bản cập nhật mới
        string currenttxt = @"C:\\nguon\current.txt";  //tạo file.txt ghi mã hiệu bản hiện tại đang dùng.
        string apppath = @"C:\\nguon\app";          //Tạo thư mục lưu chương trình sẽ giải nén của bạn.
        string apprar = @"C:\\nguon\VeDoThi.rar";       //tên file.rar chương trình của bạn sẽ được tải về từ google drive
        string rarpath = @"C:\\nguon\rarpath";      //tạo thư mục lưu file cài đặt winrar  
        string rarexe = @"C:\\nguon\rarpath\WinRaR.exe"; //file khởi động winrar
        string updateeexe = @"C:\\nguon\app\VeDoThi.exe"; //file khởi động chương trình của bạn.
        string updatetxtlink = "https://docs.google.com/uc?export=download&id=1HIEp0LC51wEF2CDwasMgkIxBefIakqEq";  //duong dan update.txt
        string applink = "";
        string cai1 = Application.StartupPath + @"\Install.exe"; //duong dan den file ơ thu muc cung với chương trình
        string cai2 = @"C:\\nguon\app\Install.exe";     //duong dan copy den thuc muc khoi dong

        #region Events
        private void frmMain_Load(object sender, EventArgs e)
        {
            g = PicPaint.CreateGraphics();

            // tạo thư mục nguồn
            create();
            //

            // code kiểm tra kết nối mạng
            checkConnectInternet();
        }
        private void btnPaint_Click(object sender, EventArgs e)
        {
            int space = 2;
            int.TryParse(txtTiLe.Text, out k);

            x0 = max_x / 2;
            y0 = max_y / 2;
            frmMain_Resize(null, null);
            trkZoom.Value = 5;
            stop = false;
            listFunction.Add(txtFunction.Text);
            PaintGraph();
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            max_x = PicPaint.Width;
            max_y = PicPaint.Height;
            //k = 30;

            x0 = (int)(max_x / 2);
            y0 = (int)(max_y / 2);

            if (chkAutoFill.Checked)
            {
                min = -x0 / k - 1;
                max = x0 / k + 1;
            }

            txtMin.Text = min.ToString();
            txtMax.Text = max.ToString();
            txtTiLe.Text = k.ToString();

        }

        private void txtTiLe_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar)) e.Handled = true;
        }

        private void txtMax_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && e.KeyChar != '-') e.Handled = true;
        }

        private void txtMin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar) && e.KeyChar != '-') e.Handled = true;
        }

        private void txtDelay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && !Char.IsControl(e.KeyChar)) e.Handled = true;
        }



        private void btnZoom_Click(object sender, EventArgs e)
        {
            trkZoom.Visible = !trkZoom.Visible;
        }



        private void trkZoom_MouseUp(object sender, MouseEventArgs e)
        {
            k = (trkZoom.Value + 1) * 5;
           
            AutoMinMax();
            PicPaint.Refresh();
            foreach (string item in listFunction)
            {
               
                PaintGraph(item);
            }
            
            trkZoom.Focus();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (txtFunction.Text != "")
                PaintGraph();
        }

        private void txtFunction_TextChanged(object sender, EventArgs e)
        {
        }

        private void btnFunc_Click(object sender, EventArgs e)
        {
            ctMenu.Show(btnFunc.Left + grpFunc.Left, btnFunc.Top + btnFunc.Height + grpFunc.Top);

        }

        private void cosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtFunction.Text += sender.ToString() + "(";
        }

        private void subToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtFunction.Text += sender.ToString().Trim();
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            btnZoomIn.Enabled = false;
            k = k + 10;
            AutoMinMax();
            PicPaint.Refresh();
            foreach (string item in listFunction)
            {

                PaintGraph(item);
            }
            btnZoomIn.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PicPaint.Refresh();
            positionY = 5;
            listFunction.Clear();
            txtFunction.Text = "";
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            if (k <= 10) return;
            btnZoomOut.Enabled = false;
            k = k - 10;
            AutoMinMax();
            PicPaint.Refresh();
            foreach (string item in listFunction)
            {

                PaintGraph(item);
            }
            btnZoomOut.Enabled = true;
        }

        private void btnAbout_Click(object sender, EventArgs e)
        {
            frmAbout frm = new frmAbout();
            frm.ShowDialog();
            btnPaint.Focus();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            x0 -= 20;
            AutoMinMax();
            PaintGraph();
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            x0 += 20;
            AutoMinMax();
            PaintGraph();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            y0 -= 20;
            AutoMinMax();
            PaintGraph();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            y0 += 20;
            AutoMinMax();
            PaintGraph();
        }

        private void PicPaint_MouseUp(object sender, MouseEventArgs e)
        {
            PicPaint.Cursor = Cursors.Default;
            int x = e.X - m_x;
            int y = e.Y - m_y;

            if (Math.Abs(x) >= 20 || Math.Abs(y) >= 20)
            {
                x0 += x;
                y0 += y;

                AutoMinMax();

                PaintGraph();
            }
        }

        private void PicPaint_MouseDown(object sender, MouseEventArgs e)
        {
            PicPaint.Cursor = Cursors.SizeAll;
            m_x = e.X;
            m_y = e.Y;
            ttStatus.Text = "Giữ và kéo chuột để di chuyển tọa độ đồ thị !";
        }


        private void chkAutoFill_CheckedChanged(object sender, EventArgs e)
        {
            txtMax.Enabled = !chkAutoFill.Checked;
            txtMin.Enabled = !chkAutoFill.Checked;
        }


        private void txtFunction_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13) btnPaint_Click(null, null);
        }
        #endregion

        #region Functions

        private void checkConnectInternet()
        {
            if (InternetConnection.IsConnectedToInternet())
            {
                WebClient ud = new WebClient();
                ud.DownloadFileCompleted += new AsyncCompletedEventHandler(udcom);
                Uri update = new Uri(updatetxtlink);
                ud.DownloadFileAsync(update, updatetxt);
            }
            else
                MessageBox.Show("Không thể kiểm tra bản cập nhật do chưa kết nối mạng");


        }

        private void create()
        {
            if (Directory.Exists(nguon))
            {
                //tạo file current.txt
                StreamWriter st = new StreamWriter(currenttxt);
                st.WriteLine(lbCurVersion.Text);
                st.Close();
                if (Directory.Exists(apppath))
                {

                }
                else
                {
                    Directory.CreateDirectory(apppath);
                }
            }
            else
            {
                Directory.CreateDirectory(nguon);
                Directory.CreateDirectory(apppath);

            }
        }
        private void udcom(object sender, AsyncCompletedEventArgs e)
        {
            //doc file này cho vào label 2
            StreamReader st = new StreamReader(updatetxt);
            lbNewVersion.Text = st.ReadLine();
            st.Close();
            //thưc hien kiểm tra phiên bản de thong bao cap nhat
            if (lbCurVersion.Text == lbNewVersion.Text)
            {
                //nếu bằng nhau thì không có bản cập nhật
                MessageBox.Show("Không có bản cập nhật mới");
            }
            else //nếu không bằng nhau
            {
                //thong báo có bản cập nhật và phiên bản mới
                DialogResult di = MessageBox.Show("Phiên bản cập nhật mới " + lbNewVersion.Text + " Đã sẵng sàn cài đặt",
                    "Bản cập nhật mới", MessageBoxButtons.YesNo);
                if (di == DialogResult.Yes) //neu nhan yes thì cập nhật
                {
                    //copy chương trình cài đặt trung gian vào c:\\nguon
                    if (File.Exists(cai1))
                        File.Copy(cai1, cai2, true);
                    else
                        File.Copy(cai1, cai2);
                    //chạy cài trung gian

                    Process.Start(cai1);

                    // thoát chương trình này
                    Application.Exit();
                }
        }
        //tao file phiên bản hiện tại

    }
        private void VeTrucToaDo()
        {

            Pen pen = new Pen(Color.Black, 2);
            g.DrawLine(pen, 1, y0, max_x, y0);
            g.DrawLine(pen, x0, 1, x0, max_y);

            Font f = new Font("Tahoma", 10);
            Brush br = new SolidBrush(Color.Red);

            g.DrawString("O", f, br, x0 - 15, y0);
            g.DrawString("X", f, br, max_x - 20, y0);
            g.DrawString("Y", f, br, x0 + 3, 1);

            Pen pen_x = new Pen(Color.Gray, 1);

            int i;
            int j = 0;
            for (i = x0 + k; i < max_x; i += k)
            {
                g.DrawLine(pen_x, i, y0 - 3, i, y0 + 2);
                g.DrawString((++j).ToString(), f, br, i, y0 - 3);
            }
            j = 0;
            for (i = x0 - k; i > 0; i -= k)
            {
                g.DrawLine(pen_x, i, y0 - 3, i, y0 + 2);
                g.DrawString((--j).ToString(), f, br, i, y0 - 3);
            }
            j = 0;
            for (i = y0 + k; i < max_y; i += k)
            {
                g.DrawLine(pen_x, x0 - 3, i, x0 + 2, i);
                g.DrawString((--j).ToString(), f, br, x0 + 2, i);
            }
            j = 0;
            for (i = y0 - k; i > 0; i -= k)
            {
                g.DrawLine(pen_x, x0 - 3, i, x0 + 2, i);
                g.DrawString((++j).ToString(), f, br, x0 + 2, i);
            }


            Pen pen_limit = new Pen(Color.Red, 2);

            g.DrawLine(pen_limit, x0 + (min * k), y0 - 5, x0 + (min * k), y0 + 5);
            g.DrawLine(pen_limit, x0 + (max * k), y0 - 5, x0 + (max * k), y0 + 5);

        }

        private void VeDoThi()
        {
            int limit = max_y / 30 * k;

            Pen pen = new Pen(listColor[colorPositon], 2);
            colorPositon++;
            if (colorPositon == 4)
                colorPositon = 0;
            x = min;
            dx = 1.0f / k;

            fx1 = f(x);
            x1 = x0 + (int)(x * k);
            y1 = y0 - (int)(fx1 * k);


            if (stop) return;

            while (x < max)
            {
                x += dx;
                fx2 = f(x);
                x2 = x0 + (int)(x * k);
                y2 = y0 - (int)(fx2 * k);

                try
                {
                    if (!(fx1 * fx2 < 0 && Math.Abs((int)(fx1 - fx2)) > k))
                    {
                        g.DrawLine(pen, x1, y1, x2, y2);
                        if (delay > 0) System.Threading.Thread.Sleep(delay);
                    }
                }
                catch { }

                x1 = x2;
                y1 = y2;
                fx1 = fx2;
            }

        }

        private void PaintGraph()
        {
            //PicPaint.Refresh();
            fn.Parse(txtFunction.Text.ToLower());

            fn.Infix2Postfix();
            if (fn.Variables.Count == 0 && txtFunction.Text != "")
            {
                //PicPaint.Refresh();
                float _y = float.Parse(txtFunction.Text);
                VeTrucToaDo();
                Application.DoEvents();

                Pen pen1 = new Pen(Color.Blue, 2);

                g.DrawLine(pen1, x0 * min, y0 - _y * k, x0 * max, y0 - _y * k);
                g.DrawString("y=" + txtFunction.Text, new Font("Tahoma", 10), new SolidBrush(listColor[colorPositon]), 5, positionY += 20);


                return;
            }
            arr = fn.Variables;
            g.DrawString("y=" + txtFunction.Text, new Font("Tahoma", 10), new SolidBrush(listColor[colorPositon]), 5, positionY += 20);

            if (arr.Count != 1)
            {
                MessageBox.Show("Biểu thức không hợp lệ. Vui lòng nhập lại !\n\nVí dụ: (sin(x)+3)/(x+4)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                if (arr[0].ToString() != "x")
                {
                    MessageBox.Show("Biểu thức không hợp lệ. Vui lòng nhập lại !\n\nVí dụ: (sin(x)+3)/(x+4)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


            //PicPaint.Refresh();
            btnPaint.Enabled = false;
            Application.DoEvents();


            int.TryParse(txtMin.Text, out min);
            int.TryParse(txtMax.Text, out max);


            ttStatus.Text = "Trạng thái: Đang vẽ ....";

            VeTrucToaDo();
            Application.DoEvents();

            Pen pen = new Pen(Color.Red, 2);

            VeDoThi();
            btnPaint.Enabled = true;
            delay = 0;
            ttStatus.Text = "Trạng thái: Hoàn tất !";
        }

        private void PaintGraph(string fun)
        {
            //PicPaint.Refresh();
            fn.Parse(fun.ToLower());

            fn.Infix2Postfix();
            if (fn.Variables.Count == 0 && txtFunction.Text != "")
            {
                //PicPaint.Refresh();
                float _y = float.Parse(txtFunction.Text);
                VeTrucToaDo();
                Application.DoEvents();

                Pen pen1 = new Pen(Color.Blue, 2);

                g.DrawLine(pen1, x0 * min, y0 - _y * k, x0 * max, y0 - _y * k);



                return;
            }
            arr = fn.Variables;
            g.DrawString("y=" + txtFunction.Text, new Font("Tahoma", 10), new SolidBrush(listColor[colorPositon]), 5, positionY += 20);
            if (arr.Count != 1)
            {
                MessageBox.Show("Biểu thức không hợp lệ. Vui lòng nhập lại !\n\nVí dụ: (sin(x)+3)/(x+4)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                if (arr[0].ToString() != "x")
                {
                    MessageBox.Show("Biểu thức không hợp lệ. Vui lòng nhập lại !\n\nVí dụ: (sin(x)+3)/(x+4)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }


            //PicPaint.Refresh();
            btnPaint.Enabled = false;
            Application.DoEvents();


            int.TryParse(txtMin.Text, out min);
            int.TryParse(txtMax.Text, out max);


            ttStatus.Text = "Trạng thái: Đang vẽ ....";

            VeTrucToaDo();
            Application.DoEvents();

            Pen pen = new Pen(Color.Red, 2);

            VeDoThi();
            btnPaint.Enabled = true;
            delay = 0;
            ttStatus.Text = "Trạng thái: Hoàn tất !";
        }

        private double f(double x)
        {
            //return Math.Tan(x);

            Symbol sl;
            sl.m_type = EB.Math.Type.Variable;
            sl.m_name = "x";
            sl.m_value = x;

            arr[0] = sl;
            fn.Variables = arr;
            fn.EvaluatePostfix();

            if (fn.Error)
            {
                MessageBox.Show(fn.ErrorDescription, "Error !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                stop = true;
            }
            return fn.Result;
        }

        private void AutoMinMax()
        {
            if (chkAutoFill.Checked)
            {
                min = -x0 / k - 1;
                max = (max_x - x0) / k + 1;

                txtMin.Text = min.ToString();
                txtMax.Text = max.ToString();
            }
        }
        #endregion
    }
}