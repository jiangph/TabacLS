using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;


using VectorDraw.Professional.vdFigures;
//为了更新块新加的document
using VectorDraw.Generics;
using VectorDraw.Geometry;
using VectorDraw.Professional.Constants;
using VectorDraw.Professional.vdCollections;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdObjects;
using VectorDraw.Professional.vdPrimaries;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using Microsoft.Office.Interop.Excel;



namespace CADTools
{
    public partial class Information : Form
    {
       // private List<vdText> times;  //日期
       // private List<vdText> projectNumbers;    //图号
       // private List<vdText> projectNames;  //工程名
       // private List<vdText> P_projectNames; //子图名 
        //private List<vdText> ratio;//比例 
       // private List<vdText> projectStage;//设计阶段
        private List<vdPolyline> rootpolyline; //子图框个数
        //
        private vdDocument document;
        private Graph graph;

        public Information(Graph graph, List<vdText> times, List<vdText> projectNames, List<vdText> projectNumbers, List<vdText> P_projectNames, List<vdText> ratio, List<vdText> projectStage, List<vdPolyline> rootpolyline)
        {
            InitializeComponent();
            this.document = graph.getDocument();
            this.graph = graph;
            graph.times = times; //获得日期
            graph.projectNames = projectNames; //获得工程名
            graph.projectNumbers = projectNumbers; //获得图号
            graph.subProjectNames = P_projectNames; //获得子图名
            graph.ratios = ratio; //比例
            graph.projectStages = projectStage;   //设计阶段         
            this.rootpolyline = rootpolyline;

            this.showParameters();
            this.label1.Text = "当前工程的子图数目：" + graph.times.Count;
        }



        public void showParameters()
        {
            int count = rootpolyline.Count;
            for (int i = 0; i < count; i++)
            {
                this.dataGridView1.Rows.Add();

                this.dataGridView1.Rows[i].Cells[0].Value = i + 1 + "";

                this.dataGridView1.Rows[i].Cells[1].Value = graph.times[i].TextString;
                this.dataGridView1.Rows[i].Cells[2].Value = graph.projectNumbers[i].TextString;
                this.dataGridView1.Rows[i].Cells[3].Value = graph.projectNames[i].TextString;
                this.dataGridView1.Rows[i].Cells[4].Value = graph.subProjectNames[i].TextString;
                this.dataGridView1.Rows[i].Cells[5].Value = graph.ratios[i].TextString;
                this.dataGridView1.Rows[i].Cells[6].Value = graph.projectStages[i].TextString;
            }
        }

        //private void modify_Click(object sender, EventArgs e)
        //{
        //    int count = graph.times.Count;
        //    for (int i = 0; i < count; i++)
        //    {
        //        //修改各个部分
        //        graph.times[i].TextString = this.dataGridView1.Rows[i].Cells[1].Value.ToString();        //修改日期
        //        graph.times[i].Update();
        //        graph.projectNumbers[i].TextString = this.dataGridView1.Rows[i].Cells[2].Value.ToString();    //修改图号
        //        graph.projectNumbers[i].Update();
        //        graph.projectNames[i].TextString = this.dataGridView1.Rows[i].Cells[3].Value.ToString();  //修改项目名
        //        graph.projectNames[i].Update();
        //        graph.subProjectNames[i].TextString = this.dataGridView1.Rows[i].Cells[4].Value.ToString();    //修改子图名
        //        graph.subProjectNames[i].Update();
        //        graph.ratios[i].TextString = this.dataGridView1.Rows[i].Cells[5].Value.ToString();  //修改比例
        //        graph.ratios[i].Update();
        //        graph.projectStages[i].TextString = this.dataGridView1.Rows[i].Cells[6].Value.ToString(); //修改设计阶段
        //        graph.projectStages[i].Update();
        //    }
        //    //MainForm1 main = new MainForm1();
        //    for (int i = 0; i < document.Model.Entities.Count; i++)
        //    {
        //        string name = document.Model.Entities[i]._TypeName;
        //        if (name.Equals("vdInsert"))
        //        {
        //            vdInsert a = (vdInsert)document.Model.Entities[i];
        //            vdEntities entities = a.Explode();
        //            for (int j = 0; j < entities.Count; j++)
        //            {
        //                if (entities[j]._TypeName.Equals("vdText"))
        //                {
        //                    entities[j].Update();
        //                }
        //            }
        //        }
        //    }
        //    document.Redraw(true);
        //    this.Hide();

        //}

        private void modify_Click_1(object sender, EventArgs e)
        {
            int count = graph.times.Count;
            for (int i = 0; i < count; i++)
            {
                //修改各个部分
                graph.times[i].TextString = this.dataGridView1.Rows[i].Cells[1].Value.ToString();        //修改日期

                graph.times[i].Update();

                graph.projectNumbers[i].TextString = this.dataGridView1.Rows[i].Cells[2].Value.ToString();    //修改图号
                graph.projectNumbers[i].Update();

                graph.projectNames[i].TextString = this.dataGridView1.Rows[i].Cells[3].Value.ToString();  //修改项目名
                graph.projectNames[i].Update();

                graph.subProjectNames[i].TextString = this.dataGridView1.Rows[i].Cells[4].Value.ToString();    //修改子图名
                graph.subProjectNames[i].Update();

                graph.ratios[i].TextString = this.dataGridView1.Rows[i].Cells[5].Value.ToString();  //修改比例
                graph.ratios[i].Update();

                graph.projectStages[i].TextString = this.dataGridView1.Rows[i].Cells[6].Value.ToString(); //修改设计阶段
                graph.projectStages[i].Update();

            }
            graph.getLayout().Update();
            document.Redraw(true);
            graph.initialFigures();

            this.Hide();

        }

