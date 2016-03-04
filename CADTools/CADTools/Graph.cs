using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

using VectorDraw.Generics;
using VectorDraw.Geometry;
using VectorDraw.Professional.Constants;
using VectorDraw.Professional.vdCollections;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdObjects;
using VectorDraw.Professional.vdPrimaries;
using System.Text.RegularExpressions;
using VectorDraw.Professional.Components;

namespace CADTools
{
    public class Graph
    {
        private List<Figure> figures;
        private vdDocument document;
        private vdLayout layout;

        private List<vdPolyline> polyLineEntities;
        public List<vdPolyline> rootPolyLine;
        private int[] parent;
        private List<vdEntities> splitEntities;
        private vdEntities allEntities;

        //text index
        public List<vdText> times;
        public List<vdText> projectNumbers;
        public List<vdText> projectNames;
        public List<vdText> subProjectNames;
        public List<vdText> projectStages;
        public List<vdText> ratios;
        //new vdText for all text
        private List<vdText> textEntities;
        //qrcode
        private List<vdImage> qrcodes;
        //directory
        private ChooseDirectory cd;

        public Graph(vdLayout layout)
        {
            this.layout = layout;
            document = layout.Document;
            VectorDraw.Professional.vdObjects.vdDocument documentemp = layout.Document;
            figures = new List<Figure>();
            polyLineEntities = new List<vdPolyline>();
            rootPolyLine = new List<vdPolyline>();
            splitEntities = new List<vdEntities>();
            allEntities = layout.Entities;

            times = new List<vdText>();
            projectNumbers = new List<vdText>();
            projectNames = new List<vdText>();
            subProjectNames = new List<vdText>();
            qrcodes = new List<vdImage>();
            projectStages = new List<vdText>();
            ratios = new List<vdText>();
            textEntities = new List<vdText>();
            //MessageBox.Show("number is: " + document.Model.Entities);
            //this.split();
            //this.writeRootPolylines();
            //this.reconduct(documentemp);
        }

        public vdLayout getLayout()
        {
            return layout;
        }

        public vdDocument getDocument()
        {
            return this.document;
        }

        public List<vdText> getTimes()
        {
            return times;
        }

        public List<vdText> getProjectNumbers()
        {
            return projectNumbers;
        }

        public List<vdText> getProjectNames()
        {
            return projectNames;
        }

        public List<vdPolyline> getRootPolyline()
        {
            return rootPolyLine;
        }

        public List<vdText> getSubProjectNames()
        {
            return subProjectNames;
        }

        public List<vdText> getRatios()
        {
            return ratios;
        }

        public List<vdText> getProjectStages()
        {
            return projectStages;
        }

        public List<vdImage> getQRCodes()
        {
            return qrcodes;
        }

        /// <summary>
        /// 匹配时间，项目名，项目号
        /// </summary>
        public void analyzeText()
        {
            times.Clear();
            projectNames.Clear();
            projectNumbers.Clear();
            for (int i = 0; i < splitEntities.Count; i++)
            {
                for (int j = 0; j < splitEntities[i].Count; j++)
                {
                    if (splitEntities[i][j]._TypeName.Equals("vdText"))
                    {
                        vdText text = (vdText)splitEntities[i][j];
                        string textstring = text.TextString;
                        if (Regex.IsMatch(textstring, @"^((((1[6-9]|[2-9]\d)\d{2}).(0?[13578]|1[02]).(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2}).(0?[13456789]|1[012]).(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2}).0?2.(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$"))
                        {
                            times.Add(text);
                        }
                        else if (Regex.IsMatch(textstring, @"^(\d{4}.\d{2})$"))
                        {
                            times.Add(text);
                        }
                        else if (textstring.Contains("B805"))
                        {
                            projectNumbers.Add(text);
                        }
                        else if (Regex.IsMatch(textstring, @"^(\d{4} \d{2}-\d{5}-\d{2})$"))
                        {
                            projectNumbers.Add(text);
                        }
                        else if (textstring.Contains("回路图") || textstring.Contains("子排图") || textstring.Contains("原理图"))
                        {
                            projectNames.Add(text);
                        }
                    }
                }
            }
        }

        public List<Figure> getFigures()
        {
            return figures;
        }

        /// <summary>
        /// 拆图，初始化出figures，不保存至磁盘中
        /// </summary>
        public void split()
        {
            this.addAllPolylines(layout.Entities);//添加polylines
            this.analyze();//分析根ploylines
            this.initialFigures();
        }
        
        /// <summary>
        /// 拆图后，根据rootPloylines，拆出各个小figure
        /// </summary>
        public void initialFigures()
        {
            splitEntities.Clear();
            for (int i = 0; i < rootPolyLine.Count; i++)
            {
                vdEntities entities = this.selectrectangle(rootPolyLine[i]);
                if (entities != null)
                {
                    splitEntities.Add(entities);
                }
            }
            //MessageBox.Show("" + splitEntities.Count);
            figures.Clear();
            for (int i = 0; i < splitEntities.Count; i++)
            {
                vdDocumentComponent doccomponent = new vdDocumentComponent();
                vdDocument temp = doccomponent.Document;
                foreach (vdFigure figure in splitEntities[i])
                {
                    //figure.Deleted = false;
                    temp.Model.Entities.AddItem(figure.Clone(temp));
                }
                if (temp != null)
                {
                    temp.Model.ZoomExtents();
                    //temp.Redraw(true);
                    Figure figure = new Figure(temp);
                    figures.Add(figure);
                }
                else
                {
                    MessageBox.Show("null");
                }

            }
            
        }

