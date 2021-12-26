using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lexical;
namespace LexicalGui 
{
    public partial class Form1 : Form
    {
        Lexical.access acc = new access();
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            textBox2.Text = "";
            textBox3.Text = "";
            Lexical.Program.res = "";

            Lexical.Program.table = "";
            // send file to Main
            string te = textBox1.Text;
            acc.insert(te);
            textBox2.Text += Lexical.Program.res;
            textBox3.Text += Lexical.Program.table;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "choose Text file";
            fdlg.Filter = "txt files (*.txt)|*.txt";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = System.IO.File.ReadAllText(fdlg.FileName);
            }
        }
    }
}