        private void Information_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        //return 
        public List<vdText> getTimes()
        {
            return graph.times;
        }

        public List<vdText> getProjectNumbers()
        {
            return graph.projectNumbers;
        }

        public List<vdText> getProjectNames()
        {
            return graph.projectNames;
        }

        public List<vdText> getP_projectNames()
        {
            return graph.subProjectNames ;
        }

        public List<vdText> getratio()
        {
            return graph.ratios;
        }

        public List<vdText> getporjectstage()
        {
            return graph.projectStages;
        }

        /// <summary>
        /// 导入信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importButton_Click(object sender, EventArgs e)
        {
            //打开一个文件选择框
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Excel文件";
            ofd.FileName = "";
            //ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);//为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
           // ofd.InitialDirectory = System.Windows.Forms.Application.StartupPath;
            ofd.Filter = "Excel 工作簿 (*.xlsx)|*.xlsx|Excel 97-2003工作簿 (*.xls)|*.xls";
            ofd.ValidateNames = true;     //文件有效性验证ValidateNames，验证用户输入是否是一个有效的Windows文件名
            ofd.CheckFileExists = true;  //验证路径有效性
            ofd.CheckPathExists = true; //验证文件有效性

            String strName = String.Empty;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                strName = ofd.FileName;
            }
            if (strName == "")
            {
                MessageBox.Show("没有选择Excel文件！无法进行数据导入");
                return;
            }
            //调用导入数据方法
            ExcelToDataGridView(strName, this.dataGridView1);
        }

        /// <summary>
        /// 将excel数据导入到datagridview中
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="dgv"></param>
        public void ExcelToDataGridView(String filePath, DataGridView dgv)
        {
            //根据路径打开一个Excel文件并将数据填充到DataSet中
            OleDbConnection conn = null;
            try
            {
                //string strConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source = " + filePath + ";Extended Properties ='Excel 8.0;HDR=NO;IMEX=1'";//导入时包含Excel中的第一行数据，并且将数字和字符混合的单元格视为文本进行导入
                String strConn = "";
                string fileType = System.IO.Path.GetExtension(filePath);
                if (string.IsNullOrEmpty(fileType))
                {
                    return;
                }
                if (fileType.Equals(".xls"))
                    strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 8.0;HDR=YES;IMEX=1\"";
                else
                    strConn = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + filePath + ";" + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1\"";
                conn = new OleDbConnection(strConn);
                if (conn != null)
                {
                    conn.Open();
                }
                //获取第一个工作簿名
                System.Data.DataTable dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                String sheetName = dt.Rows[0]["Table_Name"].ToString();
                string strExcel = "Select  * from  [" + sheetName + "]";
                OleDbDataAdapter myCommand = new OleDbDataAdapter(strExcel, strConn);
                DataSet ds = new DataSet();
                myCommand.Fill(ds);

                //根据DataGridView的列构造一个新的DataTable
                System.Data.DataTable tb = ds.Tables[0];
                int rows = tb.Rows.Count;
                int cols = tb.Columns.Count;
                //根据Excel的行逐一对上面构造的DataTable的列进行赋值

                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        this.dataGridView1.Rows[i].Cells[j].Value = tb.Rows[i][j];
                    }
                }
                this.dataGridView1.Update();
                MessageBox.Show("导入成功!");
            }
            catch (Exception e)
            {
                MessageBox.Show("插入数据失败" + e.ToString());
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 导出信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exportButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Excel 工作簿 (*.xlsx)|*.xlsx|Excel 97-2003工作簿 (*.xls)|*.xls";

            String strName = String.Empty;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                strName = sfd.FileName;
            }
            if (strName == "")
            {
                MessageBox.Show("没有选择Excel文件！无法进行数据导出");
                return;
            }
            DataGridViewToExcel(strName, this.dataGridView1);
        }


        /// <summary>
        /// 将datagridview里面的数据写入到文件中去
        /// </summary>
        /// <param name="strName"></param>
        /// <param name="dataGridView"></param>
        private void DataGridViewToExcel(string strName, DataGridView dataGridView)
        {
            Microsoft.Office.Interop.Excel.Application excel =
               new Microsoft.Office.Interop.Excel.Application();
            try
            {
                excel.Visible = false;
                Workbook wBook = excel.Workbooks.Add();
                Worksheet wSheet = wBook.ActiveSheet as Worksheet;
                int row = dataGridView.Rows.Count;
                int col = dataGridView.Columns.Count;

                for (int i = 0; i < row; i++)
                {
                    for (int j = 0; j < col; j++)
                    {
                        Object value = dataGridView.Rows[i].Cells[j].Value;
                        if (value != null)
                        {
                            wSheet.Cells[i + 2, j + 1] = value.ToString();
                        }
                    }
                }

                for (int i = 0; i < col; i++)
                {
                    wSheet.Cells[1, i + 1] = dataGridView.Columns[i].Name;
                }
                //设置禁止弹出保存和覆盖的询问提示框   
                excel.DisplayAlerts = true;
                excel.AlertBeforeOverwriting = true;
                //保存工作簿   
                //wBook.Save();
                //保存excel文件
                //excel.SaveWorkspace(strName);
                wSheet.SaveAs(strName);
                excel.Quit();
                MessageBox.Show("导出成功!");
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
                MessageBox.Show("导出Excel出错！错误原因：" + err.Message, "提示信息",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                excel = null;
            }
        }

    }
}
