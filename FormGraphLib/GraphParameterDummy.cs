using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormGraphLib
{
    public class GraphParameterDummy : SingleTon<GraphParameterDummy>
    {
        // Linear Chart
        public double Amp_Ref_Level = 0.0;
        public double Amp_Attenuator = 20;
        public int Amp_Preamp = 1; // on/off
        public double Amp_Scale_Div = 10.0; // 10
        public double Amp_Offset = 0; // 0
        public double Amp_Offset_ToDevice = 0; // 0

        // Freqs
        private const double Freq_Init_Center = 55.0 * 1e9; // Hz
        private const double Freq_Init_Span = 0.2 * 1e9; // Hz

        public double Freq_Center = Freq_Init_Center;
        public double Freq_Start = Freq_Init_Center - Freq_Init_Span / 2;
        public double Freq_Stop = Freq_Init_Center + Freq_Init_Span / 2;
        public double Span_Freq = Freq_Init_Span;

        // Color
        // public Pen GridLineColor = new Pen(Color.Gray); // Border Line Color
        public Pen GridLineColor = new Pen(Color.White); // Border Line Color
        public Color BackgroundColor = Color.Black;
        public SolidBrush GridLineSolidBrush = new SolidBrush(Color.FromArgb(255, 199, 199, 199));


        // Constellation Chart
        public int ABox1_X = 3;
        public int ABox1_Y = 160;
        public int ABox1_Width = 430;
        public int ABox1_Height = 430;

        public int ABox2_X = 3 + 430; // ABox1_Width;
        public int ABox2_Y = 160;
        public int ABox2_Width = 260;
        public int ABox2_Height = 430;

        public int ABox_PBoxLT_X = 10;
        public int ABox_PBoxLT_Y = 160;
        public int ABox_PBoxSizeWidth = 1082;
        public int ABox_PBoxSizeHeight = 786 - 160; // ABox_PBoxLT_Y;
        public int ABox_PBox_Bottom = 786;

        public int ABox_PaddingX = 2;
        public int ABox_PaddingY = 2;
        public int Title_Y = 10;

        public int Title_Height = 30;

        // Constellation Devide Space Length
        public int DevideX = 8;
        public int DevideY = 8;

        public int PaddingX = 66;
        public int PaddingY = 66;

        public static int MaxChannel = 14;
        public static int MaxConstellationData = 1024;

        public double[,] CH_X = new double[MaxChannel, MaxConstellationData];
        public double[,] CH_Y = new double[MaxChannel, MaxConstellationData];
        public double[,] CH_R = new double[MaxChannel, MaxConstellationData];

        public Color[] ChannelColor = new Color[] {
            Color.BlueViolet, Color.Gold, Color.FromArgb(255,254,124,0), Color.Cyan,
            Color.FromArgb(255,129,199,132), Color.FromArgb(255,192,202,51), Color.FromArgb(255,244,67,54), Color.FromArgb(255,46,134,193),
            Color.FromArgb(255,46,134,193),Color.FromArgb(255,46,134,193),Color.FromArgb(255,46,134,193), Color.Red};

        public string[] ChannelName = new string[] { "PSS", "SSS", "PBCH", "PBCH DMRS","", "", "", "", "", "", "" }; // 채널의 이름
        public int ChannelCount = 4;

    }

    public class SingleTon<T> where T : class, new()
    {
        private static T instance;
        private static readonly object padlock = new object();

        #region Singleton을 위해 해당 객체에 접근하기 위한 Instance
        public static T Instance
        {
            get
            {
                // MultiThread Safe 위한 lock
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new T();
                    }
                    return instance;
                }
            }
        }
        #endregion
    }
}
