using GLGraphLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace FormGraph_DotNet6
{
    public class SpectrumViewModel : ViewModelBase
    {
        private Marker marker;

        private Trace trace;

        public Marker MarkerInfo { 
            get { return marker; }
            set { marker = value; NotifyPropertyChanged("MarkerInfo"); }
        }

        public Trace TraceInfo
        {
            get { return trace; }
            set { trace = value; NotifyPropertyChanged("TraceInfo"); }
        }

        int totalLength = 1001;

        public SpectrumViewModel()
        {
            MarkerInfo = new Marker(0);
            TraceInfo = new Trace(totalLength);
        }

        public void AddTraceData()
        {
            var data = new List<double>();
            var data2 = new List<double>();

            for (int i = 0; i < totalLength; i++)
            {
                data.Add(-0.1 * i);
            }

            TraceInfo.SetData(0, data);
            TraceInfo.SetData(1, data2);
        }

        public void AddTraceIQ()
        {
            int scale = 20000;
            var data = new List<double>();
            var data2 = new List<double>();

            for (int i = 0; i < totalLength; i++)
            {
                double angle = -90 + 1 * i; // -90에서 90까지의 각도 (90도 틀어진 값)
                double radians = angle * Math.PI / 180; // 라디안 값으로 변환
                data.Add(scale * Math.Cos(radians)); // 코사인 값 추가
                data2.Add(scale * Math.Sin(radians)); // 사인 값 추가
            }

            TraceInfo.SetData(0, data);
            TraceInfo.SetData(1, data2);
        }
    }
}
