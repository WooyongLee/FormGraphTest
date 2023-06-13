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
            // param data의 length가 다르다면 새로 생성
            if (data.Length > 0 && this.data.Length != data.Length)
            {
                this.data = new double[data.Length];
            }

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
