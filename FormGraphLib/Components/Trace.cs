using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormGraphLib
{
    public class Trace
    {
        public static readonly int MAX_TRACE_COUNT = 4;

        public const int MAX_SAMPLE = 2002;

        public int WorkDetectorLL = 0;

        public double[] data = new double[MAX_SAMPLE];
    }
}
