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

            DrawAxis(gl);

            DrawData(gl);


            gl.PopMatrix();

            // Flush OpenGL
            gl.Flush();
        }

        private void DrawAxis(OpenGL gl)
        {
            // 중심점 도시
            var cx = CurrentControlWidth / 2 + PaddingHorizontal;
            var cy = CurrentControlHeight / 2 + PaddingVertical;
            var rd = CurrentControlHeight / 2; // 반지름 
            var cr = rd;

            Pen gridLineColor = new Pen(Color.White, 0.1f);
            //gl.DrawArc(gridLineColor, cx - cr, cy - cr, 2 * cr, 2 * cr, 0, 360);
            ////float h = (pbMainPaddingY + 2 * cr) / 2;
            //var ax = cy;
            //gl.DrawLine(gridLineColor, cx - cr, ax, cx + cr, ax);

            //if (!initAxis)
            //{
            //    initAxis = true;

            //    xMinCircle = cx - cr;
            //    xMaxCircle = cx + cr;
            //    yMinCircle = cy - cr;
            //    yMaxCircle = cy + cr;

            //    Console.WriteLine("circle x min, max = " + xMinCircle + ", " + xMaxCircle);
            //    Console.WriteLine("circle y min, max = " + yMinCircle + ", " + yMaxCircle);
            //}

            var textColor = Brushes.White;
            int fontSize = 10;
        }

        private void DrawData(OpenGL gl)
        {

        }

        override public void InitProperty()
        {

        }

        public override void UpdateTheme()
        {

        }
    }
}
