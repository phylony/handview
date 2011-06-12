using System;
using System.Collections.Generic;
using System.Linq;
using Emgu.CV.Structure;
using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Drawing;
using System.Runtime.InteropServices;


namespace HandGestureRecognition
{
    /// <summary>
    /// hand tracking class which implements the hand tracking algorithm
    /// </summary>
    class HandTracking
    {

        [DllImport("user32.dll", EntryPoint = "GetCursorPos")]
        static extern bool GetCursorPos(ref Point lpPoint);
        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        static extern bool SetCursorPos(int X, int Y);


        Image<Bgr, byte> colored_temp_image = null;

        Image<Gray, byte> previous_image = null,
                          current_image = null,
                          temp_Image = null;

        double quality,
               furthest_removed_precent,
               furthest_relocate_precent;
               



        List<double> temp = null;

        private const int c_WinSize = 10,
                          levels = 5;

        int tempX,
            tempY,
            block_size,
            kernel_size,
            min_distance,
            tracking_features_number,
            number_of_bad_features = 0,
            feature_default_speed = 0;
        
        
        DIST_TYPE dt = DIST_TYPE.CV_DIST_L2;


        
        PointF[][] foundFeaturesInChannels = null;


        PointF[] best_tracked_feature_Array = null,
                 previous_features = null,
                 current_features = null;
      
        public   PointF new_center_pt ,
                        old_cursor_location_pt,
                        new_cursor_location_pt;



        byte[] status = null;
        float[] point_error = null;

        Size window_size ;
        MCvTermCriteria criteria ;



        Dictionary<int, double> features = null,
                                 feature_speeds = null;
        Rectangle contour_rect ;

        HashSet<PointF> good_features = null,
                        bad_features = null,
                        normal_features = null;

        Random rand = null;

        bool[] lost_feature = null;
      
        public double global_time { get; set; }
        public int id { get; set; }


        /// <summary>
        /// class constructor 
        /// </summary>
        /// <param name="width">image width</param>
        /// <param name="height">image height</param>
        /// <param name="center">circle center</param>
        public HandTracking(int width, int height , PointF center)
        {
            // TODO: Complete member initialization
            this.current_image = new Image<Gray, byte>(width, height);
            this.previous_image = new Image<Gray, byte>(width, height);
            this.temp_Image = new Image<Gray, byte>(width, height);
            this.colored_temp_image = new Image<Bgr, byte>(width, height);
            this.new_center_pt = center;

            rand = new Random();
            Initialize();
        }

        public HandTracking()
        {
            // TODO: Complete member initialization
        }


        private void Initialize()
        {

            kernel_size = 3;
            quality = 0.1;
            block_size = 5;
            min_distance = 5;
            furthest_removed_precent = 0.1;
            furthest_relocate_precent = 0.1;
            tracking_features_number = 30;

            criteria = new MCvTermCriteria(20, 0.03);
            window_size = new System.Drawing.Size(3, 3);

            good_features = new HashSet<PointF>();
            bad_features = new HashSet<PointF>();
            normal_features = new HashSet<PointF>();

            temp = new List<double>(tracking_features_number);

            features = new Dictionary<int, double>(tracking_features_number);
            feature_speeds = new Dictionary<int, double>(tracking_features_number);

            lost_feature = new bool[30];

            new_center_pt = new PointF();
            old_cursor_location_pt = new PointF();
            new_cursor_location_pt = new PointF();
        }


