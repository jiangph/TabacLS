using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VectorDraw.Geometry;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdObjects;
using VectorDraw.Professional.vdPrimaries;

namespace CADTools
{
    public partial class MainForm : Form
    {
        private String openFilePath;//打开文件的路径
        private vdDocument document;//全局的文档
        private Graph graph;
        private int selectedLayout;//当前选择的布局

        public MainForm()
        {
            InitializeComponent();
            this.openFilePath = "";
            this.document = null;
            this.graph = null;
            selectedLayout = -1;
        }

        /// <summary>
        /// 打开一个新文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openButtonFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                //打开一个文件选择框
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Title = "dwg文件";
                ofd.FileName = "";
                //ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);//为了获取特定的系统文件夹，可以使用System.Environment类的静态方法GetFolderPath()。该方法接受一个Environment.SpecialFolder枚举，其中可以定义要返回路径的哪个系统目录
               // ofd.InitialDirectory = System.Windows.Forms.Application.StartupPath;
                ofd.Filter = "dwg文件 (*.dwg)|*.dwg|所有文件 (*.*)|*.*";
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
                   // MessageBox.Show("没有选择文件！无法打开");
                    this.openFilePath = "";
                    return;
                }
                this.openFilePath = strName;
                vdFramedControl1.BaseControl.ActiveDocument.Open(strName);
                document = vdFramedControl1.BaseControl.ActiveDocument;
                if (document != null)
                {
                    selectedLayout = -1;
                    this.initial(document.LayOuts.Count + 1);
                }
            }
            catch
            {
                MessageBox.Show("打开文件失败!");
            }
        }

        /// <summary>
        /// 初始化布局列表
        /// </summary>
        /// <param name="size"></param>
        private void initial(int size)
        {
            this.layoutsComboBox.Properties.Items.Clear();
            if (size >= 1)
            {
                this.layoutsComboBox.Properties.Items.Add("Model");
                for (int i = 1; i < size; i++)
                {
                    this.layoutsComboBox.Properties.Items.Add("布局" + i);
                }
                this.layoutsComboBox.SelectedIndex = 0;
                if (selectedLayout == -1)
                {
                    this.layoutsComboBox_SelectedIndexChanged(null, null);
                }
            }
        }

        /// <summary>
        /// 保存当前dwg文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveButtonFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (openFilePath == "")
                {
                    saveAsButtonFile_ItemClick(sender, e);
                }
                else
                {
                    vdFramedControl1.BaseControl.ActiveDocument.SaveAs(this.openFilePath);
                }
            }
            catch
            {
                MessageBox.Show("保存失败!");
            }
        }

        /// <summary>
        /// 将当前文件另存为新文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAsButtonFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "dwg文件 (*.dwg) | *.dwg";

                String strName = String.Empty;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    strName = sfd.FileName;
                }
                if (strName == "")
                {
                    MessageBox.Show("没有选择dwg文件！");
                    return;
                }
                vdFramedControl1.BaseControl.ActiveDocument.SaveAs(strName);
            }
            catch
            {
                MessageBox.Show("另存为文件失败!");
            }
        }

        /// <summary>
        /// 退出应用程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitButtonFile_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// 显示属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showAttributesButtonTool_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                vdFramedControl1.vdGrid.SelectedObject = vdFramedControl1.BaseControl.ActiveDocument;
                vdFramedControl1.SetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.PropertyGrid, true);
            }
            catch
            {
                MessageBox.Show("显示属性失败！");
            }
        }

        /// <summary>
        /// 隐藏属性
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hideAttributesButtonTool_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                vdFramedControl1.SetLayoutStyle(vdControls.vdFramedControl.LayoutStyle.PropertyGrid, false);
            }
            catch
            {
                MessageBox.Show("隐藏属性失败！");
            }
        }

        /// <summary>
        /// 拆图为dwg格式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitDWGButtonTool_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (selectedLayout == -1)
                {
                    MessageBox.Show("请先选择当前布局");
                    return;
                }
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                String path = Application.StartupPath + "splitResults\\dwg";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    path = fbd.SelectedPath;
                }
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                List<Figure> figures = graph.getFigures();
                if (figures == null || figures.Count == 0)
                {
                    graph.initialFigures();
                    figures = graph.getFigures();
                }
                int i = 1;
                ProgressBar pb = new ProgressBar(0, 0, figures.Count);
                pb.Show();
                List<vdPolyline> rootPolyLine = graph.getRootPolyline();
                graph.analyzeTextGraph(rootPolyLine);
                List<vdText> projectNumbers = graph.getProjectNumbers();
                List<vdText> projectNames = graph.getProjectNames();
                foreach (Figure figure in figures)
                {
                    String name = projectNumbers[i - 1].TextString + " " +
                            projectNames[i - 1].TextString;
                    vdDocument document = figure.getDocument();
                    document.SaveAs(path + "\\" + name + ".vdcl");
                    String title = "共有" + figures.Count + "张子图，当前已拆分" + i + "张子图，请稍候!";
                    pb.setTitle(title);
                    pb.process(i++);
                }
                pb.Hide();
                MessageBox.Show("拆图完毕!");
            }
            catch
            {
                MessageBox.Show("拆图失败！");
            }
        }

        /// <summary>
        /// 拆图为pdf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitPDFButtonTool_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (selectedLayout == -1)
                {
                    MessageBox.Show("请先选择当前布局");
                    return;
                }
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                String path = Application.StartupPath + "splitResults\\PDF";
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    path = fbd.SelectedPath;
                }
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                List<Figure> figures = graph.getFigures();
                if (figures == null || figures.Count == 0)
                {
                    graph.initialFigures();
                    figures = graph.getFigures();
                }
                int count = figures.Count;
                ProgressBar pb = new ProgressBar(0, 0, count);
                pb.Show();
                int i = 1;
                graph.analyzeText();
                List<vdPolyline> rootPolyLine = graph.getRootPolyline();
                graph.analyzeTextGraph(rootPolyLine);
                List<vdText> projectNumbers = graph.getProjectNumbers();
                List<vdText> projectNames = graph.getProjectNames();
                foreach (Figure figure in figures)
                {
                    String name = projectNumbers[i - 1].TextString + " " +
                        projectNames[i - 1].TextString;
                    figure.saveAsPdf(path + "\\" + name + ".pdf");
                    String title = "共有" + count + "张子图，当前已保存" + i + "个子图，请稍候!";
                    pb.setTitle(title);
                    pb.process(i++);
                }
                pb.Hide();
                MessageBox.Show("拆图完毕！");
            }
            catch
            {
                MessageBox.Show("拆图失败!");
            }
        }

        /// <summary>
        /// 切换布局事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void layoutsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int selected = this.layoutsComboBox.SelectedIndex;
                vdLayout layout = null;
                if (selected == 0)
                {
                    layout = document.Model;
                }
                else if (selected>=1)
                {
                    layout = document.LayOuts[selected - 1];
                }
                selectedLayout = selected;
                if (selectedLayout == 0)
                {
                    this.currentLayoutValueLabel.Text = "Model";
                }
                else if (selectedLayout > 0)
                {
                    this.currentLayoutValueLabel.Text = "布局" + selectedLayout;
                }
                if (graph != null)
                {
                    graph.Clear();          
                }
                graph = new Graph(layout);
                graph.split();
                MessageBox.Show("切换布局完毕！");
            }
            catch
            {
                MessageBox.Show("切换布局失败！");
            }
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printButtonTool_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (this.selectedLayout == -1)
                {
                    MessageBox.Show("请先选择当前布局");
                    return;
                }
                graph.analyzeText();
                List<vdPolyline> rootPolyLine = graph.getRootPolyline();
                graph.analyzeTextGraph(rootPolyLine);
                PrintDocument pd = new PrintDocument(graph);
                pd.Show();
            }
            catch
            {
                MessageBox.Show("打印失败！");
            }
        }

        /// <summary>
        /// 显示二维码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showQRCodeButtonTool_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (graph == null)
                {
                    MessageBox.Show("请先打开文件");
                    return;
                }
                if (this.selectedLayout == -1)
                {
                    MessageBox.Show("请先选择当前布局");
                    return;
                }
                List<vdImage> qrcodes = graph.getQRCodes();
                if (qrcodes.Count == 0)
                {
                    MessageBox.Show("请先添加二维码");
                    return;
                }
                foreach (vdImage image in qrcodes)
                {
                    vdDocument document = vdFramedControl1.BaseControl.ActiveDocument.Document;
                    image.visibility = VectorDraw.Professional.vdPrimaries.vdFigure.VisibilityEnum.Visible;
                    image.Update();
                    document.Redraw(true);
                }
                graph.initialFigures();
                MessageBox.Show("显示二维码完毕!");
            }
            catch
            {
                MessageBox.Show("显示二维码失败！");
            }

        }

        /// <summary>
        /// 隐藏二维码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteQRCodeButtonTool_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (graph == null)
                {
                    MessageBox.Show("请先打开文件");
                    return;
                }
                if (this.selectedLayout == -1)
                {
                    MessageBox.Show("请先选择当前布局");
                    return;
                }
                List<vdImage> qrcodes = graph.getQRCodes();
                if (qrcodes.Count == 0)
                {
                    MessageBox.Show("请先添加二维码");
                    return;
                }
                foreach (vdImage image in qrcodes)
                {
                    vdDocument document = vdFramedControl1.BaseControl.ActiveDocument.Document;
                    image.visibility = VectorDraw.Professional.vdPrimaries.vdFigure.VisibilityEnum.Invisible;
                    image.Update();
                    document.Redraw(true);
                }
                graph.initialFigures();
                MessageBox.Show("隐藏二维码完毕!");
            }
            catch
            {
                MessageBox.Show("隐藏二维码失败!");
            }
        }

        /// <summary>
        /// 添加二维码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addQRCodeButtonTool_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            try
            {
                if (graph == null)
                {
                    MessageBox.Show("请先打开文件");
                    return;
                }
                if (this.selectedLayout == -1)
                {
                    MessageBox.Show("请先选择当前布局");
                    return;
                }
                graph.analyzeText();
                graph.analyzeTextGraph(graph.getRootPolyline());
                graph.addQRCodes();
                this.Update();
            }
            catch
            {
                MessageBox.Show("添加二维码失败！");
            }
        }

        /// <summary>
        /// 导航栏打开文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openNavButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            this.openButtonFile_ItemClick(sender, null);
        }

        /// <summary>
        /// 导航栏添加二维码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addQRcodeNavButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            this.addQRCodeButtonTool_ItemClick(sender, null);
        }

        /// <summary>
        /// 导航栏显示二维码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void showQRCodeNavButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            this.showQRCodeButtonTool_ItemClick(sender, null);
        }

        /// <summary>
        /// 导航栏隐藏二维码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hideQRCodeNavButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            this.deleteQRCodeButtonTool_ItemClick(sender, null);
        }

        /// <summary>
        /// 导航栏打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printNavButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            this.printButtonTool_ItemClick(sender, null);
        }

        /// <summary>
        /// 导航栏拆分为dwg
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitDWGNavButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            this.splitDWGButtonTool_ItemClick(sender, null);
        }

        /// <summary>
        /// 导航栏拆分为pdf
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void splitPDFNavbutton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            this.splitPDFButtonTool_ItemClick(sender, null);
        }

        /// <summary>
        /// 导航栏退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitNavButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            this.exitButtonFile_ItemClick(sender, null);
        }

        /// <summary>
        /// 帮助菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void helpMenu_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Help help = new Help();
            help.Show();
        }

        /// <summary>
        /// 修改内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void modifyContextNavButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            //graph = new Graph(this.document);
            try {
                graph.analyzeText();
                List<vdPolyline> root = graph.getRootPolyline();
                graph.analyzeTextGraph(root);
                Information information = new Information(graph, graph.getTimes(), graph.getProjectNames(), graph.getProjectNumbers(), graph.getSubProjectNames(), graph.getRatios(), graph.getProjectStages(), graph.getRootPolyline());
                information.ShowDialog();
            }
            catch
            {
                MessageBox.Show("图签解析失败，可能存在未标准化的图签，请联系设计人员。");
            }
        } 

      

        /// <summary>
        /// 设置字体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setFontButtonTool_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Fonts font = new Fonts();
            font.Show();
        }

        /// <summary>
        /// 改变字体
        /// </summary>
        /// <param name="fontName"></param>
        internal void changeFontFile(string fontName)
        {
            graph.analyzeText();
            graph.analyzeTextGraph(graph.getRootPolyline());
            List<vdPolyline> rootPolyline = graph.getRootPolyline();
            for (int i = 0; i < rootPolyline.Count; i++)
            {
                try
                {
                    List<vdText> timeFont = graph.getTimes();
                    for (int j = 0; j < timeFont.Count; j++)
                    {
                        // vdFramedControl1.BaseControl.ActiveDocument.TextStyles.Standard.FontFile = fontName;
                        timeFont[j].Style.FontFile = fontName;
                        timeFont[j].Update();
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("工程图中存在为定义的区域,请确认");
                }
            }
        }

        private void directoryNavButton_LinkClicked(object sender, DevExpress.XtraNavBar.NavBarLinkEventArgs e)
        {
            try
            {
                graph.addDirectory();
                MessageBox.Show("添加目录成功！");
            }
            catch 
            {
                MessageBox.Show("添加目录失败！");
            }
        }

        private void generateDirectoryButtonTool_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.directoryNavButton_LinkClicked(null, null);
        }

        private void reviseButtonTool_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            this.modifyContextNavButton_LinkClicked(null, null);
        }
    }
}
