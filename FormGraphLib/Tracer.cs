using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormGraphLib
{
    public class Tracer
    {
        public static readonly int MaxTotalTraceCount = 4;

        public const int MaxSample = 2002;

        public double[] data = new double[MaxSample];

        public int WorkMode = 0;
        public int WorkType = 0;

        static public Pen[] LineColor = new Pen[] { Pens.DarkOrange, Pens.DarkCyan, Pens.DarkGoldenrod, Pens.Green };

        public Tracer()
        {
            // Generate Sample Data
            Random random = new Random();

            // Draw Random (아무 신호가 없는 잡음을 표현)
            for (int i = 0; i < MaxSample; i++)
            {
                int value = random.Next(-67, -53);

                data[i] = value;
            }

            // Draw Linear
            //for (int i = 0; i < MaxSample; i++)
            //{
            //    // -100 ~ 0
            //    data[i] = -100 + i * 0.05;
            //}
        }

        public void SetData(double[] dArray)
        {
            var length = dArray.Length;
            for ( int i = 0; i < length; i++)
            {
                data[i] = dArray[i];
            }
        }
    }
}
