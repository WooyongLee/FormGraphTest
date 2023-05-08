using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLGraphLib
{
    public class CalcUtil
    {
        public static double CalculatePercentile(double min, double max, double input)
        {
            // Calculate the range between the min and max values
            double range = max - min;

            // Calculate the input's position in the range as a percentage
            double inputPercentile = (input - min) / range;

            return inputPercentile;
        }
    }
}
