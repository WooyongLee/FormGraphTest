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

        private bool isLoaded = false;

        // Dragging variable for Marker
        bool isDragOn = false;
        int mouseX = 0; // For Dragging
        int mouseY = 0;
        int selectedMarkerIdx = 0;
        
        public FormGraphMain()
        {
            InitializeComponent();

            this.pictureBox1.Paint += PictureBox1_Paint;
            this.pictureBox1.MouseMove += PictureBox1_MouseMove;
            this.pictureBox1.MouseDown += PictureBox1_MouseDown;
            this.pictureBox1.MouseUp += PictureBox1_MouseUp;
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            isDragOn = false;

            // To Do :: Zero Span인 경우 수행하지 않음

            if (selectedMarkerIdx == -1)
            {
                return;
            }

            int x = e.X;
            int y = e.Y;
            int TX = x - GraphComponent.MainPaddingX;
            int TY = y - GraphComponent.MainPaddingY;

            int limitX = x + GraphComponent.MaxWidth - 1;
            if (TX < 0) TX = 0;
            if (TX > limitX) TX = limitX;

            var marker = GraphComponent.MarkerN[selectedMarkerIdx];
            marker.LX = TX;



        }

        private void PictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // To Do :: Zero Span인 경우 수행하지 않음

            int x = e.X;
            int y = e.Y;
            int TX = x - GraphComponent.MainPaddingX;
            int TY = y - GraphComponent.MainPaddingY;

            int limitX = x + GraphComponent.MaxWidth - 1;
            if (TX < 0) return;
            if (TX > limitX) return;

            int tempidx = -1;
            int min = 100000;

            // 클릭한 지점과 가장 가까운 Marker를 찾기 위함
            for (int i = Marker.MaxMarkerCount - 1; i >= 0; i--)
            {
                // To Do :: Peak 설정이 Peak 또는 Min 일 시, 작동안되도록 하기

                var marker = GraphComponent.MarkerN[i];
                if (marker.eMarkerType != EMarkerType.Fixed)
                {
                    continue;
                }

                if (marker.IsTurnOn)
                {
                    continue;
                }

                int d = Math.Abs(TX - marker.LX);
                if (d < min)
                {
                    min = d;
                    tempidx = i;
                }
            } // end for

            // Marker가 클릭한 위치로 부터 일정 거리 안쪽에 있을 경우
            if (min < 5)
            {
                var marker = GraphComponent.MarkerN[tempidx];
                
                if (marker.eMarkerType == EMarkerType.Fixed)
                {
                    return;
                }

                isDragOn = true;
                selectedMarkerIdx = tempidx;
            }

        }

        // Mouse Move
        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int x = e.X;
            int y = e.Y;
            int TX = x - GraphComponent.MainPaddingX;
            int limitX = x + GraphComponent.MaxWidth - 1;
            if (TX < 0) return;
            if (TX > limitX) return;

            mouseX = x;
            mouseY = y;
        }

        public override void Refresh()
        {
            base.Refresh();

            this.pictureBox1.Refresh();
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
                if (pictureBox1.IsDisposed)
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

            DrawMarker(g);

            DrawMouseMove(g);
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
                for ( int i = 0; i < DataLength; i++)
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
            for ( int i = 1; i < GraphComponent.XAxisLineCount + 1; i++)
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

            for ( int i = 0; i < GraphComponent.YAxisLineCount; i++ )
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

        // Marker를 도시함
        private void DrawMarker(Graphics g)
        {
            int ix = 0;

            if (GraphComponent.IsShowSample)
            {
                MakeSampleMarker();
            }

            // Zero Span일 때 미도시

            // 선택된 마커가 없을 때 미도시

            // 추후 가져온 마커로 설정, 일단 그냥 생성
            Marker marker = GraphComponent.MarkerN[ix];

            if (marker == null)
            {
                return;
            }

            if ( !marker.IsTurnOn)
            {
                return;
            }

            // 일단 0 번째 인자로 설정
            Pen Pen1 = Marker.LineColor[ix];
            Color color1 = Marker.LineColor2[ix];

            // To Do :: LX 및 LY 할당
            int LLX = 100; 
            int LLY = 0; 
            if (LLX >= 1000) LLX = 1000;
            if (LLX < 0) LLX = 0;

            int MX = LLX + GraphComponent.MainPaddingX;
            int MY = LLY + GraphComponent.MainPaddingY;

            // Sample Span
            int sampleSpan = 1001;
            var sp = sampleSpan - 1;
            double span1PixelHz = GraphComponent.SpanFreq / sp;
            long temp = (long)((double)(marker.LX - sp / 2) * span1PixelHz);
            double Hz1 = GraphComponent.FreqCenter + temp;
            string ss = Unit1000String.GetFreqStringWithUnit(Hz1, 0);

            var wType = marker.eMarkerType;
            // Delta인 경우에 추가적인 string 표시
            if (wType == EMarkerType.Delta)
            {
                // delta target relative index
                var rix = marker.RelativeIndex;
                if ( rix != ix)
                {
                    // delta target marker에 대한 hz를 도시
                    double Hz2 = (LLX - GraphComponent.MarkerN[rix].LX) * span1PixelHz;
                    ss = Unit1000String.GetFreqStringWithUnit(Hz2, 0);
                    ss = "∆ " + ss;
                }
            }

            int tix = marker.TargetTraceIndex;
            int td = 0; // TraceN[tix].WorkDetectorLL
            int ixx = marker.LX;
            if (td == 0)
            {
                ixx = marker.LX * 2 + 1;
            }
            double val_Y = -((double)GraphComponent.TraceN[tix].data[ixx] + GraphComponent.AmpOffset);
            
            string sss = string.Format("{0:0.0}", -val_Y);

            float step = 1.0f;
            int stepi = (int)step;
            double scale = GraphComponent.AmpScaleDiv * GraphComponent.AmpScaleDivRatio;
            scale = 1 / scale;
            double yy = val_Y;
            double yy2 = 0;
            yy2 += GraphComponent.AmpRefLevel;
            yy += yy2;
            yy *= scale * ((float)GraphComponent.MainHeight / 100);

            if ( wType == EMarkerType.Fixed )
            {
                PointF beginPoint4 = new PointF((float)MX, GraphComponent.MainPaddingY);
                PointF endPoint4 = new PointF((float)MX, GraphComponent.MainPaddingY + GraphComponent.MainHeight);
                Pen Pen2 = new Pen(Color.RoyalBlue, 1);

                g.DrawLine(Pen1, beginPoint4, endPoint4);
            }

            double posY = GraphComponent.MainPaddingY + yy;
            if ( CheckY(ref posY, GraphComponent.MaxHeight) != 0) //  y에 대한 limitaion
            {
                return;
            }

            // // 마름모 및 안에 문자열을 도시하지 않음 - 이와 관련한 이유에 대해서 별다른 설명이 없는듯
            // const int TriangleH = 15;
            // Point[] TriangleInv = new Point[] { new Point(-9, -TriangleH), new Point(0, -TriangleH * 2), new Point(9, -TriangleH), new Point(0, 0) };
            // for (int i = 0; i < 4; i++)
            // {
            //     TriangleInv[i].X += MX;
            //     TriangleInv[i].Y += (int)posY;
            // }
            // SolidBrush brush = new SolidBrush(color1);
            // g.DrawPolygon(Pen1, TriangleInv);
                
            // GLUtil.FONT drawFont = new GLUtil.FONT(FontNameD, 10);
            // SolidBrush drawBrush = new SolidBrush(color1);
            // float ssx = (float)(marker.LX + GraphComponent.MainPaddingX);
            // float ssy = (float)GraphComponent.MainHeight - GraphComponent.MainPaddingY - 17;
            // if ( posY >= ssy)
            // {
            //     ssy = (float)GraphComponent.MainPaddingY + 4;
            // }
            // StringFormat drawFormat = new StringFormat();
            // // drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
                
            // // show str. number in diamond
            // ssx = (float)GraphComponent.MainPaddingX + marker.LX;
            // drawFormat.FormatFlags = 0;
            // string ssN = string.Format("{0}", ix + 1);
            // g.DrawString(ssN, drawFont, drawBrush, ssx - 5 * ssN.Length, (int)posY - 22, drawFormat);
                
            // ssx = (float)(GraphComponent.MainPaddingX + marker.LX - 16);
            // ssy = (float)posY - TriangleH - 16;
            // drawFormat.FormatFlags = 0;

            // if ( wType == EMarkerType.Fixed)
            // {
            //     string ssf = "F";
            //     g.DrawString(ssf, drawFont, drawBrush, ssx + 12, GraphComponent.MainPaddingY - 16, drawFormat);
            // }
        }

        // Mouse Move 
        private void DrawMouseMove(Graphics g)
        {
            // To Do :: Selected Marker가 없는 경우에 대한 처리
            // To Do :: Zero Span인 경우에 대한 처리
            
            if (!isDragOn)
            {
                return;
            }

            if ( mouseX < GraphComponent.MainPaddingX || mouseX >= GraphComponent.MainWidth + GraphComponent.MainPaddingX)
            {
                return;
            }

            Pen LineColor = Marker.LineColor[selectedMarkerIdx];
            Color color = Marker.LineColor2[selectedMarkerIdx];

            int LLX = mouseX - GraphComponent.MainPaddingX;
            if (LLX >= 1000) LLX = 1000;
            if (LLX < 0) LLX = 0;

            GraphComponent.MarkerN[selectedMarkerIdx].LX = LLX;

            // To Do :: Draw Marker 부분과 공통된 부분의 코드로 확인해보고 하나의 함수로 만들기
            int sampleSpan = 1001;
            var sp = sampleSpan - 1;
            double span1PixelHz = GraphComponent.SpanFreq / sp;
            long temp = (long)((double)(LLX - sp / 2) * span1PixelHz);
            double Hz1 = GraphComponent.FreqCenter + temp;
            string ss = Unit1000String.GetFreqStringWithUnit(Hz1, 0);

            GraphComponent.MarkerN[selectedMarkerIdx].Freq = Hz1;
            var wType = GraphComponent.MarkerN[selectedMarkerIdx].eMarkerType;
            if (wType == EMarkerType.Delta)
            {
                int rix = GraphComponent.MarkerN[selectedMarkerIdx].RelativeIndex;
                if ( rix != selectedMarkerIdx)
                {
                    double Hz2 = (LLX - GraphComponent.MarkerN[rix].LX ) * span1PixelHz;
                    ss = Unit1000String.GetFreqStringWithUnit(Hz2, 0);
                    ss = "∆ " + ss;
                }
            }

            int tix = GraphComponent.MarkerN[selectedMarkerIdx].TargetTraceIndex;
            int td = GraphComponent.TraceN[tix].WorkDetectorLL;
            int ixx = LLX;
            if (td == 0) ixx = LLX * 2 + 1;
            double valY = -((double)GraphComponent.TraceN[tix].data[ixx] + GraphComponent.AmpOffset);
            string sss = string.Format("{0:0.0}", -valY);

            float step = 1.0f;
            int stepi = (int)step;
            double scale = GraphComponent.AmpScaleDiv * GraphComponent.AmpScaleDivRatio;
            scale = 1 / scale;
            double yy = valY;
            double yy2 = 0;
            yy2 += GraphComponent.AmpRefLevel;
            yy += yy2;
            yy *= scale * ((float)GraphComponent.MainHeight / 100);

            const int TriangleH = 19;

            PointF beginPoint4 = new PointF((float)mouseX, GraphComponent.MainPaddingY);
            PointF endPoint4 = new PointF((float)mouseX, GraphComponent.MainPaddingY + GraphComponent.MainHeight);
            g.DrawLine(LineColor, beginPoint4, endPoint4);

            double posY = GraphComponent.MainPaddingY + yy;
            if (CheckY(ref posY, GraphComponent.MaxHeight) != 0)
            {
                return;
            }

            Point[] TriangleInv = new Point[] { new Point(-10, -TriangleH), new Point(10, -TriangleH), new Point(0, 0) };
            for (int i = 0; i < 3; i++)
            {
                TriangleInv[i].X += mouseX;
                TriangleInv[i].Y += (int)posY;
            }
            g.DrawPolygon(LineColor, TriangleInv);

            Font drawFont = new Font(FontNameD, 10);
            SolidBrush drawBrush = new SolidBrush(color);
            float ssx = (float)mouseX;
            float ssy = (float)GraphComponent.MainHeight - GraphComponent.MainPaddingY - 17;
            if (posY >= ssy)
            {
                ssy = (float)GraphComponent.MainPaddingY + 4;
            }
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = StringFormatFlags.DirectionVertical;
            g.DrawString(ss, drawFont, drawBrush, ssx, ssy, drawFormat);

            ssx = (float)(GraphComponent.MainPaddingX + LLX - 16);
            ssy = (float)posY - TriangleH - 16;
            drawFormat.FormatFlags = 0;
            g.DrawString(sss, drawFont, drawBrush, ssx, ssy, drawFormat);

            if (wType == EMarkerType.Fixed)
            {
                string ssf = "F";
                g.DrawString(ssf, drawFont, drawBrush, ssx + 12, GraphComponent.MainPaddingY - 16, drawFormat);
            }
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

        private void MakeSampleMarker()
        {
            GraphComponent.MarkerN[0].IsTurnOn = true;

            GraphComponent.MarkerN[0].eMarkerType = EMarkerType.Normal;

            GraphComponent.MarkerN[0].LX = 100;
            GraphComponent.MarkerN[0].LY = 100;
        }
    }
}
