using SharpGL.SceneGraph.Raytracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Text.RegularExpressions;

namespace GLGraphLib
{
    public class Marker
    {
        public static event EventHandler CreateMarkerEvent;
        public static event EventHandler MoveMarkerPositionEvent;

        public static readonly int MaxMarkerCount = 10;

        public bool[] IsVisible { get; private set; }

        // Marker의 위치
        private MarkerPosition[] Points { get; set; }

        // Fixed Marker 대상 인덱스 리스트
        private List<int> FixedMarkerIndexList { get; set; }

        // Delta Marker 대상 인덱스 페어 리스트
        private List<IndexPair> DeltaMarkerIndexList { get; set; }

        // 현재 선택된 Marker 인덱스
        internal int SelectedMarkerIndex { get; set; } = 0;

        private int TotalDataLength;

        public Marker(int totalDataLength)
        {
            Points = new MarkerPosition[MaxMarkerCount];

            this.TotalDataLength = totalDataLength;

            IsVisible = new bool[MaxMarkerCount];

            FixedMarkerIndexList = new List<int>();
            DeltaMarkerIndexList = new List<IndexPair>();
        }

        // Index를 받아서 Point를 반환함
        public MarkerPosition? GetPoints(int markerIndex)
        {
            // if (markerIndex >= SelectedMarkerIndex) return null;
            if (Points[markerIndex] == null) return null;
            return Points[markerIndex];
        }

        public void MovePoint(int point)
        {
            MoveMarkerPositionEvent.Invoke(point, new EventArgs());
        }

        public void Show(int markerIndex)
        {
            if (IsVisible[markerIndex]) return; 
            CreateMarkerEvent.Invoke(markerIndex, new EventArgs());
            IsVisible[markerIndex] = true;
        }

        public void Hide(int markerIndex)
        {
            if (!IsVisible[markerIndex]) return;
            if (GetPoints(markerIndex) != null)
            {
                RemoveMarker(markerIndex);
            }
        }

        internal void AddPoint(int markerIndex, float x, float y, double realX, double realY, int currentPos)
        {
            Points[markerIndex] = new MarkerPosition(x, y, realX, realY, currentPos, TotalDataLength);
        }

        // Marker Point를 Spectrum 데이터 변화에 따라 갱신함
        internal void RenewPoint(float x, float y, int markerIndex)
        {
            if (Points[markerIndex] != null)
            {
                if (Points[markerIndex].X == x)
                {
                    Points[markerIndex].SetPosition(x, y);
                }
            }
        }

        // 선택된 Marker의 위치를 Refresh
        internal void RenewSelectedMarker(float x, float y, double valueX, double valueY, int dataPosition)
        {
            Points[SelectedMarkerIndex].SetPosition(x, y, valueX, valueY, dataPosition);
        }

        internal void RenewMarkerIQ(float x, float y, double valueX, double valueY, int dataPosition, int index)
        {
            Points[index].SetPosition(x, y, valueX, valueY, dataPosition);
        }

        // Not Null Position
        internal int GetTotalPosCount()
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

        internal bool RemoveMarker(int markerIndex)
        {
            if (markerIndex >= MaxMarkerCount) return false;

            Points[markerIndex] = null;

            IsVisible[markerIndex] = false;

            return true;
        }

        // Fixed Marker를 설정하거나 제거함
        internal bool SetFixedList(int markerIndex)
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
        internal bool IsFixed(int markerIndex)
        {
            return FixedMarkerIndexList.Any(n => n == markerIndex);
        }

        // Delta Marker를 설정하거나 제거함
        internal bool SetDeltaList(int sourceIndex, int targetIndex)
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

        internal bool IsDelta(int sourceIndex)
        {
            // Find sourceIndex of target index
            // DeltaMarkerIndexList.Find(pair => pair.SourceIndex == sourceIndex)?.TargetIndex

            return DeltaMarkerIndexList.Any(n => n.SourceIndex == sourceIndex);
        }

        internal int? GetDeltaTargetIndex(int sourceIndex)
        {
            return DeltaMarkerIndexList.Find(pair => pair.SourceIndex == sourceIndex)?.TargetIndex;
        }

        public class MarkerPosition
        {
            float x, y;

            // Real X Value (Frequency)
            public double ValueX { get; set; }

            // Real Y Value (Amplitude)
            public double ValueY { get; set; }

            // 현재 Marker가 위치한 Trace Index
            public int TargetTraceIndex { get; set; }

            // Screen X
            public float X { get { return x; } private set { x = value; } }

            // Screen Y
            public float Y { get { return y; } private set { y = value; } }

            // 현재 위치
            public int Current { get; set; }

            // 전체 위치
            public int Total { get; set; }

            public MarkerPosition(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            public MarkerPosition(float x, float y, double valX, double valY, int current, int total)
            {
                SetPosition(x, y, valX, valY, current);
                Total = total;
            }

            public void SetPosition(float x, float y)
            {
                this.x = x;
                this.y = y;
            }

            public void SetPosition(float x, float y, double valX, double valY, int current)
            {
                this.x = x;
                this.y = y;

                this.ValueX = valX;
                this.ValueY = valY;

                this.Current = current;
            }
        }
    }
}
