using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chat
{
    public partial class Form1 : Form
    {
        TcpClient clientSocket = new TcpClient();
        NetworkStream serverStream = default(NetworkStream);
        string readData = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            send();
        }

        private void send()
        {
            if (textBox2.Text != "")
            {
                textBox1.ReadOnly = false;
                byte[] outStream = Encoding.ASCII.GetBytes(textBox2.Text + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();
                textBox1.ReadOnly = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox3.Text != "" && textBox4.Text != "" && checkIP(textBox4.Text) && textBox4.Text.Contains("192.168."))
            {
                textBox1.ReadOnly = false;
                readData = "Conected to Chat Server ...";
                msg();
                clientSocket.Connect(textBox4.Text, 8888);
                serverStream = clientSocket.GetStream();

                byte[] outStream = Encoding.ASCII.GetBytes(textBox3.Text + "$");
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                Thread ctThread = new Thread(getMessage);
                ctThread.Start();

                button1.Enabled = true;
                button2.Enabled = false;
                textBox1.ReadOnly = true;
                textBox2.ReadOnly = false;
                textBox3.ReadOnly = true;
                textBox4.ReadOnly = true;
            }
        }

        private void getMessage()
        {
            while (true)
            {
                serverStream = clientSocket.GetStream();
                int buffSize = 0;
                byte[] inStream = new byte[65538];
                buffSize = clientSocket.ReceiveBufferSize;
                serverStream.Read(inStream, 0, buffSize);
                string returndata = Encoding.ASCII.GetString(inStream);
                readData = "" + returndata;
                msg();
            }
        }

        private void msg()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(msg));
            else if (textBox1.Text == "")
                textBox1.Text = textBox1.Text + " >> " + readData;
            else
                textBox1.Text = textBox1.Text + Environment.NewLine + " >> " + readData;
        }

        private bool checkIP(string input)
        {
            IPAddress address;
            if (IPAddress.TryParse(input, out address))
            {
                switch (address.AddressFamily)
                {
                    case AddressFamily.InterNetwork:
                        return true;
                        break;
                    default:
                        return false;
                        break;
                }
            }
            else
                return false;
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                send();
                textBox2.Text = "";
            }
        }
    }
}