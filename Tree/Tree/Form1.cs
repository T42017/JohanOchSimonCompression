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
        private Dictionary<char, int> Frequencydictionary = new Dictionary<char, int>();
        private int antal;
        public Form1()
        {
            InitializeComponent();
            hobbit = File.ReadAllText(System.IO.Directory.GetCurrentDirectory()+"/lord_of_the_rings-chapter1.txt");
            label3.Text = hobbit;
            foreach (char karak in hobbit)
            { 
                if (Frequencydictionary.ContainsKey(karak))
                {
                    Frequencydictionary[karak]++;
                }
                else
                {
                   Frequencydictionary.Add(karak,1);
                }                
            }            
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
