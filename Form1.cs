using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;    //additional
using System.Threading;    //additional


namespace RS232
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        string TxtComDefault = "COM1";
        string TxtBaudDefault = "9600";
        string TxtDataBitDefault = "7";

        string[] ParityChoose = new string[] { "None", "Odd", "Even" };

        COMPROT COMTMP = new COMPROT();



        private void Form1_Load(object sender, EventArgs e)
        {
            TxtCom.Text = TxtComDefault;
            TxtBaud.Text = TxtBaudDefault;
            TxtDataBit.Text = TxtDataBitDefault;

            ComboBoxParity.Items.AddRange(ParityChoose);
            ComboBoxParity.SelectedItem = ParityChoose[2];
        }





        private void BtnDefault_Click(object sender, EventArgs e)
        {
            TxtCom.Text = TxtComDefault;
            TxtBaud.Text = TxtBaudDefault;
            ComboBoxParity.SelectedItem = ParityChoose[2];
            TxtDataBit.Text = TxtDataBitDefault;
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {

            COMTMP.Initialize(TxtCom.Text, TxtBaud.Text, ParityChoose[ComboBoxParity.SelectedIndex], TxtDataBit.Text);


        }

        private void BtnDisconnect_Click(object sender, EventArgs e)
        {
            COMTMP.Disconnect();
        }


        private void BtnWrite_Click(object sender, EventArgs e)
        {
            COMTMP.Write(TxtWrite.Text);
        }



        ////////////////////
        //Timer
        ////////////////////
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (COMTMP.Tmp.IsOpen == true)
            {
                BtnConnect.BackColor = Color.LimeGreen;
            }
            else
            {
                BtnConnect.BackColor = Color.Red;
            }
            
            if (COMTMP.TmpLog != "")
            {
                UpdateTextBox(ref TxtLog, ref COMTMP.TmpLog);
            }

            COMTMP.Read();


        }


        ////////////////////
        //method
        ////////////////////
        public void UpdateTextBox(ref TextBox textbox, ref string strNew)
        {
            textbox.Text += strNew + "\r\n";
            textbox.SelectionStart = textbox.Text.Length;
            textbox.ScrollToCaret();
            strNew = "";
        }








        ////////////////////
        //CLASS
        ////////////////////
        public class COMPROT
        {
            public SerialPort Tmp = new SerialPort();

            public string TmpLog = "";
            public string ReadTmp = "";




            public void Initialize(string name, string baud, string pty, string data)
            {
                TryReOpen();

                try
                {
                    Tmp.PortName = name;
                    Tmp.BaudRate = Convert.ToInt32(baud);
                    Tmp.DataBits = Convert.ToInt32(data);

                    Tmp.Parity = Parity.None;
                    if (pty == "Odd") Tmp.Parity = Parity.Odd;
                    if (pty == "Even") Tmp.Parity = Parity.Even;

                }
                catch
                {
                    TmpLog += AddTime() + Tmp.PortName + "  Conncet Parameter error!\r\n";
                }

                this.Connect();

            }


            public void TryReOpen()
            {
                if (Tmp.IsOpen == true)
                {
                    TmpLog += AddTime() + Tmp.PortName + "  try to re-open.....\r\n";
                    this.Disconnect();
                }
            }


            public void Connect()
            {
                TryReOpen();

                try
                {
                    Tmp.Open();
                }
                catch
                {
                    TmpLog += AddTime() + Tmp.PortName + "  Open() error!\r\n";
                }


                if (Tmp.IsOpen == true)
                {
                    TmpLog += AddTime() + Tmp.PortName + "  Open sussess!\r\n";
                }
                else
                {
                    TmpLog += AddTime() + Tmp.PortName + "  Open fail...\r\n";
                }

            }



            public void Disconnect()
            {
                try
                {
                    Tmp.Close();
                }
                catch
                {
                    TmpLog += AddTime() + Tmp.PortName + "  Close() error!\r\n";
                }


                if (Tmp.IsOpen == false)
                {
                    TmpLog += AddTime() + Tmp.PortName + "  Close success!\r\n";
                }
                else
                {
                    TmpLog += AddTime() + Tmp.PortName + "  Close fail...\r\n";
                }

            }


            public void Read()
            {
                if (Tmp.IsOpen == false)
                {
                    return;
                }
                else
                {
                    ReadTmp = Tmp.ReadExisting();
                }



                if (ReadTmp == "")
                {
                    return;
                }

                try
                {
                    TmpLog += AddTime() + "< " + ReadTmp + "\r\n";
                }
                catch
                {
                    TmpLog += AddTime() + Tmp.PortName + "  Read fail...\r\n";
                }

                ReadTmp = "";
            }



            public void Write(string str)
            {
                try
                {
                    Tmp.Write(str + "\n");
                    TmpLog += AddTime() + ">>>>> " + str + "\r\n";
                }
                catch
                {
                    TmpLog += AddTime() + Tmp.PortName + "  Write fail...\r\n";
                }
            }





            public string AddTime()
            {
                return (DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "    ");
            }


        }


    }
}
