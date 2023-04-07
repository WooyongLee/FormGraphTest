using SharpGL.WPF;
using SharpGL;
using System.Windows.Controls;
using System;
using System.Windows;
using System.Threading;
using System.Drawing;
using SharpGL.SceneGraph;
using SharpGL.Enumerations;

namespace GLGraphLib_DotNet6
{
    /// <summary>
    /// SpectrumChart.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SpectrumChart : UserControl
    {
        // GL Drawing 갱신 속도를 측정하기 위한 타이머 및 초
        Timer specCheckTimer;
        int second = 0;

        // OpenGL Control Rendering 반복을 체크하는 Iterator 변수
        int prevIter = 0;
        int iter = 0;

        // x,y Axis 의 최대, 최소 값
        double MinX = -2.0;
        double MaxX = 2.0;

        double MinY = -100.0;
        double MaxY = 0.0;
        
        #region const
        // Marker Triangle x,y size Property
        const int markerTriangleX = 5;
        const int markerTriangleY = 8;
        const int markerOffsetY = 3;
        #endregion

        // Marker의 드래깅 플래그 및 드래깅 x 위치
        bool isDragOn = false;

        SpecturmComponent component;

        // Specrum Screen 상에서의 x-y Pair Dictionary
        ScreenPositions screenPositions;

        Marker marker;

        // Color 설정
        RGBcolor backgroundColor = new(Color.Black);
        RGBcolor axisColor = new(Color.White);
        RGBcolor spectrumColor = new(Color.Yellow);
        RGBcolor textColor = new(Color.White);
        RGBcolor markerColor = new(Color.Red);

        ESpectrumChartMode mode;

        public SpectrumChart()
        {
            InitializeComponent();

            component = new SpecturmComponent();

            screenPositions = new ScreenPositions(SpecturmComponent.TotalDataLength);

            marker = new Marker();

            specCheckTimer = new Timer(TimerCallBack);
            specCheckTimer.Change(5000, 1000); // 1초 1회

            MakeSampleData(ref component.data);

            this.SizeChanged += SpectrumChart_SizeChanged;
            this.openGLControl.OpenGLDraw += OpenGLControl_OpenGLDraw;
            this.openGLControl.Resized += OpenGLControl_Resized;

            InitProperty();
        }

        private void TimerCallBack(object? state)
        {
            Console.WriteLine(string.Format("{0} seconds, iter = {1}, gap = {2}", ++second, iter, iter - prevIter));
            prevIter = iter;
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
            // Min, Max 값 적용
            SetMinMaxXY();

            OpenGL gl = openGLControl.OpenGL;

            // Set the clear color and clear the color buffer and depth buffer
            gl.ClearColor(backgroundColor.R, backgroundColor.G, backgroundColor.B, 0.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            // Set up the projection matrix (시점 좌표를 설정)
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();

            // 투상 좌표를 정의 right = width, top = height, near = -1, far = 1
            gl.Ortho(0, CurrentControlWidth, 0, CurrentControlHeight, -1, 1);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();

            // Set the color to black
            gl.Color(backgroundColor.R, backgroundColor.G, backgroundColor.B);

            gl.PushMatrix();

            // Text 부 도시
            DrawAxisLabels(gl);

            // Lines, Point 등의 Object 도시
            DrawSpectrumAxis(gl);

            // mode에 따른 Bounding box, line 등 도시
            DrawBoundingObject(gl);

            DrawData(gl);

            DrawMarker(gl);

            gl.PopMatrix();

            // Flush OpenGL
            gl.Flush();

            // GL Control 내에서 Draw 할 때 마다 iteration 증가
            iter++;

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
                    var x = PaddingHorizontal + sizeX * col;
                    var y = PaddingVertical + sizeY * row;

                    // Draw the current square
                    gl.Begin(OpenGL.GL_LINE_LOOP);
                    gl.Color(axisColor.R, axisColor.G, axisColor.B);
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
            // GLUtil.FONT size 를 1회 이상 변경해야 Text가 도시하는 현상이 있음 (해당 구문 제거 시, 축 Text 도시하지 않음)
            int fontsize = GetFontSize();

            // define text margin
            int MarginX = (int)(PaddingHorizontal * (4.0 / 5.0));
            int MarginY = (int)(PaddingVertical * (4.0 / 7.0));
            //int TopOffset = (int)(MarginY * (2.0 / 3.0));
            //int LeftfOffset = (int)(MarginX * (2.0 / 3.0));
            int xOffset = 5;
            int xAxisXoffset = 10;
            int yAxisYOffset = 10;

            var ScreenMinX = PaddingHorizontal - MarginX - xOffset;
            var ScreenMaxX = (int)CurrentControlWidth - PaddingHorizontal * 2 + xOffset;
            var ScreenMinY = PaddingVertical - MarginY;
            var ScreenMaxY = (int)CurrentControlHeight - PaddingVertical * 2 + MarginY;

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
            for (int i = 0; i <= VerticalSpaceCount; i++)
            {
                // Y 위쪽 방향으로 yAxisYOffset 만큼 올림
                var valueY = (MinY * (VerticalSpaceCount - i) + (MaxY * i)) / VerticalSpaceCount;
                var screenY = (ScreenMinY * (VerticalSpaceCount - i) + (ScreenMaxY * i)) / VerticalSpaceCount + yAxisYOffset;

                GLUtil.DrawFormattedText(gl, valueY, (int)ScreenMinX, (int)screenY, textColor, 2, fontsize);
            }

            // 단위 도시
            gl.DrawText((int)ScreenMinX, (int)ScreenMaxY + yAxisYOffset * 3, spectrumColor.R, spectrumColor.G, spectrumColor.B, GLUtil.FONT, fontsize, "(dBm)");
        }

        // Bounding Box를 도시함
        private void DrawBoundingObject(OpenGL gl)
        {
            // 디버깅 쉽게 하기 위한 모드 변경
            mode = ESpectrumChartMode.DefaultSpecturm;

            // 반투명 Bounding Box를 Center 기준으로 Occupied BW 만큼 도시함
            if (mode == ESpectrumChartMode.ChannelPower)
            {
                // To Do :: Occupied bandwidth에 따라 Center로 부터 양 옆으로 주파수 간격 설정 필요
                // 일단 Screen Width와 관계 없이 임의로 설정
                int xBandWidth = 300;

                // Bounding Box의 Min/Max 설정
                float boundingBoxMinX = (float)(CurrentControlWidth / 2 - xBandWidth / 2);
                float boundingBoxMaxX = (float)(CurrentControlWidth / 2  + xBandWidth / 2);
                float boundingBoxMinY = PaddingVertical + 3;
                float boundingBoxMaxY = (float)(CurrentControlHeight - PaddingVertical - 3);

                // Set Blending for Transparency
                gl.Enable(OpenGL.GL_BLEND);
                gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA); // 알파 블렌딩 설정

                // Draw Bounding Box.
                gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
                gl.Begin(BeginMode.Quads);
                gl.Color(0.0f, 0.0f, 1.0f, 0.3f);
                gl.Vertex(boundingBoxMinX, boundingBoxMinY, 0.0f);
                gl.Vertex(boundingBoxMaxX, boundingBoxMinY, 0.0f);
                gl.Vertex(boundingBoxMaxX, boundingBoxMaxY, 0.0f);
                gl.Vertex(boundingBoxMinX, boundingBoxMaxY, 0.0f);
                gl.End();

                // Fill Bounding Box.
                gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
                gl.Begin(BeginMode.Quads);
                gl.Color(0.0f, 0.0f, 1.0f, 0.3f);
                gl.Vertex(boundingBoxMinX, boundingBoxMinY, 0.0f);
                gl.Vertex(boundingBoxMaxX, boundingBoxMinY, 0.0f);
                gl.Vertex(boundingBoxMaxX, boundingBoxMaxY, 0.0f);
                gl.Vertex(boundingBoxMinX, boundingBoxMaxY, 0.0f);
                gl.End();

                // Disable Blending
                gl.Disable(OpenGL.GL_BLEND);
            }

            // 전체를 다섯 칸으로 나누고(Carrier Box), 가장 center 칸 제외하고 absolute limit line과 relative limit line을 도시함
            else if ( mode == ESpectrumChartMode.ACLR)
            {
                // Set Blending for Transparency
                gl.Enable(OpenGL.GL_BLEND);
                gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

                // 일단 임의의 Y 위치로 Limite Line 들을 지정함
                var relLimitLineY = 180.0f + PaddingVertical;
                var absLimitLineY = 130.0f + PaddingVertical;

                RGBcolor boundingBoxColor = new(Color.ForestGreen);
                RGBcolor relLimitLineColor = new(Color.SkyBlue);
                RGBcolor absLimitLineColor = new(Color.Purple);
                float BoundingOffset = 2;
                float boundingBoxMinY = PaddingVertical + BoundingOffset;
                float boundingBoxMaxY = (float)(CurrentControlHeight - PaddingVertical - BoundingOffset);

                // 5개의 영역을 Overlay 하도록 구성
                int totalBoxSize = 5;
                for ( int i = 0; i < totalBoxSize; i++)
                {
                    float boundingBoxMinX = (float)(PaddingHorizontal + (CurrentControlWidth - PaddingHorizontal * 2) * i / totalBoxSize) + BoundingOffset;
                    float boundingBoxMaxX = (float)(PaddingHorizontal + (CurrentControlWidth - PaddingHorizontal * 2) * (i + 1) / totalBoxSize) - BoundingOffset;

                    // Draw Bounding Box
                    GLUtil.DrawThickBoundingBox(gl, boundingBoxMinX, boundingBoxMinY, boundingBoxMaxX, boundingBoxMaxY, boundingBoxColor, 3);
                   
                    // Draw Fill Box
                    gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
                    gl.Begin(BeginMode.Quads);
                    // 임의의 칸에 서로 다른 색상 부여
                    if (i % 2 == 0) gl.Color(1.0f, 0.0f, 0.0f, 0.25f);
                    else gl.Color(0.0f, 1.0f, 0.0f, 0.25f);
                    gl.Vertex(boundingBoxMinX, boundingBoxMinY, 0.0f);
                    gl.Vertex(boundingBoxMaxX, boundingBoxMinY, 0.0f);
                    gl.Vertex(boundingBoxMaxX, boundingBoxMaxY - 100, 0.0f);
                    gl.Vertex(boundingBoxMinX, boundingBoxMaxY - 100, 0.0f);
                    gl.End();

                    // Center 에서는 Limit Line을 그리지 않음
                    if (i == (totalBoxSize - 1) / 2.0)
                    {
                        continue;
                    }

                    // Draw Relative Limit Line
                    GLUtil.DrawThickLine(gl, boundingBoxMinX, relLimitLineY, boundingBoxMaxX, relLimitLineY, relLimitLineColor, 3);
                    
                    // Draw Absolute Limit Line
                    GLUtil.DrawThickLine(gl, boundingBoxMinX, absLimitLineY, boundingBoxMaxX, absLimitLineY, absLimitLineColor, 3);
                }

                // Disable Properties
                gl.Disable(OpenGL.GL_BLEND);
            }

            else if ( mode == ESpectrumChartMode.SEM)
            {

            }
        }

        // Data를 도시함
        private void DrawData(OpenGL gl)
        {
            if (IsLoadSample)
            {
                MakeSampleData(ref component.data);
            }

            var DataLength = component.data.Length;

            #region Use Line Strip
            // Draw Line from points
            gl.Begin(OpenGL.GL_LINE_STRIP);
            gl.Color(spectrumColor.R, spectrumColor.G, spectrumColor.B);
            for (int i = 0; i < DataLength; i++)
            {
                float currentX = (float)((double)i / DataLength * (CurrentControlWidth - PaddingHorizontal * 2.0)) + PaddingHorizontal;
                float currentY = (float)((component.data[i] - MinY) / (MaxY - MinY) * (CurrentControlHeight - PaddingVertical * 2)) + PaddingVertical;

                screenPositions.Set(currentX, currentY, i);
                marker.RenewPoint(currentX, currentY);

                // Set Vertex from current x, y
                gl.Vertex(currentX, currentY, 0);
            }
            gl.End();
            #endregion
        }

        // Marker를 도시함
        private void DrawMarker(OpenGL gl)
        {
            // Marker를 생성
            // 일단 Marker 하나만 그려보도록 함
            var markerPos = marker.GetPoints(0);

            if (markerPos != null)
            {
                // Draw Marker Line
                gl.Begin(OpenGL.GL_LINE_STRIP);
                gl.Color(markerColor.R, markerColor.G, markerColor.B);

                gl.Vertex(markerPos.X, CurrentControlHeight - PaddingVertical);
                gl.Vertex(markerPos.X, PaddingVertical);
                gl.End();

                // Draw Marker Triangle
                gl.Begin(OpenGL.GL_TRIANGLES);
                gl.Color(markerColor.R, markerColor.G, markerColor.B);

                gl.Vertex(markerPos.X, markerPos.Y + markerOffsetY);
                gl.Vertex(markerPos.X + markerTriangleX , markerPos.Y + markerTriangleY + markerOffsetY);
                gl.Vertex(markerPos.X - markerTriangleX, markerPos.Y + markerTriangleY + markerOffsetY);
                gl.End();

                int fontsize = GetFontSize();

                // Draw Marker Text
                gl.DrawText((int)(markerPos.X + markerTriangleY * 2), (int)(markerPos.Y + markerTriangleY + markerOffsetY), 
                    markerColor.R, markerColor.G, markerColor.B, GLUtil.FONT, fontsize, "M 1");
            } // end if (markerPos != null)
        }

        // Sample Data를 생성함
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
                     value = random.Next(-87, -84);
                }

