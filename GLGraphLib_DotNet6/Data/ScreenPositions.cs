using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace GLGraphLib
{
    // 화면 상의 x, y Position들을 설정함
    public class ScreenPositions
    {
        private List<float> xs;
        private List<float> ys;
        private int Length;

        public ScreenPositions(int length)
        {
            xs = new List<float>();
            ys = new List<float>();
            this.Length = length;
        }

        public void Set(float x, float y, int i)
        {
            if (xs.Count <= i)
            {
                xs.Add(0.0f);
                ys.Add(0.0f);
            }

            if (this.xs[i] != x) this.xs[i] = x;
            if (this.ys[i] != y) this.ys[i] = y;
        }

        // 선택한 x 위치로부터 가장 가까운 데이터를 반환함
        public float GetClosestData(float x, ref float closestX)
        {
            int closestIndex = Array.IndexOf(xs.ToArray(), xs.OrderBy(v => Math.Abs(v - x)).First());
            if (ys.Count < closestIndex) return 0;
            closestX = xs[closestIndex];
            return ys[closestIndex];
        }
    }
}
