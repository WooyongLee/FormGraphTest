using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormGraphLib
{
    public class Unit1000String
    {
        public static string GetFreqStringWithUnit_Old(double v)
        {
            string ss = "";
            double m = 1.0;
            if (v < 0.0)
            {
                v *= -1.0;
                m = -1.0;
            }
            if (v > 1e12)
            {
                v /= 1e12;
                ss = string.Format("{0}THz", v);
            }
            else if (v > 1e9)
            {
                v /= 1e9;
                ss = string.Format("{0}GHz", v);
            }
            else if (v > 1e6)
            {
                v /= 1e6;
                ss = string.Format("{0}MHz", v);
            }
            else if (v > 1e3)
            {
                v /= 1e3;
                ss = string.Format("{0}kHz", v);
            }
            else
            {
                ss = string.Format("{0}Hz", v);
            }
            if (m < 0.0)
                ss = "-" + ss;
            return ss;
        }

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

        public static string GetTimeStringWithUnit_Old(double v)
        {
            string ss = "";
            double m = 1.0;
            if (v < 0.0)
            {
                v *= -1.0;
                m = -1.0;
            }
            if (v > 1.0)
            {
                ss = string.Format("{0}s", v);
            }
            else if (v > (1.0 / 1e3))
            {
                v *= 1e3;
                ss = string.Format("{0}ms", v);
            }
            else if (v > (1.0 / 1e6))
            {
                v *= 1e6;
                ss = string.Format("{0}us", v);
            }
            else if (v > (1.0 / 1e9))
            {
                v *= 1e9;
                ss = string.Format("{0}ns", v);
            }
            else
            {
                v *= 1e12;
                ss = string.Format("{0}ps", v);
            }
            if (m < 0.0)
                ss = "-" + ss;

            return ss;

        }

        public static string GetTimeStringWithUnit(double v)
        {
            double m = 1.0;
            if (v < 0.0)
            {
                v *= -1.0;
                m = -1.0;
            }

            string[] unit = new string[] { "", "m", "u", "n", "p" };
            double ur = 1.0;
            int unit_ix = 4;
            for (int i = 0; i < 4; i++)
            {
                if (v >= ur)
                {
                    v *= (1 / ur);
                    unit_ix = i;
                    break;
                }
                ur /= 1e3;
            }
            if (unit_ix == 4)
            {
                v *= (1 / ur);
            }
            string ss = string.Format("{0} {1}s", v, unit[unit_ix]);

            if (m < 0.0)
                ss = "-" + ss;

            return ss;
        }
    }
}
