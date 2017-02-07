using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace ProjectSizeCalculator
{
    /// <summary>
    /// form to show size of project
    /// </summary>
    public partial class ProjectSizeForm : Form
    {
        #region Members

        private readonly List<string> _list;

        #endregion

        #region Constructors

        public ProjectSizeForm()
        {
            InitializeComponent();
            _list = new List<string> {".cs", ".aspx", ".js", ".css", ".asmx", ".htm", ".ascx", ".asax"};
        }

        #endregion

        #region Private Methods

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                var count = 0;
                treeView1.Nodes.Add(CalculateFolder(folderBrowserDialog1.SelectedPath, ref count));
            }
        }

        private TreeNode CalculateFolder(string folder, ref int count)
        {
            var treeNode = new TreeNode();
            var info = new DirectoryInfo(folder);
            int[] intCounter = {0};
            treeNode.Nodes.AddRange(Directory.GetDirectories(folder).Select(strInnerFolder => CalculateFolder(strInnerFolder, ref intCounter[0]))
                .Where(objNode => objNode.Nodes.Count > 0).ToArray());

            for (var index = 0; index < Directory.GetFiles(folder).Length; index++)
            {
                var innerFile = Directory.GetFiles(folder)[index];
                var fileInfo = new FileInfo(innerFile);
                if (_list.Contains(fileInfo.Extension))
                {
                    treeNode.Nodes.Add(CalculateFile(innerFile, ref intCounter[0]));
                }
            }
            treeNode.Tag = count;
            treeNode.Text = info.Name + @" : " + intCounter[0] + @" lines";
            treeNode.Checked = true;
            count += intCounter[0];
            return treeNode;
        }

        private static TreeNode CalculateFile(string file, ref int count)
        {
            var treeNode = new TreeNode();
            var info = new FileInfo(file);
            var stream = new StreamReader(file);
            var counter = 0;
            while (!stream.EndOfStream)
            {
                stream.ReadLine();
                counter++;
            }
            stream.Close();
            treeNode.Tag = counter;
            treeNode.Text = info.Name + @" : " + counter + @" lines";
            treeNode.Checked = true;
            count += counter;
            return treeNode;
        }

        #endregion
    }
}