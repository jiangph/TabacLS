using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdObjects;

namespace CADTools
{
    public partial class PrintDocument : Form
    {
        private Graph graph;
        private List<Figure> figures;

        public PrintDocument(Graph graph)
        {
            InitializeComponent();
            this.graph = graph;
            this.figures = graph.getFigures();
            this.loadFigures();
        }

        public void loadFigures()
        {
            int count = graph.getFigures().Count;
            List<vdPolyline> rootPolyLine = graph.getRootPolyline();
            graph.analyzeTextGraph(rootPolyLine);
            List<vdText> subProjectNames = graph.getSubProjectNames();
            List<vdText> projectNumbers = graph.getProjectNumbers();
            List<vdText> projectNames = graph.getProjectNames();
            for (int i = 0; i < count; i++)
            {
                String name = projectNumbers[i].TextString + " " +
                               projectNames[i].TextString;
                this.dataGridView1.Rows.Add();
                this.dataGridView1.Rows[i].Cells[1].Value = name;
                this.dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString(); 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                List<int> lists = new List<int>();
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    if (dataGridView1.Rows[i].Cells[0].EditedFormattedValue.Equals(true))
                    {
                        lists.Add(i);         
                    }
                }
                int count = lists.Count;
                if (count > 0)
                {
                    MessageBox.Show("当前有" + count + "个打印任务!");
                    String path = this.getPath();
                    for (int i = 0; i < count; i++)
                    {
                        int idx = lists[i];
                        print(figures[idx], idx + 1, path);
                    }
                    MessageBox.Show("打印完毕!");
                }
                else
                {
                    MessageBox.Show("当前未选择任何打印任务！");
                }
            }
            catch
            {
                MessageBox.Show("打印异常！");
            }            
        }

        private void print(Figure figure,int index,String path)
        {
            vdPrint printer = figure.getDocument().Model.Printer;
            //printer.DocumentName = lineNo.ToString();
            printer.PrinterName = path + "\\" + index+ ".pdf";
            printer.Resolution = 300;
            printer.OutInBlackWhite = true;
            printer.paperSize = new System.Drawing.Rectangle(0, 0, 827, 1169);//A4
            printer.LandScape = false;
            printer.PrintExtents();
            printer.PrintScaleToFit();
            printer.CenterDrawingToPaper();
            printer.PrintOut();
        }

        private String getPath()
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            String path = Application.StartupPath + "printResults\\PDF";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                path = fbd.SelectedPath;
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            return path;
        }

        /// <summary>
        /// 取消按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
