using SharpGL;
using SharpGL.WPF;
using System;
using System.Windows;
using System.Windows.Controls;

namespace GLGraphLib_DotNet6
{
    /// <summary>
    /// UserControl1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class BarGraphChart : ChartUserControlBase
    {
        // 반복을 체크함
        int iter = 0;

        public BarGraphChart()
        {
            InitializeComponent();

            this.InitProperty();
            this.openGLControl.OpenGLDraw += OpenGLControl_OpenGLDraw;
        }

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            OpenGL gl = openGLControl.OpenGL;

            // Set the clear color and clear the color buffer and depth buffer
            gl.ClearColor(BackgroundColor.R, BackgroundColor.G, BackgroundColor.B, 0.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Set up the projection matrix (시점 좌표를 설정)
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();

            // 투상 좌표를 정의 right = width, top = height, near = -1, far = 1
            gl.Ortho(0, CurrentControlWidth, 0, CurrentControlHeight, -1, 1);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            // Set the color to black
            gl.Color(BackgroundColor.R, BackgroundColor.G, BackgroundColor.B);

            gl.PushMatrix();

            DrawAxisLabels(gl);

            DrawAxis(gl);

            DrawBar(gl);

            gl.PopMatrix();

            // Flush OpenGL
            gl.Flush();

            // GL Control 내에서 Draw 할 때 마다 iteration 증가
            if (iter < 120000) iter++;
        }

        // 축을 도시함
        private void DrawAxis(OpenGL gl)
        {
            // Calculate the size of each square based on the row and column spacing
            int sizeX = (int)((CurrentControlWidth - PaddingHorizontal * 2) / NumOfColumn);
            int sizeY = (int)((CurrentControlHeight - PaddingVertical * 2) / NumOfRow);

            // Draw the chart
            for (int row = 0; row < NumOfRow; row++)
            {
                for (int col = 0; col < NumOfColumn; col++)
                {
                    // Calculate the position of the current square
                    var x = PaddingHorizontal + sizeX * col;
                    var y = PaddingVertical + sizeY * row;

                    // Draw the current square
                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    gl.Color(AxisColor.R, AxisColor.G, AxisColor.B);
                    gl.Vertex(x, y, 0.0f);
                    gl.Vertex(x + sizeX, y, 0.0f);
                    gl.Vertex(x + sizeX, y + sizeY, 0.0f);
                    gl.Vertex(x, y + sizeY, 0.0f);
                    gl.End();
                }
            }

            // 테두리는 가장 위로 오도록 그리기
            float zValue = 0.5f;
            sizeX = (int)(CurrentControlWidth - PaddingHorizontal * 2);
            sizeY = (int)(CurrentControlHeight - PaddingVertical * 2);
            gl.Begin(OpenGL.GL_LINE_LOOP);
            gl.Color(AxisColor.R, AxisColor.G, AxisColor.B);
            gl.Vertex(PaddingHorizontal, PaddingVertical, zValue);
            gl.Vertex(PaddingHorizontal + sizeX, PaddingVertical, zValue);
            gl.Vertex(PaddingHorizontal + sizeX, PaddingVertical + sizeY, zValue);
            gl.Vertex(PaddingHorizontal, PaddingVertical + sizeY, zValue);
            gl.End();
        }

        // 축에 텍스트를 도시함
        private void DrawAxisLabels(OpenGL gl)
        {
            // GLUtil.FONT size 를 1회 이상 변경해야 Text가 도시하는 현상이 있음 (해당 구문 제거 시, 축 Text 도시하지 않음)
            int fontsize = SpectrumChartUtil.GetFontSize(iter);
            int fontsizeX = fontsize - 1;

            // define text margin
            int MarginX = (int)(PaddingHorizontal * (4.0 / 5.0));
            int MarginY = (int)(PaddingVertical * (4.0 / 7.0));
            //int TopOffset = (int)(MarginY * (2.0 / 3.0));
            //int LeftfOffset = (int)(MarginX * (2.0 / 3.0));
            int xOffset = 5;
            int xAxisXoffset = 10 + (int)CurrentControlWidth / 14;
            int yAxisYOffset = 10;

            var ScreenMinX = PaddingHorizontal - MarginX - xOffset;
            var ScreenMaxX = (int)CurrentControlWidth - PaddingHorizontal * 2 + xOffset;
            var ScreenMinY = PaddingVertical - MarginY;
            var ScreenMaxY = (int)CurrentControlHeight - PaddingVertical * 2 + MarginY;

            // Column
            // 각 Content는 칸에 들어가기 때문에 조정함
            int numOfColSpace = NumOfColumn - 1;
            for (int i = 0; i < NumOfColumn; i++)
            {
                // X 우측 방향으로 xAxisOffset 만큼 넓힘
                var valueX = (MinX * (numOfColSpace - i) + (MaxX * i)) / numOfColSpace;
                var screenX = (ScreenMinX * (NumOfColumn - i) + (ScreenMaxX * i)) / NumOfColumn + xAxisXoffset;

                GLUtil.DrawFormattedText(gl, valueX, (int)screenX, (int)ScreenMinY, AxisColor, 2, fontsize);
            }

            // Row -> 각 칸 사이사이에 Min ... Max 
            for (int i = 0; i <= NumOfRow ; i++)
            {
                // Y 위쪽 방향으로 yAxisYOffset 만큼 올림
                var valueY = (MinY * (NumOfRow - i) + (MaxY * i)) / NumOfRow;
                var screenY = (ScreenMinY * (NumOfRow - i) + (ScreenMaxY * i)) / NumOfRow + yAxisYOffset;

                GLUtil.DrawFormattedText(gl, valueY, (int)ScreenMinX, (int)screenY, AxisColor, 2, fontsize);
            }
            
            // 최 좌측 상단에 단위 도시 (y)
            gl.DrawText((int)ScreenMinX, (int)ScreenMaxY + yAxisYOffset * 3, AxisColor.R, AxisColor.G, AxisColor.B, GLUtil.FONT, fontsize, "(dBm)");
        }

        // Bar를 도시함
        private void DrawBar(OpenGL gl)
        {
            // Sample Data 정의
            Random random = new Random();
            double[] data = new double[] { -10, -20, -30, -40, -50.0, -60, -70.0, -80, -90 };
            for ( int i = 0; i < data.Length; i++)
            {
                // data[i] = random.Next(-100, 0);
            }

            // CnfOfColumn을 벗어난 데이터는 모두 버려야 함

            // Calculate the size of each square based on the row and column spacing
            int sizeX = (int)((CurrentControlWidth - PaddingHorizontal * 2) / NumOfColumn);
            int sizeY = (int)(CurrentControlHeight - PaddingVertical * 2);
            const double BottomOffset = 0.1; // 바닥에서 살짝 띄우기 위한 Offset

            // Draw the Bar
            for (int i = 0; i < NumOfColumn; i++)
            {
                // Calculate the position and size of the current bar
                var xBottom = PaddingHorizontal + sizeX * i; // x Bottom
                var yBottom = PaddingVertical + BottomOffset; // y Bottom
                var barWidth = sizeX * 0.725f;
                var barHeight = sizeY * CalcUtil.CalculatePercentile(MinY, MaxY, data[i]);

                // Draw the current bar
                gl.Begin(OpenGL.GL_QUADS);
                gl.Color(BarColor.R, BarColor.G, BarColor.B);
                gl.Vertex(xBottom + (sizeX - barWidth) / 2, yBottom, 0.0f);
                gl.Vertex(xBottom + (sizeX + barWidth) / 2, yBottom, 0.0f);
                gl.Vertex(xBottom + (sizeX + barWidth) / 2, yBottom + barHeight, 0.0f);
                gl.Vertex(xBottom + (sizeX - barWidth) / 2, yBottom + barHeight, 0.0f);
                gl.End();
            } // end for (int i = 0; i < data.Length; i++)
        }
    }
}
