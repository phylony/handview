using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Text;
using System.Windows.Forms;
using Emgu.CV.Structure;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.VideoSurveillance;

using HandGestureRecognition.SkinDetector;
using System.Collections;


namespace HandGestureRecognition
{
    public partial class Form1 : Form
    {

        Seq<Point> convexityHull;
        Seq<MCvConvexityDefect> defects;

        Random rand = new Random();

     
        Capture grabber;
        AdaptiveSkinDetector detector;
        
        int numberOfPeaks = 0;
        int numberOfValleys = 0;
        int numberOfpoints = 20 ;
        int threshold = 80;
        double area = 0.0;
        double[] maxValues,
                 minValues;
        Point[] minLocation,
                maxLocation;

        int count = 0;


        Hsv hsv_min;
        Hsv hsv_max;
        Ycc YCrCb_min;
        Ycc YCrCb_max;

        Bgr color_blue = new Bgr(Color.DarkBlue);
        Bgr color2_brown = new Bgr(Color.Brown);

        Gray thresholdValue = new Gray(1);
        Gray MaxValue = new Gray(255);
        Gray cannyThreshold = new Gray(20);
        Gray cannyThresholdLinking = new Gray(70);
        Gray colorGray = new Gray(100);

        Point[] pointsArray;

        Image<Gray, byte> oldImage;
        Image<Gray, byte> newImageG;
        Image<Bgr, byte> newImage;
        Image<Gray, byte> tempImage;
        Image<Gray, byte> tempImage2;
        Image<Gray, byte> skin;
        Image<Gray, Single> distTransform;
        Image<Bgr, byte> hand_sep;
        Image<Bgr, byte> head_sep;
        Seq<Point> hull;
        Rectangle rect = new Rectangle();
        Rectangle roi_rect = new Rectangle();
        DIST_TYPE dt = DIST_TYPE.CV_DIST_L2;
        int kernel_size = 3;
       
        bool flag = false;
        List<Point> fingerCandiate = new List<Point>();
        List<Contour<Point>> handCandiate = new List<Contour<Point>>();
        List<KeyValuePair<Point, bool>> significantPts = new List<KeyValuePair<Point, bool>>();
        

        MCvBox2D box;
        
        private static MCvFont _font = new MCvFont(Emgu.CV.CvEnum.FONT.CV_FONT_HERSHEY_SIMPLEX, 1.0, 1.0);

        int min_distance = 5;
        int numbrt_of_features = 100;
        int numbrt_of_features_tracked = 30;
        double quality = 0.01;
        int block_size = 5;
        List<PointF> best_tracked_feature_list = new List<PointF>();
        private const int c_WinSize = 10;
        int tempX = 0,tempY = 0;
        PointF[] best_tracked_feature_Array ;
        PointF[][] foundFeaturesInChannels = null;
        bool start_tracking = false;
        /* variables for optical flow traking algorithem */
        Image<Gray, byte> prev_image;
        Image<Gray, byte> current_image;
    //    Image<Gray, byte> prev_image_pyramid;
     //   Image<Gray, byte> current_image_pyramid;
        PointF[] previous_features;
        PointF[] current_features;
        byte  [] status;
        float [] point_error;
        Size window_size;
        MCvTermCriteria criteria;
        const int levels = 5;
        List<PointF> features_need_relocate;
        int height;
        int width;
      //  Emgu.CV.CvEnum.LKFLOW_TYPE algo_flags = Emgu.CV.CvEnum.LKFLOW_TYPE.DEFAULT;
        PointF center_pts;
        System.Diagnostics.Stopwatch sw;
        double elapsed_time;
        Dictionary<int, double> features;
        public Form1()
        {
            InitializeComponent();

           

            grabber = new Emgu.CV.Capture();//@"c:\\test1.wmv"

        //    grabber.FlipHorizontal = true;
       //     grabber.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_WIDTH, 320);
        //    grabber.SetCaptureProperty(CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT, 240);
           

             height = (int)grabber.GetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_HEIGHT);
             width = (int)grabber.GetCaptureProperty(Emgu.CV.CvEnum.CAP_PROP.CV_CAP_PROP_FRAME_WIDTH);

          
          
            detector = new AdaptiveSkinDetector(1, AdaptiveSkinDetector.MorphingMethod.NONE);
            hsv_min = new Hsv(0, 45, 0); 
            hsv_max = new Hsv(20, 255, 255);            
            YCrCb_min = new Ycc(0, 131, 80);
            YCrCb_max = new Ycc(255, 185, 135);
            box = new MCvBox2D();


           

            hand_sep = new Image<Bgr,byte>(width,height);
            head_sep = new Image<Bgr,byte>(width,height);
      //      oldImage = new Image<Gray, Byte>(width, height);
        //    newImage = new Image<Bgr, Byte>(width, height);
            tempImage = new Image<Gray, Byte>(width, height);
            tempImage2 = new Image<Gray, Byte>(width, height);

         //   distTransform = new Image<Gray, Single>(width, height);
            rect = new Rectangle(20,20, 250, 250);
            features_need_relocate = new List<PointF>();
            features = new Dictionary<int, double>();
            sw = new System.Diagnostics.Stopwatch();
            skin = new Image<Gray, byte>(width, width);
        //    MessageBox.Show(height+ " "+width);

            prev_image = new Image<Gray, byte>(width, height);
            current_image = new Image<Gray, byte>(width, height);
            criteria = new MCvTermCriteria(20, 0.3);
            window_size = new System.Drawing.Size(10, 10);
            newImageG = new Image<Gray, byte>(width, height);
            Application.Idle += new EventHandler(FrameGrabber);                        
        }

