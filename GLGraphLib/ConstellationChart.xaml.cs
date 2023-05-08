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

        // ���� ǥ���ϴ� ���ڸ� ������
        private void DrawIQConstellationAxis(OpenGL gl)
        {
            // Draw the outer square (�ּ�)
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

            // Calculate the starting position for drawing the chart(������ 7�� ����)
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
                    // gl.LineWidth(2.0f); // LineWidth ���� �� Text ���ÿ� ���� �߻�
                    gl.Vertex(x, y, 0.0f);
                    gl.Vertex(x + size, y, 0.0f);
                    gl.Vertex(x + size, y + size, 0.0f);
                    gl.Vertex(x, y + size, 0.0f);
                    gl.End();
                }
            }
        }

        // �࿡ ���� ���� ������
        private void DrawAxisLabels(OpenGL gl)
        {
            // GLUtil.FONT size �� 1ȸ �̻� �����ؾ� Text�� �����ϴ� ������ ���� (�ش� ���� ���� ��, �� Text �������� ����)
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

            // �� �ܰ� Max ����
            DrawText(gl, MaxX, ScreenMaxX + xAxisXoffset, ScreenMinY, fontsize);
            DrawText(gl, MaxY, ScreenMinX, ScreenMaxY + yAxisYOffset, fontsize);
          
            // �������� Min ����
            if (MinX == MinY)
            {
                // ���� �ϴ� minimum �ؽ�Ʈ �ϳ��� ����
                DrawText(gl, MinY, ScreenMinX + xAxisXoffset, ScreenMinY, fontsize);
            }

            else
            {
                // ���� �κп� �����ϴ� ������
                DrawText(gl, MinX, ScreenMinX + TopOffset, ScreenMinY, fontsize);
                DrawText(gl, MinY, ScreenMinX, ScreenMinY + LeftfOffset, fontsize);
            }

            // �߰� ���� �� ����
            // Row
            for (int i = 1; i < NumOfRow - 1; i++)
            {
                if (i % 2 == 0)
                {
                    // X ���� �������� xAxisXOffset ��ŭ �о����
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
                    // Y ���� �������� yAxisYOffset ��ŭ �ø�
                    var valueY = (MinY * (NumOfColumn - i) + (MaxY * i)) / NumOfColumn;
                    var screenY = (ScreenMinY * (NumOfColumn - i) + (ScreenMaxY * i)) / NumOfColumn + yAxisYOffset;

                    DrawText(gl, valueY, ScreenMinX, screenY, fontsize);
                }
            }
        }

        // Text�� ������
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

        // Component�� ���Ͽ� �����͸� ����
        private void DrawData(OpenGL gl)
        {
            // ���Ƿ� ���� ������ ����
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

            // ���� text ������ Label�� ������
            DrawAxisLabels(gl);

            // ���� �׸�
            DrawIQConstellationAxis(gl);

            // �����͸� �׸�
            DrawData(gl);

            gl.PopMatrix();

            // Flush OpenGL
            gl.Flush();
        }
    }
}