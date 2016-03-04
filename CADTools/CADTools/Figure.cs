using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using VectorDraw.Professional.vdFigures;
using VectorDraw.Professional.vdObjects;
using VectorDraw.Professional.vdPrimaries;

namespace CADTools
{
    public class Figure
    {
        private Texts time;
        private Texts projectName;
        private Texts projectNumber;
        private vdFigure qrCode;
        private vdDocument document;
        private List<vdText> textEntities;

        public Figure(vdDocument document)
        {
            this.document = document;
            textEntities = new List<vdText>();
            //this.analyzeText();
        }

        /// <summary>
        /// 保存为Pdf
        /// </summary>
        public void saveAsPdf(String fileName)
        {
            try
            {
                vdPrint printer = document.Model.Printer;
                //printer.DocumentName = lineNo.ToString();
                printer.PrinterName = fileName;
                printer.Resolution = 300;
                printer.OutInBlackWhite = true;
                printer.paperSize = new System.Drawing.Rectangle(0, 0, 827, 1169);//A4
                printer.LandScape = false;
                printer.PrintExtents();
                printer.PrintScaleToFit();
                printer.CenterDrawingToPaper();
                printer.PrintOut();
            }
            catch { }
            finally
            {

            }
        }

        /// <summary>
        /// 匹配时间，项目名，项目号
        /// </summary>
        private void analyzeText()
        {
            this.addTextEntities();
            for (int i = 0; i < textEntities.Count; i++)
            {
                string textstring = textEntities[i].TextString;
                if (Regex.IsMatch(textstring, @"^((((1[6-9]|[2-9]\d)\d{2}).(0?[13578]|1[02]).(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2}).(0?[13456789]|1[012]).(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2}).0?2.(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$"))
                {
                    time = new Texts(textEntities[i]);
                } 
                else if (textEntities[i].TextString.Contains("-"))
                {
                    projectNumber = new Texts(textEntities[i]);
                }
                else
                {

                }
            }
        }

        /// <summary>
        /// 将document中所有的text添加进去
        /// </summary>
        private void addTextEntities()
        {
            foreach (vdFigure figure in document.Model.Entities) {
                if (figure._TypeName.Equals("vdText")) {
                    textEntities.Add((vdText)figure);
                }
            }
        }

        public vdDocument getDocument()
        {
            return this.document;
        }

        public Texts getTime()
        {
            return time;
        }
        public Texts getProjectName()
        {
            return projectName;
        }

        public Texts getProjectNumber()
        {
            return projectNumber;
        }

        public void setQRCode(vdFigure qrCode)
        {
            this.qrCode = qrCode;
        }

        public vdFigure getQRCode()
        {
            return qrCode;
        }
    }
}
