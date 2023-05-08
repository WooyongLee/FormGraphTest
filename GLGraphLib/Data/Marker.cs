using System.Collections.Generic;
using System.Linq;

namespace GLGraphLib
{
    public class Marker
    {
        public static readonly int MaxMarkerCount = 10;
        public int SelectedMarkerIndex { get; set; } = 0;

        // Marker의 위치
        private MarkerPosition[] Points { get; set; }

        // Fixed Marker 대상 인덱스 리스트
        public List<int> FixedMarkerIndexList { get; set; }

        // Delta Marker 대상 인덱스 페어 리스트
        public List<IndexPair> DeltaMarkerIndexList { get; set; }

        public Marker()
        {
            Points = new MarkerPosition[MaxMarkerCount];

            FixedMarkerIndexList = new List<int>();
            DeltaMarkerIndexList = new List<IndexPair>();
        }

        public void AddPoint(float x, float y, int markerIndex)
        {
            // Marker의 총 개수 제한
            if (SelectedMarkerIndex >= MaxMarkerCount) return;

            this.SelectedMarkerIndex = markerIndex;

            Points[markerIndex] = new MarkerPosition(x, y);
        }

        public bool SetPoint(float x, float y, int markerIndex)
        {
            if (SelectedMarkerIndex >= MaxMarkerCount) { return false; }

            if (Points[markerIndex]  == null) { return false; }

            Points[markerIndex].X = x;
            Points[markerIndex].Y = y;
            return true;
        }

        // Index를 받아서 Point를 반환함
        public MarkerPosition GetPoints(int markerIndex)
        {
            // if (markerIndex >= SelectedMarkerIndex) return null;
            if (Points[markerIndex] == null) return null;
            return Points[markerIndex];
        }

        // Marker Point를 Spectrum 데이터 변화에 따라 갱신함
        public void RenewPoint(float x, float y, int markerIndex)
        {
            if (Points[markerIndex] != null)
            {
                if (Points[markerIndex].X == x)
                {
                    Points[markerIndex].Y = y;
                }
            }
        }

        // Marker 위치 수정에 따른 재 지정
        public void RenewSelectedMarker(float x, float y)
        {
            Points[SelectedMarkerIndex].X = x;
            Points[SelectedMarkerIndex].Y = y;
        }

        // Not Null Position
        public int GetTotalPosCount()
        {
            int cnt = 0;
            foreach (var merkerPos in Points)
            {
                if (merkerPos != null)
                {
                    cnt++;
                }
            }
            return cnt;
        }

        public bool RemoveMarker(int markerIndex)
        {
            if (markerIndex >= MaxMarkerCount) return false;

            Points[markerIndex] = null;

            return true;
        }

        // Fixed Marker를 설정하거나 제거함
        public bool SetFixedList(int markerIndex)
        {
            bool hasIndex = FixedMarkerIndexList.Any(n => n == markerIndex);

            // 있으면 제거함
            if (hasIndex)
            {
                FixedMarkerIndexList.Remove(markerIndex);
            }

            else
            {
                FixedMarkerIndexList.Add(markerIndex);
            }
            return true;
        }

        // 현재 Index가 Fixed임을 확인함
        public bool IsFixed(int markerIndex)
        {
            return FixedMarkerIndexList.Any(n => n == markerIndex);
        }

        // Delta Marker를 설정하거나 제거함
        public bool SetDeltaList(int sourceIndex, int targetIndex)
        {
            bool hasIndex = DeltaMarkerIndexList.Any(n => n.SourceIndex == sourceIndex);
            
            if (hasIndex)
            {
                DeltaMarkerIndexList.RemoveAll(pair => pair.SourceIndex == sourceIndex);
            }

            else
            {
                DeltaMarkerIndexList.Add(new IndexPair() { SourceIndex = sourceIndex, TargetIndex = targetIndex });
            }

            return true;
        }

        public bool IsDelta(int sourceIndex)
        {
            // Find sourceIndex of target index
            // DeltaMarkerIndexList.Find(pair => pair.SourceIndex == sourceIndex)?.TargetIndex

            return DeltaMarkerIndexList.Any(n => n.SourceIndex == sourceIndex);
        }

        public int? GetDeltaTargetIndex(int sourceIndex)
        {
            return DeltaMarkerIndexList.Find(pair => pair.SourceIndex == sourceIndex)?.TargetIndex;
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
