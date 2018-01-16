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
        private string hobbit,b;
        private Dictionary<char, int> Frequencydictionary;
        public Form1()
        {
            InitializeComponent();
            Frequencydictionary = new Dictionary<char, int>();
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            hobbit = File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + "/lord_of_the_rings-chapter1.txt");
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
            foreach (KeyValuePair<char, int> varen in Frequencydictionary)
            {
                b=b+"\n"+String.Format("{0}, {1}",varen.Key, varen.Value);
            }           
            label3.Text = b;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
           
            pictureBox1.ImageLocation = Directory.GetCurrentDirectory() + "/gang.png";           
        }

       
    }
}
