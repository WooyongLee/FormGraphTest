using SharpGL.WPF;
using SharpGL;
using System.Windows.Controls;
using System;
using System.Windows.Media;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;

namespace GLGraphLib_DotNet6
{
    /// <summary>
    /// SpectrumChart.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SpectrumChart : UserControl
    {
        #region Dependency Properties
        public bool IsLoadSample
        {
            get { return (bool)GetValue(IsLoadSampleProperty); }
            set { SetValue(IsLoadSampleProperty, value); }
        }

        public static readonly DependencyProperty IsLoadSampleProperty = DependencyProperty.Register(
            "IsLoadSample",
            typeof(bool),
            typeof(SpectrumChart),
            null
            );
        #endregion

        int iter = 0;
        const string FONT = "Arial";

        double MinX = -2.0;
        double MaxX = 2.0;

        double MinY = -100.0;
        double MaxY = 0.0;

        // Chart Line Count
        int HorizontalSpaceCount = 10;
        int VerticalSpaceCount = 10;

        // Padding
        int PaddingHorizontal = 40; // x
        int PaddingVertical = 30; // y

        // 현재 컨트롤의 너비/높이
        double CurrentControlWidth = 300;
        double CurrentControlHeight = 300;

        SpecturmComponent component;

        public SpectrumChart()
        {
            InitializeComponent();

            component = new SpecturmComponent();

            MakeSampleData(ref component.data);

            this.SizeChanged += SpectrumChart_SizeChanged;
            this.openGLControl.OpenGLDraw += OpenGLControl_OpenGLDraw;
            this.openGLControl.Resized += OpenGLControl_Resized;
        }

        private void SpectrumChart_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
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

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.WPF.OpenGLRoutedEventArgs args)
        {
            OpenGL gl = openGLControl.OpenGL;

            // Set the clear color and clear the color buffer and depth buffer
            gl.ClearColor(0.0f, 0.0f, 0.0f, 0.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Set up the projection matrix (시점 좌표를 설정)
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();

            // 투상 좌표를 정의 right = width, top = height, near = -1, far = 1
            gl.Ortho(0, CurrentControlWidth, 0, CurrentControlHeight, -1, 1);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            // Set the color to black
            gl.Color(0.0f, 0.0f, 0.0f);

            gl.PushMatrix();

            DrawAxisLabels(gl);
            
            DrawSpectrumAxis(gl);

            DrawData(gl);

            gl.PopMatrix();

            // Flush OpenGL
            gl.Flush();
        }

        // 스펙트럼의 축을 도시함
        private void DrawSpectrumAxis(OpenGL gl)
        {
            // Calculate the size of each square based on the row and column spacing
            int sizeX = (int)((CurrentControlWidth - PaddingHorizontal * 2) / HorizontalSpaceCount);
            int sizeY = (int)((CurrentControlHeight - PaddingVertical * 2) / VerticalSpaceCount);

            // Draw the chart
            for (int row = 0; row < HorizontalSpaceCount; row++)
            {
                for (int col = 0; col < VerticalSpaceCount; col++)
                {
                    // Calculate the position of the current square
                    int x = PaddingHorizontal + sizeX * col;
                    int y = PaddingVertical + sizeY * row;

                    // Draw the current square
                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    gl.Color(1.0f, 1.0f, 1.0f);
                    gl.Vertex(x, y, 0.0f);
                    gl.Vertex(x + sizeX, y, 0.0f);
                    gl.Vertex(x + sizeX, y + sizeY, 0.0f);
                    gl.Vertex(x, y + sizeY, 0.0f);
                    gl.End();
                }
            }
        }

        // 스펙트럼 축 Label을 도시함
        private void DrawAxisLabels(OpenGL gl)
        {
            // font size 를 1회 이상 변경해야 Text가 도시하는 현상이 있음 (해당 구문 제거 시, 축 Text 도시하지 않음)
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

            int ScreenMinX = PaddingHorizontal - MarginX - xOffset;
            int ScreenMaxX = (int)CurrentControlWidth - PaddingHorizontal * 2 + xOffset;
            int ScreenMinY = PaddingVertical - MarginY;
            int ScreenMaxY = (int)CurrentControlHeight - PaddingVertical * 2 + MarginY;

            // Set the color to white
            gl.Color(1.0f, 1.0f, 1.0f);

            //// Row
            //int NumOfRow = HorizontalSpaceCount + 1;
            //for ( int i = 0; i <= HorizontalSpaceCount; i++ )
            //{
            //    // X 우측 방향으로 xAxisXOffset 만큼 밀어버림
            //    var valueX = (MinX * (NumOfRow - i) + (MaxX * i)) / NumOfRow;
            //    var screenX = (ScreenMinX * (NumOfRow - i) + (ScreenMaxX * i)) / NumOfRow + xAxisXoffset;

            //    DrawText(gl, valueX, screenX, ScreenMinY, fontsize);
            //}

            // Column
            int NumOfColumn = VerticalSpaceCount + 1;
            for (int i = 0; i <= VerticalSpaceCount; i++)
            {
                // Y 위쪽 방향으로 yAxisYOffset 만큼 올림
                var valueY = (MinY * (VerticalSpaceCount - i) + (MaxY * i)) / VerticalSpaceCount;
                var screenY = (ScreenMinY * (VerticalSpaceCount - i) + (ScreenMaxY * i)) / VerticalSpaceCount + yAxisYOffset;

                DrawText(gl, valueY, ScreenMinX, screenY, fontsize);
            }
            // 단위 도시
            gl.DrawText(ScreenMinX, (int)ScreenMaxY + yAxisYOffset * 3, 1.0f, 1.0f, 0.0f, FONT, fontsize, "(dBm)");
        }

        // Text를 도시함
        private void DrawText(OpenGL gl, double value, int x, int y, float size = 12.0f)
        {
            // Set String Format, 소수점 두째자리까지 표현
            var truncatedValue = Math.Round(value, 2);
            string strValue = string.Format("{0,5}", truncatedValue);

            // Draw Text
            gl.DrawText(x, y, 1.0f, 1.0f, 1.0f, FONT, size, strValue);
        }

        // Data를 도시함
        private void DrawData(OpenGL gl)
        {
            if (IsLoadSample)
            {
                MakeSampleData(ref component.data);
            }

            var DataLength = component.data.Length;

            // Draw Line from points
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Color(1.0f, 1.0f, 0.0f);
            for ( int i = 0; i < DataLength; i++ )
            {
                float currentX = (float)((double)i / DataLength * (CurrentControlWidth - PaddingHorizontal * 2.0)) + PaddingHorizontal;
                float currentY = (float)((component.data[i] - MinY) / (MaxY - MinY) * (CurrentControlHeight - PaddingVertical * 2)) + PaddingVertical;

                // Set Vertex from current x, y
                gl.Vertex(currentX, currentY, 0);
            }
            gl.End();
        }

        private void MakeSampleData(ref double[] data)
        {
            // Generate Sample Data
            Random random = new Random();
            const int MaxSample = SpecturmComponent.TotalDataLength;

            // Draw Random (아무 신호가 없는 잡음을 표현)
            for (int i = 0; i < MaxSample; i++)
            {
                var value = 0;
                
                if ( i < MaxSample / 6 || i > 5 * MaxSample / 6)
                {
                     value = random.Next(-87, -85);
                }

                else
                {
                    value = random.Next(-15, -9);
                }

                data[i] = value;
            }
        }
    }
}