        void FrameGrabber(object sender, EventArgs e)
        {
            
            newImage =  grabber.QueryFrame();//new Image<Bgr, byte>("c:\\test.jpg");

            count++;
            if (newImage != null)
            {
                current_image = newImage.Convert<Gray, byte>();
                detector.Process(newImage, tempImage);
                tempImage = tempImage.ThresholdBinary(thresholdValue, MaxValue);
                tempImage = tempImage.Dilate(2);
                tempImage = tempImage.SmoothMedian(3);
                newImageG = current_image.ThresholdBinary(new Gray(threshold), new Gray(255d));
                newImageG = newImageG.Not();
                newImageG = newImageG.And(tempImage);
                newImageG = newImageG.Dilate(1);
                if (true)
                {
                    if (!flag)
                    {
                        
                        newImage.Draw(rect, color_blue, 2);
                        //detector.Process(newImage, tempImage);
                        //tempImage = tempImage.ThresholdBinary(thresholdValue, MaxValue);
                        //tempImage = tempImage.Dilate(2);
                        //tempImage = tempImage.SmoothMedian(3);
                    }
                    else
                    {
                        if (start_tracking)
                        {

                            Console.WriteLine();
                            sw.Start();
                            OpticalFlow.PyrLK(prev_image, current_image, previous_features, window_size, levels, criteria, out current_features, out status, out point_error);
                            sw.Stop();
                            elapsed_time = sw.Elapsed.TotalMilliseconds;
                            Swap(ref current_image, ref prev_image);
                            TrackingAndRelcocating(previous_features, current_features);
                           // var sorted_dictionary = (from entry in features orderby entry.Value ascending select entry);
                           List<double> values = new List<double>(features.Values);
                            values.Sort();
                            int key = (from k in features where k.Value == values.ElementAt(0) select k.Key).FirstOrDefault();
                          
                            

                            center_pts = current_features[key];
                            rect.X = (int)center_pts.X - rect.Width / 2;
                            rect.Y = (int)center_pts.Y + rect.Height / 2;
                            //current_image.SetZero();
                            int cnt = values.Count;
                        //    center_pts = current_features[key];
                            //current_image.SetZero();
                            int cnt2 = features.Count;
                            Console.WriteLine("number inside dictionary " + cnt2);
                            best_tracked_feature_Array = FindSkinFeatures(current_features, status);// features with skin colors

                            for (int ii = 1; ii < 4; ii++)// remove 10% most far features from center point 
                            {
                                key = (from k in features where k.Value == values.ElementAt(cnt - ii) select k.Key).FirstOrDefault();
                                if (!this.features_need_relocate.Contains(current_features[key]))
                                    this.features_need_relocate.Add(current_features[key]);
                            }
                        //    best_tracked_feature_Array = FindSkinFeatures(current_features,status);// features with skin colors
                            //  Console.WriteLine(best_tracked_feature_Array.Length);
                            //   best_tracked_feature_Array = best_tracked_feature_list.ToArray();
                            if (best_tracked_feature_Array.Length == 0)
                            {
                                MessageBox.Show("the program will be crashed");
                                Application.Exit();

                            }
                            foreach (PointF p in best_tracked_feature_Array)
                            {

                                newImage.Draw(new CircleF(p, 3.0f), color2_brown, -1);
                                //tt[jj].X = (int)p.X;
                                //tt[jj].Y = (int)p.Y;
                                //jj++;
                                // Console.WriteLine(count++);
                            }
                             
                          //   box = PointCollection.MinAreaRect(best_tracked_feature_Array);
                           //  center_pts = box.center;
                            newImage.Draw(new CircleF(center_pts, 10f), new Bgr(255, 0, 0), -1);
                            newImage.Save("H:\\debug\\test" + count + ".jpg");
                        //    newImage.Draw(box, new Bgr(255, 0, 0), 1);
                        //    RelocatePoints(best_tracked_feature_Array, current_features, center_pts);
                            RelocatePoints(features_need_relocate.ToArray(), center_pts);
                            
                            previous_features = best_tracked_feature_list.ToArray();

                            //  Array.Clear(previous_features, 0, previous_features.Length);
                            //previous_features = null;
                           // Swap(ref best_tracked_feature_Array, ref previous_features);
                            current_image.Dispose();
                          //  current_image = null;
                            current_features = null;
                            features_need_relocate.Clear();
                            best_tracked_feature_list.Clear();
                            this.features.Clear();
                            values.Clear();
                          //  best_tracked_feature_Array = best_tracked_feature_list.ToArray();
                            //   Point [] tt = new Point [best_tracked_feature_30.Length];
                      //   int jj = 0;
                            //newImage.Draw(rect, color_blue, 2);
                            //foreach (PointF p in foundFeaturesInChannels[0])
                            //{

                            //    tempImage2.Draw(new CircleF(p, 3.0f), new Gray(255), 1);
                            //    //tt[jj].X = (int)p.X;
                            //    //tt[jj].Y = (int)p.Y;
                            //    //jj++;
                            //    // Console.WriteLine(count++);
                            //}


                            //   Array.Sort(best_tracked_feature_30, new PointSort(PointSort.Mode.X));
                            //  Array.Sort(best_tracked_feature_30, new PointSort(PointSort.Mode.Y));
                            //    PointF center_pts = FindCentroid(best_tracked_feature.ToArray());
                            //int temp_length = best_tracked_feature_30.Length;
                            //for (int i = 0; i < temp_length - 1; i++)
                            //{
                            //    tempX +=(int) (best_tracked_feature_30[i + 1].X + best_tracked_feature_30[i].X);
                            //    tempY += (int)(best_tracked_feature_30[i + 1].Y + best_tracked_feature_30[i].Y); 
                            //}
                            //imageBox2.Image = newImageG;
                            //imageBoxFrameGrabber.Image = tempImage2;
                            //    best_tracked_feature = Utility.DouglasPeuckerReduction(best_tracked_feature, 10.0);
                            //        Contour<Point> ff =(Contour<Point>) tt;

                         //   box = PointCollection.MinAreaRect(best_tracked_feature_Array);
                            //tempImage2.Draw(box, new Gray(255), 1);

                            //using (MemStorage mem = new MemStorage())
                            //{
                            //    best_tracked_feature = PointCollection.convexhull(best_tracked_feature_30, 
                            //                                                        mem, 
                            //                                                        Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE).ToList();
                            //}
                            //if (true)
                            //{
                            //    MessageBox.Show("d u want to show polyline");
                            //    tempImage2.DrawPolyline(tt, true, new Gray(255), 1);
                            //}
                            //      tempImage2.Draw(new Rectangle((int)center_pts.X - 3, (int)center_pts.Y - 3, 7, 7), new Gray(255), -1);
                            // draw_polyline(best_tracked_feature);
                         //   center_pts = box.center;//FindCentroid(best_tracked_feature.ToArray());
                         //   tempImage2.Draw(new CircleF(center_pts, 3.0f), new Gray(150), -1);
                         //   RelocatePoints(best_tracked_feature_list.ToArray(), foundFeaturesInChannels[0], center_pts);
                         // //  imageBox2.Image = newImageG;
                         ////   imageBoxFrameGrabber.Image = tempImage2;
                         //   //    Console.WriteLine(center_pts.X + " " + center_pts.Y);
                         //   //tempX = tempY = temp_length = 0;
                         //   previous_features = best_tracked_feature_list.ToArray();
                         //   best_tracked_feature_list.Clear();
                         
                         ////   flag = false;
                         //   start_tracking = true;
                         //   prev_image = current_image;
                         //   Console.WriteLine("cls");
                            //  Application.Exit();
                        }
                 //       else
                   //     {
                         //   Console.WriteLine();
                         //   OpticalFlow.PyrLK(prev_image, current_image, previous_features, window_size, levels, criteria, out current_features, out status, out point_error);
                         //   Swap(ref current_image, ref prev_image);
                         //   current_image.SetZero();

                         //   best_tracked_feature_Array = FindSkinFeatures(current_features);// features with skin colors
                         // //  Console.WriteLine(best_tracked_feature_Array.Length);
                         ////   best_tracked_feature_Array = best_tracked_feature_list.ToArray();
                         //   MCvBox2D box = PointCollection.MinAreaRect(best_tracked_feature_Array);
                         //   PointF center_pts = box.center;
                         //   newImage.Draw(new CircleF(center_pts, 10f), new Bgr(255,0,0), -1);
                         //   RelocatePoints(best_tracked_feature_Array, current_features, center_pts);
                        
                         //   best_tracked_feature_Array = best_tracked_feature_list.ToArray();

                         // //  Array.Clear(previous_features, 0, previous_features.Length);
                         //   previous_features = null;
                         //   Swap(ref best_tracked_feature_Array, ref previous_features);
                            
                         //   best_tracked_feature_list.Clear();
                         //   
                         //   Array.Clear(current_features, 0, current_features.Length);
                         //   Array.Clear(best_tracked_feature_Array, 0, best_tracked_feature_Array.Length);

                            

                      //  }
                    }

                }
                #region detection
                else /*if(count > 0)*/
                {
        
                    //detector.Process(newImage, tempImage);
                    //tempImage = tempImage.ThresholdBinary(thresholdValue, MaxValue);
                    //tempImage = tempImage.Dilate(2);
                    //tempImage = tempImage.SmoothMedian(3);

                    //newImageG = newImage.Convert<Gray, byte>();

                    //newImageG = newImageG.ThresholdBinary(new Gray(threshold), new Gray(255d));
                    //newImageG = newImageG.Not();
                    //newImageG = newImageG.And(tempImage);
                    //newImageG = newImageG.Dilate(1);
                    //    newImage = newImage.Copy(tempImage);

                    //  // newImageG = newImageG.Not();
                    //    skin = newImageG;
                    //   skin = skin.AbsDiff(oldImage);
                    ////   tempImage2 = tempImage2.Copy(skin);
                    //   newImage = newImage.Copy(skin);
                    //skin =skin.ThresholdBinary(new Gray(15), MaxValue);


                    //   // skin = skin.SmoothMedian(5);

                    //     skin = skin.Canny(cannyThreshold, cannyThresholdLinking);
                    //     skin = skin.Dilate(1);
                    ////   // skin = skin.PyrDown().PyrUp();




                //    oldImage = newImageG;
                    //     newImageG = newImageG.Canny(cannyThreshold, cannyThresholdLinking);
                    //     newImageG = newImageG.Canny(new Gray(120), new Gray(180));

                    //     newImageG = newImageG.Or(skin);

                    //  newImageG = newImageG.And(skin);
                    //  newImageG = newImageG.Dilate(2);



                    //     //newImageG = newImageG.Erode(1);
                    //    // newImageG = newImageG.Dilate(1);

                    //newImageG = newImageG.And(tempImage);
                    //newImageG = newImageG.Dilate(2);
                    //     newImageG = newImageG.And(skin);



                    //   newImageG = newImageG.Canny(cannyThreshold, cannyThresholdLinking);

                    //  newImageG = newImageG.Or(oldImage);
                    //    skin = skin.Canny(cannyThreshold, cannyThresholdLinking);

                 //  CvInvoke.cvDistTransform(newImageG, distTransform, dt, kernel_size, null, IntPtr.Zero);
                //    distTransform.MinMax(out minValues, out maxValues, out minLocation, out maxLocation);
                    ExtractContourAndHull(newImageG);

                    


                    // newImageG = newImageG.And(skin);
                }
                #endregion
                //if (maxValues != null && maxValues.Length != 0)
                //    for (int i = 0; i < maxValues.Length; i++)
                //        Console.WriteLine(maxValues[i] + " @ " + maxLocation[i] + " max length " + maxValues.Length);


                //minValues = null;
                //maxValues = null;
                //minLocation = null;
                //maxLocation = null;
               
                imageBoxSkin.Image = newImage;
                imageBoxFrameGrabber.Image = newImageG;
               //handImage.Image = hand_sep;
               //faceImage.Image = head_sep;
            
             
            }
        }

