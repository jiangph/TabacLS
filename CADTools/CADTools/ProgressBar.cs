using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace CADTools
{
    public partial class ProgressBar : Form
    {
        /// <summary>
        /// 步进值
        /// </summary>
        public ProgressBar(int minValue, int currentValue, int maxValue)
        {
            InitializeComponent();

            this.progressBar1.Value = currentValue;
            this.progressBar1.Minimum = minValue;
            this.progressBar1.Maximum = maxValue;
        }

        public void process(int CurrentValue)
        {
            this.progressBar1.Value = CurrentValue;
            this.updateLable();
        }

        public void setTitle(String text)
        {
            this.title.Text = text;
        }

        private void updateLable()
        {
            this.title.Update();
        }
    }
}
