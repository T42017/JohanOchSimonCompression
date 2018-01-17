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
            List<Node> nodeList = new List<Node>();

            foreach (KeyValuePair<char, int> t in frequency)
            {
                nodeList.Add(new Node(null, t.Key, t.Value));
            }

            while (nodeList.Count != 1)
            {
                Node[] nodes = GetTwoLeastFrequentNodes(nodeList);

                Node newNode = new Node(nodes[0], nodes[1], null)
                {
                    Frequency = nodes[0].Frequency + nodes[1].Frequency
                };

                nodes[0].ParentNode = newNode;
                nodes[1].ParentNode = newNode;

                nodeList.Remove(nodes[0]);
                nodeList.Remove(nodes[1]);
                nodeList.Add(newNode);
            }

            tree.RootNode = nodeList.First();

            return tree;
        }

        public Node[] GetTwoLeastFrequentNodes(List<Node> nodeList)
        {
            Node[] nodes = new Node[2];

            foreach (Node node in nodeList)
            {
                if (nodes[0] == null || node.Frequency < nodes[0].Frequency)
                {
                    nodes[0] = node;
                }
                else if (nodes[1] == null || node.Frequency < nodes[1].Frequency)
                {
                    nodes[1] = node;
                }
            }

            return nodes;
        }

        public Stream Encode(Tree tree, String data)
        {
            Stream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            foreach (char character in data)
            {
                Node node = tree.FindByValue(tree.RootNode, character);
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

        public void Decode(Tree tree, BinaryReader reader)
        {
            List<bool> bits = new List<bool>();

            for (int i = 0; i < reader.BaseStream.Length; i++)
            {
                bits.Add(reader.ReadBoolean());
            }

            Node currentNode = tree.RootNode;
            StringBuilder decodedString = new StringBuilder();
            foreach (bool b in bits)
            {
                if (currentNode == null) throw new Exception("Corrupt file");

                currentNode = b ? currentNode.LeftNode : currentNode.RightNode;

                if (!currentNode.IsLeaf) continue;

                decodedString.Append(currentNode.Value);
                currentNode = tree.RootNode;
            }

            Console.WriteLine(decodedString.ToString());
        }

    }
}
