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
        
        public Form1()
        {
            hobbit = File.ReadAllText(@"C:\Users\usr\Downloads\lord_of_the_rings-chapter1.txt");
          
            InitializeComponent();
           
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
