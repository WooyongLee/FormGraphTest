using System.Collections.Generic;
using System.Numerics;

namespace GLGraphLib
{
    public class ComplexTrace
    {
        public static readonly int MaxTraceCount = 4;

        // 최대 4 개의 Complex 데이터를 표현
        private IList<Complex>[] data;

        public ComplexTrace()
        {
            this.data = new List<Complex>[MaxTraceCount];
            for (int i = 0; i < MaxTraceCount; i++)
            {
                this.data[i] = new List<Complex>();
            }
        }

        public IList<Complex> Data(int index)
        {
            return data[index];
        }

        public void Clear()
        {
            this.data = new List<Complex>[MaxTraceCount];
            for (int i = 0; i < MaxTraceCount; i++)
            {
                this.data[i] = new List<Complex>();
            }
        }

        public void SetData(int index, IList<Complex> complexes)
        {
            if (data[index] == null)
            {
                this.data[index] = new List<Complex>();
            }

            // Copy to values at parameter index
            // data[index] = values.ToList();
            int length = data[index].Count;
            if (length < complexes.Count)
            {
                int originLength = length;
                length = complexes.Count;
                for (int i = originLength; i < length; i++)
                {
                    data[index].Add(0.0);
                }
            }
            else
            {
                // 초기화 후 재배열
                data[index].Clear();
                length = complexes.Count;
                for (int i = 0; i < length; i++)
                {
                    data[index].Add(0.0);
                }
            }

            for (int i = 0; i < length; i++)
            {
                data[index][i] = complexes[i];
            }
        }
    }
}
