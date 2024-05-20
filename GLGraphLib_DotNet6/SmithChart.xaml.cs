using SharpGL;
using SharpGL.WPF;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media;

namespace GLGraphLib
{
    /// <summary>
    /// SmithChart.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SmithChart : ChartUserControlBase
    {
        bool initAxis = false;
        float xMinCircle = 0.0f;
        float xMaxCircle = 0.0f;
        float yMaxCircle = 0.0f;
        float yMinCircle = 0.0f;

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

            // 축을 구성하는 Object 및 Text 도시
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
        private void DrawAxis(OpenGL gl)
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

            if (!initAxis)
            {
                initAxis = true;

                xMinCircle = (float)(cx - cr);
                xMaxCircle = (float)(cx + cr);
                yMinCircle = (float)(cy - cr);
                yMaxCircle = (float)(cy + cr);

                Console.WriteLine("circle x min, max = " + xMinCircle + ", " + xMaxCircle);
                Console.WriteLine("circle y min, max = " + yMinCircle + ", " + yMaxCircle);
            }

            if (IsPolarAxis)
            {
                const int N = 360;
                float[] theta = new float[N + 1];
                for (int i = 0; i <= N; i++)
                {
                    theta[i] = 2 * (float)Math.PI * i / N;
                }

                gl.Color(0.7f, 0.7f, 0.7f); // Set color for grid lines

                // Draw concentric circles
                for (double r = 0; r <= rd; r += rd / 4.0)
                {
                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    for (int i = 0; i <= N; i++)
                    {
                        var x = r * Math.Cos(theta[i]) + cx;
                        var y = r * Math.Sin(theta[i]) + cy;
                        gl.Vertex(x, y);
                    }
                    gl.End();
                }

                // Draw central cross lines
                DrawLine(gl, cx + rd, cy, cx - rd, cy);
                DrawLine(gl, cx, cy + rd, cx, cy - rd);

                // Draw 45 degree lines
                for (int theta_t = 45; theta_t <= 45 + 90 * 3; theta_t += 90)
                {
                    float cosTheta = (float)Math.Cos(theta_t * Math.PI / 180);
                    float sinTheta = (float)Math.Sin(theta_t * Math.PI / 180);
                    DrawLine(gl, cx + rd * 0.25f * cosTheta, cy + rd * 0.25f * sinTheta,
                                cx + rd * cosTheta, cy + rd * sinTheta);
                }

                // Draw 22.5 degree lines
                for (float theta_t = 22.5f; theta_t <= 22.5f + 45 * 7; theta_t += 45f)
                {
                    float cosTheta = (float)Math.Cos(theta_t * Math.PI / 180);
                    float sinTheta = (float)Math.Sin(theta_t * Math.PI / 180);
                    DrawLine(gl, cx + rd * 0.5f * cosTheta, cy + rd * 0.5f * sinTheta,
                                cx + rd * cosTheta, cy + rd * sinTheta);
                }

                // Draw 11.25 degree lines
                for (float theta_t = 22.5f / 2; theta_t <= 22.5f / 2 + 22.5f * 15; theta_t += 45f / 2)
                {
                    float cosTheta = (float)Math.Cos(theta_t * Math.PI / 180);
                    float sinTheta = (float)Math.Sin(theta_t * Math.PI / 180);
                    DrawLine(gl, cx + rd * 0.75f * cosTheta, cy + rd * 0.75f * sinTheta,
                                cx + rd * cosTheta, cy + rd * sinTheta);
                }
            }

            else
            {
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
        }

        private void DrawData(OpenGL gl)
        {
            var cx = CurrentControlWidth / 2;
            var cy = CurrentControlHeight / 2;
            var rd = (CurrentControlHeight - PaddingHorizontal) / 2; // 반지름 
            int pointOutsideOfCircleCount = 0;

            for (int i_trace = 0; i_trace < ComplexTrace.MaxTraceCount ; i_trace++)
            {
                var countOfComplexes = Complexes.Data(i_trace).Count;
                var complex = Complexes.Data(i_trace);
                var currentTraceColor = TraceColors[i_trace];
                List<PointF> points = new List<PointF>();

                for (int i = 0; i < countOfComplexes; i++ )
                {
                    var real = complex[i].Real;
                    var imag = complex[i].Imaginary;

                    real = real > 1 ? 0.9999 : real;
                    real = real < -1 ? -0.9999 : real;
                    imag = imag > 1 ? 0.9999 : imag;
                    imag = imag < -1 ? -0.9999 : imag;

                    // Convert to Screen Position
                    var x = real * rd + cx;
                    var y = imag * rd + cy;

                    // Check if the point is within the circle
                    var distanceFromCenter = (x - cx) * (x - cx) + (y - cy) * (y - cy);
                    var radiusSquared = rd * rd;

                    if (distanceFromCenter > radiusSquared - 50) // Offset을 두고 설정
                    {
                        // Calculate the angle of the line between the center and the given point
                        var angle = (float)Math.Atan2(y - cy, x - cx);

                        // Calculate the point on the circumference using trigonometry
                        x = cx + rd * (float)Math.Cos(angle);
                        y = cy + rd * (float)Math.Sin(angle);

                        pointOutsideOfCircleCount++;
                    }

                    points.Add(new PointF((float)x, (float)y));

                } // end for (int i = 0; i < Complexes.Count; i++ )

                // Draw lines connecting the points
                if (points.Count > 1)
                {
                    int cntOfClusterDensity = CalcUtil.CalculateClusterDensity(points.ToArray(), xMaxCircle, (yMinCircle + yMaxCircle) / 2.0f, 2.0)
                                            + CalcUtil.CalculateClusterDensity(points.ToArray(), xMinCircle, (yMinCircle + yMaxCircle) / 2.0f, 2.0)
                                            + CalcUtil.CalculateClusterDensity(points.ToArray(), (xMinCircle + xMaxCircle) / 2.0f, (yMinCircle + yMaxCircle) / 2.0f, 2.0);

                    // Set pen properties (color, width)
                    gl.Color(currentTraceColor.R, currentTraceColor.G, currentTraceColor.B);
                    gl.LineWidth(2.5f + cntOfClusterDensity * 0.02f);

                    int countOfPoints = points.Count;
                    for (int iter_point = 0; iter_point < countOfPoints - 1; iter_point++)
                    {
                        DrawLine(gl, points[iter_point].X, points[iter_point].Y, points[iter_point + 1].X, points[iter_point + 1].Y);
                    }
                } // end if (points.Count > 1)

            } // end for (int i_trace = 0; i_trace < ComplexTrace.MaxTraceCount ; i_trace++)
        }

        private void DrawLine(OpenGL gl, double x1, double y1, double x2, double y2)
        {
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex(x1, y1);
            gl.Vertex(x2, y2);
            gl.End();
        }
    }
}
