using SharpGL;
using SharpGL.SceneGraph;
using System.Windows;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace GLGraphLib_DotNet6
{
    /// <summary>
    /// MouseAbleChart.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MouseAbleChart : UserControl
    {
        private bool isMouseMoveChecked = false;
        private bool isMaximizeMode = false;
        private bool isDrawing = false;
        private bool isMouseWheelEnabled = false;
        private System.Windows.Point startPoint;
        private System.Windows.Point endPoint;
        private double[] originalOrtho2D = new double[4] { 0, 10, 0, 10 };
        private Point mouseDownPosition;
        private bool isPanning = false;
        private double zoomLevel = 1;
        public MouseAbleChart()
        {
            InitializeComponent();
        }

        private void openGLControl_OpenGLDraw(object sender, SharpGL.WPF.OpenGLRoutedEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            // Clear the screen and the depth buffer
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Set the line color to black
            gl.Color(0.0f, 0.0f, 0.0f, 0.0f);

            // Set the viewport
            gl.Viewport(0, 0, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);

            // Set the orthographic projection
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Ortho2D(originalOrtho2D[0], originalOrtho2D[1], originalOrtho2D[2], originalOrtho2D[3]);

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            // Draw the vertical lines
            for (int i = 0; i <= 10; i++)
            {
                gl.Begin(OpenGL.GL_LINES);
                gl.Vertex(i, 0);
                gl.Vertex(i, 10);
                gl.End();
            }

            // Draw the horizontal lines
            for (int j = 0; j <= 10; j++)
            {
                gl.Begin(OpenGL.GL_LINES);
                gl.Vertex(0, j);
                gl.Vertex(10, j);
                gl.End();
            }

            // Calculate the zoom level based on the current orthographic projection
            double zoomX = 10 / (originalOrtho2D[1] - originalOrtho2D[0]);
            double zoomY = 10 / (originalOrtho2D[3] - originalOrtho2D[2]);
            zoomLevel = Math.Min(zoomX, zoomY);

            //gl.DrawText((int)(col * (openGLControl.ActualWidth / 10) + 10),
            //    (int)(openGLControl.ActualHeight - row * (openGLControl.ActualHeight / 10) - 20),
            //    0, 0, 0, "Arial", (int)(12 * zoomLevel), text);
            // Draw numbers
            
            gl.Color(0.0f, 0.0f, 0.0f, 1.0f);
            int number = 1;
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    double cellX = col + 0.5;
                    double cellY = row + 0.5;
                    double cellXNormalized = cellX * zoomLevel;
                    double cellYNormalized = cellY * zoomLevel;

                    double minX = Math.Min(startPoint.X, endPoint.X) / openGLControl.ActualWidth * 10;
                    double maxX = Math.Max(startPoint.X, endPoint.X) / openGLControl.ActualWidth * 10;
                    double minY = (openGLControl.ActualHeight - Math.Max(startPoint.Y, endPoint.Y)) / openGLControl.ActualHeight * 10;
                    double maxY = (openGLControl.ActualHeight - Math.Min(startPoint.Y, endPoint.Y)) / openGLControl.ActualHeight * 10;

                    if (cellXNormalized >= minX && cellXNormalized <= maxX && cellYNormalized >= minY && cellYNormalized <= maxY)
                    {
                        string text = number.ToString();
                        gl.PushMatrix();
                        gl.Translate(cellX, cellY, 0);
                        gl.Scale(1 / zoomLevel, 1 / zoomLevel, 1);
                        // gl.Scale(1, -1, 1); // Invert Y axis for correct text orientation
                        gl.DrawText((int)(col * (openGLControl.ActualWidth / 10) + 10),
                (int)(openGLControl.ActualHeight - row * (openGLControl.ActualHeight / 10) - 20), 0, 0, 0, "Arial", (int)(12 * zoomLevel), text);
                        gl.PopMatrix();
                    }

                    number++;
                }
            }

            if (isDrawing)
            {
                DrawRectangle(gl, startPoint, endPoint);
            }

            // Flush the OpenGL commands
            gl.Flush();
        }

        private void openGLControl_OpenGLInitialized(object sender, SharpGL.WPF.OpenGLRoutedEventArgs args)
        {
            // Initialize the OpenGL context
            OpenGL gl = args.OpenGL;

            // Set the clear color to white
            gl.ClearColor(1.0f, 1.0f, 1.0f, 1.0f);
        }

        private void openGLControl_Resized(object sender, SharpGL.WPF.OpenGLRoutedEventArgs args)
        {
            // Get the OpenGL object
            OpenGL gl = args.OpenGL;

            // Set the viewport
            gl.Viewport(0, 0, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void MaximizeArea_Click(object sender, RoutedEventArgs e)
        {
            // init start/end point
            //startPoint = new Point();
            //endPoint = new Point();

            isMaximizeMode = true;
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            isMaximizeMode = false;
            RestoreViewport();
        }

        private void MouseWheel_Checked(object sender, RoutedEventArgs e)
        {
            isMouseWheelEnabled = true;
        }

        private void MouseWheel_Unchecked(object sender, RoutedEventArgs e)
        {
            isMouseWheelEnabled = false;
        }

        private void openGLControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            openGLControl.ContextMenu.IsOpen = true;
        }

        private void openGLControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isMaximizeMode)
            {
                startPoint = e.GetPosition(openGLControl);
                isDrawing = true;
            }

            if (isMouseMoveChecked && zoomLevel > 1)
            {
                mouseDownPosition = e.GetPosition(openGLControl);
                isPanning = true;
            }
        }

        private void openGLControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (isMaximizeMode && isDrawing)
            {
                endPoint = e.GetPosition(openGLControl);
                isDrawing = false;
                MaximizeViewport(startPoint, endPoint);
                isMaximizeMode = false;  // Prevent further area selection until "Maximize Area" is selected again
            }

            if (isPanning)
            {
                isPanning = false;
            }
        }

        private void openGLControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMaximizeMode && isDrawing)
            {
                endPoint = e.GetPosition(openGLControl);
                openGLControl.InvalidateVisual();
            }

            if (isPanning)
            {
                Point currentMousePosition = e.GetPosition(openGLControl);
                double offsetX = (currentMousePosition.X - mouseDownPosition.X) / openGLControl.ActualWidth * (originalOrtho2D[1] - originalOrtho2D[0]);
                double offsetY = (mouseDownPosition.Y - currentMousePosition.Y) / openGLControl.ActualHeight * (originalOrtho2D[3] - originalOrtho2D[2]);
                originalOrtho2D[0] -= offsetX;
                originalOrtho2D[1] -= offsetX;
                originalOrtho2D[2] -= offsetY;
                originalOrtho2D[3] -= offsetY;
                mouseDownPosition = currentMousePosition;
                openGLControl.InvalidateVisual();
            }
        }

        private void openGLControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (isMouseWheelEnabled)
            {
                Point mousePos = e.GetPosition(openGLControl);
                double delta = e.Delta > 0 ? 0.9 : 1.1; // Zoom factor

                double mouseX = mousePos.X / openGLControl.ActualWidth * 10;
                double mouseY = (openGLControl.ActualHeight - mousePos.Y) / openGLControl.ActualHeight * 10;

                double width = originalOrtho2D[1] - originalOrtho2D[0];
                double height = originalOrtho2D[3] - originalOrtho2D[2];

                double newWidth = width * delta;
                double newHeight = height * delta;

                double xOffset = mouseX - (mouseX - originalOrtho2D[0]) * delta;
                double yOffset = mouseY - (mouseY - originalOrtho2D[2]) * delta;

                originalOrtho2D[0] = xOffset;
                originalOrtho2D[1] = xOffset + newWidth;
                originalOrtho2D[2] = yOffset;
                originalOrtho2D[3] = yOffset + newHeight;

                openGLControl.InvalidateVisual();

            }
        }

        private void DrawRectangle(OpenGL gl, Point start, Point end)
        {
            gl.Color(0.0f, 0.0f, 1.0f, 1.0f);
            gl.Begin(OpenGL.GL_LINE_LOOP);
            gl.Vertex(start.X / openGLControl.ActualWidth * 10, (openGLControl.ActualHeight - start.Y) / openGLControl.ActualHeight * 10);
            gl.Vertex(end.X / openGLControl.ActualWidth * 10, (openGLControl.ActualHeight - start.Y) / openGLControl.ActualHeight * 10);
            gl.Vertex(end.X / openGLControl.ActualWidth * 10, (openGLControl.ActualHeight - end.Y) / openGLControl.ActualHeight * 10);
            gl.Vertex(start.X / openGLControl.ActualWidth * 10, (openGLControl.ActualHeight - end.Y) / openGLControl.ActualHeight * 10);
            gl.End();
        }

        private void MaximizeViewport(Point start, Point end)
        {
            double xMin = Math.Min(start.X, end.X) / openGLControl.ActualWidth * 10;
            double xMax = Math.Max(start.X, end.X) / openGLControl.ActualWidth * 10;
            double yMin = (openGLControl.ActualHeight - Math.Max(start.Y, end.Y)) / openGLControl.ActualHeight * 10;
            double yMax = (openGLControl.ActualHeight - Math.Min(start.Y, end.Y)) / openGLControl.ActualHeight * 10;

            originalOrtho2D[0] = xMin;
            originalOrtho2D[1] = xMax;
            originalOrtho2D[2] = yMin;
            originalOrtho2D[3] = yMax;

            openGLControl.InvalidateVisual();
        }

        private void RestoreViewport()
        {
            originalOrtho2D[0] = 0;
            originalOrtho2D[1] = 10;
            originalOrtho2D[2] = 0;
            originalOrtho2D[3] = 10;

            openGLControl.InvalidateVisual();
        }

        private void MouseMove_Checked(object sender, RoutedEventArgs e)
        {
            isMouseMoveChecked = true;
        }

        private void MouseMove_Unchecked(object sender, RoutedEventArgs e)
        {
            isMouseMoveChecked = false;
        }
    }
}
