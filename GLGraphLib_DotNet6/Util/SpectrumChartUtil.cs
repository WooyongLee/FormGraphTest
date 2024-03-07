﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GLGraphLib
{
    public static class SpectrumChartUtil
    {
        // Font Size 변환
        public static int GetFontSize(int iter, double width = -1)
        {
            // 시작해서 GLUtil.FONT를 1회 바꿔주는 이유
            // -> 아직까지 잘 모르겠음.. 근데 한번 안바꿔주면 Text 도시에 이상이 생김, 바꿔주니까 정상적으로 도시함
            int fontsize = 10;
            if (iter > 1) fontsize = 12;

            // Font Size를 Windows Width 에 따라 변경함
            //if (width > 0)
            //{
            //    fontsize = (int)width / 100 + 4;
            //}
            return fontsize;
        }

        // Data Index를 통해서 Screen X 좌표를 구함
        public static float GetScreenX(int xValue, int totalLength, double ControlWidth, double PaddingHorizontal)
        {
            return (float)((double)xValue / totalLength * (ControlWidth - PaddingHorizontal * 2.0)) + (float)PaddingHorizontal;
        }

        // Data Index를 통해서 Screen Y 좌표를 구함
        public static float GetScreenY(double yValue, double minY, double maxY, double ControlHeight, double PaddingVertical)
        {
            // y value가 적정범위일 때
            if (yValue <  minY)
            {
                return (float)PaddingVertical;
            }

            else if ( yValue > maxY)
            {
                return (float)(ControlHeight - PaddingVertical);
            }

            else
            {
                return (float)((yValue - minY) / (maxY - minY) * (ControlHeight - PaddingVertical * 2.0)) + (float)PaddingVertical;
            }
        }
    }
}
