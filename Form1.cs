using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using WindowsInput.Native;
using WindowsInput;
using System.Diagnostics;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;

namespace RemoteWindowsSide
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);




        public Dictionary<string, VirtualKeyCode> keycode = new Dictionary<string, VirtualKeyCode>();




        private const int MOUSEEVENTF_MOVE = 0x0001; /* mouse move */
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; /* left button down */
        private const int MOUSEEVENTF_LEFTUP = 0x0004; /* left button up */
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; /* right button down */
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;

        [DllImport("user32.dll",
            CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons,
                    int dwExtraInfo);



        [DllImport("User32")]
        static extern bool SetCursorPos(int X, int Y); //user 32 move cursor

        public Form1()
        {
            InitializeComponent();

            
            keycode.Add("A", VirtualKeyCode.VK_A);
            keycode.Add("Z", VirtualKeyCode.VK_Z);
            keycode.Add("E", VirtualKeyCode.VK_E);
            keycode.Add("R", VirtualKeyCode.VK_R);
            keycode.Add("T", VirtualKeyCode.VK_T);
            keycode.Add("Y", VirtualKeyCode.VK_Y);
            keycode.Add("U", VirtualKeyCode.VK_U);
            keycode.Add("I", VirtualKeyCode.VK_I);
            keycode.Add("O", VirtualKeyCode.VK_O);
            keycode.Add("P", VirtualKeyCode.VK_P);
            keycode.Add("Q", VirtualKeyCode.VK_Q);
            keycode.Add("S", VirtualKeyCode.VK_S);
            keycode.Add("D", VirtualKeyCode.VK_D);
            keycode.Add("F", VirtualKeyCode.VK_F);
            keycode.Add("G", VirtualKeyCode.VK_G);
            keycode.Add("H", VirtualKeyCode.VK_H);
            keycode.Add("J", VirtualKeyCode.VK_J);
            keycode.Add("K", VirtualKeyCode.VK_K);
            keycode.Add("L", VirtualKeyCode.VK_L);
            keycode.Add("M", VirtualKeyCode.VK_M);
            keycode.Add("W", VirtualKeyCode.VK_W);
            keycode.Add("X", VirtualKeyCode.VK_X);
            keycode.Add("C", VirtualKeyCode.VK_C);
            keycode.Add("V", VirtualKeyCode.VK_V);
            keycode.Add("B", VirtualKeyCode.VK_B);
            keycode.Add("N", VirtualKeyCode.VK_N);

            keycode.Add(" ", VirtualKeyCode.SPACE);

            keycode.Add("0", VirtualKeyCode.NUMPAD0);
            keycode.Add("1", VirtualKeyCode.NUMPAD1);
            keycode.Add("2", VirtualKeyCode.NUMPAD2);
            keycode.Add("3", VirtualKeyCode.NUMPAD3);
            keycode.Add("4", VirtualKeyCode.NUMPAD4);
            keycode.Add("5", VirtualKeyCode.NUMPAD5);
            keycode.Add("6", VirtualKeyCode.NUMPAD6);
            keycode.Add("7", VirtualKeyCode.NUMPAD7);
            keycode.Add("8", VirtualKeyCode.NUMPAD8);
            keycode.Add("9", VirtualKeyCode.NUMPAD9);


            //int vol = 10;
            ////AudioSwitcher.AudioApi.CoreAudio.CoreAudioDevice audioDevice = new AudioSwitcher.AudioApi.CoreAudio.CoreAudioController().DefaultPlaybackDevice;

            ////audioDevice.Volume = vol;



            //CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;

            //defaultPlaybackDevice.Volume = vol;


            



        }



        private void Form1_Load(object sender, EventArgs e)
        {
           

            label1.Text = GetLocalIPAddress();

            Task main = new Task(WaitForConn);
            main.Start();
            //await WaitForConn();

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            for (int x = 0; x < 4; x++)
                SetCursorPos(Cursor.Position.X + 10, System.Windows.Forms.Cursor.Position.Y + 10);

        }


        ASCIIEncoding encoder = new ASCIIEncoding();
        TcpListener server = new TcpListener(IPAddress.Any, 1997);



        public void WaitForConn()
        {

            server.Start();

            accept_conn();

        }

        public void  accept_conn()
        {
            server.BeginAcceptTcpClient(handle_conn, server);

            //creates the TcpClient



            //await accept_conn(client);

        }

        public string tempMsg = "";
        public string message = "";
        private void handle_conn(IAsyncResult result)
        {


            
            TcpClient client = server.EndAcceptTcpClient(result);

            NetworkStream ns = client.GetStream();

            byte[] msg = new byte[1024];     //the messages arrive as byte array
            ns.Read(msg, 0, msg.Length);


            accept_conn();

            //MessageBox.Show(encoder.GetString(msg));

            message = encoder.GetString(msg);

            //once again, checking for any other incoming connections

            // NetworkStream ns = client.GetStream();
            //while(ns.DataAvailable)
            // {
            //     byte[] msg = new byte[1024];     //the messages arrive as byte array
            //     ns.Read(msg, 0, msg.Length);

            // }

            //MessageBox.Show(encoder.GetString(msg));

            //message = encoder.GetString(msg);




            if (message.Contains("mmov_"))
            {
                tempMsg = message.Substring(5, message.Length - 5);
                movMous(tempMsg);
            }

            if (message.Contains("click"))
            {

                LeftClick(Cursor.Position.X, Cursor.Position.Y);
            }

            if (message.Contains("Rc_"))
            {

                RightClick(Cursor.Position.X, Cursor.Position.Y);
            }

            if (message.Contains("drag_"))
            {

                MouseDrag(Cursor.Position.X, Cursor.Position.Y);
            }

            if (message.Contains("ENTER_sim"))
            {

                enterSim();
            }

            if (message.Contains("Butt_"))
            {
                SendKey(message.Substring(5, message.Length - 5));
            }

            if(message.Contains("volum_"))
            {

                controleAudio(message);
            }

        }

        
        public void controleAudio(string msg)
        {

            if(msg.Substring(6, msg.Length - 6).Contains("up"))
            {
                keybd_event((byte)Keys.VolumeUp, 0, 0, 0);
            }
            if(msg.Substring(6, msg.Length - 6).Contains("down"))
            {
                keybd_event((byte)Keys.VolumeDown, 0, 0, 0);
            }
            if(msg.Substring(6, msg.Length - 6).Contains("mute"))
            {
                keybd_event((byte)Keys.VolumeMute, 0, 0, 0);
            }


        }


        public void enterSim()
        {
            sim.Keyboard.KeyPress(VirtualKeyCode.RETURN);
        }

        public void LeftClick(int x, int y)
        {
            SetCursorPos(x, y);

            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);


            //mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);
        }

        public void RightClick(int x , int y)
        {
            SetCursorPos(x, y);
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0);

            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);


        }

        public void MouseDrag(int x , int y)
        {

            SetCursorPos(x, y);

            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        }

       

        double Rx,Ry;
        public void movMous(string val)
        {
            //for (int i = 0; i < 10 ; i++)
            //{
            //    SetCursorPos(Cursor.Position.X + 10,Cursor.Position.Y );
            //    await Task.Delay(1);
            //}

            int x, y;
            string[] values = val.Split('_');

            double.TryParse(values[0],out Rx);
            double.TryParse(values[1], out Ry);



            x =(int) Math.Round(Rx);
            y = (int)Math.Round(Ry);


            SetCursorPos(Cursor.Position.X + x , Cursor.Position.Y + y ) ;
        }

       


       
        InputSimulator sim = new InputSimulator();
        public void SendKey(string msg)
        {
            if (msg.Contains("__backspace"))
            {
                sim.Keyboard.KeyPress(VirtualKeyCode.BACK);
                
            }
            else
            {

                sim.Keyboard.TextEntry(msg);


              
            }
               
           
        }


        public void sendTcp()
        {
            IPEndPoint ip = new IPEndPoint(IPAddress.Loopback, 1997);

            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

           
            if (!server.Connected)
            {
                try
                {
                    server.Connect(ip);
                }
                catch (Exception x)
                {

                    MessageBox.Show(x.Message);
                }
            }
               


                server.Send(Encoding.ASCII.GetBytes("TEST BYTES"));

        }

       
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());

            //var host2 = Dns.GetHostAddresses(Dns.GetHostName());

            //foreach (var item in host2)
            //{
            //    MessageBox.Show(item.MapToIPv4().ToString());
            //    MessageBox.Show(item.AddressFamily.ToString());
            //}

            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork) 
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Button3_Click_1(object sender, EventArgs e)
        {
            if (textBox1.Text.Contains(" "))
            {

                textBox1.Text = textBox1.Text.Replace(' ', '_');
            }
            if (textBox1.Text != "" & textBox2.Text.Length > 8)
            {
                startNetwork();
            }
            else
            {
                MessageBox.Show("SSID should be more than 8 characters without spaces or special characters, wifi should be open", "3ASBA YZZI BLA TA8FIS", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Stop);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            //sendTcp();
             this.WindowState =  FormWindowState.Minimized;
        }

        private void Button4_Click(object sender, EventArgs e)
        {

        }

        void startNetwork()
        {
            Process SetLan = new Process();
            SetLan.StartInfo.FileName = "netsh.exe";
            SetLan.StartInfo.Arguments = "wlan set hostednetwork mode=allow ssid=" + textBox1.Text + " key=" + textBox2.Text;
            SetLan.StartInfo.UseShellExecute = false;
            SetLan.StartInfo.RedirectStandardOutput = true;
            SetLan.StartInfo.CreateNoWindow = true;

            SetLan.Start();
            //string output = SetLan.StandardOutput.ReadToEnd();
            //MessageBox.Show(output);

            Process LaunchLan = new Process();
            LaunchLan.StartInfo.FileName = "netsh.exe";
            LaunchLan.StartInfo.Arguments = "wlan start hostednetwork";
            LaunchLan.StartInfo.UseShellExecute = false;
            LaunchLan.StartInfo.RedirectStandardOutput = true;
            LaunchLan.StartInfo.CreateNoWindow = true;

            LaunchLan.Start();

        }
    }
}