        public void splitDirectory(String blockname)
        {
            vdBlock vd = document.Blocks.FindName(blockname);
            vdEntities vdent = vd.Entities;
            gPoints pt3s = new gPoints();
            vdInsert insert = (vdInsert)vd.GetReferenceObjects()[0];
            pt3s.Add(insert.InsertionPoint + new VectorDraw.Geometry.gPoint(144, 48));
            pt3s.Add(insert.InsertionPoint + new VectorDraw.Geometry.gPoint(355, 48));
            pt3s.Add(insert.InsertionPoint + new VectorDraw.Geometry.gPoint(355, 345));
            pt3s.Add(insert.InsertionPoint + new VectorDraw.Geometry.gPoint(145, 346));

            vdDocumentComponent doccomponent = new vdDocumentComponent();
            vdDocument temp = doccomponent.Document;

            vdSelection sel = new vdSelection();
            sel.SetUnRegisterDocument(document);
            sel.Name = "ents";
            sel.Select(VectorDraw.Render.RenderSelect.SelectingMode.CrossingWindowPolygon, pt3s);
            if (sel.Count > 0)
            {
                temp.CommandAction.CmdErase("ALL");
                foreach (vdFigure figure in sel)
                {
                    //figure.Deleted = false;
                    temp.Model.Entities.AddItem(figure.Clone(temp));
                }
                if (temp != null)
                {
                    temp.Model.ZoomExtents();
                    //temp.Redraw(true);
                    Figure figure = new Figure(temp);
                    figures.Add(figure);
                };
            }
        }

        /// <summary>
        /// 统计这个边框里面所有的内容
        /// </summary>
        /// <param name="polyline">输入一个外边框</param>
        /// <returns>边框里面所有的内容</returns>
        private vdEntities selectrectangle(vdPolyline polyline)
        {
            vdSelection sel = new vdSelection();
            sel.SetUnRegisterDocument(document);
            gPoints pts = new gPoints();
            Vertexes vertexes = polyline.VertexList;

            for (int i = 0; i < vertexes.Count; i++)
            {
                pts.Add(vertexes[i]);
            }
            sel.Select(VectorDraw.Render.RenderSelect.SelectingMode.CrossingWindowPolygon, pts);


            if (sel.Count > 0)
            {
                vdEntities entities = new vdEntities();
                foreach (vdFigure item in sel)
                {
                    entities.AddItem(item);
                }
                return entities;
            }
            return null;
        }
        
        /// <summary>
        /// 找出entities中所有的ployline
        /// </summary>
        /// <param name="entities">document中所有的实体</param>
        private void addAllPolylines(vdEntities entities)
        {
            if (entities == null)
            {
                return;
            }
            for (int i = 0; i < entities.Count; i++)
            {
                vdFigure figure = entities[i];
                if (figure._TypeName == "vdInsert" || figure._TypeName.Equals("vdInsert"))
                {
                    vdInsert insert = (vdInsert)figure;
                    vdEntities insertEntities = insert.Explode();
                    this.addAllPolylines(insertEntities);
                }
                else if (figure._TypeName == "vdPolyline" || figure._TypeName.Equals("vdPolyline"))
                {
                    vdPolyline polyLine = (vdPolyline)figure;
                    Vertexes vertexes = polyLine.VertexList;
                    if ((vertexes.Count == 4) && polyLine.IsVisible())
                    {
                        this.addPolyLine(polyLine);
                    }
                }
                else if (figure._TypeName == "vdText" || figure._TypeName.Equals("vdText"))
                {
                    vdText text = (vdText)figure;
                    if (text.visibility == vdFigure.VisibilityEnum.Visible)
                    {
                        this.textEntities.Add(text);
                    }
                }
            }
        }

