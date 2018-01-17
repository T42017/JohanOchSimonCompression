using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tree
{
    public class Node
    {
        public bool HasBeenWritten = false;

        private Node _leftNode;
        public Node LeftNode
        {
            get => _leftNode;
            set
            {
                HasLeftNode = value != null;
                _leftNode = value;
            }
        }

        private Node _rightNode;
        public Node RightNode
        {
            get => _rightNode;
            set
            {
                HasRightNode = value != null;
                _rightNode = value;
            }
        }

        private Node _parentNode;
        public Node ParentNode
        {
            get => _parentNode;
            set
            {
                HasParentNode = value != null;
                _parentNode = value;
            }
        }
        
        public char Value;
        public int Frequency;

        public bool HasLeftNode = false, HasRightNode = false, HasParentNode = false, IsLeaf = false;

        public Node() { }

        public Node(Node left, Node right, Node parentNode)
        {
            LeftNode = left;
            RightNode = right;
            ParentNode = parentNode;
        }

        public Node(Node parentNode, char value, int frequency)
        {
            IsLeaf = true;
            ParentNode = parentNode;
            Value = value;
            Frequency = frequency;
        }

    }
}