        /// <summary>
        /// this function used to perform the " Initialization step " in Hand Tracking Algorithm 
        /// </summary>
        /// <param name="h">a contour point used to determine the bounding recatnagle around the hand</param>
        internal void ExtractFeatures(Emgu.CV.Contour<System.Drawing.Point> h)
        {
          
            GetImages();

            contour_rect = h.BoundingRectangle;
            temp_Image.ROI = contour_rect;
            current_image.ROI = contour_rect;

            foundFeaturesInChannels = current_image.GoodFeaturesToTrack(tracking_features_number, quality, min_distance, block_size);


            int founded_features = foundFeaturesInChannels[0].Length;

            for (int i = 0; i < founded_features; i++)
            {
                foundFeaturesInChannels[0][i].X += current_image.ROI.X;
                foundFeaturesInChannels[0][i].Y += current_image.ROI.Y;
                good_features.Add(foundFeaturesInChannels[0][i]);
           
            
            }

            
            
            temp_Image.ROI = Rectangle.Empty;
            current_image.ROI = Rectangle.Empty;



            int ptt = tracking_features_number - founded_features;

            if (ptt > 0)
                AddNewFeatures(ptt, foundFeaturesInChannels[0]);


            previous_features = good_features.ToArray();

            for (int i = 0; i < previous_features.Length; i++)
            {
                feature_speeds[i] = 1;
                lost_feature[i] = false;
            }

           
            good_features.Clear();
            bad_features.Clear();

   
         
            previous_image = current_image;

            Form1.NewImage = colored_temp_image;
           
        }

        /// <summary>
        /// this function implememnts the mouse movement 
        /// </summary>
        /// <param name="new_position">the old mouse crusor position</param>
        /// <param name="steps">the new mouse crusor position</param>
        public void LinearSmoothMove(Point new_position, int steps)
        {

            Point current_position = new Point();//Form1.MousePosition;// Cursor.Position;
            GetCursorPos(ref current_position);
            PointF iterPoint = current_position;

            // Find the slope of the line segment defined by start and newPosition
            PointF slope = new PointF(Math.Abs(new_position.X - current_position.X),Math.Abs(new_position.Y - current_position.Y));

            // Divide by the number of steps
            slope.X = slope.X / steps;
            slope.Y = slope.Y / steps;

            // Move the mouse to each iterative point.
            for (int i = 0; i < steps; i++)
            {
                iterPoint = new PointF(iterPoint.X + slope.X, iterPoint.Y + slope.Y);
                SetCursorPos(Point.Round(iterPoint).X,Point.Round(iterPoint).Y);
             //   System.Threading.Thread.Sleep(10);
            }

            // Move the mouse to the final destination.
            SetCursorPos(new_position.X,new_position.Y);
        }



        private Contour<Point> ExtractBiggestContour(Image<Gray, byte> local)
        {
            Contour<Point> biggestContour = null;
            MemStorage storage = new MemStorage();

            Contour<Point> contours = FindContours(local, Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE, Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_LIST, storage);


            Double Result1 = 0;
            Double Result2 = 0;
            while (contours != null)
            {
                Result1 = contours.Area;
                if (Result1 > Result2)
                {
                    Result2 = Result1;
                    biggestContour = contours;
                }
                contours = contours.HNext;

            }

            return biggestContour;
        }


        private Contour<Point> FindContours(Image<Gray, byte> local, CHAIN_APPROX_METHOD cHAIN_APPROX_METHOD, RETR_TYPE rETR_TYPE, MemStorage stor)
        {
            using (Image<Gray, byte> imagecopy = local.Copy()) //since cvFindContours modifies the content of the source, we need to make a clone
            {
                IntPtr seq = IntPtr.Zero;
                CvInvoke.cvFindContours(
                    imagecopy.Ptr,
                    stor.Ptr,
                    ref seq,
                    StructSize.MCvContour,
                    rETR_TYPE,
                    cHAIN_APPROX_METHOD,
                    new Point(local.ROI.X, local.ROI.Y));// because of ROI, the contour is offset or shifted 

                return (seq == IntPtr.Zero) ? null : new Contour<Point>(seq, stor);
            }
        }


