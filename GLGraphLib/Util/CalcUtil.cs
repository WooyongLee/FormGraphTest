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
    }
}
