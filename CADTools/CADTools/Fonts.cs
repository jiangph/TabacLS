using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Runtime.InteropServices;
using System.IO;
using System.Drawing.Text;

namespace CADTools
{
    public partial class Fonts : Form
    {
        private string fontName = "";

        public Fonts()
        {
            InitializeComponent();
            setfont();
        }

        private void setFont_Load(object sender, EventArgs e)
        {

        }



        private void savefont()
        {
            fontName = this.comboBox1.Text;
            //MessageBox.Show(fontname);
            //string fontfile = Application.StartupPath + "\\font" + "\\" + fontName;
            //PrivateFontCollection font = new PrivateFontCollection();
            //font.AddFontFile(fontfile);
            //Font myFont = new Font(font.Families[0].Name, 9F, FontStyle.Regular, GraphicsUnit.Point, ((byte)(134)));
            //this.label1.Font = myFont;
            this.Hide();
            Program.main_form.changeFontFile(fontName);
        }

        private void setfont()
        {
            string fontpath = Application.StartupPath + "\\font";
            DirectoryInfo TheFolder = new DirectoryInfo(fontpath);
            foreach (FileInfo nextfile in TheFolder.GetFiles())
            {
                this.comboBox1.Items.Add(nextfile.Name);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            savefont();
        }

        private String getFontFile()
        {
            return fontName;
        }



    }
}
