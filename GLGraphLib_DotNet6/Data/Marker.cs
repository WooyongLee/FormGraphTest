using SharpGL.SceneGraph.Raytracing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GLGraphLib
{
    public class Marker
    {
        public static readonly int MaxMarkerCount = 10;
        public int SelectedMarkerIndex { get; set; } = 0;

        // Marker의 위치
        private MarkerInfo[] Points { get; set; }

        // Fixed Marker 대상 인덱스 리스트
        private List<int> FixedMarkerIndexList { get; set; }

        // Delta Marker 대상 인덱스 페어 리스트
        private List<IndexPair> DeltaMarkerIndexList { get; set; }

        public Marker()
        {
            Points = new MarkerInfo[MaxMarkerCount];

            FixedMarkerIndexList = new List<int>();
            DeltaMarkerIndexList = new List<IndexPair>();
        }

        public int GetTargetTraceIndex(int markerIndex)
        {
            return Points[markerIndex].TargetTraceIndex;
        }

        // x, y가 screen 좌표
        public void AddPoint(int markerIndex, float x, float y,
            float screenMinX, float screenMaxX, float screenMinY, float screenMaxY,
            double realMinX, double realMaxX, double realMinY, double realMaxY)
        {
            // 입력 좌표를 screen의 최소 및 최대 값 범위에서 0 ~ 1 사이의 비율로 변환합니다.
            double ratioX = Math.Round((x - screenMinX) / (screenMaxX - screenMinX), 2);
            double ratioY = Math.Round((y - screenMinY) / (screenMaxY - screenMinY), 2);

            // 비율을 real의 범위에 맞춰 변환합니다.
            double realX = Math.Round(realMinX + ratioX * (realMaxX - realMinX), 2);
            double realY = Math.Round(realMinY + ratioY * (realMaxY - realMinY), 2);

            Points[markerIndex] = new MarkerInfo(x, y, realX, realY);
        }

        public void AddPoint(float x, float y, int markerIndex)
        {
            // Marker의 총 개수 제한
            if (SelectedMarkerIndex >= MaxMarkerCount) return;

            this.SelectedMarkerIndex = markerIndex;

            Points[markerIndex] = new MarkerInfo(x, y);
        }

        // Index를 받아서 Point를 반환함
        public MarkerInfo? GetPoints(int markerIndex)
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
                    Points[markerIndex].SetXY(x, y);
                }
            }
        }

        // 선택된 Marker의 위치를 Refresh
        public void RenewSelectedMarker(float x, float y, double valueX, double valueY)
        {
            Points[SelectedMarkerIndex].SetXY(x, y);

            Points[SelectedMarkerIndex].ValueX = valueX;
            Points[SelectedMarkerIndex].ValueY = valueY;
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

        public class MarkerInfo
        {
            float x;
            float y;

            // Frequency
            public double ValueX { get; set; }

            // Amplitude
            public double ValueY { get; set; }

            public int TargetTraceIndex { get; set; }

            public float X { get { return x; } private set { x = value; } }
            public float Y { get { return y; } private set { y = value; } }

            public MarkerInfo(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            public MarkerInfo(float x, float y, double valX, double valY)
            {
                this.x = x;
                this.y = y;

                this.ValueX = valX;
                this.ValueY = valY;
            }

            public void SetXY(float x, float y)
            {
                this.x = x;
                this.y = y;
            }
        }
    }
}
