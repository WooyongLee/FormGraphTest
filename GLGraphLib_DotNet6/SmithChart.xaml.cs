using SharpGL;
using SharpGL.WPF;
using System;
using System.Drawing;

namespace GLGraphLib
{
    /// <summary>
    /// SmithChart.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SmithChart : ChartUserControlBase
    {
        bool initAxis = false;
        double xMinCircle = 0.0f;
        double xMaxCircle = 0.0f;
        double yMaxCircle = 0.0f;
        double yMinCircle = 0.0f;

        public SmithChart()
        {
            InitializeComponent();

            this.SizeChanged += SmithChart_SizeChanged;
            this.openGLControl.OpenGLDraw += OpenGLControl_OpenGLDraw;
            this.openGLControl.Resized += OpenGLControl_Resized;

            this.InitProperty();
        }

        private void SmithChart_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CurrentControlWidth = this.ActualWidth;
                CurrentControlHeight = this.ActualHeight;
            }));
        }

        private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs args)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.Viewport(0, 0, (int)openGLControl.Width, (int)openGLControl.Height);
        }

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            OpenGL gl = openGLControl.OpenGL;

            // Set the clear color and clear the color buffer and depth buffer
            gl.ClearColor(BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, 0.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Set up the projection matrix
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();

            gl.Ortho(0, CurrentControlWidth, 0, CurrentControlHeight, -1, 1);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            // Set the color to black
            gl.Color(BackgroundColor.R, BackgroundColor.G, BackgroundColor.B);

            gl.PushMatrix();

            // Text 도시
            DrawAxisLabel(gl);

            // 축을 구성하는 Object 도시
            DrawAxis(gl);

            // Chart에 올라가는 데이터 도시
            DrawData(gl);

            gl.PopMatrix();
            gl.Flush();

            // GL Control 내에서 Draw 할 때 마다 iteration 증가
            if (iter < 200000) iter++;
        }

        /// <summary>
        /// 축의 텍스트를 도시
        /// </summary>
        /// <param name="gl"></param>
        private void DrawAxisLabel(OpenGL gl)
        {
            // 텍스트 사이즈 설정
            // GLUtil.FONT size 를 1회 이상 변경해야 Text가 도시하는 현상이 있음 (해당 구문 제거 시, 축 Text 도시하지 않음)
            int fontsize = SpectrumChartUtil.GetFontSize(iter, CurrentControlWidth);
            int fontsizeX = fontsize - 1;

            // 중심점 및 반지름 설정
            var cx = CurrentControlWidth / 2;
            var cy = CurrentControlHeight / 2 ;
            var rd = (CurrentControlHeight - PaddingHorizontal) / 2; // 반지름 
            var cr = rd;

            // 텍스트를 도시한 후 축 오브젝트를 그리는 순서로 해야 Gl Control에 실제로 텍스트를 도시할 수 있음
            // 중심 수평선 위의 텍스트 도시
            int DrawingLength = 180;
            double[] r = new double[6] { 0.0, 0.2, 0.5, 1.0, 2, 5 };
            for (int i = 0; i < r.Length; i++)
            {
                double x = r[i] / (r[i] + 1);
                double y = 0;
                double ra = 1 / (r[i] + 1);
                double[] th = new double[DrawingLength];
                for (int j = 0; j < DrawingLength; j++)
                {
                    th[j] = j * 2 * Math.PI / (DrawingLength - 1);
                }
                double[] xunit = new double[DrawingLength];
                double[] yunit = new double[DrawingLength];
                for (int j = 0; j < DrawingLength; j++)
                {
                    xunit[j] = cx + (ra * Math.Cos(th[j]) + x) * rd;
                    yunit[j] = cy + (ra * Math.Sin(th[j]) + y) * rd;
                }

                gl.Color(AxisColor.R, AxisColor.G, AxisColor.B); 
                gl.LineWidth(1.0f);
                gl.Begin(OpenGL.GL_LINE_STRIP);
                for (int j = 0; j < DrawingLength; j++)
                {
                    gl.Vertex(xunit[j], yunit[j]);
                }
                gl.End();

                string wr = r[i].ToString();
                double p = (r[i] >= 1 || r[i] == 0) ? 0.04 : 0.1; // 소수점인 경우에는 텍스트 길이가 긴 것을 고려함
                double TextOffset = 0.025; // 텍스트를 접점 기준으로 약간 좌측으로 배치
                
                var textPosX = cx + (x + ra * Math.Cos(Math.PI) - p - TextOffset) * cr;
                var textPosY = cy;

                gl.DrawText((int)textPosX, (int)textPosY, AxisColor.R, AxisColor.G, AxisColor.B, GLUtil.FONT, fontsizeX, wr);
            } // end for (int i = 0; i < r.Length; i++)

            // 중심 위 아래의 호를 따라 텍스트 설정
            double[] xx = new double[] { -5, -2, -1, -0.5, -0.2, -0.1, 0.1, 0.2, 0.5, 1, 2, 5 };
            for (int i = 0; i < xx.Length; i++)
            {
                double x1 = xx[i];
                double ye = 2 / x1 / ((1 / x1) * (1 / x1) + 1);
                double xe = -Math.Sqrt(Math.Abs(1 / (x1 * x1) - (ye - 1 / x1) * (ye - 1 / x1))) + 1;

                double step = (xx[i] < 0) ? -0.001 : 0.001; // 음수는 아래쪽의 곡선을 그리기 위함
                int numPoints = (int)(ye / step) + 1;
                double[] yunit = new double[numPoints];
                double[] xunit = new double[numPoints];
                for (int j = 0; j < numPoints; j++)
                {
                    var yTemp = (j * step);
                    yunit[j] = cy + yTemp * cr;
                    xunit[j] = cx + (-Math.Sqrt(Math.Abs(1 / (x1 * x1) - (yTemp - 1 / x1) * (yTemp - 1 / x1))) + 1) * cr;
                }

                gl.Color(AxisColor.R, AxisColor.G, AxisColor.B); // 흰색으로 설정
                gl.LineWidth(1.0f);
                gl.Begin(OpenGL.GL_LINE_STRIP);
                for (int j = 0; j < numPoints; j++)
                {
                    gl.Vertex(xunit[j], yunit[j]);
                }
                gl.End();

                if (xx[i] != 0)
                {
                    string s1 = xx[i].ToString();
                    string s2 = "j";
                    string wr = s1 + s2;

                    double mx, my;
                    if (xx[i] >= 1)
                    {
                        mx = 0.01;
                        my = 0.03;
                    }
                    else if (Math.Abs(xx[i]) > 1 && xx[i] < 0)
                    {
                        mx = 0.01;
                        my = 0.01;
                    }
                    else if (xx[i] < 0 && xx[i] > -1)
                    {
                        mx = -0.13;
                        my = -0.01;
                    }
                    else
                    {
                        mx = 0;
                        my = 0;
                    }

                    var textPosX = cx + (float)(xe + mx) * rd;
                    var textPosY = cy + (float)(ye + my) * rd;

                    gl.DrawText((int)textPosX, (int)textPosY, AxisColor.R, AxisColor.G, AxisColor.B, GLUtil.FONT, fontsizeX, wr);
                }
            } // end for (int i = 0; i < xx.Length; i++)
        }

        private void DrawAxis(OpenGL gl)
        {
            // 중심점 및 반지름 설정
            var cx = CurrentControlWidth / 2;
            var cy = CurrentControlHeight / 2;
            var rd = (CurrentControlHeight - PaddingHorizontal) / 2; // 반지름 
            var cr = rd;

            // Axis Min/Max 로그 도시
            if (!initAxis)
            {
                initAxis = true;

                xMinCircle = cx - cr;
                xMaxCircle = cx + cr;
                yMinCircle = cy - cr;
                yMaxCircle = cy + cr;

                Console.WriteLine("circle x min, max = " + xMinCircle + ", " + xMaxCircle);
                Console.WriteLine("circle y min, max = " + yMinCircle + ", " + yMaxCircle);
            }

            gl.Color(AxisColor.R, AxisColor.G, AxisColor.B);
            gl.LineWidth(0.1f);

            // 원 그리기
            gl.Begin(OpenGL.GL_LINE_LOOP);
            for (int i = 0; i < 360; i++)
            {
                double angle = i * Math.PI / 180;
                double x = cx + cr * Math.Cos(angle);
                double y = cy + cr * Math.Sin(angle);
                gl.Vertex(x, y);
            }
            gl.End();

            // 중앙 수평선 그리기
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex(cx - cr, cy);
            gl.Vertex(cx + cr, cy);
            gl.End();
        }

        private void DrawData(OpenGL gl)
        {

        }
    }
}
