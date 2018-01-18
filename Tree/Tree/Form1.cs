using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tree
{
    public partial class Form1 : Form
    {
        private string hobbit;
        private Dictionary<char, int> Frequencydictionary;
        public Form1()
        {
            InitializeComponent();
            Frequencydictionary = new Dictionary<char, int>();
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;                                           
        }
        private void button1_Click(object sender, EventArgs e)
        {           
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = Path.GetExtension(openFileDialog1.FileName);
                if (path == ".txt")
                {
                    BackgroundWorker bw = new BackgroundWorker();
                    hobbit = FileReader.ReadFile(openFileDialog1.FileName);
                    if (hobbit == null)
                        return;
                    textBox1.Text = hobbit;
                    Task.Factory.StartNew(() => Frequencydictionary = FileReader.FrequencyLookup(hobbit));                  
                }
                else if (path == ".McFile")
                {
                    System.IO.File.ReadAllBytes(openFileDialog1.FileName);
                    //Huffman.decode
                }
            }          
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (Frequencydictionary == null)
              return;  

            if(hobbit==null)
                return;         
            
            Huffman huffman = new Huffman();
            Tree tree = huffman.CreateTree(Frequencydictionary);
            Stream stream= huffman.Encode(tree, hobbit);
            BinaryReader reader=new BinaryReader(stream);
            List<byte> bytes=new List<byte>();

            byte[] numberforbytes={1,2,4,8,16,32,64,128};
            bool[] bits=new bool[8];
            int position = 0;           
            
            Task.Factory.StartNew(() => bytes = FileReader.ByteConverter(reader, bits, numberforbytes, position));         

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(saveFileDialog1.FileName);
                System.IO.File.WriteAllBytes(saveFileDialog1.FileName, bytes.ToArray());
            }   
            
        }   
        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            hobbit = "";
            Frequencydictionary.Clear();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            
            pictureBox1.ImageLocation= Path.Combine(Environment.CurrentDirectory, "gang.jpg");
        }

       
    }
}
