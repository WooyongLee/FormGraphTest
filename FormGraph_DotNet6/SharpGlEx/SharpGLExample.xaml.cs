using System.Windows.Controls;

using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Assets;
using SharpGL.Version;
using SharpGL.WPF;

namespace FormGraph_DotNet6
{
    /// <summary>
    /// SharpGLExample.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SharpGLExample : UserControl
    {
        public SharpGLExample()
        {
            InitializeComponent();

            this.openGLControl.OpenGLDraw += OpenGLControl_OpenGLDraw;
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
            gl.Ortho(-1.0, 1.0, -1.0, 1.0, -1.0, 1.0);

            // Set up the modelview matrix
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            // Set the color to yellow
            gl.Color(1.0f, 1.0f, 0.0f);

            // Draw the constellation chart
            gl.Begin(OpenGL.GL_LINES);
            gl.Vertex(-0.5f, 0.0f, 0.0f);
            gl.Vertex(0.5f, 0.0f, 0.0f);
            gl.Vertex(0.0f, -0.5f, 0.0f);
            gl.Vertex(0.0f, 0.5f, 0.0f);
            gl.End();
        }
    }
}
