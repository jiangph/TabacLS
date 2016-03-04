using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.QRCode.Codec;

namespace CADTools
{
    class QRCode
    {
        private String QRCodeImgPath;
        private String text;
        private int imgSize;

        public QRCode(String text) : this(text,8) {}

        public QRCode(String text, int imgSize)
        {
            this.text = text;
            this.imgSize = imgSize;
        }

        /// <summary>
        /// 生成二维码图片并保存到本地
        /// </summary>
        public void generateQRCodeImg()
        {
            
            String strpath = Application.StartupPath + "\\ImageCodes\\"; 
            Bitmap bmp = this.GetDimensionalCode(text);
           
            this.SaveImg(strpath, bmp);
        }

        /// <summary>
        /// 二维码的路径
        /// </summary>
        /// <returns>返回二维码路径</returns>
        public String getQRCodeImgPath()
        {            
            return QRCodeImgPath;
        }

        /// <summary>
        /// 生成二维码图片
        /// </summary>
        /// <param name="text">二维码的内容</param>
        /// <returns>二维码图片</returns>
        private Bitmap GetDimensionalCode(String text)
        {
            Bitmap bmp = null;
            try
            {
                QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
                qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                qrCodeEncoder.QRCodeScale = imgSize;
                qrCodeEncoder.QRCodeVersion = 0;
                qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                bmp = qrCodeEncoder.Encode(text);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
            return bmp;
        }

        /// <summary>
        /// 保存二维码图片到本地
        /// </summary>
        /// <param name="strPath">保存图片的路径</param>
        /// <param name="img">待保存的图片</param>
        public void SaveImg(string strPath, Bitmap img)
        {
            //保存图片到目录  
            try
            {
                if (img == null)
                {
                    MessageBox.Show("图片不能为空");
                    return;
                }
                if (!Directory.Exists(strPath))
                {
                    Directory.CreateDirectory(strPath);
                }
                string guid = this.getCurrentDateTime() + ".png";
                QRCodeImgPath = strPath + guid;
                //MessageBox.Show(QRCodeImgPath);
                img.Save(strPath+guid, System.Drawing.Imaging.ImageFormat.Png);
                img.Dispose();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        /// <summary>
        /// 返回时间的字符串year+month+day+hour+minute+second
        /// </summary>
        /// <returns>时间的字符串</returns>
        private String getCurrentDateTime()
        {
            DateTime dt = DateTime.Now;
            String str = "" + dt.Year + dt.Month + dt.Day + dt.Hour + dt.Minute + dt.Second;
            return str;
        }





    }
}
