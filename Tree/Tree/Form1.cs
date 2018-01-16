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
           
            //foreach (KeyValuePair<char, int> varen in Frequencydictionary)
            //{
            //    b = b + "\n" + String.Format("{0}, {1}", varen.Key, varen.Value);
            //}
            //label3.Text = b;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                hobbit = File.ReadAllText(openFileDialog1.FileName);
            }            
            textBox1.Text = hobbit;
            Frequencydictionary.Clear();
            foreach (char karak in hobbit)
            {
                if (Frequencydictionary.ContainsKey(karak))
                {
                    Frequencydictionary[karak]++;
                }
                else
                {
                    Frequencydictionary.Add(karak, 1);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }   



        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            hobbit = "";
            Frequencydictionary.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = System.IO.Directory.GetCurrentDirectory() + "/gang.jpg";
        }

       
    }
}
