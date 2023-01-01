using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisPrCore.Datamodel
{
    public class TreeNode<T> where T : new()
    {
        public List<TreeNode<T>> Children { get; set; }
        public T Item { get; set; }

        public TreeNode() : this(new T())
        {
        }

        public TreeNode(T item)
        {
            Item = item;
            Children = new List<TreeNode<T>>();
        }

        public TreeNode<T> AddChild(T item)
        {
            TreeNode<T> nodeItem = new TreeNode<T>(item);
            Children.Add(nodeItem);
            return nodeItem;
        }
    }
}