        /// <summary>
        /// calculate the feature speeds 
        /// </summary>
        /// <param name="founded_features">the founded features </param>
        private void CalculateFeatureVelocity(PointF[] founded_features)
        {

            int key = -1,
                length = founded_features.Length,
                velocity_features_cnt = 0;
          
            double factor = 0.0;

                    

            for (int i = 0; i < length; i++)
            {
                if (status[i] == 0)
                {

                    feature_speeds[i] = feature_default_speed;
           
                    number_of_bad_features++;
                    continue;
                }

                velocity_features_cnt++;
                factor = CalcVelocity(founded_features[i], previous_features[i]);
                feature_speeds[i] += factor;
              
            }

            temp.AddRange(feature_speeds.Values);
            temp.Sort();
            int cnt2 = temp.Count;


            for (int ii = 1; ii <= velocity_features_cnt; ii++)
            {

                  key = (from k in feature_speeds where k.Value == temp.ElementAt(cnt2 - ii) select k.Key).FirstOrDefault();
                  if(ii > 15)
                  {
                      bad_features.Add(founded_features[key]);
                      
                  }
                  else 
                  {
                      good_features.Add(founded_features[key]);
                  }
                  

            }
          
            temp.Clear();
        }

        /// <summary>
        /// find the center of an object of a transform image
        /// </summary>
        /// <param name="binary_image"></param>
        /// <returns></returns>
        private PointF FindCentroidByDistanceTrans(Image<Gray, byte> binary_image)
        {
            double max_value = 0.0d,
                   min_value = 0.0d;

            Point max_location = new Point(0, 0),
                  min_location = new Point(0, 0);

            using (Image<Gray, float> distTransform = new Image<Gray, float>(binary_image.Width, binary_image.Height))
            {

                CvInvoke.cvDistTransform(binary_image, distTransform, dt, kernel_size, null, IntPtr.Zero);

                CvInvoke.cvMinMaxLoc(distTransform, ref min_value, ref max_value, ref min_location, ref max_location, IntPtr.Zero);

            }



          
            return max_location;

        


        }


        /// <summary>
        /// add new features if any feature has been removed or lost
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="other_points"></param>
        private void AddNewFeatures(int counter, PointF[] other_points)
        {
            int icounter,
                x,
                y;

            PointF pt = new PointF();

            for (int i = 0; i < counter; i++)
            {
                icounter = 0;
                do
                {
                    icounter++;
                    pt.X = x = rand.Next(contour_rect.X, contour_rect.X + contour_rect.Width);
                    pt.Y = y = rand.Next(contour_rect.Y, contour_rect.Y + contour_rect.Height);
                    

                } while (!(temp_Image[Math.Abs(y), Math.Abs(x)].Intensity > 200 
                           && NotTooCloseToOthers(pt, other_points)) 
                           && icounter < 20);

                if (icounter < 20)
                {

                    good_features.Add(pt);
                    other_points = good_features.ToArray();
                    colored_temp_image.Draw(new CircleF(pt, 3f), new Bgr(0, 0, 255), 1);

                }

            }

        }


        /// <summary>
        /// test if the features has been passed the flocking conditions
        /// </summary>
        /// <param name="current_features_local">the features to be tested</param>
        /// <returns>the passed features successfuly</returns>
        private PointF[] FindSkinColoredFeatures(PointF[] current_features_local)
        {
            // 
            int width = temp_Image.Width;// remove these variables to the constructor
            int height = temp_Image.Height;

            for (int i = 0; i < current_features_local.Length; i++)
            {


                    tempX = Convert.ToInt32(Math.Abs(current_features_local[i].X));// instead of these , use Point.Round function
                    tempY = Convert.ToInt32(Math.Abs(current_features_local[i].Y));

                    if (tempX >= width)// or u can use Math.Min(tempX,width-1);
                    {
                        current_features_local[i].X = width - 1;
                        tempX = (int)current_features_local[i].X;
                    }
                    if (tempY >= height)
                    {
                        current_features_local[i].Y = height - 1;
                        tempY = (int)current_features_local[i].Y;
                    }

               

                    if (!(temp_Image[tempY, tempX].Intensity > 200 &&
                        contour_rect.Contains(new Point(tempX, tempY)) &&
                        NotTooCloseToOthers(current_features_local[i], good_features.ToArray(),i)))// overloading function
                   
                    {
                      
                        if(good_features.Remove(current_features_local[i]))
                            number_of_bad_features++;                        
                    }

            }


            return good_features.ToArray();
        }



