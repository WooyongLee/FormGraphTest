using System.Collections.Generic;

namespace GLGraphLib_DotNet6
{
    public class Marker
    {
        private const int MaxMarkerCount = 10;
        public int SelectedMarkerIndex { get; set; } = 0;
        private List<MarkerPosition> Points { get; set; }

        public Marker()
        {
            Points = new List<MarkerPosition>();
        }

        public void AddPoint(float x, float y)
        {
            // Marker의 총 개수 제한
            if (Points.Count > MaxMarkerCount) return;

            Points.Add(new MarkerPosition(x, y));
        }

        // Index를 받아서 Point를 반환함
        public MarkerPosition? GetPoints(int i)
        {
            if (i >= Points.Count) return null;
            return Points[i];
        }

        // Marker Point를 Spectrum 데이터 변화에 따라 갱신함
        public void RenewPoint(float x, float y)
        {
            for ( int i = 0; i < Points.Count; i++ )
            {
                if (Points[i].X == x)
                {
                    Points[i].Y = y;
                    break;
                }
            }
        }

        public void RenewSelectedMarker(float x, float y)
        {
            Points[SelectedMarkerIndex].X = x;
            Points[SelectedMarkerIndex].Y = y;
        }

        public int GetMarkerTotalCount()
        {
            return Points.Count;
        }


        public class MarkerPosition
        {
            float x;
            float y;

            public float X { get { return x; } set { x = value; } }
            public float Y { get { return y; } set { y = value; } }

            public MarkerPosition(float x, float y)
            {
                this.x = x;
                this.y = y;
            }
        }
    }

}
