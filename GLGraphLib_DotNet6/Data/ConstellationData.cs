using System.Collections.Generic;

namespace GLGraphLib_DotNet6
{
    public class ConstellationData
    {
        Dictionary<int, List<double>> CH_X;
        Dictionary<int, List<double>> CH_Y;

        public ConstellationData()
        {
            CH_X = new Dictionary<int, List<double>>();
            CH_Y = new Dictionary<int, List<double>>();
        }

        public ConstellationData(int channelLength)
        {
            CH_X = new Dictionary<int, List<double>>();
            CH_Y = new Dictionary<int, List<double>>();

            for (int i = 0; i < channelLength; i++)
            {
                CH_X.Add(i, new List<double>());
                CH_Y.Add(i, new List<double>());
            }
        }

        public void Add(int channelIndex, double x, double y)
        {
            if (!CH_X.ContainsKey(channelIndex))
            {
                CH_X.Add(channelIndex, new List<double>());
            }

            if (!CH_Y.ContainsKey(channelIndex))
            {
                CH_Y.Add(channelIndex, new List<double>());
            }

            CH_X[channelIndex].Add(x);
            CH_Y[channelIndex].Add(y);
        }

        public int GetLength()
        {
            return CH_X.Count;
        }

        public List<double> GetChannelX(int index)
        {
            if (!CH_X.ContainsKey(index))
            {
                return new List<double>();
            }
            return CH_X[index];
        }

        public List<double> GetChannelY(int index)
        {
            if (!CH_Y.ContainsKey(index))
            {
                return new List<double>();
            }
            return CH_Y[index];
        }
    }
}