        private bool NotTooCloseToOthers(PointF our_point, PointF[] current_features_locals, int j)// overloading function
        {
            double distTemp = 0.0;
            bool ok = true;
            j = IndexOfPoint(current_features_locals, our_point);
            for (int i = 0; i < current_features_locals.Length; i++)
            {

                if (i == j)
                    continue;

                distTemp = FindDistance(our_point, current_features_locals[i]);
                if (distTemp < 3.0)
                {

                    ok = false;

                    break;
                }


            }
            return ok;
        }

        private bool NotTooCloseToOthers(PointF our_point, PointF[] current_features_locals)// overloading function
        {
            double distTemp = 0.0;
            bool ok = true;

            for (int i = 0; i < current_features_locals.Length; i++)
            {


                distTemp = FindDistance(our_point, current_features_locals[i]);
                if (distTemp < 3.0)
                {

                    ok = false;

                    break;
                }
             

            }
            return ok;
        }



        private double CalcVelocity(PointF pt1, PointF pt2)
        {
            //sw.Elapsed.TotalMilliseconds;

            return (FindDistance(pt1, pt2)) / global_time;
        }

        private double FindDistance(PointF p1, PointF p2)
        {
            double dx = p2.X - p1.X;
            double dy = p2.Y - p1.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// this function perform "Tracking step" in Hand Tracking Algorithm 
        /// </summary>
        /// <param name="elapsed_time">the interval time between frame i and frame i+1 used to calculate the feature speed</param>
        internal void StartTracking(double elapsed_time)
        {
            
        
            global_time += elapsed_time;
   
            GetImages();

            OpticalFlow.PyrLK(previous_image, current_image, previous_features, window_size, levels, criteria, out current_features, out status, out point_error);

            previous_image = current_image;

            CalculateFeatureVelocity(current_features);

            old_cursor_location_pt = new_cursor_location_pt;

            new_center_pt = GetCentroid(good_features.ToArray());

            ExtractHandRegion(new_center_pt);

            new_cursor_location_pt = contour_rect.Location;

            double movement_sensitivity = FindDistance(new_cursor_location_pt, old_cursor_location_pt);
            if(movement_sensitivity > 10)// to control the small movement of hand 
                LinearSmoothMove(Point.Round(new_center_pt), Convert.ToInt32(movement_sensitivity));

            best_tracked_feature_Array = FindSkinColoredFeatures(good_features.ToArray());
           

            if (best_tracked_feature_Array.Length == 0)
            {

                Console.WriteLine("exception inside number of good_features {0} ", good_features.Count);
                Console.WriteLine("exception inside best_tracked_feature_Array.Length == 0 ");

            }

            AddNewFeatures(number_of_bad_features, best_tracked_feature_Array);// u can move these line above the  
            //old_cursor_location_pt = new_cursor_location_pt;
            // and use good_features instead of best_tracked_feature_Array



            int diff = 30 - good_features.Count;
            if (diff > 0)
                AddNewFeatures(diff, good_features.ToArray());

            foreach (PointF p in good_features)
                colored_temp_image.Draw(new CircleF(p, 3f), new Bgr(Color.Cyan), -1);

            colored_temp_image.Draw(new CircleF(new_center_pt, 10f), new Bgr(Color.Blue), -1);
          
            previous_features = good_features.ToArray();
                     
            current_features = null;
            status = null;
            good_features.Clear();
            bad_features.Clear();
            number_of_bad_features = 0;

            //colored_temp_image.Save("H:\\debug\\test" + Convert.ToInt32(global_time) + ".jpg");

            Form1.NewImage = colored_temp_image;

        }
   

        private void GetImages()
        {
            current_image = Form1.Current_Image;
            colored_temp_image = Form1.NewImage;
            temp_Image = Form1.NewImageG;
        }


        /// <summary>
        /// used to calculate the median point 
        /// </summary>
        /// <param name="all_features">the features needed to extract the median point</param>
        /// <returns>median point</returns>
        private PointF GetCentroid(PointF[] all_features)
        {
            int cnt,
                key = -1,
                length = all_features.Length,
                removed_points_num = (int)(furthest_removed_precent * tracking_features_number),
                relcoate_points_num = (int)(furthest_relocate_precent * tracking_features_number),
                affected_points_num = removed_points_num + relcoate_points_num;


           
            features = CalculatePointsDistance(all_features);

            temp.AddRange(features.Values);
            temp.Sort();
      
            cnt = temp.Count;
            key = (from k in features where k.Value == temp.ElementAt(0) select k.Key).FirstOrDefault();

            PointF center = all_features[key];

            

            if (bad_features.Any())
            {
                temp.Clear();
                features.Clear();
                
                
                good_features.UnionWith(bad_features);
                
                all_features = good_features.ToArray();
               
                features = CalculatePointsDistance(all_features);
                
                temp = new List<double>(features.Values);
                temp.Sort();
                cnt = temp.Count;
                
               
                
                for (int ii = 1; ii <= affected_points_num; ii++) 
                {
                    key = (from k in features where k.Value == temp.ElementAt(cnt - ii) select k.Key).FirstOrDefault();
                    if (good_features.Remove(all_features[key]))
                    {
                        
                        if (ii > removed_points_num)
                        {
                            good_features.Add(RelocateTooFarPoint(all_features[key], center));
                        
                        }
                        else
                        {
                        
                            number_of_bad_features++;
                        }
                    }
                    

                }
            }
        

            features.Clear();
            temp.Clear();


            return center;

        }

        private Dictionary<int, double> CalculatePointsDistance(PointF[] all_features)
        {
            int length = all_features.Length;
            
            double distance_sum;
            
            Dictionary<int, double> temp = new Dictionary<int, double>(length);
           
            for (int i = 0; i < length; i++)
            {
                distance_sum = 0.0d;


                for (int j = 0; j < length; j++)
                {
                    if (i == j)
                    {
                        continue;

                    }

                    distance_sum += FindDistance(all_features[i], all_features[j]);

                }
                temp.Add(i, distance_sum);


            }

            return temp;
        }


        private int IndexOfPoint(PointF[] arr, PointF value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == value)
                {
                    return i;
                }
            }
            return -1;
        }

