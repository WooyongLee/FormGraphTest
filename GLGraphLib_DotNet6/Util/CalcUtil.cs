using System;
using System.Drawing;

namespace GLGraphLib
{
    public class CalcUtil
    {
        public static double CalculatePercentile(double min, double max, double input)
        {
            if (input < min) input = min;
            if (input > max) input = max;

            // Calculate the [input's position in the range as a percentage] / [range between the min and max values]
            double inputPercentile = (input - min) / (max - min);

            return inputPercentile;
        }

        // Calculate the Euclidean distance between two points
        private static double CalculateDistance(float x1, float y1, float x2, float y2)
        {
            return Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
        }

        // Calculate the number of points within a given radius of a specific point
        public static int CalculateClusterDensity(PointF[] points, float centerX, float centerY, double radius)
        {
            int count = 0;

            // Loop through all points and count those within the radius of the specified center point
            foreach (var point in points)
            {
                double distance = CalculateDistance(centerX, centerY, point.X, point.Y);

                if (distance <= radius)
                {
                    count++;
                }
            }

            return count;
        }
    }
}
