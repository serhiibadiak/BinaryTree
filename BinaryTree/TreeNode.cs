using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace BinaryTree
{
    class TreeNode<T> : IComparable<T>, IComparable
    {
        private readonly IComparer<T> _comparer;
        private T data = default;
        public T Data
        {
            get
            {
                return data;
            }

            set
            {
                if (value is null) throw new ArgumentNullException(nameof(value));
                else this.data = value;
            }

        }
        public TreeNode<T> Right { get; set; }
        public TreeNode<T> Left { get; set; }
        public TreeNode(T data)
        {
            this.Data = data;
        }
        public TreeNode(T data, TreeNode<T> left, TreeNode<T> right)
        {
            this.Data = data;
            this.Left = left;
            this.Right = right;
        }
        public TreeNode(IComparer<T> comparer)
        {
            _comparer = comparer;
        }

        public int CompareTo([AllowNull] T other)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
