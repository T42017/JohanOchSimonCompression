using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree
{
    public class Huffman
    {

        public Tree CreateTree(Dictionary<Char, int> frequency)
        {
            var tree = new Tree();

            foreach (KeyValuePair<char, int> t in frequency)
            {
                tree.Add(new Node(null, t.Key, t.Value));
            }

            while (!tree.HasSingelRootNode())
            {
                Node[] nodes = tree.GetTwoLeastFrequentNodes();

                Node newNode = new Node(nodes[0], nodes[1], null)
                {
                    Frequency = nodes[0].Frequency + nodes[1].Frequency
                };

                nodes[0].ParentNode = newNode;
                nodes[1].ParentNode = newNode;

                tree.RemoveRange(nodes);
                tree.Add(newNode);
            }

            return tree;
        }

        public Stream Encode(Tree tree, String data)
        {
            Stream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            foreach (char character in data)
            {
                Node node = tree.FindByValue(tree.GetRootNodes().FirstOrDefault(), character);
                List<bool> bits = tree.GetPathToNode(node);
                
                foreach (var bit in bits)
                {
                    writer.Write(bit);
                }
            }
            writer.Flush();
            writer.BaseStream.Position = 0;

            return stream;
        }

    }
}