        //private PointF MedianPoint()
        //{
            
        //}


        private void TrackingAndRelcocating(PointF[] previous_features_locals, PointF[] current_features_locals)
        {
             int distTemp ;
             double   Temp ;
          //   bool ok = true;


            for (int i = 0; i < current_features_locals.Length; i++)
            {
                
                distTemp = 0;
                Temp = 0.0;
               // ok = true;
                for (int j = 0; j < current_features_locals.Length; j++)
                {
                    if (i != j)
                    {
                        distTemp = (int)find_distance(current_features_locals[i],current_features_locals[j]);
                        if(distTemp < 3)
                        {
                            if(! features_need_relocate.Contains(current_features[i]))
                                this.features_need_relocate.Add(current_features_locals[i]);
                            //ok = false;
                            Temp = Double.MaxValue;
                            break;
                        }
                        else
                        {
                           // best_tracked_feature_list.Add(current_features_locals[i]);
                            double Vi = calc_velocity(current_features_locals[i],previous_features_locals[i]);
                            double Vj = calc_velocity(current_features_locals[j],previous_features_locals[j]);
                            double factor = FindVelocityFactor(Vi,Vj);
                            double d = factor * distTemp;
                            Temp += d;
                            

                        }
                    }
                }
              //  if(ok)
                 features.Add(i,Temp);
            }
        }

