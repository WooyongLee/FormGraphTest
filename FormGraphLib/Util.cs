namespace FormGraphLib
{
    public static class Util
    {
        public static string GetFreqStringWithUnit(double v, int s_option)
        {
            double m = 1.0;
            if (v < 0.0)
            {
                v *= -1.0;
                m = -1.0;
            }
            string[] unit = new string[] { "T", "G", "M", "k", "" };
            double ur = 1e12;
            int unit_ix = 4;
            for (int i = 0; i < 4; i++)
            {
                if (v >= ur)
                {
                    v /= ur;
                    unit_ix = i;
                    break;
                }
                ur /= 1e3;
            }

            string ss = "";
            switch (s_option)
            {
                case 0:
                    ss = string.Format("{0}{1}Hz", v, unit[unit_ix]);
                    break;
                case 1:
                    ss = string.Format("{0:0.000 000 000}{1}Hz", v, unit[unit_ix]);
                    break;
            }

            if (m < 0.0)
                ss = "-" + ss;
            return ss;
        }
    }
}
