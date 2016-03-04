using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CADTools
{

    public partial class ChooseDirectory : Form
    {
        public string directName = "";
        public int subProjectNum;

        public ChooseDirectory(int num)
        {
            InitializeComponent();
            this.subProjectNum = num;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            directName = this.comboBox1.SelectedValue.ToString();
            DialogResult = DialogResult.OK;
            this.Close();           
        }

        private void directionary_Load(object sender, EventArgs e)
        {
            String xmlpath = Application.StartupPath + "\\" + "border" + ".xml";
            DataSet xmlread = new DataSet();
            xmlread.ReadXml(xmlpath);
            //xmlread.ReadXml(xmlpath);
            comboBox1.DataSource = xmlread.Tables[0];
            comboBox1.DisplayMember = "name";
            comboBox1.ValueMember = "blockname";
            this.label2.Text = subProjectNum + "";
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.comboBox1.SelectedValue.ToString().Equals("目录"))
            {
                this.label4.Text = (subProjectNum / 24 + 1) + "";
            }
            else
            {
                label4.Text = subProjectNum / 24 + "";
            }
        }

        public string getDirectoryName()
        {
            return directName;
        }

    }
}
