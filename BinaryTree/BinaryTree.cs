using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BinaryTree
{
  public class BinaryTree<T> : IEnumerable<T>
    {
        private TreeNode<T> Root { get; set; }
        public IComparer<T> _comparer = Comparer<T>.Default;
        public delegate void TreeEventHandler(object sender, TreeEventArgs<T> args);
        public event TreeEventHandler ElementAdded;
        public event TreeEventHandler ElementRemoved;
        public int Count { get; private set; }
        public BinaryTree()
        {
            if (!typeof(IComparable<T>).IsAssignableFrom(typeof(T)))
                throw new ArgumentException("impossible to compare");
            _comparer = Comparer<T>.Default;

            ElementAdded += (sender, args) => { };
            ElementRemoved += (sender, args) => { };

        }
        public BinaryTree(IComparer<T> comparer)
        {
            _comparer = comparer;
            ElementAdded += (sender, args) => { };
            ElementRemoved += (sender, args) => { };
        }
        public void Add(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            else if (Count == 0)
            {
                Root = new TreeNode<T>(item);
                Count++;
            }
            else
            {
                TreeNode<T> current = Root;
                while(current != null)
                {
                    if(_comparer.Compare(item, current.Data) >= 0)
                    {
                        if (current.Right == null)
                        {
                            current.Right = new TreeNode<T>(item);
                            break;
                        }
                        else current = current.Right;
                    }
                    else if (_comparer.Compare(item, current.Data) < 0)
                    {
                        if (current.Left == null)
                        {
                            current.Left = new TreeNode<T>(item);
                            break;
                        }
                        else current = current.Left;
                    }
                }
                Count++;
                ElementAdded(this, new TreeEventArgs<T>(item, " is added"));
            }
        }
        TreeNode<T> deleteRec(TreeNode<T> root, T key)
        {
            /* Base Case: If the tree is empty */
            if (root == null)
                return root;

            /* Otherwise, recur down the tree */
            if (_comparer.Compare(key, root.Data) < 0)
                root.Left = deleteRec(root.Left, key);
            else if (_comparer.Compare(key, root.Data) > 0)
                root.Right = deleteRec(root.Right, key);

            // if key is same as root's key, then This is the
            // node to be deleted
            else
            {
                // node with only one child or no child
                if (root.Left == null)
                    return root.Right;
                else if (root.Right == null)
                    return root.Left;

                // node with two children: Get the
                // inorder successor (smallest
                // in the right subtree)
                //root.key = minValue(root.right);
                var next = new BinaryTree<T>();
                next.Root = root.Right;
                root.Data = next.TreeMin();

                // Delete the inorder successor
                root.Right = deleteRec(root.Right, root.Data);
            }
            return root;
        }
        private TreeNode<T> DeleteNode(TreeNode<T> root, TreeNode<T> deleteNode)
        {
            if (root == null)
            {
                return root;
            }
            if (_comparer.Compare(deleteNode.Data, root.Data) == -1)
            {
                root.Left = DeleteNode(root.Left, deleteNode);
            }
            if (_comparer.Compare(deleteNode.Data, root.Data) == 1)
            {
                root.Right = DeleteNode(root.Right, deleteNode);
            }
            if (_comparer.Compare(deleteNode.Data, root.Data) == -0)
            {

                if (root.Left == null && root.Right == null)
                {
                    root = null;
                    return root;
                }
                else if (root.Left == null)
                {
                    TreeNode<T> temp = root;
                    root = root.Right;
                    temp = null;
                }
                else if (root.Right == null)
                {
                    TreeNode<T> temp = root;
                    root = root.Left;
                    temp = null;
                }
                else
                {
                    TreeNode<T> min = new TreeNode<T>(MinValue(root.Right));
                    ElementRemoved(sender: min.Data, new TreeEventArgs<T>(min.Data, " is removed"));
                    root.Data = min.Data;
                    root.Right = DeleteNode(root.Right, min);
                }
            }

            return root;
        }
        private T MinValue(TreeNode<T> node)
        {
            var min = node.Data;

            while (node.Left != null)
            {
                min = node.Left.Data;
                node = node.Left;
            }

            return min;
        }
        public bool Remove(T item)
        {
            if (item == null)
                return false;
            if (!Contains(item))
                return false;

            TreeNode<T> deleteNode = new TreeNode<T>(item);
            Root = DeleteNode(Root, deleteNode);
            Count--;
            ElementRemoved(deleteNode, new TreeEventArgs<T>(deleteNode.Data, "removed"));
            return true;
            /*if (item == null) return false;
            else if (!Contains(item)) return false;
            else
            {
                Root = deleteRec(Root, item);
                if (Root != null)
                {
                    ElementRemoved(this, new TreeEventArgs<T>(item, " is removed"));
                    Count--;
                    return true;
                }
                else return false;
            }*/

            /*if (item == null) return false;
            else
            {
                if (Root == null)
                return false;

            TreeNode<T> current = Root, parent = null;

            int result;
            do
            {
                result = _comparer.Compare(item, current.Data);
                if (result < 0)
                {
                    parent = current;
                    current = current.Left;
                }
                else if (result > 0)
                {
                    parent = current;
                    current = current.Right;
                }
                if (current == null)
                    return false;
            }
            while (result != 0);

            if (current.Right == null)
            {
                if (current == Root)
                    Root = current.Left;
                else
                {
                    result = _comparer.Compare(current.Data, parent.Data);
                    if (result < 0)
                        parent.Left = current.Left;
                    else
                        parent.Right = current.Left;
                }
            }
            else if (current.Right.Left == null)
            {
                current.Right.Left = current.Left;
                if (current == Root)
                    Root = current.Right;
                else
                {
                    result = _comparer.Compare(current.Data, parent.Data);
                    if (result < 0)
                        parent.Left = current.Right;
                    else
                        parent.Right = current.Right;
                }
            }
            else
            {
                TreeNode<T> min = current.Right.Left, prev = current.Right;
                while (min.Left != null)
                {
                    prev = min;
                    min = min.Left;
                }
                prev.Left = min.Right;
                min.Left = current.Left;
                min.Right = current.Right;

                if (current == Root)
                    Root = min;
                else
                {
                    result = _comparer.Compare(current.Data, parent.Data);
                    if (result < 0)
                        parent.Left = min;
                    else
                        parent.Right = min;
                }
            }
                --Count;
                ElementRemoved(this, new TreeEventArgs<T>(item, " is removed"));
                return true;
        }*/
        }
        public T TreeMax()
        {
            if (Count == 0)
                throw new InvalidOperationException();

            TreeNode<T> current = Root;

            while (current.Right != null)
                current = current.Right;

            return current.Data;
        }
        public T TreeMin()
        {
            if (Count == 0)
                throw new InvalidOperationException();

            TreeNode<T> current = Root;

            while (current.Left != null)
                current = current.Left;

            return current.Data;

        }
        public bool Contains(T data)
        {
            if (data == null) return false; //throw new ArgumentNullException(nameof(data));
            else if (Root == null) return false;
            else
            {
                var current = Root;
                while (current != null)
                {
                    if (_comparer.Compare(data, current.Data) == 0) return true;
                    else if (_comparer.Compare(data, current.Data) > 0)
                    {
                        current = current.Right;
                    }
                    else if (_comparer.Compare(data, current.Data) < 0)
                    {
                        current = current.Left;
                    }
                }
            }
            return false;
        }
        public IEnumerable<T> Traverse(TraverseType traverseType)
        {
            switch (traverseType)
            {
                case TraverseType.InOrder:
                    if (Root == null)
                        yield break;

                    var stack = new Stack<TreeNode<T>>();
                    var node = Root;

                    while (stack.Count > 0 || node != null)
                    {
                        if (node == null)
                        {
                            node = stack.Pop();
                            yield return node.Data;
                            node = node.Right;
                        }
                        else
                        {
                            stack.Push(node);
                            node = node.Left;
                        }
                    }
                    break;
                case TraverseType.PreOrder:
                    if (Root == null)
                        yield break;

                    stack = new Stack<TreeNode<T>>();
                    stack.Push(Root);

                    while (stack.Count > 0)
                    {
                        node = stack.Pop();
                        yield return node.Data;
                        if (node.Right != null)
                            stack.Push(node.Right);
                        if (node.Left != null)
                            stack.Push(node.Left);
                    }
                    break;
                case TraverseType.PostOrder:
                    if (Root == null)
                        yield break;

                    stack = new Stack<TreeNode<T>>();
                    node = Root;

                    while (stack.Count > 0 || node != null)
                    {
                        if (node == null)
                        {
                            node = stack.Pop();
                            if (stack.Count > 0 && node.Right == stack.Peek())
                            {
                                stack.Pop();
                                stack.Push(node);
                                node = node.Right;
                            }
                            else
                            {
                                yield return node.Data;
                                node = null;
                            }
                        }
                        else
                        {
                            if (node.Right != null)
                                stack.Push(node.Right);
                            stack.Push(node);
                            node = node.Left;
                        }
                    }
                    break;
            }
        }
        public IEnumerator<T> GetEnumerator()
        {
            if (Root == null)
                yield break;

            var stack = new Stack<TreeNode<T>>();
            var node = Root;

            while (stack.Count > 0 || node != null)
            {
                if (node == null)
                {
                    node = stack.Pop();
                    yield return node.Data;
                    node = node.Right;
                }
                else
                {
                    stack.Push(node);
                    node = node.Left;
                }
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (Root == null)
                yield break;

            var stack = new Stack<TreeNode<T>>();
            var node = Root;

            while (stack.Count > 0 || node != null)
            {
                if (node == null)
                {
                    node = stack.Pop();
                    yield return node.Data;
                    node = node.Right;
                }
                else
                {
                    stack.Push(node);
                    node = node.Left;
                }
            }
        }
    }
}
