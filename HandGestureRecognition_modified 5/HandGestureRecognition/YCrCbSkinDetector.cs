using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emgu.CV.Structure;
using Emgu.CV;

namespace HandGestureRecognition.SkinDetector
{
    public class YCrCbSkinDetector
    {
        public  Image<Gray, byte> DetectSkin(Image<Bgr, byte> Img, IColor min, IColor max)
        {
            
            Image<Ycc, Byte> currentYCrCbFrame = Img.Convert<Ycc, Byte>();
            Image<Gray, byte> skin = new Image<Gray, byte>(Img.Width, Img.Height);
            skin = currentYCrCbFrame.InRange((Ycc)min,(Ycc) max);
            StructuringElementEx rect_12 = new StructuringElementEx(12, 12, 6, 6, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
            CvInvoke.cvErode(skin, skin, rect_12, 1);
            StructuringElementEx rect_6 = new StructuringElementEx(6, 6, 3, 3, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
            CvInvoke.cvDilate(skin, skin, rect_6, 2);
            return skin;
        }

        public Image<Gray, byte> DetectSkinRGB(Image<Bgr, byte> Img)
        {

            Image<Gray, Byte>[] channels = Img.Split();
            Image<Gray, Byte> imgBGR_Blue = channels[0];   // blue channel
            Image<Gray, Byte> imgBGR_Green = channels[1];   // green channel
            Image<Gray, Byte> imgBGR_Red= channels[1];   // red channel
            Image<Gray, Byte> imgBGR_Red_Temp ;
            Image<Gray, Byte> imgBGR_Green_Temp;
            Image<Gray, Byte> imgBGR_Blue_Temp;
            Image<Gray, byte> skin = new Image<Gray, byte>(Img.Width, Img.Height);

            imgBGR_Red_Temp = imgBGR_Red.ThresholdToZero(new Gray(80));
            imgBGR_Green_Temp = imgBGR_Red.Sub(imgBGR_Green).InRange(new Gray(0), new Gray(56));
            imgBGR_Blue_Temp = imgBGR_Red.Sub(imgBGR_Blue).InRange(new Gray(0), new Gray(98));

            skin = imgBGR_Blue_Temp.Xor(imgBGR_Green_Temp.Xor(imgBGR_Red_Temp)).Not();
            
            StructuringElementEx rect_12 = new StructuringElementEx(12, 12, 6, 6, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
            CvInvoke.cvErode(skin, skin, rect_12, 1);
            StructuringElementEx rect_6 = new StructuringElementEx(6, 6, 3, 3, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT);
            CvInvoke.cvDilate(skin, skin, rect_6, 2);
            return skin;
        }
        
    }
}
