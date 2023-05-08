using System.Data.Common;

namespace GLGraphLib
{
    public class BarComponent
    {
        private double[] data;

        public BarComponent(int lengthOfCol)
        {
            data = new double[lengthOfCol];
        }

        public double GetData(int index)
        {
            return data[index];
        }

        public void SetData(double[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (i < this.data.Length)
                {
                    this.data[i] = data[i];
                }
            }
        }
    }
}
