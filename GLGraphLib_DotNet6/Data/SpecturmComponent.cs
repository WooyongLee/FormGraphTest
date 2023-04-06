using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLGraphLib_DotNet6
{
    public class SpecturmComponent
    {
        public const int TotalDataLength = 1001;

        public double[] data;

        public SpecturmComponent()
        {
            data = new double[TotalDataLength];
        }
    }
}