        private double FindVelocityFactor(double Vi_local, double Vj_local)
        {
            double temp = Vj_local/(Vi_local+Vj_local);
            return (temp * temp);
            
        }

        private double calc_velocity(PointF pt1, PointF pt2)
        {
            //sw.Elapsed.TotalMilliseconds;
            
            return (find_distance(pt1, pt2)) / elapsed_time;
        }


        private /*List<PointF>*/PointF []  FindSkinFeatures(PointF[] current_features_local,byte [] found_features)
        {
            int tempX, tempY;
           
            Console.WriteLine("height " + newImageG.Height + " width " + newImageG.Width);
            Console.WriteLine("# of features before skin " + current_features_local.Length);
            for (int i = 0; i < current_features_local.Length; i++)
            {
                if (found_features[i] == 0)// feature i not found in current frame
                {
                    Console.WriteLine("feature "+i+" not found");
                    continue;
                }
                else
                {
                    
                    tempX = Convert.ToInt32(Math.Abs(current_features_local[i].X));
                    tempY = Convert.ToInt32(Math.Abs(current_features_local[i].Y));
                    Console.WriteLine("height " + tempX + " width " + tempY);
                    if (/*tempX < height && tempY < width &&*/ newImageG[tempX, tempY].Intensity > 200 && rect.Contains(new Point(tempX,tempY)))
                    {
                        best_tracked_feature_list.Add(current_features_local[i]);
                    }
                    else
                    {

                        if (!this.features_need_relocate.Contains(current_features_local[i]))
                            this.features_need_relocate.Add(current_features[i]);
                    }
                }
            }
                //foreach (PointF p in current_features_local)
                //{
                //    tempX = Convert.ToInt32(Math.Abs(p.X));
                //    tempY = Convert.ToInt32(Math.Abs(p.Y));
                //    Console.WriteLine("height " + tempX + " width " + tempY);
                //    if()
                ////    if (tempX < height && tempY < width && (int)newImageG.Data[tempX, tempY, 0] == 255)
                ////    {
                ////        best_tracked_feature_list.Add(p);
                ////    }

                //}
                Console.WriteLine("inside FindSkinFeatures after Count is " + best_tracked_feature_list.Count);
            return best_tracked_feature_list.ToArray();
        }

