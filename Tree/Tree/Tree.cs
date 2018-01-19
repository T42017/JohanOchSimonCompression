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

        public bool[] GetPathToNode(Node node)
        {
            List<bool> bits = new List<bool>();
            
            Node currentNode = node.ParentNode;
            Node lastNode = node;
            while (currentNode != null)
            {
                if (currentNode.HasLeftNode && currentNode.LeftNode.Equals(lastNode))
                    bits.Add(false);
                else if (currentNode.HasRightNode && currentNode.RightNode.Equals(lastNode))
                    bits.Add(true);

                lastNode = currentNode;
                currentNode = currentNode.ParentNode;
            }

            return bits.ToArray().Reverse().ToArray();
        }

        public Node FindByValue(Node startNode, char value)
        {
            if (startNode.IsLeaf && startNode.Value == value)
                return startNode;

            if (startNode.HasLeftNode)
            {
                Node n1 = FindByValue(startNode.LeftNode, value);
                if (n1 != null) return n1;
            }

            if (startNode.HasRightNode)
            {
                Node n2 = FindByValue(startNode.RightNode, value);
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

                    byte[] numberForBytes = new byte[] { 128, 64, 32, 16, 8, 4, 2, 1 };
                    byte[] bytes = Encoding.Unicode.GetBytes(new[] {currentNode.Value});

                    foreach (var b in bytes)
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
                        writer.Write(true); // 1 for startNode
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
        
        public long FromBits(Stream stream)
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
                if (bit) // 1 for startNode
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

                    byte[] numberForBytes = new byte[] { 128, 64, 32, 16, 8, 4, 2, 1 };
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
            return reader.BaseStream.Position;
        }

        public long FromBytes(byte[] bytes)
        {
            Stream stream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(stream);

            byte[] numberForBytes = new byte[] { 128, 64, 32, 16, 8, 4, 2, 1 };
            foreach (var b in bytes)
            {
                for (var i = 0; i < 8; i++)
                {
                    writer.Write((b & numberForBytes[i]) != 0);
                }
            }
            writer.Flush();
            return FromBits(stream);
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

        public static void Print(Node startNode)
        {
            if (startNode.IsLeaf)
                Console.WriteLine("leaf " + startNode.Value);
            else
            {
                Console.WriteLine("startNode " + startNode.Frequency);
            }

            if (startNode.HasLeftNode)
            {
                Console.WriteLine("left");
                Print(startNode.LeftNode);
            }
            if (startNode.HasRightNode)
            {
                Console.WriteLine("right");
                Print(startNode.RightNode);
            }
            Console.WriteLine("back");
        }
    }
}
