using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tree
{
    public class Tree
    {
        public Node RootNode;

        public Tree()
        {
        }

        public List<bool> GetPathToNode(Node node)
        {
            List<bool> bits = new List<bool>();
            
            Node currentNode = node.ParentNode;
            Node lastNode = node;
            while (currentNode != null)
            {
                if (currentNode.HasLeftNode && currentNode.LeftNode.Equals(lastNode))
                    bits.Add(true);
                else if (currentNode.HasRightNode && currentNode.RightNode.Equals(lastNode))
                    bits.Add(false);

                lastNode = currentNode;
                currentNode = currentNode.ParentNode;
            }

            return bits;
        }

        public Node FindByValue(Node node, char value)
        {

            if (node.IsLeaf && node.Value == value)
                return node;

            if (node.HasLeftNode)
            {
                Node n1 = FindByValue(node.LeftNode, value);
                if (n1 != null) return n1;
            }

            if (node.HasRightNode)
            {
                Node n2 = FindByValue(node.RightNode, value);
                if (n2 != null) return n2;
            }

            return null;
        }

        public Stream ToBits()
        {
            Stream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            Node currentNode = RootNode;
            if (currentNode == null) throw new Exception("Nodes are empty");

            SetNodeAndChildrenToUnwritten(RootNode);

            while (true)
            {
                if (currentNode.IsLeaf)
                {
                    writer.Write(false); // 0 for leaf

                    byte[] numberForBytes = new byte[] {1, 2, 4, 8, 16, 32, 64, 128};
                    byte[] bytes = Encoding.Unicode.GetBytes(new[] {currentNode.Value});

                    foreach (byte b in bytes)
                    {
                        for (var j = 0; j < 8; j++)
                        {
                            byte newByte = (byte) (b & numberForBytes[j]);
                            writer.Write(newByte != 0);
                        }
                    }

                    currentNode.HasBeenWritten = true;
                    currentNode = currentNode.ParentNode;
                }
                else
                {
                    if (!currentNode.HasBeenWritten)
                    {
                        writer.Write(true); // 1 for node
                        currentNode.HasBeenWritten = true;
                    }

                    if (!currentNode.LeftNode.HasBeenWritten)
                        currentNode = currentNode.LeftNode;
                    else if (!currentNode.RightNode.HasBeenWritten)
                        currentNode = currentNode.RightNode;
                    else
                    {
                        if (!currentNode.HasParentNode)
                            break;
                        currentNode = currentNode.ParentNode;
                    }
                }
            }

            writer.Flush();
            writer.BaseStream.Position = 0;
            return stream;
        }

        public byte[] ToByte()
        {
            Stream stream = ToBits();
            BinaryReader reader = new BinaryReader(stream);

            List<byte> bytes = new List<byte>();
            byte[] numberforbytes = { 1, 2, 4, 8, 16, 32, 64, 128 };
            bool[] bits = new bool[8];
            int position = 0;

            for (var i = 0; i < reader.BaseStream.Length; i++)
            {
                bits[position] = reader.ReadBoolean();
                position = (position + 1) % 8;

                if (position != 0) continue;

                byte b = 0;
                for (var j = 0; j < bits.Length; j++)
                {
                    b = (byte) (bits[j] ? b + numberforbytes[j] : b);
                }
                bytes.Add(b);
            }

            return bytes.ToArray();
        }

        public void FromBits(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            reader.BaseStream.Position = 0;

            Node rootNode = null;
            Node lastNode = null;
            bool done = false;

            while (!done)
            {
                bool bit = reader.ReadBoolean();

                Node currentNode = null;
                if (bit) // 1 for node
                {
                    currentNode = new Node();

                    if (lastNode != null)
                    {
                        currentNode.ParentNode = lastNode;

                        if (!lastNode.HasLeftNode)
                            lastNode.LeftNode = currentNode;
                        else if (!lastNode.HasRightNode)
                            lastNode.RightNode = currentNode;

                        lastNode = GetNodeWithOneChild(currentNode);
                    }
                }
                else // 0 for leaf
                {
                    currentNode = new Node {IsLeaf = true};

                    byte[] numberForBytes = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 };
                    byte[] valueInByte =  new byte[Encoding.Unicode.GetByteCount(new char[1])];

                    for (var i = 0; i < valueInByte.Length; i++)
                    {
                        for (var j = 0; j < 8; j++)
                        {
                            var bitForValue = reader.ReadBoolean();
                            valueInByte[i] = (byte) (valueInByte[i] + (bitForValue ? numberForBytes[j] : 0));
                        }
                    }

                    char[] chars = Encoding.Unicode.GetChars(valueInByte);
                    currentNode.Value = chars[0];

                    for (int i = 0; i < chars.Length; i++)
                        Console.WriteLine(chars[i]);

                    if (lastNode != null)
                    {
                        if (!lastNode.HasLeftNode)
                            lastNode.LeftNode = currentNode;
                        else if (!lastNode.HasRightNode)
                            lastNode.RightNode = currentNode;
                    }
                    currentNode.ParentNode = lastNode;

                    lastNode = GetNodeWithOneChild(lastNode);

                    if (lastNode.ParentNode == null && lastNode.HasLeftNode && lastNode.HasRightNode)
                    {
                        break;
                    }
                }

                if (rootNode == null)
                {
                    rootNode = currentNode;
                    lastNode = rootNode;
                }

                if (lastNode == null) done = true;

            }

            RootNode = rootNode;
        }

        public void FromBytes(byte[] bytes)
        {
            Stream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            byte[] numberForBytes = new byte[] {1, 2, 4, 8, 16, 32, 64, 128};
            foreach (byte b in bytes)
            {
                for (var i = 0; i < 8; i++)
                {
                    writer.Write((b & numberForBytes[i]) != 0);
                }
            }
            writer.Flush();
            FromBits(stream);
        }

        public Node GetNodeWithOneChild(Node node)
        {
            if (!node.HasParentNode) return node;

            if (!node.IsLeaf && (!node.HasLeftNode || !node.HasRightNode)) return node;

            return GetNodeWithOneChild(node.ParentNode);
        }

        public void SetNodeAndChildrenToUnwritten(Node node)
        {
            node.HasBeenWritten = false;
            if (node.IsLeaf) return;

            SetNodeAndChildrenToUnwritten(node.LeftNode);
            SetNodeAndChildrenToUnwritten(node.RightNode);
        }

        public void Print(Node node)
        {
            if (node.IsLeaf)
                Console.WriteLine("leaf " + node.Value);
            else
            {
                Console.WriteLine("node " + node.Frequency);
            }

            if (node.HasLeftNode)
            {
                Console.WriteLine("left");
                Print(node.LeftNode);
            }
            if (node.HasRightNode)
            {
                Console.WriteLine("right");
                Print(node.RightNode);
            }
            Console.WriteLine("back");
        }
    }
}