        /// <summary>
        /// 检测每个polyline的父亲
        /// </summary>
        public void analyze()
        {
            int size = polyLineEntities.Count;
            this.initial(size);
            Boolean flag = true;
            for (int i = 0; i < size; i++)
            {
                if (parent[i] == -1)
                {
                    flag = false;
                    for (int j = 0; j != i && j < size; j++)
                    {
                        int value = this.getParent(polyLineEntities[i], polyLineEntities[j]);
                        if (value == -1)
                        {
                            parent[i] = j;
                            flag = true;
                        }
                        else if (value == 1)
                        {
                            parent[j] = i;
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                }
            }
            //analyse the final root of figures
            for (int i = 0; i < parent.Length; i++)
            {
                if (parent[i] == -1)
                {
                    vdPolyline polyline = polyLineEntities[i];
                    Vertex vertex = polyline.VertexList[0];
                    rootPolyLine.Add(polyline);

                }
            }
            //MessageBox.Show(rootPolyLine.Count + "");
        }

        /// <summary>
        /// 判断两个polyline谁是父亲，被谁包围
        /// </summary>
        /// <param name="polyline1"></param>
        /// <param name="polyline2"></param>
        /// <returns>1:polylin1是父亲,-1:polyline2是父亲,0:都不是</returns>
        private int getParent(vdPolyline polyline1, vdPolyline polyline2)
        {
            Vertexes vertexes1 = polyline1.VertexList;
            Vertexes vertexes2 = polyline2.VertexList;
            //Vertex a = vertexes1[0], b = vertexes1[1], c = vertexes1[2], d = vertexes1[3];
            Vertex topLeft1 = vertexes1[0], topRight1 = vertexes1[1], downRight1 = vertexes1[2], downLeft1 = vertexes1[3];
            Vertex topLeft2 = vertexes2[0], topRight2 = vertexes2[1], downRight2 = vertexes2[2], downLeft2 = vertexes2[3];
            //MessageBox.Show(this.output(polyline1) + this.output(polyline2));
            if ((topLeft1.x >= topLeft2.x && topLeft1.y <= topLeft2.y) &&
                (topRight1.x <= topRight2.x && topRight1.y <= topRight2.y) &&
                (downRight1.x <= downRight2.x && downRight1.y >= downRight2.y) &&
                (downLeft1.x >= downLeft2.x && downLeft1.y >= downLeft2.y))
            {
                return -1; //polyline1 is covered by polyline2
            }
            if ((topLeft2.x >= topLeft1.x && topLeft2.y <= topLeft1.y) &&
                (topRight2.x <= topRight1.x && topRight2.y <= topRight1.y) &&
                (downRight2.x <= downRight1.x && downRight2.y >= downRight1.y) &&
                (downLeft2.x >= downLeft1.x && downLeft2.y >= downLeft1.y))
            {
                return 1; //polyline2 is covered by polyline1
            }
            return 0;
        }

        /// <summary>
        /// 初始化parent数组为-1，-1代表没有父亲，即根polyline
        /// </summary>
        /// <param name="size">polyline的个数</param>
        private void initial(int size)
        {
            parent = new int[size];
            for (int i = 0; i < parent.Length; i++)
            {
                parent[i] = -1;
            }
        }

        /// <summary>
        /// 将polyline转变一致方向并加入到polyline集合中去
        /// </summary>
        /// <param name="polyLine">一个polyline实例</param>
        private void addPolyLine(vdPolyline polyLine)
        {
            vdPolyline temp = this.changeDirection(polyLine);
            //vdPolyline temp = polyLine;
            if (temp != null)
            {
                polyLineEntities.Add(temp);
            }
        }

        /// <summary>
        /// 保证plolyline中的点是按照左上，右上，右下，左下的顺序排列
        /// </summary>
        /// <param name="polyline">polyline实例</param>
        /// <returns>转换方向后的polyline</returns>
        private vdPolyline changeDirection(vdPolyline polyline)
        {
            Vertexes vertexes = polyline.VertexList;
            for (int i = 0; i < 4; i++)
            {
                for (int j = i + 1; j < 4; j++)
                {
                    if (vertexes[i].x > vertexes[j].x)
                    {
                        double temp = vertexes[j].x;
                        vertexes[j].x = vertexes[i].x;
                        vertexes[i].x = temp;
                        temp = vertexes[j].y;
                        vertexes[j].y = vertexes[i].y;
                        vertexes[i].y = temp;
                    }
                }
            }
            //order by x ascend
            if (vertexes[0].x == vertexes[2].x)
            {
                return null;
            }
            if (vertexes[1].x == vertexes[3].x)
            {
                return null;
            }
            //MessageBox.Show(vertexes[0].x + " " + vertexes[0].y + "\n" + vertexes[1].x + " " + vertexes[1].y + "\n"
            // +vertexes[2].x + " " + vertexes[2].y +"\n"+vertexes[3].x + " " + vertexes[3].y +"\n");
            vdPolyline newPolyline = new vdPolyline();
            if (vertexes[0].y < vertexes[1].y)
            {
                newPolyline.VertexList.Add(vertexes[1]);
                newPolyline.VertexList.Add(vertexes[0]);
            }
            else
            {
                newPolyline.VertexList.Add(vertexes[0]);
                newPolyline.VertexList.Add(vertexes[1]);
            }
            Vertex last = newPolyline.VertexList[1];
            newPolyline.VertexList.RemoveItem(last);
            if (vertexes[2].y < vertexes[3].y)
            {
                newPolyline.VertexList.Add(vertexes[3]);
                newPolyline.VertexList.Add(vertexes[2]);
            }
            else
            {
                newPolyline.VertexList.Add(vertexes[2]);
                newPolyline.VertexList.Add(vertexes[3]);
            }
            newPolyline.VertexList.Add(last);
            return newPolyline;
        }

        private void writeRootPolylines()
        {
            StreamWriter sw = new StreamWriter(new FileStream(Application.StartupPath + "\\vertexes.txt", FileMode.Create), Encoding.Default);
            for (int i = 0; i < rootPolyLine.Count; i++)
            {
                sw.WriteLine(this.output(rootPolyLine[i]));
            }
            sw.Close();

        }
        /// <summary>
        /// 打印polyline的四个点
        /// </summary>
        /// <param name="polyLine"></param>
        /// <returns></returns>
        private String output(vdPolyline polyLine)
        {
            Vertexes vertexes = polyLine.VertexList;
            String str = "";
            for (int i = 0; i < vertexes.Count; i++)
            {
                str += vertexes[i].x + " " + vertexes[i].y + " ";
            }
            str += polyLine.Area();
            return str;
        }
        
        /// <summary>
        /// 添加二维码
        /// </summary>
        public void addQRCodes()
        {
            try
            {
                qrcodes.Clear();
                int count = rootPolyLine.Count;
                ProgressBar pb = new ProgressBar(0, 0, count);
                pb.Show();
                for (int i = 0; i < count; i++)
                {
                    String result = times[i].TextString.ToString() + "\n"
                        + subProjectNames[i].TextString.ToString() + "\n"
                        + projectNumbers[i].TextString.ToString() + "\n"
                        + ratios[i].TextString.ToString()+"\n"
                        + projectStages[i].TextString.ToString() ;
                    
                    QRCode qr = new QRCode(result, 4);
                    qr.generateQRCodeImg();
                    System.Threading.Thread.Sleep(1000);
                    String path = qr.getQRCodeImgPath();
                    addImageItems(path);
                    vdImage image = addImagesEntities(rootPolyLine[i]);
                    qrcodes.Add(image);
                    pb.process(i);
                    String title = "共需要添加" + count + "张二维码，当前已添加" + i + "个二维码，请稍候!";
                    pb.setTitle(title);
                }
                document.Redraw(true);
                pb.Hide();
                this.initialFigures();
                MessageBox.Show("添加二维码完毕!");
            }
            catch
            {
                MessageBox.Show("添加二维码失败!");
            }
        }

        ///<summary>
        ///生成二维码
        ///
        private void addImageItems(String path)
        {
            try
            {
                VectorDraw.Professional.vdPrimaries.vdImageDef imagedef1 = new VectorDraw.Professional.vdPrimaries.vdImageDef();
                imagedef1.SetUnRegisterDocument(document);
                imagedef1.setDocumentDefaults();
                imagedef1.Name = "Image2";
                imagedef1.FileName = path;
                document.Images.AddItem(imagedef1);
                document.Update();
            }
            catch
            {
                MessageBox.Show("添加二维码失败!");
            }
        }

        /// <summary>
        /// 在实体中加入图片，插入至具体的位置
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private vdImage addImagesEntities(vdPolyline root)
        {
            try
            {
                VectorDraw.Professional.vdFigures.vdImage image = new VectorDraw.Professional.vdFigures.vdImage();
                image.SetUnRegisterDocument(layout.Document);
                image.setDocumentDefaults();
                image.ImageDefinition = layout.Document.Images[0];
                image.Height = 20;
                //图像的插入位置
                Vertexes vertexes = root.VertexList;
                gPoint insertPoint = new gPoint();
                insertPoint.x = vertexes[0].x;
                insertPoint.y = vertexes[0].y-20;
                insertPoint.z = vertexes[0].z;
                image.InsertionPoint = insertPoint;
                //image.InsertionPoint = new VectorDraw.Geometry.gPoint(250.0, 250.0);
                layout.Entities.AddItem(image);
                layout.Update();
                //layout.Document.Redraw(true);
                document.Update();
                return image;
            }
            catch
            {
                MessageBox.Show("添加二维码失败!");
            }
            return null;
        }

        /// <summary>
        /// 分析图签信息jiang
        /// </summary>
        /// <param name="rootpolyline1"></param>
        public void analyzeTextGraph(List<vdPolyline> rootpolyline1)
        {
            projectNames.Clear();
            subProjectNames.Clear();
            projectNumbers.Clear();
            times.Clear();
            projectStages.Clear();
            ratios.Clear();
            for (int i = 0; i < rootpolyline1.Count; i++)
            {
                //先遍历图框，在遍历文字
                //若有那个字段没有出现，我们自动new一个新的text插入到图框中

                gPoints pointsProjectName = new gPoints();      //工程名
                gPoints pointGraphProjectName = new gPoints();    //子图名
                gPoints pointGraphNumber = new gPoints(); //图号
                gPoints pointData = new gPoints(); //日期
                gPoints pointRatio = new gPoints(); //比例
                gPoints pointStage = new gPoints(); //施工阶段

                //调用changedirection函数保证四个点的方向都是一致的
                vdPolyline newRootpolyline = this.changeDirection(rootpolyline1[i]);
                //判断图框类型,确定图框类型
                double lengthPolyline = newRootpolyline.VertexList[1].x - newRootpolyline.VertexList[0].x;
                double heightPolyline = newRootpolyline.VertexList[1].y - newRootpolyline.VertexList[2].y;
                //根据长高确定图框类型
                string borderName = null;
                //A3
                if (Math.Abs(lengthPolyline - 297) <= 3 && Math.Abs(heightPolyline - 420) <= 3)
                {

                    borderName = "A3";
                }

                //A4
                if (Math.Abs(lengthPolyline - 420) <= 3 && Math.Abs(heightPolyline - 297) <= 3)
                {
                    //工程名
                    pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-95, 45));
                    pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-44, 33));
                    pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-95, 33));
                    borderName = "A4";
                    //各个子图名
                    pointGraphProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-105, 33));
                    pointGraphProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointGraphProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-5, 12));
                    pointGraphProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    //图号
                    pointGraphNumber.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-70, 12));
                    pointGraphNumber.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointGraphNumber.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-5, 5));
                    pointGraphNumber.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    //日期
                    pointData.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-120, 12));
                    pointData.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointData.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-105, 5));
                    pointData.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    //比例
                    pointRatio.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-120, 19));
                    pointRatio.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointRatio.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-95, 12));
                    pointRatio.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    //施工阶段
                    pointStage.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-30, 48));
                    pointStage.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointStage.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-16, 30));
                    pointStage.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));

                }
                //A5
                else if (Math.Abs(lengthPolyline - 594) <= 3 && Math.Abs(heightPolyline - 420) <= 3)
                {
                    //工程名
                    pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-100, 50));
                    pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-55, 38));
                    pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    //各个子图名
                    pointGraphProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-100, 38));
                    pointGraphProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointGraphProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-10, 17));
                    pointGraphProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    //图号
                    pointGraphNumber.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-74, 17));
                    pointGraphNumber.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointGraphNumber.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-10, 10));
                    pointGraphNumber.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    //日期
                    pointData.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-125, 17));
                    pointData.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointData.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-100, 10));
                    pointData.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    //比例
                    pointRatio.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-100, 24));
                    pointRatio.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointRatio.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-100, 17));
                    pointRatio.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    //施工阶段
                    pointStage.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-35, 45));
                    pointStage.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointStage.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-20, 38));
                    pointStage.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));

                    borderName = "A5";

                }
                //A6
                else if (Math.Abs(lengthPolyline - 743) <= 3 && Math.Abs(heightPolyline - 420) <= 3)
                {
                    pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-100, 45));
                    pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-45, 38));
                    pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    //各个子图名
                    pointGraphProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-100, 38));
                    pointGraphProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointGraphProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-10, 17));
                    pointGraphProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    //图号
                    pointGraphNumber.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-74, 17));
                    pointGraphNumber.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointGraphNumber.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-10, 10));
                    pointGraphNumber.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    //日期
                    pointData.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-125, 17));
                    pointData.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointData.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-100, 10));
                    pointData.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    //比例
                    pointRatio.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-100, 24));
                    pointRatio.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointRatio.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-100, 17));
                    pointRatio.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    borderName = "A6";
                    //施工阶段
                    pointStage.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-35, 45));
                    pointStage.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointStage.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-20, 38));
                    pointStage.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                }
                //其他种类的情况
                else
                {
                    pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-100, 55));
                    pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    //pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(-45, 33));
                    pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    pointsProjectName.Add(newRootpolyline.VertexList[2] + new VectorDraw.Geometry.gPoint(0, 0));
                    borderName = "您使用了不校准图框";

                }
                for (int j = 0; j < textEntities.Count; j++)
                {
                    vdText tempText = new vdText();

                    tempText = textEntities[j];

                    //将每副图的六个信息传入到相应的数组里面去
                    if ((tempText.InsertionPoint.x > pointsProjectName[0].x && tempText.InsertionPoint.y < pointsProjectName[0].y) && tempText.InsertionPoint.x < pointsProjectName[2].x && tempText.InsertionPoint.y > pointsProjectName[2].y)
                    {
                        //若字符串为工程名
                        projectNames.Add(tempText);
                    }
                    else if ((tempText.InsertionPoint.x > pointGraphProjectName[0].x && tempText.InsertionPoint.y < pointGraphProjectName[0].y) && tempText.InsertionPoint.x < pointGraphProjectName[2].x && tempText.InsertionPoint.y > pointGraphProjectName[2].y)
                    {
                        //子图名
                        subProjectNames.Add(tempText);
                    }
                    else if ((tempText.InsertionPoint.x > pointGraphNumber[0].x && tempText.InsertionPoint.y < pointGraphNumber[0].y) && tempText.InsertionPoint.x < pointGraphNumber[2].x && tempText.InsertionPoint.y > pointGraphNumber[2].y && tempText.TextString.Contains("-"))
                    {
                        //图号
                        projectNumbers.Add(tempText);
                    }
                    else if ((tempText.InsertionPoint.x > pointData[0].x && tempText.InsertionPoint.y < pointData[0].y) && tempText.InsertionPoint.x < pointData[2].x && tempText.InsertionPoint.y > pointData[2].y)
                    {
                        //日期
                        times.Add(tempText);
                    }
                    else if ((tempText.InsertionPoint.x > pointRatio[0].x && tempText.InsertionPoint.y < pointRatio[0].y) && tempText.InsertionPoint.x < pointRatio[2].x && tempText.InsertionPoint.y > pointRatio[2].y)
                    {
                        //比例
                        ratios.Add(tempText);
                    }
                    else if ((tempText.InsertionPoint.x > pointStage[0].x && tempText.InsertionPoint.y < pointStage[0].y) && tempText.InsertionPoint.x < pointStage[2].x && tempText.InsertionPoint.y > pointStage[2].y && tempText.TextString.Length == 3 && tempText.TextString.Contains("图"))
                    {
                        //施工阶段
                        projectStages.Add(tempText);
                    }
                }

                //判断数组的长度，数组的长度应与当前的rootpolyline保持一致，否则new一个加进去，同时需要注意的是判断当前图框的类型，决定插入的位置
                if (projectNames.Count < (i + 1))   //工程名
                {
                    int x = projectNames.Count;
                    while (x != (i + 1))
                    {
                        vdText tmpText = new vdText();
                        tmpText.InsertionPoint = pointsProjectName[0] + new gPoint(3, -10);
                        tmpText.TextString = "null";
                        tmpText.Update();
                        projectNames.Add(tmpText);
                        x++;
                    }
                }
                else if (projectNames.Count > (i + 1))
                {
                    int x = projectNames.Count;
                    while (x != (i + 1))
                    {
                        projectNames.RemoveAt(x - 1);
                        x--;
                    }
                }
                if (subProjectNames.Count < (i + 1))   //子图名
                {
                    int x = subProjectNames.Count;
                    while (x != (i + 1))
                    {
                        vdText tmpText = new vdText();
                        tmpText.InsertionPoint = pointGraphProjectName[0] + new gPoint(3, -10);
                        tmpText.TextString = "null";
                        tmpText.Update();
                        subProjectNames.Add(tmpText);
                        x++;
                    }
                }
                else if (subProjectNames.Count > (i + 1))
                {
                    int x = subProjectNames.Count;
                    while (x != (i + 1))
                    {
                        subProjectNames.RemoveAt(x - 1);
                        x--;
                    }
                }
                if (projectNumbers.Count < (i + 1))   //图号
                {
                    int x = projectNumbers.Count;
                    while (x != (i + 1))
                    {
                        vdText tmpText = new vdText();
                        tmpText.SetUnRegisterDocument(document);
                        tmpText.setDocumentDefaults();
                        if (borderName == "A4")
                        {

                            tmpText.InsertionPoint = pointRatio[0] + new gPoint(8, -1);
                        }

                        else if (borderName == "A5")
                            tmpText.InsertionPoint = pointRatio[0] + new gPoint(8, -1);
                        else if (borderName == "A6")
                            tmpText.InsertionPoint = pointRatio[0] + new gPoint(8, -1);

                        tmpText.PenColor.ColorIndex = 3;
                        tmpText.Height = 3.5;
                        tmpText.WidthFactor = 0.7;
                        //将字写回到document中
                        tmpText.TextString = "";
                        tmpText.Update();
                        document.ActiveLayOut.Entities.AddItem(tmpText);
                        //document.Model.Entities.AddItem(tmpText);
                        document.Redraw(true);

                        projectNumbers.Add(tmpText);
                        x++;
                    }
                }
                else if (projectNumbers.Count > (i + 1))
                {
                    int x = projectNumbers.Count;
                    while (x != (i + 1))
                    {
                        projectNumbers.RemoveAt(x - 1);
                        x--;
                    }
                }
                if (times.Count < (i + 1))   //日期
                {
                    int x = times.Count;
                    while (x != (i + 1))
                    {

                        vdText tmpText = new vdText();
                        tmpText.SetUnRegisterDocument(document);
                        tmpText.setDocumentDefaults();
                        if (borderName == "A4")
                        {

                            tmpText.InsertionPoint = pointRatio[0] + new gPoint(8, -12);
                        }

                        else if (borderName == "A5")
                            tmpText.InsertionPoint = pointRatio[0] + new gPoint(-12, -12);
                        else if (borderName == "A6")
                            tmpText.InsertionPoint = pointRatio[0] + new gPoint(-12, -12);

                        tmpText.PenColor.ColorIndex = 3;
                        tmpText.Height = 3.5;
                        tmpText.WidthFactor = 0.7;
                        //将字写回到document中
                        tmpText.TextString = "0000.00.00";
                        tmpText.Update();
                        document.ActiveLayOut.Entities.AddItem(tmpText);
                        //document.Model.Entities.AddItem(tmpText);
                        document.Redraw(true);

                        //document.Update();
                        times.Add(tmpText);
                        x++;
                    }
                }
                else if (times.Count > (i + 1))
                {
                    int x = times.Count;
                    while (x != (i + 1))
                    {
                        times.RemoveAt(x - 1);
                        x--;
                    }
                }

                if (ratios.Count < (i + 1))   //比例
                {
                    int x = ratios.Count;
                    while (x != (i + 1))
                    {

                        vdText tmpText = new vdText();
                        tmpText.SetUnRegisterDocument(document);
                        tmpText.setDocumentDefaults();
                        if (borderName == "A4")
                        {

                            tmpText.InsertionPoint = pointRatio[0] + new gPoint(8, -5);
                        }

                        else if (borderName == "A5")
                            tmpText.InsertionPoint = pointRatio[0] + new gPoint(-12, -5);
                        else if (borderName == "A6")
                            tmpText.InsertionPoint = pointRatio[0] + new gPoint(-12, -5);

                        tmpText.PenColor.ColorIndex = 3;
                        tmpText.Height = 3.5;
                        tmpText.WidthFactor = 0.7;
                        //将字写回到document中
                       tmpText.TextString = " ";
                        tmpText.Update();
                        document.ActiveLayOut.Entities.AddItem(tmpText);
                        //document.Model.Entities.AddItem(tmpText);
                        document.Redraw(true);

                        //document.Update();
                        ratios.Add(tmpText);
                        x++;
                    }
                }
                else if (ratios.Count > (i + 1))
                {
                    int x = ratios.Count;
                    while (x != (i + 1))
                    {
                        ratios.RemoveAt(x - 1);
                        x--;
                    }
                }
                if (projectStages.Count < (i + 1))   //设计阶段
                {
                    int x = projectStages.Count;
                    while (x != (i + 1))
                    {
                        vdText tmpText = new vdText();
                        tmpText.SetUnRegisterDocument(document);
                        tmpText.setDocumentDefaults();
                        if (borderName == "A4")
                        {

                            tmpText.InsertionPoint = pointStage[0] + new gPoint(4, -4);
                        }

                        else if (borderName == "A5")
                            tmpText.InsertionPoint = pointStage[0] + new gPoint(4, -4);
                        else if (borderName == "A6")
                            tmpText.InsertionPoint = pointStage[0] + new gPoint(4, -4);

                        tmpText.PenColor.ColorIndex = 3;
                        tmpText.Height = 3.5;
                        tmpText.WidthFactor = 0.7;
                        //将字写回到document中
                        tmpText.TextString = "施工图";
                        tmpText.Update();
                        document.ActiveLayOut.Entities.AddItem(tmpText);
                        //document.Model.Entities.AddItem(tmpText);
                        document.Redraw(true);

                        projectStages.Add(tmpText);
                        x++;
                    }
                }
                else if (projectStages.Count > (i + 1))
                {
                    int x = projectStages.Count;
                    while (x != (i + 1))
                    {
                        projectStages.RemoveAt(x - 1);
                        x--;
                    }
                }
            }
        }

        internal void generateDirect(string directName)
        {
            this.analyzeText();
            List<vdPolyline> rootPolyline = this.getRootPolyline();
            this.analyzeTextGraph(rootPolyline);
            List<vdText> times1 = this.getTimes();
            List<vdText> projectName1 = this.getSubProjectNames();
            List<vdText> projectNumber1 = this.getProjectNumbers();

            int x = 174, y = 247;
            //
            try
            {
                int dirNum = 24;
                dirNum = (dirNum / 25) + 1;
                for (int i = 0; i < dirNum; i++)
                {
                    // MessageBox.Show(i + "");
                    //string dirname = "目录";
                    string blockname = directName;
                    //将生成的目录名block存入到相应的list数组中
                    // blocknamelist_dir.Add(blockname);
                    // generateborder(dirname, blockname);
                    AddExternalReferences_dir(directName, blockname);
                    AddReferencesInserts_dir(blockname);
                }
                //在生成好图框的基础上，将图序号改变加入到目录中
                for (int j = 0; j < times1.Count; j++)
                {
                    generateborderinformation_dir(directName, j, x, y, projectNumber1, projectName1);
                    y = y - 8;
                }
                //MessageBox.Show("目录成功添加");
            }
            catch
            {
                MessageBox.Show("添加图框失败，请检查根目录是否有图框。");
            }
        }

        /// <summary>
        /// 从目录中找到图框
        /// </summary>
        /// <param name="bordername"></param>
        /// <param name="blockname"></param>
        public void AddExternalReferences_dir(String bordername, String blockname)
        {
            try
            {
                string path = Application.StartupPath + "\\" + bordername + ".vdcl";
                VectorDraw.Professional.vdPrimaries.vdBlock xref = new VectorDraw.Professional.vdPrimaries.vdBlock();
                //MainForm1 m = new MainForm1();
                xref.SetUnRegisterDocument(document);
                xref.setDocumentDefaults();
                xref.Name = blockname;
                xref.ExternalReferencePath = path;

                xref.Update();
                document.Blocks.AddItem(xref);

            }
            catch
            {
                MessageBox.Show("未找到图框！");
            }
        }

        /// <summary>
        /// 图框以insert方式插入进去
        /// </summary>
        /// <param name="blockname"></param>
        public void AddReferencesInserts_dir(String blockname)
        {
            try
            {
                VectorDraw.Professional.vdFigures.vdInsert ins = new VectorDraw.Professional.vdFigures.vdInsert();
                ins.SetUnRegisterDocument(document);
                ins.setDocumentDefaults();
                VectorDraw.Professional.vdPrimaries.vdBlock blk = document.Blocks.FindName(blockname);
                if (blk != null)
                {
                    ins.Block = blk;
                    ins.InsertionPoint = new gPoint(0, 0, 0);
                    document.Model.Entities.AddItem(ins);
                }
                document.Model.ZoomExtents();
                document.Redraw(true);

            }
            catch
            {
                MessageBox.Show("插入图框失败！");
            }
        }

        /// <summary>
        /// 更新整个，目录
        /// </summary>
        /// <param name="blockname">块名</param>
        /// <param name="sequencenum">图框每一项的编号</param>
        /// <param name="x">插入距离横坐标</param>
        /// <param name="y">插入距离纵坐标</param>
        /// <param name="projecnumbers">项目编号集合</param>
        /// <param name="projectnames">项目名称集合</param>
        public void generateborderinformation_dir(String blockname, int sequencenum, int x, int y, List<vdText> projecnumbers, List<vdText> projectnames)
        {

            //字序号
            VectorDraw.Professional.vdFigures.vdText onetext = new VectorDraw.Professional.vdFigures.vdText();
            onetext.SetUnRegisterDocument(document);
            onetext.setDocumentDefaults();
            onetext.PenColor.ColorIndex = 3;
            onetext.Height = 3;
            onetext.WidthFactor = 0.7;
            //onetext.Style = "华文彩云";
            //text 插入位置
            VectorDraw.Professional.vdPrimaries.vdBlock vd = document.Blocks.FindName(blockname);
            vdInsert insert = new vdInsert();

            insert = (vdInsert)vd.GetReferenceObjects()[0];

            //MessageBox.Show(projectname_list[sequencenum] + "");
            int sequencenum1 = sequencenum + 1;
            string dirstring = sequencenum1 + "";
            onetext.TextString = dirstring;
            onetext.InsertionPoint = insert.InsertionPoint + new VectorDraw.Geometry.gPoint(x, y);
            //onetext.Update();
            vd.Entities.AddItem(onetext);
            vd.Update();

            //图号
            VectorDraw.Professional.vdFigures.vdText onetext1 = new VectorDraw.Professional.vdFigures.vdText();
            onetext1.SetUnRegisterDocument(document);
            onetext1.setDocumentDefaults();
            onetext1.PenColor.ColorIndex = 3;
            onetext1.Height = 3;
            onetext1.WidthFactor = 0.7;
            //onetext.Style = "华文彩云";
            //text 插入位置
            //VectorDraw.Professional.vdPrimaries.vdBlock vd = vdFramedControl1.BaseControl.ActiveDocument.Blocks.FindName(blockname);
            vdInsert insert1 = new vdInsert();

            insert1 = (vdInsert)vd.GetReferenceObjects()[0];
            string dirstring1 = projecnumbers[sequencenum].TextString;
            onetext1.TextString = dirstring1;
            onetext1.InsertionPoint = insert.InsertionPoint + new VectorDraw.Geometry.gPoint(x + 9, y);
            onetext1.Update();
            vd.Entities.AddItem(onetext1);
            vd.Update();

            //子图名
            VectorDraw.Professional.vdFigures.vdText onetext2 = new VectorDraw.Professional.vdFigures.vdText();
            onetext2.SetUnRegisterDocument(document);
            onetext2.setDocumentDefaults();
            onetext2.PenColor.ColorIndex = 3;
            onetext2.Height = 3;
            onetext2.WidthFactor = 0.7;
            //onetext.Style = "华文彩云";
            //text 插入位置
            // VectorDraw.Professional.vdPrimaries.vdBlock vd = vdFramedControl1.BaseControl.ActiveDocument.Blocks.FindName(blockname);
            vdInsert insert2 = new vdInsert();

            insert = (vdInsert)vd.GetReferenceObjects()[0];
            string dirstring2 = projectnames[sequencenum].TextString;
            onetext2.TextString = dirstring2;
            onetext2.InsertionPoint = insert.InsertionPoint + new VectorDraw.Geometry.gPoint(x + 52, y);
            vd.Entities.AddItem(onetext2);
            vd.Update();

        }


        /// <summary>
        /// 添加目录
        /// </summary>
        public void addDirectory()
        {
            this.analyzeText();
            List<vdPolyline> rootNum = this.getRootPolyline();
            ChooseDirectory cd = new ChooseDirectory(rootNum.Count);
            if (cd.ShowDialog() == DialogResult.OK)
            {
                String directname = cd.getDirectoryName();
                this.generateDirect(directname);
                layout.Update();
                document.Redraw(true);
                this.splitDirectory(directname);
            }
            else
            {

            }

        }

       
        public void Clear()
        {
            figures.Clear();
            polyLineEntities.Clear();
            splitEntities.Clear();
            times.Clear();
            projectNames.Clear();
            projectNumbers.Clear();
            projectStages.Clear();
            ratios.Clear();
        }
    
    }
}