        static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }


     //   private void RelocatePoints(PointF[] best_tracked_feature_local, PointF[] all_features, PointF center_point)
     //   {
     //       List<PointF> temp = new List<PointF>();
     //       Console.WriteLine("inside RelocatePoints before Count is " + best_tracked_feature_list.Count);
     //       for (int i = 0; i < all_features.Length; i++)
     //       {
     //           if (Array.IndexOf(best_tracked_feature_local, all_features[i]) < 0)
     //           {
     //               temp.Add(all_features[i]);
     //               //best_tracked_feature_list.Add(all_features[i]);
     //           }       
     //       }
           
     ////       MessageBox.Show(temp.Count + "");
     //       PointF middle = new PointF();
     //       foreach (PointF pt in temp)
     //       {
     //           middle.X = (pt.X + center_point.X) / 2;
     //           middle.Y = (pt.Y + center_point.Y) / 2;
     //           best_tracked_feature_list.Add(middle);

     //     //      MessageBox.Show(middle.X+ " next point "+middle.Y);
     //      //     tempImage2.Draw(new CircleF(middle, 3.0f), new Gray(255), 1);
     //     //     imageBox2.Image = tempImage2;

     //       }

     //       Console.WriteLine("inside RelocatePoints after Count is " + best_tracked_feature_list.Count);
     //   }

        private void RelocatePoints(PointF[] all_features, PointF center_point)
        {
            List<PointF> temp = all_features.ToList();
            //Console.WriteLine("inside RelocatePoints before Count is " + best_tracked_feature_list.Count);
            //for (int i = 0; i < all_features.Length; i++)
            //{
            //    if (Array.IndexOf(best_tracked_feature_local, all_features[i]) < 0)
            //    {
            //        temp.Add(all_features[i]);
            //        //best_tracked_feature_list.Add(all_features[i]);
            //    }       
            //}

            //       MessageBox.Show(temp.Count + "");
            PointF middle = new PointF();
            int index = 0;
            foreach (PointF pt in all_features)
            {
                index = (best_tracked_feature_list.IndexOf(pt));
                if (index > -1)
                {
                    best_tracked_feature_list.RemoveAt(index);
                }
                middle.X = (pt.X + center_point.X) / 2;
                middle.Y = (pt.Y + center_point.Y) / 2;
              
                best_tracked_feature_list.Add(middle);

                //      MessageBox.Show(middle.X+ " next point "+middle.Y);
                //     tempImage2.Draw(new CircleF(middle, 3.0f), new Gray(255), 1);
                //     imageBox2.Image = tempImage2;

            }

            Console.WriteLine("inside RelocatePoints after Count is " + best_tracked_feature_list.Count);
        }
               
        private void ExtractContourAndHull(Image<Gray, byte> skin)
        {
            using (MemStorage storage = new MemStorage())
            {
                Contour<Point> temp;
                
                //Console.WriteLine("inside using");
                
                

                for (Contour<Point> i = skin.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
                                                          Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_EXTERNAL, 
                                                          storage); 
                                    i != null; 
                                    i = i.HNext)
                {
                    area = i.BoundingRectangle.Height * i.BoundingRectangle.Width;

                 
                    
                    if (area > 3000 )
                    {
                        
                        //newImageG.Draw(i.GetMinAreaRect(), colorGray, 2);
                      //  Console.WriteLine(i.GetMinAreaRect().size);
                        pointsArray = i.ToArray();

                        for (int j = numberOfpoints; j < pointsArray.Length - numberOfpoints; j++)
                        {
                          //  int d = 30 + j;
                            //int middle = (int)(d - (30 / 2));
                            //LineSegment2D first = new LineSegment2D(pointsArray[j],pointsArray[middle]);
                            //LineSegment2D second = new LineSegment2D(pointsArray[middle], pointsArray[d]);

                            double[,] v1 = new double[2, 1] { { pointsArray[j].X - pointsArray[j - numberOfpoints].X }, { pointsArray[j].Y - pointsArray[j - numberOfpoints].Y } };
                            double[,] v2 = new double[2, 1] { { pointsArray[j].X - pointsArray[j + numberOfpoints].X }, { pointsArray[j].Y - pointsArray[j + numberOfpoints].Y } };

                            // this equation is quoted from http://www.mathworks.com/matlabcentral/newsreader/view_thread/276582
                            // and it is working very good
                            double angle = Math.Atan2(Math.Abs(det(v1, v2)), dot(v1, v2)) * (180.0 / Math.PI);
                            if (angle < 90 )
                            {
                                count++;
                              //  fingerCandiate.Add(pointsArray[middle]);
                                     int direction = dir(pointsArray[j-numberOfpoints],pointsArray[j], pointsArray[j+numberOfpoints]);
                                if(direction > 0)
                                {
                                    newImage.Draw(new CircleF(pointsArray[j], 2), color2_brown, 2);//valley brown true
                                    numberOfValleys++;
                                    significantPts.Add(new KeyValuePair<Point, bool>(pointsArray[j], true));
                                }
                                else
                                {
                                    newImage.Draw(new CircleF(pointsArray[j], 2), color_blue, 2);//peak blue false
                                    numberOfPeaks++;
                                    significantPts.Add(new KeyValuePair<Point, bool>(pointsArray[j], false));
                                }
                                
                               // newImage.Draw(first, color, 1);//blue
                            //    newImage.Draw(second, color2, 1);//brown
                                j += numberOfpoints;
                              //  Console.WriteLine(angle);
                                
                            }
                        }

                        if (numberOfPeaks >= 5 && numberOfValleys >= 4)
                        {
                           

                            handCandiate.Add(i);

                        }
                        count = 0;
                        numberOfPeaks = 0;
                        numberOfValleys = 0;
                        
                            //  temp = i.ApproxPoly(5.0);
                            //    newImage.Draw(i, color, 2);
                            //  i = i.ApproxPoly(10);//The subroutine ContourPerimeter() will take a contour and return its length.

                            //pointsArray = temp.ToArray();
                            //LineSegment2D[] edges = PointCollection.PolyLine(pointsArray, false);
                            //for (int j = 0; j < edges.Length - 1; j++)
                            //{


                            //    double angle = Math.Abs(
                            //      edges[j+1].GetExteriorAngleDegree(edges[j]));
                            //    if (angle < 60 && angle > 10)
                            //    {
                            //        newImage.Draw(edges[j], color2, 1);
                            //        newImage.Draw(edges[j + 1], color, 1);
                            //    }
                            //}

                          //  newImage.Draw(i, color2, 1);
                            //  int test = CvInvoke.cvCheckContourConvexity(temp);

                            //  if (test == 0)
                            //  {



                           // convexityHull = i.GetConvexHull(Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE, storage);
                           // defects = i.GetConvexityDefacts(storage, Emgu.CV.CvEnum.ORIENTATION.CV_CLOCKWISE);
                           //// newImage.Draw(temp, color, 0);
                           ////Console.WriteLine(defects.Total + "   "+i.Total);
                           // foreach (MCvConvexityDefect test in defects)
                           // {
                           //    // newImage.Draw(new CircleF(test.DepthPoint, 5), color, 0);
                           //    // newImage.Draw(new CircleF(test.StartPoint, 3), new Bgr(Color.White), 3);
                           //   //  newImage.Draw(new CircleF(test.EndPoint, 3), new Bgr(Color.Black), 3);
                           // }
                                                                
                            //if (temp.Total == 2)
                            //{
                            //    Point[] pts = i.ToArray();
                            //    LineSegment2D[] edges = PointCollection.PolyLine(pts, false);
                            //    double angle = Math.Abs(edges[1].GetExteriorAngleDegree(edges[0]));
                                
                            //    if (angle > 10 && angle < 90)
                            //    {
                            //        fingerCandidate.Add(new KeyValuePair<LineSegment2D, LineSegment2D>(edges[0], edges[1]));
                                    
                            //    }
                            //}
                            //newImageG.Draw(temp.GetMinAreaRect(), new Gray(100), 2);
                          //  Console.WriteLine("inside if");
                            
                            //newImageG.Draw(temp, colorGray, 0);
                            //newImageG.Draw(CvInvoke.cvBoundingRect(i, true), new Gray(100), 2);
                      //  }

                    }
                }

               // var h = hhh.OrderByDescending(key => key.Value);
                if (handCandiate.Count != 0)
                {

                    foreach (Contour<Point> hand in handCandiate)
                    {
                        newImage.Draw(hand, color2_brown, 2);
                      //  newImage.Draw(hand.BoundingRectangle, color2_brown, 1);
                        roi_rect = hand.BoundingRectangle;
                        CvInvoke.cvSetImageROI(skin, roi_rect);
                        distTransform = new Image<Gray, float>(roi_rect.Width, roi_rect.Height);
                        CvInvoke.cvDistTransform(skin, distTransform, dt, kernel_size, null, IntPtr.Zero);
                          distTransform.MinMax(out minValues, out maxValues, out minLocation, out maxLocation);
                         
                        
                        if (maxValues != null && maxValues.Length != 0)
                            for (int i = 0; i < maxValues.Length; i++)
                            {
                                Console.WriteLine(maxValues[i] + " @ " + maxLocation[i] + " max length " + maxValues.Length);
                                newImage.Draw(new CircleF(maxLocation[i],Convert.ToSingle( maxValues[i])), color_blue, 2);
                            }

                        CvInvoke.cvResetImageROI(skin);
                          minValues = null;
                          maxValues = null;
                          minLocation = null;
                          maxLocation = null;
                        //head_sep
                        //if (maxLocation_Point != null && maxLocation_Point.Length != 0)
                        //{
                        //    double dist = hand.Distance(maxLocation_Point[0]);
                        //    if (dist > 0)
                        //        newImage.Draw(new CircleF(maxLocation_Point[0], 5), color_blue, 1);
                        //}
                        

                    }

                    //foreach (var key in sortedList)
                    //{
                    //    Console.WriteLine("{0}: {1}", key, hhh[key]);
                    //}

                }

                handCandiate.Clear();
                //foreach (Point t in fingerCandiate)
                //{
                //    newImage.Draw(new CircleF(t,2), color2, 2);
                //}

                //fingerCandiate.Clear();
                //foreach (KeyValuePair<LineSegment2D, LineSegment2D> edgesTemp in fingerCandidate)
                //{
                //   // Console.WriteLine(count++);
                //    newImage.Draw(new LineSegment2D(edgesTemp.Key.P2, edgesTemp.Value.P1), color, 2);
                //}
                //CvInvoke.cvDrawContours(newImageG, contours, new MCvScalar(255, 255, 255), new MCvScalar(100, 100, 100), 0, 0, Emgu.CV.CvEnum.LINE_TYPE.EIGHT_CONNECTED, new Point(0, 0));
                
                //for (Contour<Point> i = contours; i != null; i = i.HNext)
                //{
                //    area = i.BoundingRectangle.Height * i.BoundingRectangle.Width;
                //    //PointCollection.BoundingRectangle(i.ToArray());


                // //   if (area > 3000 )
                //   // {
                //     //   hand_sep = i.

                //        temp = i.ApproxPoly(10);
                //        newImage.Draw(temp.GetMinAreaRect(), new Bgr(Color.Chocolate), 2);
                       
                //     //   newImage.Draw(temp, new Bgr(Color.Chocolate), 2);
                //      ///  newImage.Draw(CvInvoke.cvBoundingRect(i,true),new Bgr(Color.Black),2);
                        
                    
                //   // }
                //}



            }
        }

        private void draw_polyline(List<PointF> pts)
        {
            PointF[] simplified_ploy = pts.ToArray();
            //int length = pts.Count;
            //PointF original_point = new PointF(0.0F, 0.0F);
            //int iteration = 1;
            //PointF[] simplified_ploy = new PointF[pts.Count];
            //simplified_ploy[0] = find_nearest_point(pts, original_point);

            //pts.RemoveAt(pts.IndexOf(simplified_ploy[0]));
            //simplified_ploy[0] = pts.ElementAt(0);
            //MessageBox.Show(pts.Count + "before algorithem");
            //PointF [] simplified_ploy = Utility.DouglasPeuckerReduction(pts, 10.0).ToArray();

            //int length = simplified_ploy.Length;
            //MessageBox.Show(length + "after algorithem");
            //for (int i = 0; i < length; i++)
            //{
            //    LineSegment2DF line = new LineSegment2DF(simplified_ploy[i], simplified_ploy[(i + 1) % length]);
            //    tempImage2.Draw(line, colorGray, 2);
            //    MessageBox.Show("draw next line");
            //    imageBoxFrameGrabber.Image = tempImage2;
            //}

            //LineSegment2DF line;
            //for (; ; )
            //{
            //    //    if (i + 1 != 0)
            //    //   {
            //    if (iteration >= length)
            //        break;
            //    else
            //    {
            //        PointF nearest_point = find_nearest_point(pts, simplified_ploy[(iteration - 1)]);
            //        simplified_ploy[iteration] = nearest_point;

            //        pts.RemoveAt(pts.IndexOf(simplified_ploy[(iteration - 1)]));
            //        iteration++;
            //    }

            //}

           int length = simplified_ploy.Length;
            for (int i = 0; i < length; i++)
            {
                LineSegment2DF line = new LineSegment2DF(simplified_ploy[i], simplified_ploy[(i + 1) % length]);
                tempImage2.Draw(line, colorGray, 2);
            //    MessageBox.Show("draw next line");
             //   Console.WriteLine(simplified_ploy[i] + "  " + simplified_ploy[(i + 1) % length]);
                imageBoxFrameGrabber.Image = tempImage2;
            }
            // line = new LineSegment2DF(pts[i], nearest_point);
            //     }
            //else
            //{
            //    line = new LineSegment2DF(pts[i], pts[(i + 1) % pts.Length]);
            //}



        }

        private PointF find_nearest_point(List<PointF> pts,PointF point)
        {
            float min_dist = float.MaxValue,temp; 
            PointF nearest_point = new PointF(), 
                   current_point = point;

            for (int j = 0; j < pts.Count; j++)
            {
                temp = find_distance(current_point, pts[j]);
                if (temp < min_dist && temp != 0)
                {
                    min_dist = temp;
                    nearest_point = pts[j];
                }

            }

            return nearest_point;
        }

        private float find_distance(PointF p1, PointF p2)
        {
            float dx = p2.X - p1.X;
            float dy = p2.Y - p1.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);  
        }




        public PointF FindCentroid(PointF[] Points)
        {
            // Add the first point at the end of the array.
            int nuPoints = Points.Length;
            PointF[] pts = new PointF[nuPoints + 1];
            Points.CopyTo(pts, 0);
            pts[nuPoints] = Points[0];

            // Find the centroid.
            float X = 0;
            float Y = 0;
            float second_factor;
            for (int i = 0; i < nuPoints; i++)
            {
                second_factor =
                    pts[i].X * pts[i + 1].Y -
                    pts[i + 1].X * pts[i].Y;
                X += (pts[i].X + pts[i + 1].X) * second_factor;
                Y += (pts[i].Y + pts[i + 1].Y) * second_factor;
            }

            // Divide by 6 times the polygon's area.
            float polygon_area = PolygonArea(Points);
            X /= (6 * polygon_area);
            Y /= (6 * polygon_area);

            // If the values are negative, the polygon is
            // oriented counterclockwise so reverse the signs.
            if (X < 0)
            {
                X = -X;
                Y = -Y;
            }

            return new PointF(X, Y);
        }

        public double SignedPolygonArea2(PointF[] polygon)
        {
            int N = polygon.Length;
            int i, j;
            double area = 0;

            for (i = 0; i < N; i++)
            {
                j = (i + 1) % N;
                area += polygon[i].X * polygon[j].Y;
                area -= polygon[i].Y * polygon[j].X;
            }
            area /= 2.0;

            //return (area);
            return(area < 0 ? -area : area); //for unsigned
        }


        //public float PolygonArea(PointF[] Points)
        //{
        //    // Return the absolute value of the signed area.
        //    // The signed area is negative if the polyogn is
        //    // oriented clockwise.
        //    return Math.Abs(SignedPolygonArea(Points));
        //}

        private float PolygonArea(PointF[] Points)
        {
            // Add the first point to the end.
            int nuPoints = Points.Length;
            PointF[] pts = new PointF[nuPoints + 1];
            Points.CopyTo(pts, 0);
            pts[nuPoints] = Points[0];

            // Get the areas.
            float area = 0;
            for (int i = 0; i < nuPoints; i++)
            {
                area +=
                    (pts[i + 1].X - pts[i].X) *
                    (pts[i + 1].Y + pts[i].Y) / 2;
            }

            // Return the result.
            return Math.Abs(area);
        }


        private PointF CenterPoint(PointF[] points)
        {
            float Cx = 0.0F, Cy = 0.0F, A = 0.0F, temp1 = 0.0F, temp2 = 0.0F, temp3 = 0.0F;
            PointF []pts = new PointF[points.Length + 1];
            points.CopyTo(pts, 0);
            pts[points.Length] = points[0];
            for (int i = 0; i < points.Length; i++)
            {

                temp3 = ((pts[i].X * pts[i + 1].Y) - (pts[i + 1].X * pts[i].Y));
                Cx += (pts[i].X + pts[i + 1].X) * temp3;
                Cy += (pts[i].Y + pts[i + 1].Y) * temp3;
            //    A += temp3;
            }

            A = Math.Abs((float)GetArea(points));
         //   float temp4 = (1 / (6 * A));

            Cx /= (6 * A);
            Cy /= (6 * A);

            return new PointF(Math.Abs(Cx), Math.Abs(Cy));

        }

        private int dir(Point point1, Point point2,Point point3)
        {
            //int horizontal_leg = point2.X - point1.X;
            //int vertical_leg = point2.Y - point1.Y;
            

            //double result = Math.Atan2(vertical_leg, horizontal_leg) * (180.0 / Math.PI);
        //    Console.WriteLine("x = " + horizontal_leg + " ; y = " + vertical_leg+" angle = "+result);
           // return (result < 90 && result > 0) ? true : false; 
            //this equation is quoted from wikipedia http://en.wikipedia.org/wiki/Cross_product#Computational_geometry
            int result = ((point2.X - point1.X) * (point3.Y - point1.Y) - (point2.Y - point1.Y) * (point3.X - point1.X));
           
            return result;     

        }

        private double dot(double[,] v1, double[,] v2)
        {
            return ((v1[0, 0] * v2[0, 0]) + (v1[1, 0] * v2[1, 0]));
        }

        private double det(double[,] v1, double[,] v2)
        {
            return ((v1[0, 0] * v2[1, 0]) - (v1[1, 0] * v2[0, 0]));
        }

        static double GetDeterminant(double x1, double y1, double x2, double y2)
        {
            return x1 * y2 - x2 * y1;
        }

        static double GetArea(PointF[] vertices)
        {
            if (vertices.Length < 3)
            {
                return 0;
            }
            double area = GetDeterminant(vertices[vertices.Length - 1].X, vertices[vertices.Length - 1].Y, vertices[0].X, vertices[0].Y);
            for (int i = 1; i < vertices.Length; i++)
            {
                area += GetDeterminant(vertices[i - 1].X, vertices[i - 1].Y, vertices[i].X, vertices[i].Y);
            }
            return area / 2;
        }


        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 187)
            {
                if (threshold < 253)
                {
                    threshold += 2;
                }
            }
            else if (e.KeyValue == 189)
            {
                if (threshold > 2)
                {
                    threshold -= 2;
                }
            }
            else if (e.KeyValue == 13)
             {
                
              //  newImage.ROI = rect;
              //  tempImage.ROI = rect;
               //skin = new Image<Gray, byte>(rect.Width, rect.Height);
                //tempImage2 = new Image<Gray, byte>(rect.Width, rect.Height,new Gray(0));
            //    skin = tempImage;
              //  detector.Process(newImage, tempImage);
        //        newImageG = newImage.Convert<Gray, byte>();
                //newImageG.ROI = rect;
                CvInvoke.cvSetImageROI(newImageG, rect);
                foundFeaturesInChannels = newImageG.GoodFeaturesToTrack(numbrt_of_features_tracked, quality, min_distance, block_size);
                newImageG.FindCornerSubPix(foundFeaturesInChannels,new Size(10,10),new Size(-1,-1),new MCvTermCriteria(20,0.03));
                CvInvoke.cvResetImageROI(newImageG);
                //newImageG = newImageG.ThresholdBinary(new Gray(threshold), new Gray(255d));
                //newImageG = newImageG.Not();
                //newImageG = newImageG.And(tempImage);
                //newImageG = newImageG.Dilate(1);
              //  skin = newImageG;
             //   MessageBox.Show(foundFeaturesInChannels[0].Length + "");
                foreach (PointF p in foundFeaturesInChannels[0])
                {

                   // if (newImageG[(int)p.X, (int)p.Y].Intensity == 255)
                   // {
                        best_tracked_feature_list.Add(p);
                   // }
                }
            //    best_tracked_feature_Array = best_tracked_feature_list.ToArray();

                //box = PointCollection.MinAreaRect(best_tracked_feature_Array);
                //center_pts = box.center;//FindCentroid(best_tracked_feature.ToArray());
                //tempImage2.Draw(new CircleF(center_pts, 3.0f), new Gray(150), -1);
                //RelocatePoints(best_tracked_feature_Array, foundFeaturesInChannels[0], center_pts);
                //  imageBox2.Image = newImageG;
                //   imageBoxFrameGrabber.Image = tempImage2;
                //    Console.WriteLine(center_pts.X + " " + center_pts.Y);
                //tempX = tempY = temp_length = 0;
                previous_features = best_tracked_feature_list.ToArray();

                Console.WriteLine("# of previous features " + previous_features.Length);
                best_tracked_feature_list.Clear();
                best_tracked_feature_Array = null;
                //   flag = false;
                start_tracking = true;
                prev_image = current_image;
                Console.WriteLine("cls");
                //newImage.ROI = Rectangle.Empty;
                //tempImage.ROI = Rectangle.Empty;
                
               // foundFeaturesInChannels[0].Initialize();                                                                          
                flag = true;
            }
            else
                MessageBox.Show("key pressed " + e.KeyValue.ToString());
                  
            
        }

        //private void seprate_Image(List<Contour<Point>> Saved)
        //{
        //    Rectangle ROI_rect = new Rectangle();
        //   // MessageBox.Show("inside if "+Saved.Capacity);
        //    if (Saved.Capacity == 4)
        //    {
               
        //        Contour<Point> i = Saved.ElementAt<Contour<Point>>(0);
        //        ROI_rect = CvInvoke.cvBoundingRect(i, true);
        //        CvInvoke.cvSetImageROI(newImage, ROI_rect);
        //        //newImage.ROI = ROI_rect;
        //        hand_sep = newImage.Copy();

        //        i = Saved.ElementAt<Contour<Point>>(1);
        //        ROI_rect = CvInvoke.cvBoundingRect(i, true);
        //        CvInvoke.cvSetImageROI(newImage, ROI_rect);
        //        //newImage.ROI = ROI_rect;
        //        head_sep = newImage.Copy();
        //        CvInvoke.cvResetImageROI(newImage); 
        //       // newImage.ROI = null;

        //    }
        //}

      
                                      
    }

    public class PointSort : IComparer
    {
        public enum Mode
        {
            X,
            Y
        }

        Mode currentMode = Mode.X;

        public PointSort(Mode mode)
        {
            currentMode = mode;
        }

        //Comparing function
        //Returns one of three values - 0 (equal), 1 (greater than), 2 (less than)
        int IComparer.Compare(object a, object b)
        {
            PointF point1 = (PointF)a;
            PointF point2 = (PointF)b;

            if (currentMode == Mode.X) //Compare X values
            {
                if (point1.X > point2.X)
                    return 1;
                else if (point1.X < point2.X)
                    return -1;
                else
                    return 0;
            }
            else
            {
                if (point1.Y > point2.Y) //Compare Y Values
                    return 1;
                else if (point1.Y < point2.Y)
                    return -1;
                else
                    return 0;
            }
        }
    }
}