using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormGraphLib
{
    public class Marker
    {
        public const int MaxMarkerCount = 10;
        public const int tr = 255;

        public bool IsTurnOn { get; set; } = false;

        static public Pen[] LineColor = new Pen[MaxMarkerCount] { 
            Pens.Red, Pens.Red, Pens.Red, Pens.Red, Pens.Red,
             Pens.Red, Pens.Red, Pens.Red, Pens.Red, Pens.Red
        };

        static public Color[] LineColor2 = new Color[MaxMarkerCount] {
            Color.FromArgb(tr,0,110,170), Color.FromArgb(tr,140,140,240),
            Color.FromArgb(tr,200,40,40), Color.FromArgb(tr,220,100,0), Color.FromArgb(tr,220,0,220),
            Color.FromArgb(tr,220,100,220), Color.FromArgb(tr,50,160,130), Color.FromArgb(tr,0,220,0),
            Color.FromArgb(tr,170,220,20), Color.FromArgb(tr,0,220,200)
            };

        // Local - Screen Coord
        public int LX = 0;
        public int LY = 0;

        public EMarkerType eMarkerType { get; set; } = EMarkerType.Off;

        // Delta인 경우 현재 Marker에 대한 Relative Index 
        public int RelativeIndex { get; set; } = -1;

        // 현재 어떤 Trace의 Marker임을 확인하기 위한 Trace Index
        public int TargetTraceIndex { get; set; } = 0;

        // Frequency
        public double Freq { get; set; }
    }

    public enum EMarkerType
    {
        Off = 0,
        Normal = 1,
        Delta = 2,
        Fixed = 3,
    }

}
