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
        private string _hobbit;
        private Dictionary<char, int> _frequencydictionary;

        public Form1()
        {
            InitializeComponent();
            _frequencydictionary = new Dictionary<char, int>();                                
        }

        private void LoadBtn_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string path = Path.GetExtension(openFileDialog1.FileName);
                if (path == ".txt")
                {
                    BackgroundWorker bw = new BackgroundWorker();
                    _hobbit = FileReader.ReadFile(openFileDialog1.FileName);
                    if (_hobbit == null)
                        return;
                    textBox1.Text = _hobbit;
                    Task.Factory.StartNew(() => _frequencydictionary = FileReader.FrequencyLookup(_hobbit));
                }
                else if (path == ".McFile")
                {
                    List<byte> byteList = new List<byte>();
                    byteList.AddRange(File.ReadAllBytes(openFileDialog1.FileName));
                    
                    UInt64 length = BitConverter.ToUInt64(byteList.ToArray(), 0);

                    for (int i = 0; i < 8; i++)
                        byteList.RemoveAt(0);

                    Stream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    BinaryReader reader = new BinaryReader(stream);

                    byte[] numberForBytes = new byte[] { 128, 64, 32, 16, 8, 4, 2, 1 };
                    foreach (var b in byteList)
                    {
                        for (var i = 0; i < 8; i++)
                        {
                            writer.Write((b & numberForBytes[i]) != 0);
                        }
                    }
                    writer.Flush();

                    Huffman huffman = new Huffman();
                    Tree tree = new Tree();
                    long position = tree.FromBytes(byteList.ToArray());

                    reader.BaseStream.Position = position;

                    textBox1.Text = huffman.Decode(tree, reader, length);
                }
            }          
        }

        private void SAveBtn_Click(object sender, EventArgs e)
        {
            if (_frequencydictionary == null)
              return;  
            if(_hobbit==null)
                return;
            
            Huffman huffman = new Huffman();
            Tree tree = huffman.CreateTree(_frequencydictionary);
            Tree.Print(tree.RootNode);
            Stream stream= huffman.Encode(tree, _hobbit);

            BinaryReader dataReader = new BinaryReader(stream);
            List<byte> bytes=new List<byte>();
            byte[] numberforbytes={ 128, 64, 32, 16, 8, 4, 2, 1 };
            bool[] bits=new bool[8];
            int position = 0;           

            Stream treeBitStream = tree.ToBits();
            BinaryReader treeReader = new BinaryReader(treeBitStream);

            UInt64 treeLength = (ulong) treeReader.BaseStream.Length;
            UInt64 dataLength = (ulong) dataReader.BaseStream.Length;
            UInt64 length = treeLength + dataLength;
            ulong realLength = (length + (8 - (length % 8)));

            bytes.AddRange(BitConverter.GetBytes(length));
            
            for (var i = 0; i < (int) treeLength; i++)
            {
                bits[position] = i < (int) treeLength && treeReader.ReadBoolean();
                position = (position + 1) % 8;

                if (position != 0) continue;

                byte b = 0;
                for (var j = 0; j < bits.Length; j++)
                {
                    b = (byte)(bits[j] ? b + numberforbytes[j] : b);
                }
                bytes.Add(b);
            }

            for (var i = 0; i < (int) (realLength - treeLength); i++)
            {
                bits[position] = i < (int) dataLength && dataReader.ReadBoolean();
                position = (position + 1) % 8;
                if (position != 0) continue;

                byte b = 0;
                for (var j = 0; j < bits.Length; j++)
                {
                    b = (byte) (bits[j] ? b+numberforbytes[j] : b);
                }
                bytes.Add(b);
            }

            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            Console.WriteLine(saveFileDialog1.FileName);
            File.WriteAllBytes(saveFileDialog1.FileName, bytes.ToArray());
        }

        private void ClearBtn_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            _hobbit = "";
            _frequencydictionary.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            pictureBox1.ImageLocation= Path.Combine(Environment.CurrentDirectory, "gang.jpg");
        }

       
    }
}
