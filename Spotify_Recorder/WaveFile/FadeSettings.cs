using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Xml.Serialization;

namespace Spotify_Recorder
{
    /// <summary>
    /// Class that holds fade settings. The fade is described by the type and different parameters.
    /// </summary>
    public class FadeSettings
    {
        /// <summary>
        /// Where should the fade begin in ms
        /// </summary>
        public double FadeStartTime_ms { get; set; }

        /// <summary>
        /// How long should the fade last in ms
        /// </summary>
        public double FadeLength_ms { get; protected set; }

        /// <summary>
        /// Begin fade factor
        /// </summary>
        public double FadeBeginFactor { get; protected set; }

        /// <summary>
        /// End fade factor
        /// </summary>
        public double FadeEndFactor { get; protected set; }

        /// <summary>
        /// Fade only left or right channel or both
        /// </summary>
        public AudioChannels FadeChannels { get; protected set; }

        /// <summary>
        /// Type of the fade (Linear, Log, Hyp, Custom or Undo cases)
        /// </summary>
        public FadeTypes FadeType { get; protected set; }

        /// <summary>
        /// Factor that is used by the Log and Hyp fades
        /// </summary>
        public double FadeShapeFactor { get; protected set; }

        /// <summary>
        /// Fade points that describe the custom fade. Only used when FadeType is CUSTOM or UNDO_CUSTOM!
        /// </summary>
        public List<PointF> FadePoints { get; private set; }

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Create a new instance of FadeSettings class. The FadeType isn't allowed to be CUSTOM or UNDO_CUSTOM!!! Use other constructor instead to create CUSTOM fade settings.
        /// </summary>
        /// <param name="fadeStartTime_ms">Where should the fade begin in ms</param>
        /// <param name="fadeLength_ms">How long should the fade last in ms</param>
        /// <param name="fadeBeginFactor">Begin fade factor</param>
        /// <param name="fadeEndFactor">End fade factor</param>
        /// <param name="fadeChannels">Fade only left or right channel or both</param>
        /// <param name="fadeType">Type of the fade (Linear, Log, Hyp or Undo cases). Isn't allowed to be CUSTOM or UNDO_CUSTOM!!!</param>
        /// <param name="fadeShapeFactor">Factor that is used by the Log and Hyp fades</param>
        public FadeSettings(double fadeStartTime_ms, double fadeLength_ms, double fadeBeginFactor, double fadeEndFactor, AudioChannels fadeChannels, FadeTypes fadeType, double fadeShapeFactor = 10)
        {
            FadeStartTime_ms = fadeStartTime_ms;
            FadeLength_ms = fadeLength_ms;
            FadeBeginFactor = fadeBeginFactor;
            FadeEndFactor = fadeEndFactor;
            FadeChannels = fadeChannels;
            FadeShapeFactor = fadeShapeFactor;

            if (fadeType == FadeTypes.CUSTOM || fadeType == FadeTypes.UNDO_CUSTOM) { FadeType = FadeTypes.LINEAR; }
            else { FadeType = fadeType; }
        }

        //**********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Create a new instance of FadeSettings class. The FadeType must be CUSTOM or UNDO_CUSTOM!!!
        /// </summary>
        /// <param name="fadeStartTime_ms">The fade begin time. Time offset of FadePoints</param>
        /// <param name="fadeType">fade type. Must be CUSTOM or UNDO_CUSTOM!!!</param>
        /// <param name="fadePoints">Fade points that describe the custom fade</param>
        /// <param name="fadeChannels">channels that are faded</param>
        public FadeSettings(double fadeStartTime_ms, FadeTypes fadeType, List<PointF> fadePoints, AudioChannels fadeChannels)
        {
            FadePoints = fadePoints;
            if (FadePoints != null && FadePoints.Count > 0)
            {
                FadePoints = FadePoints.OrderBy(p => p.X).ToList();             //Sort after X-coordinate
                
                FadeLength_ms = (FadePoints.Last().X - FadePoints.First().X);
                FadeBeginFactor = FadePoints.First().Y;
                FadeEndFactor = FadePoints.Last().Y;
            }

            FadeStartTime_ms = fadeStartTime_ms;
            FadeChannels = fadeChannels;

            if (fadeType != FadeTypes.CUSTOM && fadeType != FadeTypes.UNDO_CUSTOM) { FadeType = FadeTypes.CUSTOM; }
            else { FadeType = fadeType; }
        }

