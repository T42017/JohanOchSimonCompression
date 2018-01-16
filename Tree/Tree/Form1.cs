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
                    hobbit = File.ReadAllText(openFileDialog1.FileName);

                    if (hobbit == null)
                        return;
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
                else if (path == ".McFile")
                {
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
            for (int i = 0; i < reader.BaseStream.Length; i++)
            {
                bits[position] = reader.ReadBoolean();
                position = (position + 1)%8;
                if (position == 0)
                {
                    byte b = 0;
                    for (int j = 0; j < bits.Length; j++)
                    {
                        b = (byte) (bits[j]?b+numberforbytes[j]:b);
                    }
                    bytes.Add(b);
                }
            }
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
            pictureBox1.ImageLocation = System.IO.Directory.GetCurrentDirectory() + "/gang.jpg";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
              
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
