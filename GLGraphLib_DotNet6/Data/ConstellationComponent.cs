using System;
using System.Drawing;

namespace GLGraphLib
{
    public class ConstellationComponent
    {
        public static double NullValue = -99999;

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
        public double[,] CH_X = new double[MaxChannel, MaxConstellationData];

        /// <summary>
        /// y 좌표 값
        /// </summary>
        public double[,] CH_Y = new double[MaxChannel, MaxConstellationData];

        /// <summary>
        /// 각 Channel 표현할 색상
        /// </summary>
        public Color[] ChannelColors = new Color[] {
            Color.BlueViolet, Color.Gold, Color.FromArgb(255,254,124,0), Color.Cyan,
            Color.FromArgb(255,129,199,132), Color.FromArgb(255,192,202,51), Color.FromArgb(255,244,67,54), Color.FromArgb(255,46,134,193),
            Color.FromArgb(255,46,134,193),Color.FromArgb(255,46,134,193),Color.FromArgb(255,46,134,193), Color.Red};

        public ConstellationComponent()
        {
            // Constellation Data에 기본값 세팅
            for ( int i = 0; i < MaxChannel; i++)
            {
                for ( int j = 0; j < MaxConstellationData; j++)
                {
                    CH_X[i, j] = NullValue;
                    CH_Y[i, j] = NullValue;
                }
            }
        }

        float division = 255f;
        public float GetNormalizedR(int channelIndex)
        {
            return ChannelColors[channelIndex].R / division;
        }

        public float GetNormalizedG(int channelIndex)
        {
            return ChannelColors[channelIndex].G / division;
        }

        public float GetNormalizedB(int channelIndex)
        {
            return ChannelColors[channelIndex].B / division;
        }

        public void SetChannelX(int channel, int constellation, double value)
        {
            CH_X[channel, constellation] = value;
        }

        public void SetChannelY(int channel, int constellation, double value)
        {
            CH_Y[channel, constellation] = value;
        }
    }
}