        //**********************************************************************************************************************************************************************************************************

        /// <summary>
        /// Create a new instance of FadeSettings class. The FadeType must be CUSTOM or UNDO_CUSTOM!!!
        /// </summary>
        /// <param name="fadeStartTime_ms">The fade begin time. Time offset of FadePoints</param>
        /// <param name="fadeType">fade type. Must be CUSTOM or UNDO_CUSTOM!!!</param>
        /// <param name="fadePointsFile">XML file that contains the fade points that describe the custom fade</param>
        /// <param name="fadeChannels">channels that are faded</param>
        public FadeSettings(double fadeStartTime_ms, FadeTypes fadeType, string fadePointsFile, AudioChannels fadeChannels)
        {
            FadeStartTime_ms = fadeStartTime_ms;

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<PointF>));
                TextReader textReader = new StreamReader(fadePointsFile);
                FadePoints = (List<PointF>)serializer.Deserialize(textReader);
                textReader.Close();
            }
            catch (Exception)
            { }

            if (FadePoints != null)
            {
                FadePoints = FadePoints.OrderBy(p => p.X).ToList();             //Sort after X-coordinate

                FadeLength_ms = (FadePoints.Last().X - FadePoints.First().X);
                FadeBeginFactor = FadePoints.First().Y;
                FadeEndFactor = FadePoints.Last().Y;
                FadeChannels = fadeChannels;
                FadeShapeFactor = 10;

                if (fadeType != FadeTypes.CUSTOM && fadeType != FadeTypes.UNDO_CUSTOM) { FadeType = FadeTypes.CUSTOM; }
                else { FadeType = fadeType; }
            }
        }

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Save the FadePoints to the given XML file. Only possible if the FadeType is CUSTOM or UNDO_CUSTOM!!! Otherwise nothing is done.
        /// </summary>
        /// <param name="fadePointsFile">XML file that contains the fade points that describe the custom fade</param>
        public void SaveFadePoints(string fadePointsFile)
        {
            if (FadeType == FadeTypes.CUSTOM || FadeType == FadeTypes.UNDO_CUSTOM)
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<PointF>));
                TextWriter textWriter = new StreamWriter(fadePointsFile);
                serializer.Serialize(textWriter, FadePoints);
                textWriter.Close();

                /*StreamWriter writer = new StreamWriter(fadePointsFile.Replace(".xml", "") + "_2.txt");        //Save the points to a text file with the format "X Y"
                System.Globalization.NumberFormatInfo numberFormat = new System.Globalization.NumberFormatInfo() { NumberDecimalSeparator = "." };
                foreach (PointF p in FadePoints)
                {
                    writer.WriteLine(p.X.ToString(numberFormat) + " " + p.Y.ToString(numberFormat));
                }
                writer.Close();*/
            }
        }

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Calculate the fade factor using the given time and all other properties
        /// </summary>
        /// <param name="currentTime_ms">time in ms where the fade factor should be calculated</param>
        /// <returns>fade factor</returns>
        public virtual double GetFadeFactor(double currentTime_ms)
        {
            if(FadeBeginFactor > FadeEndFactor && FadeBeginFactor > 1)
            {
                FadeBeginFactor = 1;
                FadeEndFactor *= (1 / FadeBeginFactor);
            }
            else if (FadeEndFactor > FadeBeginFactor && FadeEndFactor > 1)
            {
                FadeEndFactor = 1;
                FadeBeginFactor *= (1 / FadeEndFactor);
            }

            double factor = 0;
            double time_hysteresis = 0.000001;
            if (currentTime_ms >= (FadeStartTime_ms - time_hysteresis) && currentTime_ms <= (FadeStartTime_ms + time_hysteresis))
            {
                return FadeBeginFactor;
            }
            else if(currentTime_ms >= (FadeStartTime_ms + FadeLength_ms - time_hysteresis) && currentTime_ms <= (FadeStartTime_ms + FadeLength_ms + time_hysteresis))
            {
                return FadeEndFactor;
            }

            switch (FadeType)
            {
                case FadeTypes.LINEAR:
                {
                    factor = GetLinearFactor(new PointF((float)FadeStartTime_ms, (float)FadeBeginFactor), new PointF((float)(FadeStartTime_ms + FadeLength_ms), (float)FadeEndFactor), currentTime_ms);
                    break;
                }
                case FadeTypes.LOG:
                {
                    factor = GetLogFactor(new PointF((float)FadeStartTime_ms, (float)FadeBeginFactor), new PointF((float)(FadeStartTime_ms + FadeLength_ms), (float)FadeEndFactor), currentTime_ms, FadeShapeFactor);
                    break;
                }
                case FadeTypes.HYPERBEL:
                {
                    factor = GetHypFactor(new PointF((float)FadeStartTime_ms, (float)FadeBeginFactor), new PointF((float)(FadeStartTime_ms + FadeLength_ms), (float)FadeEndFactor), currentTime_ms, FadeShapeFactor);
                    break;
                }
                case FadeTypes.CUSTOM:
                {
                    factor = GetCustomFactor(currentTime_ms);
                    break;
                }
                case FadeTypes.UNDO_LINEAR:
                {
                    factor = 1 / GetLinearFactor(new PointF((float)FadeStartTime_ms, (float)FadeBeginFactor), new PointF((float)(FadeStartTime_ms + FadeLength_ms), (float)FadeEndFactor), currentTime_ms); ;
                    break;
                }
                case FadeTypes.UNDO_LOG:
                {
                    factor = 1 / GetLogFactor(new PointF((float)FadeStartTime_ms, (float)FadeBeginFactor), new PointF((float)(FadeStartTime_ms + FadeLength_ms), (float)FadeEndFactor), currentTime_ms, FadeShapeFactor); ;
                    break;
                }
                case FadeTypes.UNDO_HYPERBEL:
                {
                    factor = 1 / GetHypFactor(new PointF((float)FadeStartTime_ms, (float)FadeBeginFactor), new PointF((float)(FadeStartTime_ms + FadeLength_ms), (float)FadeEndFactor), currentTime_ms, FadeShapeFactor); ;
                    break;
                }
                case FadeTypes.UNDO_CUSTOM:
                {
                    factor = 1 / GetCustomFactor(currentTime_ms);
                    break;
                }
            }

            return factor;
        }

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Get linear fade factor
        /// </summary>
        /// <param name="Start">start point of straight line</param>
        /// <param name="End">end point of straight line</param>
        /// <param name="x">x-coordinate where to calculate the y-value</param>
        /// <returns>y-value</returns>
        private double GetLinearFactor(PointF Start, PointF End, double x)
        {
            // y = ((y1-y0)/(x1-x0)) * (x - x0) + y0
            double factor = ((End.Y - Start.Y) / (End.X - Start.X)) * (x - Start.X) + Start.Y;
            return factor;
        }

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Get logarithmic fade factor
        /// </summary>
        /// <param name="Start">first point of logarithmic function</param>
        /// <param name="End">second point of logarithmic function</param>
        /// <param name="x">x-coordinate where to calculate the y-value</param>
        /// <param name="shape">shape factor that is used to calculate the log function</param>
        /// <returns>y-value</returns>
        private double GetLogFactor(PointF Start, PointF End, double x, double shape)
        {
            double factor = 1, c0 = shape, c1 = 0, c2 = 0;

            if (End.Y >= Start.Y)       //Fade in
            {
                // y = log_c0(x + c1) + c2 
                c1 = (End.X * Math.Pow(c0, Start.Y - End.Y) - Start.X) / (1 - Math.Pow(c0, Start.Y - End.Y));
                c2 = Start.Y - Math.Log(Start.X + c1, c0);
                factor = (x == Start.X) ? Start.Y : (Math.Log(x + c1, c0) + c2);
            }
            else                        //Fade out
            {
                // y = log_c0(-x + c1) + c2 
                c1 = Start.X - (((End.X - Start.X) * Math.Pow(c0, Start.Y - End.Y)) / (1 - Math.Pow(c0, Start.Y - End.Y)));
                c2 = Start.Y - Math.Log(-Start.X + c1 + (End.X - Start.X), c0);
                factor = (x == Start.X) ? Start.Y : (Math.Log(-x + c1, c0) + c2);
            }
            return factor;
        }

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Get hyperbolic fade factor
        /// </summary>
        /// <param name="Start">first point of hyperbolic function</param>
        /// <param name="End">second point of hyperbolic function</param>
        /// <param name="x">x-coordinate where to calculate the y-value</param>
        /// <param name="shape">shape factor that is used to calculate the hyp function</param>
        /// <returns>y-value</returns>
        private double GetHypFactor(PointF Start, PointF End, double x, double shape)
        {
            double factor = 1, c0 = shape, c1 = 0, c2 = 0;
            double fade_length = End.X - Start.X;

            if (End.Y >= Start.Y)       //Fade in
            {
                // y = (c0 / (-x + c1)) + c2
                c1 = Start.X + (fade_length / 2) + 0.5 * Math.Sqrt(Math.Pow(fade_length, 2) - ((4 * c0 * fade_length) / (Start.Y - End.Y)));
                c2 = Start.Y - (c0 / (-Start.X + c1));
                factor = (c0 / (-x + c1)) + c2;
            }
            else                        //Fade out
            {
                // y = (c0 / (x + c1)) + c2
                c1 = -Start.X - (fade_length / 2) + 0.5 * Math.Sqrt(Math.Pow(fade_length, 2) + ((4 * c0 * fade_length) / (Start.Y - End.Y)));
                c2 = Start.Y - (c0 / (Start.X + c1));
                factor = (c0 / (x + c1)) + c2;
            }
            return factor;
        }

        //##########################################################################################################################################################################################################

        /// <summary>
        /// Calculate the fade factor using the given time, the FadeStartTime_ms and the list of FadePoints 
        /// </summary>
        /// <param name="time_ms">time in ms where the fade factor should be calculated.</param>
        /// <returns>fade factor</returns>
        private double GetCustomFactor(double time_ms)
        {
            for (int i = 0; i < FadePoints.Count; i++)
            {
                if(i == 0 && time_ms < (FadePoints[i].X + FadeStartTime_ms))        //before first FadePoint
                {
                    return 1;
                }
                else if (time_ms == (FadePoints[i].X + FadeStartTime_ms))           //exactly at any FadePoint
                {
                    return FadePoints[i].Y;
                }
                else if (i == (FadePoints.Count - 1) && time_ms > (FadePoints[i].X + FadeStartTime_ms))        //after last FadePoint
                {
                    return 1;
                }
                else if(i == (FadePoints.Count - 1) && time_ms > (FadePoints[i - 1].X + FadeStartTime_ms) && time_ms < (FadePoints[i].X + FadeStartTime_ms))    //between last FadePoint and previous FadePoint
                {
                    PointF p1 = FadePoints[i - 1];
                    PointF p2 = FadePoints[i];

                    double y = ((p2.Y - p1.Y) / (p2.X - p1.X)) * (time_ms - (p1.X + FadeStartTime_ms)) + p1.Y;      //Equation of straight line through p1 and p2
                    return y;
                }
                else if(time_ms > (FadePoints[i].X + FadeStartTime_ms) && time_ms < (FadePoints[i + 1].X + FadeStartTime_ms))       //between 2 FadePoints
                {
                    PointF p1 = FadePoints[i];
                    PointF p2 = FadePoints[i + 1];

                    double y = ((p2.Y - p1.Y) / (p2.X - p1.X)) * (time_ms - (p1.X + FadeStartTime_ms)) + p1.Y;      //Equation of straight line through p1 and p2
                    return y;
                }
            }
            return 1;
        }

    }
}
