using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Softcam
{
    public partial class Form1 : Softcam.YahooForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
           
        //    FileStream Fs = new FileStream(@"C:\Documents and Settings\Alireza\Desktop\ali.txt", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        //    StreamWriter Sw = new StreamWriter(Fs);

        //    string[] FilesName = Directory.GetFiles(@"C:\Documents and Settings\Alireza\Desktop\Win7 Cursor");

        //    for (int i = 0; i < FilesName.Length; i++)
        //    {
        //        Sw.Write(FilesName[i] + "," + Environment.NewLine);
        //    }
        //    Sw.Close();
        //    Fs.Close();
        }

        private void Form1_HandledException(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

    }
}