        private PointF RelocateTooFarPoint(PointF feature, PointF center_point)
        {




            PointF middle = new PointF();

            middle.X = (int)((feature.X + center_point.X) * 0.5);
            middle.Y = (int)((feature.Y + center_point.Y) * 0.5);

           
            LineSegment2DF line = new LineSegment2DF(feature, middle);
           // colored_temp_image.Draw(new CircleF(feature, 3f), new Bgr(Color.Black), 2);
            colored_temp_image.Draw(new CircleF(middle, 3f), new Bgr(0, 255, 0), 2);
            colored_temp_image.Draw(line, new Bgr(Color.DeepPink), 2);

            return middle;



        }

        private void ExtractHandRegion(PointF center_point)
        {
            // CvInvoke.cvBoundingRect(contour.Ptr, 1); //this is required before calling the InContour function

            //featuesInCurrentRegion = Array.FindAll(matchedFeature,
            //   delegate(MatchedSURFFeature f)
            //   { return contour.InContour(f.ObservedFeature.Point.pt) >= 0; });
            // use the above method to test which points inside the contour


            using (MemStorage storage = new MemStorage())
            {
                for (Contour<Point> i = temp_Image.FindContours(Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
                                                          Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_EXTERNAL,
                                                          storage);
                                    i != null;
                                    i = i.HNext)
                {
                    // t = i;
                    //    CvInvoke.cvBoundingRect(i.Ptr, 1);
                    if (i.InContour(center_point) > 0)
                    {
                        colored_temp_image.Draw(i, new Bgr(Color.Black), 2);
                     contour_rect = i.BoundingRectangle;
                     colored_temp_image.Draw(contour_rect, new Bgr(Color.Blue), 2);
                        break;
                    }
                }

            }

        }
    }
}
