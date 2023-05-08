using System;
using System.Linq;

namespace GLGraphLib
{
    // 화면 상의 x, y Position들을 설정함
    public class ScreenPositions
    {
        private float[] xArray;
        private float[] yArray;
        private int Length;

        public ScreenPositions(int length)
        {
            xArray = new float[length];
            yArray = new float[length];
            this.Length = length;
        }

        public void Set(float x, float y, int i)
        {
            if (i > Length) return;

            if (this.xArray[i] != x) this.xArray[i] = x;
            if (this.yArray[i] != y) this.yArray[i] = y;
        }

        // 선택한 x 위치로부터 가장 가까운 데이터를 반환함
        public float GetClosestData(float x, ref float closestX)
        {
            int closestIndex = Array.IndexOf(xArray, xArray.OrderBy(v => Math.Abs(v - x)).First());
            if (yArray.Length < closestIndex) return 0;
            closestX = xArray[closestIndex];
            return yArray[closestIndex];
        }
    }
}
