using System;
using GLGraphLib;
using SharpGL;
using SharpGL.WPF;

namespace GLGraphLib
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ConstellationChart : ChartUserControlBase
    {
        // drawing iteration
        int iter = 0;

        ConstellationComponent component;

        public ConstellationChart()
        {
            InitializeComponent();

            component = new ConstellationComponent();
            
            this.SizeChanged += ConstellationChart_SizeChanged;
            this.openGLControl.OpenGLDraw += OpenGLControl_OpenGLDraw;
            this.openGLControl.Resized += OpenGLControl_Resized;

            this.InitProperty();
        }

        private void OpenGLControl_Resized(object sender, OpenGLRoutedEventArgs args)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.Viewport(0, 0, (int)openGLControl.Width, (int)openGLControl.Height);
        }

        private void ConstellationChart_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                CurrentControlWidth = this.ActualWidth;
                CurrentControlHeight = this.ActualHeight;
            }));
        }

        // 축을 표현하는 격자를 도시함
        private void DrawIQConstellationAxis(OpenGL gl)
        {
            // Draw the outer square (주석)
            //gl.Begin(OpenGL.GL_LINE_LOOP);
            //gl.Color(1.0f, 1.0f, 0.0f);
            //gl.LineWidth(5.0f);
            //gl.Vertex(0, 0);
            //gl.Vertex(0, CurrentControlHeight);
            //gl.Vertex(CurrentControlWidth, CurrentControlHeight);
            //gl.Vertex(CurrentControlWidth, 0);
            //gl.End();

            // Calculate the size of each square based on the row and column spacing
            int size = (int)Math.Min((CurrentControlWidth - (PaddingHorizontal + PaddingHorizontal)) / NumOfColumn,
                (CurrentControlHeight - (PaddingVertical + PaddingVertical)) / NumOfRow);

            // Calculate the starting position for drawing the chart(원점은 7시 기준)
            var startX = PaddingHorizontal;
            var startY = PaddingVertical;

            // Draw the chart
            for (int row = 0; row < NumOfRow; row++)
            {
                for (int col = 0; col < NumOfColumn; col++)
                {
                    // Calculate the position of the current square
                    var x = startX + size * col;
                    var y = startY + size * row;

                    // Draw the current square
                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    gl.Color(1.0f, 1.0f, 1.0f);
                    // gl.LineWidth(2.0f); // LineWidth 적용 시 Text 도시에 문제 발생
                    gl.Vertex(x, y, 0.0f);
                    gl.Vertex(x + size, y, 0.0f);
                    gl.Vertex(x + size, y + size, 0.0f);
                    gl.Vertex(x, y + size, 0.0f);
                    gl.End();
                }
            }
        }

        // 축에 들어가는 수를 도시함
        private void DrawAxisLabels(OpenGL gl)
        {
            // GLUtil.FONT size 를 1회 이상 변경해야 Text가 도시하는 현상이 있음 (해당 구문 제거 시, 축 Text 도시하지 않음)
            int fontsize = 10;
            if (iter++ > 1) fontsize = 12;

            // define text margin
            int MarginX = (int)(PaddingHorizontal * (4.0 / 5.0));
            int MarginY = (int)(PaddingVertical * (4.0 / 7.0));
            int TopOffset = (int)(MarginY * (2.0 / 3.0));
            int LeftfOffset = (int)(MarginX * (2.0 / 3.0));
            int xOffset = 5;
            int xAxisXoffset = 10;
            int yAxisYOffset = 10;

            var ScreenMinX = PaddingHorizontal - MarginX - xOffset;
            var ScreenMaxX = (int)CurrentControlWidth - PaddingHorizontal - PaddingHorizontal + xOffset;
            var ScreenMinY = PaddingVertical - MarginY;
            var ScreenMaxY = (int)CurrentControlHeight - PaddingVertical - PaddingVertical + MarginY ;

            // Set the color to white
            gl.Color(1.0f, 1.0f, 1.0f);

            // 최 외곽 Max 도시
            DrawText(gl, MaxX, ScreenMaxX + xAxisXoffset, ScreenMinY, fontsize);
            DrawText(gl, MaxY, ScreenMinX, ScreenMaxY + yAxisYOffset, fontsize);
          
            // 원점지역 Min 도시
            if (MinX == MinY)
            {
                // 좌측 하단 minimum 텍스트 하나는 생략
                DrawText(gl, MinY, ScreenMinX + xAxisXoffset, ScreenMinY, fontsize);
            }

            else
            {
                // 원점 부분에 적용하는 오프셋
                DrawText(gl, MinX, ScreenMinX + TopOffset, ScreenMinY, fontsize);
                DrawText(gl, MinY, ScreenMinX, ScreenMinY + LeftfOffset, fontsize);
            }

            // 중간 지점 수 도시
            // Row
            for (int i = 1; i < NumOfRow - 1; i++)
            {
                if (i % 2 == 0)
                {
                    // X 우측 방향으로 xAxisXOffset 만큼 밀어버림
                    var valueX = (MinX * (NumOfRow - i) + (MaxX * i)) / NumOfRow;
                    var screenX = (ScreenMinX * (NumOfRow - i) + (ScreenMaxX * i)) / NumOfRow + xAxisXoffset;

                    DrawText(gl, valueX, screenX, ScreenMinY, fontsize);
                }
            }

            // Column
            for (int i = 1; i < NumOfColumn - 1; i++)
            {
                if (i % 2 == 0)
                {
                    // Y 위쪽 방향으로 yAxisYOffset 만큼 올림
                    var valueY = (MinY * (NumOfColumn - i) + (MaxY * i)) / NumOfColumn;
                    var screenY = (ScreenMinY * (NumOfColumn - i) + (ScreenMaxY * i)) / NumOfColumn + yAxisYOffset;

                    DrawText(gl, valueY, ScreenMinX, screenY, fontsize);
                }
            }
        }

        // Text를 도시함
        private void DrawText(OpenGL gl, double value, float x, float y, float size = 12.0f)
        {
            // Set String Format
            string strValue = string.Format("{0,5:N1}", value);

            // Draw Text
            gl.DrawText((int)x, (int)y, 1.0f, 1.0f, 1.0f, GLUtil.FONT, size, strValue);
        }

        // Create Sample Data
        public void MakeDataForTest()
        {
            Random random = new Random();
            Random random2 = new Random();

            component.CH_X[0, 0] = 0.0;
            component.CH_Y[0, 0] = 0.0;

            // 4 Channel Test
            for ( int i = 0; i < 4; i++)
            {
                for (int j = 0; j <= 10; j++)
                {
                    //var randomValue = random.Next(-1, 1);
                    //var randomValue2 = random2.Next(-1, 1);
                    var randomValue = random.Next(2) == 0 ? -1 : 1;
                    var randomValue2 = random2.Next(2) == 0 ? -1 : 1;

                    var randomOffsetX = random.Next(-20, 20) / 100.0;
                    var randomOffsetY = random2.Next(-20, 20) / 100.0;

                    if (randomValue == 0 || randomValue2 == 0) continue;

                    component.CH_X[i, j] = randomValue - randomValue2 * randomOffsetX;
                    component.CH_Y[i, j] = randomValue2 - randomValue * randomOffsetY;
                }
            }
        }

        // Component를 통하여 데이터를 도시
        private void DrawData(OpenGL gl)
        {
            // 임의로 샘플 데이터 도시
            if (IsLoadSample)
            {
                MakeDataForTest();
            }

            for ( int i = 0; i < ConstellationComponent.MaxChannel; i++)
            {
                for ( int j = 0; j < ConstellationComponent.MaxConstellationData; j++)
                {
                    if (component.CH_X[i, j] != ConstellationComponent.NullValue && component.CH_Y[i, j] != ConstellationComponent.NullValue)
                    {
                        float x = (float)(((component.CH_X[i, j] - MinX) / (MaxX- MinX)) * (CurrentControlWidth - PaddingHorizontal * 2)) + PaddingHorizontal;
                        float y = (float)(((component.CH_Y[i, j] -MinY) / (MaxY - MinY)) * (CurrentControlHeight - PaddingVertical * 2)) + PaddingVertical;

                        // gl.Color(1.0f, 1.0f, 0.0f); // yellow
                        gl.Color(component.GetNormalizedR(i), component.GetNormalizedG(i), component.GetNormalizedB(i));
                        gl.PointSize(5.0f);
                        gl.Begin(OpenGL.GL_POINTS);
                        gl.Vertex(x, y, 0.0f);
                        gl.End();
                    } // end if : Check X, Y Null Value
                } // end for : j (Constellation Data)
            } // end for : i (Channel)
        }

        private void OpenGLControl_OpenGLDraw(object sender, OpenGLRoutedEventArgs args)
        {
            // Get the OpenGL object
            OpenGL gl = openGLControl.OpenGL;

            // Set the clear color and clear the color buffer and depth buffer
            gl.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Set up the projection matrix
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();

            gl.Ortho(0, CurrentControlWidth, 0, CurrentControlHeight, -1, 1);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            // Set the color to black
            gl.Color(0.0f, 0.0f, 0.0f);

            gl.PushMatrix();

            // 범주 text 형태의 Label을 도시함
            DrawAxisLabels(gl);

            // 축을 그림
            DrawIQConstellationAxis(gl);

            // 데이터를 그림
            DrawData(gl);

            gl.PopMatrix();

            // Flush OpenGL
            gl.Flush();
        }
    }
}