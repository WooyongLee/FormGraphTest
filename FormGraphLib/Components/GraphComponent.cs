using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormGraphLib
{
    /// <summary>
    /// Graph Chart 설정 파라미터 Component
    /// </summary>
    public class GraphComponent : Common
    {
        /// <summary>
        /// Max Chart View Width
        /// </summary>
        public static int MaxWidth = 1200;

        /// <summary>
        /// Max Chart View Height
        /// </summary>
        public static int MaxHeight = 800;

        /// <summary>
        /// Main View Width
        /// </summary>
        public static int MainWidth = MaxWidth * 5 / 6;

        /// <summary>
        /// Main View Height
        /// </summary>
        public static int MainHeight = MaxHeight * 5 / 7;

        /// <summary>
        /// Amplitude - Reference Level
        /// </summary>
        public static double AmpRefLevel = 0.0;

        /// <summary>
        /// Amplitude Offset
        /// </summary>
        public static double AmpOffset = 0.0;

        /// <summary>
        /// Amplitude - Scale Division
        /// Y Step
        /// </summary>
        public static double AmpScaleDiv = 10.0; // 10

        /// <summary>
        /// Amp Scale을 얼마만큼의 비율로 나눌 것인지
        /// </summary>
        public static double AmpScaleDivRatio = 10.0 / 100.0;

        /// <summary>
        /// 초기 Center Frequency
        /// </summary>
        private static double FreqInitCenter = 55.0 * 1e9; // Hz

        /// <summary>
        /// 초기 Span
        /// </summary>
        private static double FreqInitSpan = 0.2 * 1e9; // Hz

        /// <summary>
        /// Center Frequency
        /// </summary>
        public static double FreqCenter = FreqInitCenter;

        /// <summary>
        /// Start Frequency
        /// </summary>
        public static double FreqStart = FreqInitCenter - FreqInitSpan / 2;

        /// <summary>
        /// Stop Frequency
        /// </summary>
        public static double FreqStop = FreqInitCenter + FreqInitSpan / 2;

        /// <summary>
        /// Span
        /// </summary>
        public static double SpanFreq = FreqInitSpan;

        /// <summary>
        /// Line Color
        /// </summary>
        public static Pen LineColor = new Pen(Color.Yellow);

        /// <summary>
        /// x축에서 정의하는 선의 갯수
        /// </summary>
        public static int XAxisLineCount = 10;

        /// <summary>
        /// y축에서 정의하는 선의 갯수
        /// </summary>
        public static int YAxisLineCount = 10;

        /// <summary>
        /// Base Padding
        /// </summary>
        public static int BasePaddingY = 40;

        /// <summary>
        /// X Padding
        /// </summary>
        public static int MainPaddingX = 70;

        /// <summary>
        /// Y Padding
        /// </summary>
        public static int MainPaddingY = BasePaddingY;

        /// <summary>
        /// 세로 Line 개수
        /// </summary>
        public static int yTotalLines = 10 + 1;

        /// <summary>
        /// 세로 단위 이름
        /// </summary>
        public static string yAxisLegendName = "(dbm)";

        /// <summary>
        /// 단위 색상
        /// </summary>
        public static SolidBrush YAxisUnitTextColor = new SolidBrush(Color.Yellow);

        /// <summary>
        /// x Legend Visibility 설정
        /// </summary>
        public static bool IsVisibleXaxis = false;

        /// <summary>
        /// Sample Data 설정 여부
        /// </summary>
        public static bool IsShowSample = true;

        // Markers
        public static Marker[] MarkerN;

        // Traces
        public static Trace[] TraceN;

        // 초기화
        public static void Init()
        {
            FreqCenter = FreqInitCenter;
            FreqStart = FreqInitCenter - FreqInitSpan / 2;
            FreqStop = FreqInitCenter + FreqInitSpan / 2;
            SpanFreq = FreqInitSpan;

            MainWidth = MaxWidth * 5 / 6;
            MainHeight = MaxHeight * 5 / 7;
            MainPaddingY = BasePaddingY;

            // Marker 배열 초기화
            if (MarkerN == null)
            {
                MarkerN = new Marker[Marker.MaxMarkerCount];
                for ( int i = 0; i < Marker.MaxMarkerCount ; i++ )
                {
                    MarkerN[i] = new Marker();
                }
            }

            // Trace 배열 초기화
            if (TraceN == null)
            {
                TraceN = new Trace[Trace.MAX_TRACE_COUNT];
                for ( int i = 0; i < Trace.MAX_TRACE_COUNT; i++)
                {
                    TraceN[i] = new Trace();

                    // Trace Data 초기화
                    int dataLength = TraceN[i].data.Length;
                    for ( int j = 0; j < dataLength; j++ )
                    {
                        TraceN[i].data[j] = 0.0;
                    }
                }
            }
        }
    }
}
