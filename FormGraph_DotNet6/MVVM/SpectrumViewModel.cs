using GLGraphLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
