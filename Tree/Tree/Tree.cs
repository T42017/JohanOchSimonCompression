using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            writer.BaseStream.Position = 0;
            writer.Flush();
            return stream;
        }

        public void FromBits(Stream stream)
        {
            RootNode = null;
            BinaryReader reader = new BinaryReader(stream);
            reader.BaseStream.Position = 0;
            Node rootNode = null;
            Node lastNode = null;
            Node currentNode = null;
            int bitCount = 0;

            while (bitCount < reader.BaseStream.Length)
            {
                bool bit = reader.ReadBoolean();
                bitCount++;

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


                    if (rootNode == null)
                    {
                        rootNode = currentNode;
                        lastNode = rootNode;
                    }
                }
                else // 0 for leaf
                {
                    currentNode = new Node();
                    currentNode.IsLeaf = true;

                    byte[] numberForBytes = new byte[] { 1, 2, 4, 8, 16, 32, 64, 128 };
                    byte[] valueInByte =  new byte[Encoding.Unicode.GetByteCount(new char[1])];

                    for (int i = 0; i < valueInByte.Length; i++)
                    {
                        for (var j = 0; j < 8; j++)
                        {
                            bool bitForValue = reader.ReadBoolean();
                            bitCount++;
                            valueInByte[i] = (byte) (valueInByte[i] + (bitForValue ? numberForBytes[j] : 0));
                        }
                    }

                    currentNode.Value = Encoding.Unicode.GetChars(valueInByte)[0];

                    if (lastNode != null)
                    {
                        if (!lastNode.HasLeftNode)
                            lastNode.LeftNode = currentNode;
                        else if (!lastNode.HasRightNode)
                            lastNode.RightNode = currentNode;
                        
                        currentNode.ParentNode = lastNode;

                        lastNode = GetNodeWithOneChild(lastNode);
                    }
                    
                    if (currentNode.HasParentNode)
                        currentNode = currentNode.ParentNode;
                }
            }

            RootNode = rootNode;
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
