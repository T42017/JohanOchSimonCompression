using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree
{
    public class Tree
    {
        private List<Node> NodeList;

        public Tree()
        {
            NodeList = new List<Node>();
        }

        public void Add(Node node)
        {
            NodeList.Add(node);
        }

        public void Remove(Node node)
        {
            NodeList.Remove(node);
        }

        public void RemoveRange(Node[] nodes)
        {
            foreach (Node node in nodes)
                NodeList.Remove(node);
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

        public Node[] GetTwoLeastFrequentNodes()
        {
            Node[] nodes = new Node[2];

            foreach (Node node in NodeList)
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

        public List<Node> GetRootNodes()
        {
            return NodeList.Where(node => node.ParentNode == null).ToList();
        }

        public bool HasSingelRootNode()
        {
            return GetRootNodes().Count <= 1;
        }

    }
}
