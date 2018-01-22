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
                    byte[] byteList = File.ReadAllBytes(openFileDialog1.FileName);

                    Stream stream = new MemoryStream();
                    BinaryWriter writer = new BinaryWriter(stream);
                    BinaryReader reader = new BinaryReader(stream);
                    byte[] numberForBytes = { 128, 64, 32, 16, 8, 4, 2, 1 };

                    var firstByte = byteList[0];
                    var bitsNotUsed = 0;
                    for (int i = 0; i < 3; i++)
                    {
                        bitsNotUsed += (byte) ((firstByte & numberForBytes[i]) != 0 ? numberForBytes[i + 5] : 0);
                    }

                    for (var i = 0; i < byteList.Length; i++)
                    {
                        var ignoreEndBits = 0;
                        var ignoteStartBits = 0;

                        if (i == 0) // first byte
                            ignoteStartBits = 3;
                        else if (i == byteList.Length - 1) // last byte
                            ignoreEndBits = bitsNotUsed;

                        for (var j = ignoteStartBits; j < 8 - ignoreEndBits; j++)
                        {
                            writer.Write((byteList[i] & numberForBytes[j]) != 0);
                        }
                    }
                    writer.Flush();

                    Huffman huffman = new Huffman();
                    Tree tree = new Tree();
                    reader.BaseStream.Position = 0;
                    tree.FromBits(reader);

                    textBox1.Text = huffman.Decode(tree, reader);
                }
            }          
        }

        private void SAveBtn_Click(object sender, EventArgs e)
        {
            if (_frequencydictionary == null)
              return;  
            if(_hobbit==null)
                return;

            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;
            
            Huffman huffman = new Huffman();
            Tree tree = huffman.CreateTree(_frequencydictionary);
            Stream stream = huffman.Encode(tree, _hobbit);

            BinaryReader dataReader = new BinaryReader(stream);
            List<byte> bytes = new List<byte>();
            byte[] numberforbytes={ 128, 64, 32, 16, 8, 4, 2, 1 };
            bool[] bits = new bool[8];
            int position = 3;

            Stream treeBitStream = tree.ToBits();
            BinaryReader treeReader = new BinaryReader(treeBitStream);
            treeReader.BaseStream.Position = 0;

            long treeLength = treeReader.BaseStream.Length;
            long dataLength = dataReader.BaseStream.Length;
            ulong lengthInBits = (ulong) (treeLength + dataLength + 3);
            ulong fileLengthInBits = (lengthInBits + (8 - (lengthInBits % 8)));
            
            // Tree
            for (var i = position; i < (int) treeLength + position; i++)
            {
                bits[position] = treeReader.ReadBoolean();
                position = (position + 1) % 8;

                if (position != 0) continue;

                byte b = 0;
                for (var j = 0; j < bits.Length; j++)
                {
                    b = (byte)(bits[j] ? b + numberforbytes[j] : b);
                }
                bytes.Add(b);
            }

            var amountOfBitNoUsed = (byte)(fileLengthInBits - lengthInBits);
            for (var i = 0; i < 3; i++)
            {
                bytes[0] += (byte)((amountOfBitNoUsed & numberforbytes[i + 5]) != 0 ? numberforbytes[i] : 0);
            }

            // Data
            for (var i = 0; i < (int) dataLength + amountOfBitNoUsed; i++)
            {
                bits[position] = i < dataLength && dataReader.ReadBoolean();
                position = (position + 1) % 8;

                if (position != 0) continue;

                byte b = 0;
                for (var j = 0; j < bits.Length; j++)
                {
                    b = (byte) (bits[j] ? b + numberforbytes[j] : b);
                }
                bytes.Add(b);
            }
            
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
