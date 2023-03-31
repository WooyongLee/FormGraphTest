using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormGraphLib_DotNet6
{
    public partial class FormGraphMain : UserControl
    {
        public const string FontNameD = "Microsoft Sans Serif";
        private bool isLoaded = false;
        public FormGraphMain()
        {
            InitializeComponent();

            this.pictureBox1.Paint += PictureBox1_Paint;
        }

        public override void Refresh()
        {
            base.Refresh();

            this.pictureBox1.Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // Load가 먹지 않아서 Paint Event 호출 시 초기화
            if (!isLoaded)
            {
                isLoaded = true;

                this.Initform();

                return;
            }

            this.Invoke(new Action(() =>
            {
                if ( pictureBox1.IsDisposed )
                {
                    return;
                }

                this.Draw(e.Graphics);

                pictureBox1.Invalidate();
            }));
        }

        public void Initform()
        {
            // Initalize Component
            GraphComponent.Init();

            // Set Size
            this.pictureBox1.Size = new System.Drawing.Size(GraphComponent.MaxWidth, GraphComponent.MaxHeight);

            // Set Background Color
            this.pictureBox1.BackColor = Common.BackgroundColor;
        }

        public void Draw(Graphics g)
        {
            var startY = GraphComponent.AmpRefLevel;
            var stepY = GraphComponent.AmpScaleDiv;

            DrawAxis(g, startY, stepY);

            // Draw Data Trace
            // Trace 1개만 활성화 되어있다고 가정
            DrawTrace(g, 0);
        }

        // Trace가 여러개 있을 때에 대한 Draw함수, 일단 단일 Trace에 대해서만 도시
        private void DrawTrace(Graphics g, int i)
        {
            double[] data = new double[2002];
            if (GraphComponent.IsShowSample) MakeSampleData(ref data);
            DrawLinesFromData(g, GraphComponent.LineColor, data);
        }

        private void DrawLinesFromData(Graphics g, Pen lineColor, double[] data)
        {
            int td = 0;
            int DataLength = data.Length / 2; //  1001; // 1001 / 2 + 1 

            var LimitY_Main = GraphComponent.MainPaddingY + GraphComponent.MainHeight;

            // var step = 1.0f;
            var step = (float)GraphComponent.MainWidth / (float)DataLength;

            double tempY = 0.0;

            int pbMainHeightHalf = GraphComponent.MainHeight / 2;

            PointF beginPoint4 = new PointF(GraphComponent.MainPaddingX, GraphComponent.MainPaddingY + pbMainHeightHalf);
            PointF endPoint4 = new PointF(GraphComponent.MainPaddingX + 1, GraphComponent.MainPaddingY + pbMainHeightHalf);

            // Amplitude Scale Division을 10으로 나눈 Scale을 설정함
            double Scale = GraphComponent.AmpScaleDiv * GraphComponent.AmpScaleDivRatio;
            Scale = 1 / Scale;

            if (td == 0)
            {
                // normal일 경우는 1001 개의 데이터이다.
                for (int i = 0; i < DataLength; i++)
                {
                    // 음수로 전환함, 음의 값으로 커질 경우 그래프의 y 값이 증가하므로
                    double yy_min = -(data[i * 2] + GraphComponent.AmpOffset);
                    double yy_max = -(data[i * 2 + 1] + GraphComponent.AmpOffset);

                    double yy2 = 0;
                    yy2 += GraphComponent.AmpRefLevel;

                    // y 최소값에 대한 Scaling
                    yy_min += yy2;
                    yy_min *= Scale * ((float)GraphComponent.MainHeight / 100);

                    // y 최대값에 대한 Scaling
                    yy_max += yy2;
                    yy_max *= Scale * ((float)GraphComponent.MainHeight / 100);

                    float xx = i * step;

                    beginPoint4 = new PointF(GraphComponent.MainPaddingX + xx, GraphComponent.MainPaddingY + (float)yy_min);
                    tempY = beginPoint4.Y;
                    if (CheckY(ref tempY, LimitY_Main) != 0)
                    {
                        continue;
                    }
                    beginPoint4.Y = (float)tempY;
                    endPoint4 = new PointF(GraphComponent.MainPaddingX + xx, GraphComponent.MainPaddingY + (float)yy_max);
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

                //for ( int i = 0; i < DataLength - 1; i++)
                //{
                //    double yy = -(data[i] + GraphComponent.AmpOffset);
                //    double yy2 = 0;
                //    yy2 += GraphComponent.AmpRefLevel;
                //    yy += yy2;
                //    yy *= Scale * ((float)GraphComponent.MainHeight / 100);

                //    if (i == 0)
                //    {
                //        beginPoint4 = new PointF(GraphComponent.MainPaddingX, GraphComponent.MainPaddingY + (float)yy);
                //        tempY = beginPoint4.Y;
                //        if (CheckY(ref tempY, LimitY_Main) != 0)
                //        {
                //            continue;
                //        }
                //        beginPoint4.Y = (float)tempY;
                //        continue;
                //    }

                //    float xx = i * step;
                //    endPoint4 = new PointF(GraphComponent.MainPaddingX + xx, GraphComponent.MainPaddingY + (float)yy);
                //    tempY = endPoint4.Y;
                //    if (CheckY(ref tempY, LimitY_Main) != 0)
                //    {
                //        continue;
                //    }
                //    endPoint4.Y = (float)tempY;
                //    // 두 번째 Line의 잠시 주석
                //    // g.DrawLine(lineColor, beginPoint4, endPoint4);

                //    beginPoint4.X = endPoint4.X;
                //    beginPoint4.Y = endPoint4.Y;
                //} // end for ( int i = 0; i < DataLength - 1

            } // end if (td == 0)

            #region Legacy (주석)
            //else
            //{
            //    for ( int i = 0; i < DataLength; i++)
            //    {
            //        double yy = -(data[i] + GraphComponent.AmpOffset);
            //        double yy2 = 0;
            //        yy2 += GraphComponent.AmpRefLevel;

            //        yy += yy2;
            //        yy *= Scale * ((float)GraphComponent.MainHeight / 100);

            //        if (i == 0)
            //        {
            //            beginPoint4 = new PointF(GraphComponent.MainPaddingX, GraphComponent.MainPaddingY + (float)yy);
            //            tempY = beginPoint4.Y;
            //            if (CheckY(ref tempY, LimitY_Main) != 0)
            //            {
            //                continue;
            //            }
            //            beginPoint4.Y = (float)tempY;
            //            continue;
            //        }

            //        float xx = i * step;

            //        endPoint4 = new PointF(GraphComponent.MainPaddingX + xx, GraphComponent.MainPaddingY + (float)yy);
            //        tempY = endPoint4.Y;
            //        if (CheckY(ref tempY, LimitY_Main) != 0)
            //        {
            //            continue;
            //        }
            //        endPoint4.Y = (float)tempY;
            //        g.DrawLine(lineColor, beginPoint4, endPoint4);
            //    } // end for
            //} // end else
            #endregion
        }

        public void DrawAxis(Graphics g, double startY, double stepY)
        {
            var gridLineColor = GraphComponent.GridLineColor;

            // Draw X Axis
            float xStep = (float)GraphComponent.MainWidth / (float)GraphComponent.XAxisLineCount;
            float yL = -GraphComponent.MainHeight;
            float yLineOfAxis = GraphComponent.MainHeight;

            // 축의 큰 틀을 그림
            g.DrawLine(gridLineColor, GraphComponent.MainPaddingX, GraphComponent.MainPaddingY + yLineOfAxis,
                GraphComponent.MainPaddingX + GraphComponent.MainWidth, GraphComponent.MainPaddingY + yLineOfAxis);

            // X 축
            for (int i = 1; i < GraphComponent.XAxisLineCount + 1; i++)
            {
                // Iterator에 Step 곱연산
                float xx = i * xStep;

                // x Step 만큼 구분하여 Count 만큼의 선을 그림
                PointF beginPoint = new PointF(GraphComponent.MainPaddingX + xx, GraphComponent.MainPaddingY + yLineOfAxis);
                PointF endPoint = new PointF(GraphComponent.MainPaddingX + xx, GraphComponent.MainPaddingY + yLineOfAxis + yL);
                g.DrawLine(gridLineColor, beginPoint, endPoint);
            }

            // Y 축
            float yStep = (float)GraphComponent.MainHeight / GraphComponent.XAxisLineCount;
            float xLineOfXYaxis = 0;
            float xL = -GraphComponent.MainWidth;
            g.DrawLine(gridLineColor, GraphComponent.MainPaddingX + xLineOfXYaxis, GraphComponent.MainPaddingY,
                GraphComponent.MainPaddingX + xLineOfXYaxis, GraphComponent.MainPaddingY + GraphComponent.MainHeight);

            for (int i = 0; i < GraphComponent.YAxisLineCount; i++)
            {
                // Iterator에 Step 곱연산
                float yy = i * yStep;

                // y Step 만큼 구분하여 Count 만큼의 선을 그림
                PointF beginPoint = new PointF(GraphComponent.MainPaddingX + xLineOfXYaxis - xL, GraphComponent.MainPaddingY + yy);
                PointF endPoint = new PointF(GraphComponent.MainPaddingX + xLineOfXYaxis, GraphComponent.MainPaddingY + yy);
                g.DrawLine(gridLineColor, beginPoint, endPoint);
            }

            var drawFont = new Font(FontNameD, 10);
            var drawBrush = new SolidBrush(GraphComponent.ForegroundColor);
            float ssx = GraphComponent.MainPaddingX; // top left X margin
            float ssy = GraphComponent.MainPaddingY + GraphComponent.MainHeight + 5; // top left Y margin
            var drawStringFormat = new StringFormat();

            // x String
            if (GraphComponent.IsVisibleXaxis)
            {
                // Mhz Scale로 변환하여 표현
                const double MhzScale = 1e6;
                var xString1 = string.Format("{0:0.0}", GraphComponent.FreqStart / MhzScale);
                var xString2 = string.Format("{0:0.0}", GraphComponent.FreqCenter / MhzScale);
                var xString3 = string.Format("{0:0.0}", GraphComponent.FreqStop / MhzScale);

                g.DrawString(xString1, drawFont, drawBrush, ssx, ssy, drawStringFormat);
                ssx = GraphComponent.MainPaddingX + 5 * xStep - 17;
                g.DrawString(xString2, drawFont, drawBrush, ssx, ssy, drawStringFormat);
                ssx = GraphComponent.MainPaddingX + xLineOfXYaxis - xL;
                drawStringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;
                g.DrawString("(MHz)", drawFont, drawBrush, ssx, ssy, drawStringFormat);
                g.DrawString(xString3, drawFont, drawBrush, ssx - 40, ssy, drawStringFormat);
            }

            ssx = GraphComponent.MainPaddingX;

            // y String
            string[] yString = new string[GraphComponent.yTotalLines];
            for (int i = 0; i < GraphComponent.yTotalLines; i++)
            {
                double temp = startY - i * stepY;
                yString[i] = string.Format("{0:0.0}", Math.Abs(temp));
                // 음수인 경우 부호
                // 오른쪽에서 왼쪽으로 표시하기 때문에 부호 붙이는 위치 반대로
                if (temp < 0.0) yString[i] = yString[i] + "-";
                float yy = i * yStep;
                ssy = GraphComponent.MainPaddingY + yy;
                if (i == 0)
                {
                    var unitDrawBrush = GraphComponent.YAxisUnitTextColor;
                    drawStringFormat.FormatFlags = 0;
                    g.DrawString("(dBm)", drawFont, unitDrawBrush, 0, ssy - 9 - 15, drawStringFormat); // 단위
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

        private void MakeSampleData(ref double[] data)
        {
            // Generate Sample Data
            Random random = new Random();
            const int MaxSample = 2002;

            // Draw Random (아무 신호가 없는 잡음을 표현)
            for (int i = 0; i < MaxSample; i++)
            {
                int value = random.Next(-67, -53);

                data[i] = value;
            }

            // Draw Linear
            //for (int i = 0; i < MaxSample; i++)
            //{
            //    // -100 ~ 0
            //    data[i] = -100 + i * 0.05;
            //}
        }
    }
}
