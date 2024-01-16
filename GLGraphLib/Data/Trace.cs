using SharpGL.SceneGraph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GLGraphLib
{
    public class Trace
    {
        public static readonly int MaxTraceCount = 4;

        public static readonly int TotalDataLength = 1001;

        // 4개의 Trace의 데이터를 표현
        private List<double>[] data;

        public Trace(int totalDataLength)
        {
            data = new List<double>[MaxTraceCount];
        }

        // Get Data at Trace Index
        public List<double> GetData(int index)
        {
            return data[index];
        }

        // Set Data at Trace Index
        public void SetData(double[, ] values, int index, int totalDataLength)
        {
            if (data[index] == null)
            {
                // List 초기화 및 기본값(0)으로 채우기
                data[index] = new List<double>();
                Enumerable.Range(0, totalDataLength).ToList().ForEach(i => data[index].Add(0));
            }

            var list = data[index];
            for (int i = 0; i < totalDataLength; i++) 
            {
                list[i] = values[index, i];
            }
        }

        public bool ClearData(int index) 
        {
            data[index] = null;
            return true;
        }

        public void MakeSampleData(int index, int totalDataLength)
        {
            // Generate Sample Data
            Random random = new Random();
            int MaxSample = totalDataLength;

            var dataAtIndex = data[index];
            
            // Draw Random (아무 신호가 없는 잡음을 표현)
            for (int i = 0; i < MaxSample; i++)
            {
                if (i < MaxSample / 6 || i > 5 * MaxSample / 6)
                {
                    dataAtIndex[i] = random.Next(-87, -84);
                }

                else
                {
                    dataAtIndex[i] = random.Next(-20, -9);
                }
            }

            data[index] = dataAtIndex;
        }
    }
}
