using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tree
{
    public partial class Form1 : Form
    {
        private string hobbit;
        private int antal,antal1;
        public Form1()
        {
            InitializeComponent();
            hobbit = File.ReadAllText(System.IO.Directory.GetCurrentDirectory()+"/lord_of_the_rings-chapter1.txt");
            label3.Text = hobbit;

            foreach (char mellan in hobbit)
            {
                if(mellan == ' ')
                antal++;

                antal1++;
            }
            label1.Text = antal.ToString();
            label2.Text = antal1.ToString();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
