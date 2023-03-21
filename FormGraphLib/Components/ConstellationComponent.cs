using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormGraphLib
{
    /// <summary>
    /// Constellation Chart 설정 파라미터 Component
    /// </summary>
    public class ConstellationComponent : Common
    {
        /// <summary>
        /// Max Constellation ChartView  Width
        /// </summary>
        public static int MaxWidth = 500;

        /// <summary>
        /// Max Constellation ChartView Height
        /// </summary>
        public static int MaxHeight = 500;

        /// <summary>
        /// X Margin
        /// </summary>
        public static int ABoxX = 3;

        /// <summary>
        /// Y Margin
        /// </summary>
        public static int ABoxY = 2;

        /// <summary>
        /// Box 전체 너비
        /// </summary>
        public static int ABoxWidth = MaxWidth * 3 / 4;

        /// <summary>
        /// Box 전체 높이
        /// </summary>
        public static int ABoxHeight = MaxHeight * 3 / 4;

        /// <summary>
        /// x축 최대 값
        /// </summary>
        public static double Xmax = 2.0;

        /// <summary>
        /// y축 최대 값 
        /// </summary>
        public static double Ymax = 2.0;

        /// <summary>
        /// Constellation Devide Space X Length
        /// </summary>
        public static int DevideX = 8;

        /// <summary>
        /// Constellation Devide Space Y Length
        /// </summary>
        public static int DevideY = 8;

        /// <summary>
        /// Padding X
        /// </summary>
        public static int PaddingX = 33;

        /// <summary>
        /// Padding Y
        /// </summary>
        public static int PaddingY = 33;

        /// <summary>
        /// 총 Channel 개수
        /// </summary>
        public static int MaxChannel = 14;

        /// <summary>
        /// 채널 당 최대 Constellation 표현 개수
        /// </summary>
        public static int MaxConstellationData = 1024;

        /// <summary>
        /// x 좌표 값
        /// </summary>
        public static double[,] CH_X = new double[MaxChannel, MaxConstellationData];

        /// <summary>
        /// y 좌표 값
        /// </summary>
        public static double[,] CH_Y = new double[MaxChannel, MaxConstellationData];

        /// <summary>
        /// 각 Channel 표현할 색상
        /// </summary>
        public static Color[] ChannelColors = new Color[] {
            Color.BlueViolet, Color.Gold, Color.FromArgb(255,254,124,0), Color.Cyan,
            Color.FromArgb(255,129,199,132), Color.FromArgb(255,192,202,51), Color.FromArgb(255,244,67,54), Color.FromArgb(255,46,134,193),
            Color.FromArgb(255,46,134,193),Color.FromArgb(255,46,134,193),Color.FromArgb(255,46,134,193), Color.Red};

        /// <summary>
        /// Channel 표현 여부
        /// 초기설정은 모두 보이기
        /// </summary>
        public static bool[] IsVisibleChannel = Enumerable.Repeat(true, MaxChannel).ToArray();

        /// <summary>
        /// Channel Name
        /// </summary>
        public static string[] ChannelNames = new string[] { "PSS", "SSS", "PBCH", "PBCH DMRS", "", "", "", "", "", "", "" }; // 채널의 이름
        
        /// <summary>
        /// 활성화 채널 개수
        /// </summary>
        public static int ChannelCount = 4;

        /// <summary>
        /// 범례 표시 시 한 행에 나타날 데이터 개수
        /// </summary>
        public static int LengthInOneRow = 4;

        /// <summary>
        /// 범례 표시 여부
        /// </summary>
        public static bool IsShowBottom = false;

        /// <summary>
        /// 샘플 데이터 표시 여부
        /// </summary>
        public static bool IsShowSample = true;

        public static void Init()
        {
            ABoxWidth = MaxWidth * 3 / 4;
            ABoxHeight = MaxHeight * 3 / 4;
        }
    }
}
