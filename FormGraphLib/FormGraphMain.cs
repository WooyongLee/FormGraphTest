using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormGraphLib
{
    public partial class FormGraphMain: UserControl
    {
        public const string FontNameD = "Microsoft Sans Serif";

        // Max ChartView Size
        static int MaxWidth = 1200;
        static int MaxHeight = 640;

        // Main View Size
        int pbMainWidth = 1001;
        int pbMainHeight = 390;

        // 축에서 정의하는 선의 갯수
        int xAxisLineCount = 10;
        int yAxisLineCount = 10;

        const int BasePaddingY = 40; // 좌표 표시 공간
        int pbMainPaddingX = 70; // 문자열을 보여주는 부분 크기
        int pbMainPaddingY = BasePaddingY;

        const int DataLength = 1001; // 1001 / 2 + 1 

        public FormGraphMain()
        {
            InitializeComponent();

            this.Initform();

            pictureBox1.Paint += PictureBox1_Paint;
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            this.Draw(e.Graphics);

            pictureBox1.Invalidate();
        }

        public void Initform()
        {
            // Set Background Color
            this.pictureBox1.BackColor = GraphParameterDummy.Instance.BackgroundColor;
        }

        public void Draw(Graphics g)
        {
            var startY = GraphParameterDummy.Instance.Amp_Ref_Level;
            var stepY = GraphParameterDummy.Instance.Amp_Scale_Div;

            DrawAxis(g, startY, stepY);

            // Draw Data Trace
            // Trace 1개만 활성화 되어있다고 가정
            DrawTrace(g, 0);
            // for ( int i = 0; i < Tracer.MaxTotalTraceCount; i++) { }


        }

        private void DrawTrace(Graphics g, int i)
        {
            Tracer TracerZ = new Tracer();
            int tracerWorkDetectorLL = 0;
            int td = tracerWorkDetectorLL; // ?? WorkDetectorLL 변수의 의미
            DrawLinesFromData(g, Tracer.LineColor[i], TracerZ.data, td);
        }

        private void DrawLinesFromData(Graphics g, Pen lineColor, double[] data, int td)
        {
            var LimitY_Main = pbMainPaddingY + pbMainHeight;

            var step = 1.0f;
            var stepi = (int)step;

            double tempY = 0.0;

            int pbMainHeightHalf = pbMainHeight / 2;

            PointF beginPoint4 = new PointF(pbMainPaddingX, pbMainPaddingY + pbMainHeightHalf);
            PointF endPoint4 = new PointF(pbMainPaddingX + 1, pbMainPaddingY + pbMainHeightHalf);

            // Amplitude Scale Division을 10으로 나눈 Scale을 설정함
            double Scale = GraphParameterDummy.Instance.Amp_Scale_Div * (10.0 / 100.0);
            Scale = 1 / Scale;

            if (td == 0)
            {
                // normal일 경우는 1001 개의 데이터이다.
                for ( int i = 0; i < DataLength; i++)
                {
                    // 음수로 전환함, 음의 값으로 커질 경우 그래프의 y 값이 증가하므로
                    double yy_min = -(data[i * 2] + GraphParameterDummy.Instance.Amp_Offset);
                    double yy_max = -(data[i * 2 + 1] + GraphParameterDummy.Instance.Amp_Offset); 

                    double yy2 = 0;
                    yy2 += GraphParameterDummy.Instance.Amp_Ref_Level;
                    
                    // y 최소값에 대한 Scaling
                    yy_min += yy2;
                    yy_min *= Scale * ((float)pbMainHeight / 100);

                    // y 최대값에 대한 Scaling
                    yy_max += yy2;
                    yy_max *= Scale * ((float)pbMainHeight / 100);

                    float xx = i * step;

                    beginPoint4 = new PointF(pbMainPaddingX + xx, pbMainPaddingY + (float)yy_min);
                    tempY = beginPoint4.Y;
                    if (CheckY(ref tempY, LimitY_Main) != 0)
                    {
                        continue;
                    }
                    beginPoint4.Y = (float)tempY;
                    endPoint4 = new PointF(pbMainPaddingX + xx, pbMainPaddingY + (float)yy_max);
                    tempY = endPoint4.Y;
                    if (CheckY(ref tempY, LimitY_Main) != 0)
                    {
                        continue;
                    }
                    endPoint4.Y = (float)tempY;
                    g.DrawLine(lineColor, beginPoint4, endPoint4);
                } // end for ( int i = 0; i < DataLength

                // Change LineColor
                lineColor = Pens.Blue;

                for ( int i = 0; i < DataLength - 1; i++)
                {
                    double yy = -(data[i] + GraphParameterDummy.Instance.Amp_Offset);
                    double yy2 = 0;
                    yy2 += GraphParameterDummy.Instance.Amp_Ref_Level;
                    yy += yy2;
                    yy *= Scale * ((float)pbMainHeight / 100);

                    if (i == 0)
                    {
                        beginPoint4 = new PointF(pbMainPaddingX, pbMainPaddingY + (float)yy);
                        tempY = beginPoint4.Y;
                        if (CheckY(ref tempY, LimitY_Main) != 0)
                        {
                            continue;
                        }
                        beginPoint4.Y = (float)tempY;
                        continue;
                    }

                    float xx = i * step;
                    endPoint4 = new PointF(pbMainPaddingX + xx, pbMainPaddingY + (float)yy);
                    tempY = endPoint4.Y;
                    if (CheckY(ref tempY, LimitY_Main) != 0)
                    {
                        continue;
                    }
                    endPoint4.Y = (float)tempY;
                    // 두 번째 Line의 잠시 주석
                    // g.DrawLine(lineColor, beginPoint4, endPoint4);

                    beginPoint4.X = endPoint4.X;
                    beginPoint4.Y = endPoint4.Y;
                } // end for ( int i = 0; i < DataLength - 1

            } // end if (td == 0)

            else
            {
                for ( int i = 0; i < DataLength; i++)
                {
                    double yy = -(data[i] + GraphParameterDummy.Instance.Amp_Offset);
                    double yy2 = 0;
                    yy2 += GraphParameterDummy.Instance.Amp_Ref_Level;

                    yy += yy2;
                    yy *= Scale * ((float)pbMainHeight / 100);

                    if (i == 0)
                    {
                        beginPoint4 = new PointF(pbMainPaddingX, pbMainPaddingY + (float)yy);
                        tempY = beginPoint4.Y;
                        if (CheckY(ref tempY, LimitY_Main) != 0)
                        {
                            continue;
                        }
                        beginPoint4.Y = (float)tempY;
                        continue;
                    }

                    float xx = i * step;

                    endPoint4 = new PointF(pbMainPaddingX + xx, pbMainPaddingY + (float)yy);
                    tempY = endPoint4.Y;
                    if (CheckY(ref tempY, LimitY_Main) != 0)
                    {
                        continue;
                    }
                    endPoint4.Y = (float)tempY;
                    g.DrawLine(lineColor, beginPoint4, endPoint4);
                } // end for
            } // end else
        }

        public void DrawAxis(Graphics g, double startY, double stepY)
        {
            var gridLineColor = GraphParameterDummy.Instance.GridLineColor;

            // Draw X Axis
            float xStep = (float)pbMainWidth / xAxisLineCount;
            float yL = -pbMainHeight;
            float yLineOfAxis = pbMainHeight;

            // 축의 큰 틀을 그림
            g.DrawLine(gridLineColor, pbMainPaddingX, pbMainPaddingY + yLineOfAxis,
                pbMainPaddingX + pbMainWidth, pbMainPaddingY + yLineOfAxis);
            
            // X 축
            for ( int i = 1; i < xAxisLineCount + 1; i++)
            {
                // Iterator에 Step 곱연산
                float xx = i * xStep;

                // x Step 만큼 구분하여 Count 만큼의 선을 그림
                PointF beginPoint = new PointF(pbMainPaddingX + xx, pbMainPaddingY + yLineOfAxis);
                PointF endPoint = new PointF(pbMainPaddingX + xx, pbMainPaddingY + yLineOfAxis + yL);
                g.DrawLine(gridLineColor, beginPoint, endPoint);
            }

            // Y 축
            float yStep = (float)pbMainHeight / yAxisLineCount;
            float xLineOfXYaxis = 0;
            float xL = -pbMainWidth;
            g.DrawLine(gridLineColor, pbMainPaddingX + xLineOfXYaxis, pbMainPaddingY, pbMainPaddingX + xLineOfXYaxis, pbMainPaddingY + pbMainHeight);

            for ( int i = 0; i < yAxisLineCount; i++ )
            {
                // Iterator에 Step 곱연산
                float yy = i * yStep;

                // y Step 만큼 구분하여 Count 만큼의 선을 그림
                PointF beginPoint = new PointF(pbMainPaddingX + xLineOfXYaxis - xL, pbMainPaddingY + yy);
                PointF endPoint = new PointF(pbMainPaddingX + xLineOfXYaxis, pbMainPaddingY + yy);
                g.DrawLine(gridLineColor, beginPoint, endPoint);
            }

            var drawFont = new Font(FontNameD, 10);
            var drawBrush = new SolidBrush(Color.Black);
            float ssx = pbMainPaddingX; // top left X margin
            float ssy = pbMainPaddingY + pbMainHeight + 5; // top left Y margin
            var drawStringFormat = new StringFormat();

            // x String
            const double MhzScale = 1e6;
            double vv1 = GraphParameterDummy.Instance.Freq_Start / MhzScale; // Mhz로
            var xString1 = string.Format("{0:0.0}", vv1);
            if (vv1 < 0.1)
            {
                xString1 = Util.GetFreqStringWithUnit(GraphParameterDummy.Instance.Freq_Start, 0);
            }
            var xString2 = string.Format("{0:0.0}", GraphParameterDummy.Instance.Freq_Center / MhzScale);
            var xString3 = string.Format("{0:0.0}", GraphParameterDummy.Instance.Freq_Stop / MhzScale);

            g.DrawString(xString1, drawFont, drawBrush, ssx, ssy, drawStringFormat);
            ssx = pbMainPaddingX + 5 * xStep - 17;
            g.DrawString(xString2, drawFont, drawBrush, ssx, ssy, drawStringFormat);
            ssx = pbMainPaddingX + xLineOfXYaxis - xL;
            drawStringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            g.DrawString("(MHz)", drawFont, drawBrush, ssx, ssy, drawStringFormat);
            g.DrawString(xString3, drawFont, drawBrush, ssx - 40, ssy, drawStringFormat);

            ssx = pbMainPaddingX;
            const int count = 11;
            
            // y String
            string[] yString = new string[count];
            for (int i = 0; i < count; i++)
            {
                double temp = startY - i * stepY;
                yString[i] = string.Format("{0:0.0}", Math.Abs(temp));
                if (temp < 0.0) yString[i] = "-" + yString[i]; // 음수인 경우 부호
                float yy = i * yStep;
                ssy = pbMainPaddingY + yy;
                if (i == 0)
                {
                    drawStringFormat.FormatFlags = 0;
                    g.DrawString("(dBm)", drawFont, drawBrush, 0, ssy - 9 - 15, drawStringFormat); // 단위
                }
                drawStringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                g.DrawString(yString[i], drawFont, drawBrush, ssx, ssy - 9, drawStringFormat);
            } // end for
        }

        // y의 Limit 값과 비교하여 y의 값을 제한함
        private int CheckY(ref double y, double LimitY)
        {
            // y의 값이 타당하지 않거나 너무 크면 그릴때 에러 생긴다. 
            // 제한된 값을 넘지 않도록 한다.

            if (y == double.NaN)
            {
                y = 0;
                return 1;
            }
            if (y == double.PositiveInfinity)
            {
                y = LimitY;
                return 0;
            }
            if (y == double.NegativeInfinity)
            {
                y = -LimitY;
                return 0;
            }

            if (y > LimitY) y = LimitY;
            else if (y < -LimitY) y = -LimitY;

            return 0;
        }
    }
}