                else
                {
                    value = random.Next(-20, -9);
                }

                data[i] = value;
            }
        }

        bool isMarkerExist = false;

        #region Mouse Event At Control
        // OpenGL Control Mouse Down Event Handler
        private void openGLControl_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // 화면 상에 클릭한 지점 구해오기
            var clickedPoint = e.GetPosition(null);
            var screenX = (float)clickedPoint.X;

            // 클릭한 지점에 대한 예외처리
            if (screenX < 0 || screenX > CurrentControlWidth) return;

            // Marker 있을 경우, 클릭한 근처에 가까운 Marker를 탐색
            if (isMarkerExist)
            {
                int totalMarkerCount = marker.GetMarkerTotalCount();

                // 설정된 모든 Marker를 대상으로 Iteration 수행
                for ( int i = 0; i < totalMarkerCount; i++)
                {
                    var markerPt = marker.GetPoints(i);

                    // Marker의 HitTest 처리 부분
                    if (markerPt != null)
                    {
                        // Marker Point x와 클릭한 위치의 차가 일정 범위 내일 때 Dragging을 감지
                        var d = Math.Abs(screenX - markerPt.X);

                        if (d < 5)
                        {
                            // To Do :: Marker Type Fixed인 경우에는 Dragging 처리 하지 않도록 설정
                            
                            isDragOn = true;
                            marker.SelectedMarkerIndex = i;
                        }
                    }
                } // end for ( int i = 0; i < totalMarkerCount; i++)
            } // end if (isMarkerExist)

            // Marker 없으면 생성
            else
            {
                isMarkerExist = true;

                // 화면에서 클릭한 가장 가까운 Spectrum 데이터 X 위치를 찾아 Y 데이터를 가져옴
                var valueY = screenPositions.GetClosestData(screenX, ref screenX);

                marker.AddPoint(screenX, valueY);
            }
        }

        // OpenGL Control Mouse Move Event Handler
        private void openGLControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // 마커 선택하여 드래깅 시에 처리
            if (isDragOn)
            {
                var mousePoint = e.GetPosition(null);
                var screenX = (float)mousePoint.X; // 현재 따라오는 마우스 x 위치 
                if (screenX < 0 || screenX > CurrentControlWidth) return;

                // 현재 x 위치에서 가장 가까운 y위치 설정
                var valueY = screenPositions.GetClosestData(screenX, ref screenX);

                // Marker Y 위치도 Renew 한 뒤, 선택한 Marker에 대한 위치 확정
                marker.RenewSelectedMarker(screenX, valueY);
            }
        }

        // OpenGL Control Mouse Up Event Handler
        private void openGLControl_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (isDragOn)
            {
                isDragOn = false;

                var mousePoint = e.GetPosition(null);
                var screenX = (float)mousePoint.X; // 현재 놓은 마우스 x 위치 
                if (screenX < 0 || screenX > CurrentControlWidth) return;

                // 현재 x 위치에서 가장 가까운 y위치 설정
                var valueY = screenPositions.GetClosestData(screenX, ref screenX);

                // Marker Y 위치도 Renew 한 뒤, 선택한 Marker에 대한 위치 확정
                marker.RenewSelectedMarker(screenX, valueY);
            }
        }
        #endregion

        // FontSize를 반환함
        private int GetFontSize()
        {
            // 시작해서 GLUtil.FONT를 1회 바꿔주는 이유
            // -> 아직까지 잘 모르겠음.. 근데 한번 안바꿔주면 Text 도시에 이상이 생김, 바꿔주니까 정상적으로 도시함
            int fontsize = 10;
            if (iter > 1) fontsize = 12;
            return fontsize;
        }

        // 축에 표현할 최소/최대 x, y를 Spectrum Parameter(DependencyObject)에서 설정한 값으로 변환해서 적용함
        private void SetMinMaxXY()
        {
            this.MinX = this.CenterFrequency - this.Span / 2.0;
            this.MaxX = this.CenterFrequency + this.Span / 2.0;

            this.MinY = this.RefLevel - this.DivScale * this.VerticalSpaceCount;
            this.MaxY = this.RefLevel;
        }
    }
}
