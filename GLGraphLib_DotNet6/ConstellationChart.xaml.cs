using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Windows.Shapes;
using SharpGL;
using SharpGL.SceneGraph;
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
        float Legend_Offset = 50;
        ConstellationComponent component;

        public ConstellationChart()
        {
            InitializeComponent();

            CH_X = new double[ConstellationComponent.MaxChannel, ConstellationComponent.MaxConstellationData];
            CH_Y = new double[ConstellationComponent.MaxChannel, ConstellationComponent.MaxConstellationData];
            component = new ConstellationComponent(CH_X,CH_Y);
            
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

            var yScaledPadding = IsShowLegend ? PaddingVertical + Legend_Offset : PaddingVertical;
            var scaledHeight = IsShowLegend ? CurrentControlHeight - Legend_Offset : CurrentControlHeight;

            var ScreenMaxX = Math.Round(CurrentControlWidth) - PaddingHorizontal - PaddingHorizontal;
            var ScreenMaxY = Math.Round(scaledHeight) - PaddingVertical - PaddingVertical;

            var ScreenStandard = Math.Min(ScreenMaxX, ScreenMaxY);

            float x = (float)((0 - MinX) / (MaxX - MinX) * ScreenStandard) + PaddingHorizontal;
            float y = (float)((0 - MinY) / (MaxY - MinY) * ScreenStandard) + yScaledPadding;

            // Console.WriteLine("ScreenStandard = " + ScreenStandard + ", x = " + x + " , y = " + y);
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

            var scaledHeight = IsShowLegend ? CurrentControlHeight - Legend_Offset : CurrentControlHeight;

            // Calculate the size of each square based on the row and column spacing
            var colSize = (CurrentControlWidth - (PaddingHorizontal + PaddingHorizontal)) / (float)NumOfColumn;
            var rowSize = (scaledHeight - (PaddingVertical + PaddingVertical)) / (float)NumOfRow;

            float size = (float)Math.Min(colSize, rowSize);

            // Calculate the starting position for drawing the chart(원점은 7시 기준)
            var startX = PaddingHorizontal;
            var startY = IsShowLegend ? PaddingVertical + Legend_Offset : PaddingVertical;

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
                    gl.Color(AxisColor.R, AxisColor.G, AxisColor.B);
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

            var ScaledHeight = IsShowLegend ? (int)CurrentControlHeight - Legend_Offset : (int)CurrentControlHeight;

            var ScreenMinX = PaddingHorizontal - MarginX - xOffset;
            var ScreenMaxX = (int)CurrentControlWidth - PaddingHorizontal - PaddingHorizontal + xOffset;
            // var ScreenMinY = (int)PaddingVertical - MarginY;
            var ScreenMinY = (IsShowLegend ? (int)PaddingVertical + Legend_Offset : (int)PaddingVertical) - MarginY;
            var ScreenMaxY = (int)ScaledHeight - PaddingVertical - PaddingVertical + MarginY ;

            var ScreenStandard = Math.Min(ScreenMaxX, ScreenMaxY);
            var LegendOffset = IsShowLegend ? 50 : 0;

            // Set the color to white
            gl.Color(AxisColor.R, AxisColor.G, AxisColor.B);

            // 최 외곽 Max 도시
            DrawText(gl, MaxX, ScreenStandard + xAxisXoffset, ScreenMinY, fontsize);
            DrawText(gl, MaxY, ScreenMinX, ScreenStandard + yAxisYOffset + LegendOffset, fontsize);
          
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
                    var screenX = (ScreenMinX * (NumOfRow - i) + (ScreenStandard * i)) / NumOfRow + xAxisXoffset;

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
                    var screenY = (ScreenMinY * (NumOfColumn - i) + ((LegendOffset + ScreenStandard) * i)) / NumOfColumn + yAxisYOffset;

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
            gl.DrawText((int)x, (int)y, AxisColor.R, AxisColor.G, AxisColor.B, GLUtil.FONT, size, strValue);
        }

        private void DrawText(OpenGL gl, string strText, float x, float y, float size = 12.0f)
        {
            var encodingBytes = System.Text.Encoding.ASCII.GetBytes(strText);

            var encodingStr = System.Text.Encoding.UTF8.GetString(encodingBytes);

            // Draw Text -> Font는 vendana로 고정, MS. San Sarif 사용 시 의도하지 않은 문자열을 도시함
            gl.DrawText((int)x, (int)y, AxisColor.R, AxisColor.G, AxisColor.B, "verdana", size, encodingStr);
        }

        // Create Sample Data
        public void MakeDataForTest()
        {
            Random random = new Random();
            Random random2 = new Random();

            component.SetChannelValue(CH_X, 0, 0, 0.0);
            component.SetChannelValue(CH_Y, 0, 0, 0.0);

            int row = 8;

            // 4 Channel Test
            for ( int i = 0; i < 8; i++)
            {
                for (int j = 0; j <= row; j++)
                {
                    component.SetChannelValue(CH_X, i, j, j * 0.5 - 2);
                    component.SetChannelValue(CH_Y, i, j, 2 - i * 0.5 );
                }
            }
        }

        // Component를 통하여 데이터를 도시
        private void DrawData(OpenGL gl)
        {
            // 임의로 샘플 데이터 도시
            if (IsLoadSample)
            {
                // MakeDataForTest();
            }

            var xLength = CH_X.Length;
            var yLength = CH_Y.Length;

            var yScaledPadding = IsShowLegend ? PaddingVertical + Legend_Offset : PaddingVertical;

            var scaledHeight = IsShowLegend ? CurrentControlHeight - Legend_Offset : CurrentControlHeight;

            var ScreenMaxX = Math.Round(CurrentControlWidth) - PaddingHorizontal - PaddingHorizontal;
            var ScreenMaxY = Math.Round(scaledHeight) - PaddingVertical - PaddingVertical;

            var ScreenStandard = Math.Min(ScreenMaxX, ScreenMaxY);

            for ( int i = 0; i < ConstellationComponent.MaxChannel; i++)
            {
                for ( int j = 0; j < ConstellationComponent.MaxConstellationData; j++)
                {
                    // 설정한 Index 범위를 벗어날 때
                    if (i >= CH_X.GetLength(0) || j >= CH_X.GetLength(1))
                    {
                        break;
                    }

                    if (i >= CH_Y.GetLength(0) || j >= CH_Y.GetLength(1))
                    {
                        break;
                    }

                    if ( CH_X[i, j] != 0 && CH_Y[i, j] != 0)
                    {
                        //float x = (float)((CH_X[i, j] - MinX) / (MaxX - MinX) * (CurrentControlWidth - PaddingHorizontal * 2)) + PaddingHorizontal;
                        //float y = (float)((CH_Y[i, j] - MinY) / (MaxY - MinY) * (CurrentControlHeight - yScaledPadding * 2)) + yScaledPadding;
                        float x = (float)((CH_X[i, j] - MinX) / (MaxX- MinX) * ScreenStandard) + PaddingHorizontal;
                        float y = (float)((CH_Y[i, j] -MinY) / (MaxY - MinY) * ScreenStandard) + yScaledPadding;
                        
                        #region Min/Max 처리
                        // Min/Max 처리
                        if (CH_X[i, j] < MinX) x = PaddingHorizontal;
                        if (CH_X[i, j] > MaxX) x = (float)CurrentControlWidth - PaddingHorizontal;
                        if (CH_Y[i, j] < MinY) y = yScaledPadding;
                        if (CH_Y[i, j] > MaxY) y = (float)CurrentControlHeight - yScaledPadding;
                        #endregion

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

            // UpdateTheme();

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

            // 범주 text 형태의 Label을 도시함
            DrawAxisLabels(gl);

            // 축을 그림
            DrawIQConstellationAxis(gl);

            // 데이터를 그림
            DrawData(gl);

            // 범례를 그림
            DrawLegend(gl);

            gl.PopMatrix();

            // Flush OpenGL
            gl.Flush();
        }

        private void DrawLegend(OpenGL gl)
        {
            if (IsShowLegend)
            {
                // 임시로 Sample Data 도시
                // string[] strLegendContent = { "legend1", "legend2", "long_legend" };

                // Define the maximum length for a line
                var maxLineWidth = Math.Round(CurrentControlWidth) - PaddingHorizontal * 3; // Adjust this value as needed

                if (StrLegend != null)
                {
                    // Legend Position
                    int StartX = 10;
                    int StartY = (int)Legend_Offset - 5;
                    int xPosition = StartX;
                    int yPosition = StartY;
                    int index = 0;

                    const int rectangleX = 4;
                    const int rectangleY = 12;

                    foreach (var strContent in StrLegend)
                    {
                        // Draw Rectangle
                        gl.Color(ConstellationComponent.ChannelColors[index].R, ConstellationComponent.ChannelColors[index].G, ConstellationComponent.ChannelColors[index].B);

                        gl.Begin(OpenGL.GL_QUADS);
                        gl.Vertex(xPosition + rectangleX, yPosition, 0.0f);
                        gl.Vertex(xPosition + rectangleY, yPosition, 0.0f);
                        gl.Vertex(xPosition + rectangleY, yPosition - (rectangleY - rectangleX), 0.0f);
                        gl.Vertex(xPosition + rectangleX, yPosition - (rectangleY - rectangleX), 0.0f);
                        gl.End();

                        DrawText(gl, strContent, xPosition + rectangleY + 10, yPosition - 8, 10);

                        xPosition += (strContent.Length * 10);

                        // Set Next Position
                        if (xPosition > maxLineWidth)
                        {
                            xPosition = StartX; // reset to front
                            yPosition -= 10; // enter 1 line
                        }

                        index++;
                    } // end foreach (var strContent in StrLegend)
                } // end if (StrLegend != null)
            } // end if (IsShowLegend)
        }
    }
}