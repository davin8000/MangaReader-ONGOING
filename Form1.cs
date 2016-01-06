using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using TreeViewSerialization;

namespace MangaReader
{
    public partial class Form1 : Form
    {
        TreeViewSerializer treexml = new TreeViewSerializer();
        string filename = "Manga.xml";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (File.Exists(filename)) treexml.DeserializeTreeView(treeView1, filename);//load saved treeview
            button2.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)//add manga
        {
            FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
            folderBrowserDialog1.ShowNewFolderButton = false;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                ListDirectory(treeView1, folderBrowserDialog1.SelectedPath);
                save_TreeView_State();
            }
        }

        private void save_TreeView_State()
        {
            treexml.SerializeTreeView(treeView1, filename);
            treeView1.Nodes.Clear();
            treexml.DeserializeTreeView(treeView1, filename);
        }

        private static void ListDirectory(TreeView treeView, string path)
        {
            var stack = new Stack<TreeNode>();
            var rootDirectory = new DirectoryInfo(path);
            var node = new TreeNode(rootDirectory.Name) { Tag = rootDirectory };//root folder node
            stack.Push(node);
            while (stack.Count > 0)
            {
                var currentNode = stack.Pop();
                var directoryInfo = (DirectoryInfo)currentNode.Tag;
                foreach (var directory in directoryInfo.GetDirectories())
                {
                    var childDirectoryNode = new TreeNode(directory.Name) { Tag = directory };//sub folder node
                    currentNode.Nodes.Add(childDirectoryNode);
                    stack.Push(childDirectoryNode);
                }
                foreach (var file in directoryInfo.GetFiles())
                {
                    if(file.Extension!=".db")//filter for thumbs.db
                        currentNode.Nodes.Add(new TreeNode(file.Name) { Tag = file });//file node
                }
            }
            treeView.Nodes.Add(node);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            /*TreeNode selected = null;
            try { selected = treeView1.SelectedNode; }
            catch { }
            try { selected = treeView1.SelectedNode.FirstNode; }
            catch { }
            try { selected = treeView1.SelectedNode.FirstNode.FirstNode; }
            catch { }
            treeView1.SelectedNode = selected;
            if (treeView1.SelectedNode != selected) treeView1_AfterSelect(this, e);*/
            string selected = null;
            try { selected = Path.Combine(treeView1.SelectedNode.Parent.Parent.Tag.ToString(), treeView1.SelectedNode.Parent.Tag.ToString(), treeView1.SelectedNode.Tag.ToString()); }
            catch { }
            try { selected = Path.Combine(treeView1.SelectedNode.Parent.Tag.ToString(), treeView1.SelectedNode.Tag.ToString(), treeView1.SelectedNode.FirstNode.Tag.ToString()); }
            catch { }
            try { selected = Path.Combine(treeView1.SelectedNode.Tag.ToString(), treeView1.SelectedNode.FirstNode.Tag.ToString(), treeView1.SelectedNode.FirstNode.FirstNode.Tag.ToString()); }
            catch { }
            label1.Text = selected;
            button2.Enabled = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //label1.Text = selected;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            treexml.SerializeTreeView(treeView1, filename);
        }

        private void treeView1_AfterExpand(object sender, TreeViewEventArgs e)
        {
            //save_TreeView_State();
        }
    }
}
