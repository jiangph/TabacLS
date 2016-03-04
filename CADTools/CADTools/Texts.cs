using System;
using System.Collections.Generic;
using System.Text;
using VectorDraw.Professional.vdFigures;

namespace CADTools
{
    public class Texts
    {
        private vdText text;
        private String context;
        private String font;
        private double height;

        public Texts(vdText text)
        {
            this.text = text;
            this.setContext(text.TextString);
            this.setFont(text.Style.FontFile);
            this.setHeight(text.Height);
        }

        public Texts(String context,String font,int height)
        {
            this.context = context;
            this.font = font;
            this.height = height;
            text = new vdText();
        }

        public void setContext(String context)
        {
            this.context = context;
        }

        public String getContext()
        {
            return this.context;
        }

        public void setFont(String font)
        {
            this.font = font;
        }

        public String getFont()
        {
            return this.font;
        }

        public void setHeight(double height)
        {
            this.height = height;
        }

        public double getHeight()
        {
            return this.height;
        }

        public vdText getVdText()
        {
            text.TextString = this.getContext();
            text.Style.FontFile = this.getFont();
            text.Height = this.getHeight();
            text.Update();
            return text;
        }

    }
}
