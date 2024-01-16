using SharpGL;
using SharpGL.Enumerations;
using SharpGL.WPF;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Controls;

namespace GLGraphLib
{
    /// <summary>
    /// SpectrumChart.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SpectrumChart : ChartUserControlBase
    {
        // GL Drawing 갱신 속도를 측정하기 위한 타이머 및 초
        Timer specCheckTimer;
        int second = 0;

        // OpenGL Control Rendering 반복을 체크하는 Iterator 변수
        int prevIter = 0;
        int iter = 0;

        #region const
        // Marker Triangle x,y size Property
        const int markerTriangleX = 5;
        const int markerTriangleY = 8;
        const int markerOffsetY = 3;
        #endregion

        // Marker의 드래깅 플래그 및 드래깅 x 위치
        bool isDragOn = false;

        // Specrum Screen 상에서의 x-y Pair Dictionary
        ScreenPositions screenPositions;

        // Trace 객체
        Trace trace;

        // Marker 객체
        Marker marker;

        // Color 설정
        RGBcolor markerColor = new RGBcolor(Color.Red);
        RGBcolor markerHighlightColor = new RGBcolor(Color.Orange);
        RGBcolor[] spectrumColors;

        ESpectrumChartMode mode;

        #region public
        public bool IsSetMinMax = false;

        #endregion


        public SpectrumChart()
        {
            InitializeComponent();

            // 기본 Total Length 설정
            TotalDataLength = 1001;
            screenPositions = new ScreenPositions(TotalDataLength);

            SpectrumData = new double[Trace.MaxTraceCount, TotalDataLength];

            marker = new Marker();
            trace = new Trace(TotalDataLength);

            specCheckTimer = new Timer(TimerCallBack);
            specCheckTimer.Change(4999, 999); // 1초 1회

            this.SizeChanged += SpectrumChart_SizeChanged;
            this.openGLControl.OpenGLDraw += OpenGLControl_OpenGLDraw;
            this.openGLControl.Resized += OpenGLControl_Resized;

            spectrumColors = new RGBcolor[4] { spectrumColor1, spectrumColor2, spectrumColor3, spectrumColor4 };

            // Dependency Property Set Owner
            IsLoadSampleProperty.AddOwner(typeof(SpectrumChart));

            InitProperty();

            for (int i = 0; i < Trace.MaxTraceCount; i++)
            {
                trace.SetData(SpectrumData, i, TotalDataLength);
            }
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
            // 초기 값 적용
            if (iter == 0)
            {
                 // InitProperty();
            }

            // Min, Max 값 적용
            if (IsSetMinMax)
            {
                SetMinMaxXY();
                IsSetMinMax = false;
            }

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
            if (iter < 200000) iter++;
        }

        // 스펙트럼의 축을 도시함
        private void DrawSpectrumAxis(OpenGL gl)
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
        }

        // 스펙트럼 축 Label을 도시함
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
            int xAxisXoffset = 10;
            int yAxisYOffset = 10;

            var ScreenMinX = PaddingHorizontal - MarginX - xOffset;
            var ScreenMaxX = (int)CurrentControlWidth - PaddingHorizontal * 2 + xOffset;
            var ScreenMinY = PaddingVertical - MarginY;
            var ScreenMaxY = (int)CurrentControlHeight - PaddingVertical * 2 + MarginY;

            // Row
            if (IsShowXaxisText)
            {
                // Start x Position
                var valueX = CenterFrequency - Span / 2;
                var screenX = ScreenMinX + xAxisXoffset * 3;

                GLUtil.DrawFormattedText(gl, valueX, (int)screenX, (int)ScreenMinY, AxisColor, 5, fontsizeX);

                // Center x Position
                valueX = CenterFrequency;
                screenX = (ScreenMinX + ScreenMaxX) / 2 + xAxisXoffset * 2;

                GLUtil.DrawFormattedText(gl, valueX, (int)screenX, (int)ScreenMinY, AxisColor, 5, fontsizeX);

                // End x Position
                valueX = CenterFrequency + Span / 2;
                screenX = ScreenMaxX - xAxisXoffset * 2;

                GLUtil.DrawFormattedText(gl, valueX, (int)screenX, (int)ScreenMinY, AxisColor, 5, fontsizeX);

                // 단위 도시 (x)
                gl.DrawText((int)ScreenMaxX + xAxisXoffset * 3, (int)ScreenMinY, spectrumColor1.R, spectrumColor1.G, spectrumColor1.B, GLUtil.FONT, fontsizeX, "(MHz)");
            } // end if (IsShowXaxisText)

            // Column
            for (int i = 0; i <= NumOfColumn; i++)
            {
                // Y 위쪽 방향으로 yAxisYOffset 만큼 올림
                var valueY = (MinY * (NumOfColumn - i) + (MaxY * i)) / NumOfColumn;
                var screenY = (ScreenMinY * (NumOfColumn - i) + (ScreenMaxY * i)) / NumOfColumn + yAxisYOffset;

                GLUtil.DrawFormattedText(gl, valueY, (int)ScreenMinX, (int)screenY, AxisColor, 2, fontsize);
            }

            // 단위 도시 (y)
            gl.DrawText((int)ScreenMinX, (int)ScreenMaxY + yAxisYOffset * 3, spectrumColor1.R, spectrumColor1.G, spectrumColor1.B, GLUtil.FONT, fontsize, "(dBm)");
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
                float boundingBoxMaxX = (float)(CurrentControlWidth / 2 + xBandWidth / 2);
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
            else if (mode == ESpectrumChartMode.ACLR)
            {
                // Set Blending for Transparency
                gl.Enable(OpenGL.GL_BLEND);
                gl.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);

                // 일단 임의의 Y 위치로 Limite Line 들을 지정함
                var relLimitLineY = 180.0f + PaddingVertical;
                var absLimitLineY = 130.0f + PaddingVertical;

                RGBcolor boundingBoxColor = new RGBcolor(Color.ForestGreen);
                RGBcolor relLimitLineColor = new RGBcolor(Color.SkyBlue);
                RGBcolor absLimitLineColor = new RGBcolor(Color.Purple);
                float BoundingOffset = 2;
                float boundingBoxMinY = PaddingVertical + BoundingOffset;
                float boundingBoxMaxY = (float)(CurrentControlHeight - PaddingVertical - BoundingOffset);

                // 5개의 영역을 Overlay 하도록 구성
                int totalBoxSize = 5;
                for (int i = 0; i < totalBoxSize; i++)
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

            else if (mode == ESpectrumChartMode.SEM)
            {

            }
        }

        // Data를 도시함
        private void DrawData(OpenGL gl)
        {
            for (int traceIndex = 0; traceIndex < Trace.MaxTraceCount; traceIndex++)
            {
                // Spectrum Visible 할 때만 가능
                if (!IsVisibleSpectrum[traceIndex])
                {
                    continue;
                }

                this.MakeTrace(traceIndex);

                // 해당 Index에 대한 Trace 데이터
                var traceData = trace.GetData(traceIndex);

                if (traceData != null)
                {
                    if (IsLoadSample)
                    {
                        trace.MakeSampleData(traceIndex, TotalDataLength);
                    }

                    #region Use Line Strip
                    var spectrumColor = spectrumColors[traceIndex];

                    // Draw Line from points
                    gl.Begin(OpenGL.GL_LINE_STRIP);
                    gl.Color(spectrumColor.R, spectrumColor.G, spectrumColor.B);
                    for (int i = 0; i < TotalDataLength; i++)
                    {
                        float currentX = SpectrumChartUtil.GetScreenX(i, TotalDataLength, CurrentControlWidth, PaddingHorizontal);
                        float currentY = SpectrumChartUtil.GetScreenY(traceData[i], MinY, MaxY, CurrentControlHeight, PaddingVertical);

                        screenPositions.Set(currentX, currentY, i);

                        // Renew All Markers
                        for (int markerIndex = 0; markerIndex < Marker.MaxMarkerCount; markerIndex++)
                        {
                            marker.RenewPoint(currentX, currentY, markerIndex);
                        }

                        // Set Vertex from current x, y
                        gl.Vertex(currentX, currentY, 0);
                    }
                    gl.End();
                    #endregion
                } // end if (traceData != null)
            } // end for (int traceIndex = 0; traceIndex < Trace.MaxTraceCount; traceIndex++) 
        }

        // Marker를 도시함
        private void DrawMarker(OpenGL gl)
        {
            // Marker를 생성
            // 일단 Marker 하나만 그려보도록 함
            for (int i = 0; i < Marker.MaxMarkerCount; i++)
            {
                var markerPos = marker.GetPoints(i);

                if (markerPos != null)
                {
                    // Highlight Marker의 색 구분을 위한 분기
                    var drawColor = markerColor;
                    if (i == marker.SelectedMarkerIndex)
                    {
                        drawColor = markerHighlightColor;
                    }

                    // Draw Marker Line
                    gl.Begin(OpenGL.GL_LINE_STRIP);
                    gl.Color(drawColor.R, drawColor.G, drawColor.B);

                    gl.Vertex(markerPos.X, CurrentControlHeight - PaddingVertical);
                    gl.Vertex(markerPos.X, PaddingVertical);
                    gl.End();

                    // Draw Marker Triangle
                    gl.Begin(OpenGL.GL_TRIANGLES);
                    gl.Color(drawColor.R, drawColor.G, drawColor.B);

                    gl.Vertex(markerPos.X, markerPos.Y + markerOffsetY);
                    gl.Vertex(markerPos.X + markerTriangleX, markerPos.Y + markerTriangleY + markerOffsetY);
                    gl.Vertex(markerPos.X - markerTriangleX, markerPos.Y + markerTriangleY + markerOffsetY);
                    gl.End();

                    int fontsize = SpectrumChartUtil.GetFontSize(iter);

                    // Draw Marker Text
                    string strMarkerText = string.Format("M {0}", i + 1);

                    // Fixed Mark
                    if (marker.IsFixed(i))
                    {
                        strMarkerText = "Fix " + strMarkerText;
                    }

                    // Delta Mark
                    if (marker.IsDelta(i))
                    {
                        // Additional Target Index Mark
                        var targetIndex = marker.GetDeltaTargetIndex(i) + 1;
                        if (targetIndex != null)
                        {
                            strMarkerText += string.Format(" | {0}", targetIndex);
                            fontsize = 12;
                        }
                    }

                    gl.DrawText((int)(markerPos.X + markerTriangleY * 2), (int)(markerPos.Y + markerTriangleY + markerOffsetY),
                            drawColor.R, drawColor.G, drawColor.B, GLUtil.FONT, fontsize, strMarkerText);
                } // end if (markerPos != null)
            } // end for (int i = 0; i < Marker.MaxMarkerCount; i++)
        }

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
            int totalMarkerPositionCount = marker.GetTotalPosCount();

            // 설정된 모든 Marker를 대상으로 Iteration 수행
            for (int i = 0; i < Marker.MaxMarkerCount; i++)
            {
                var markerPt = marker.GetPoints(i);

                // Marker의 HitTest 처리 부분
                if (markerPt != null)
                {
                    // Marker Point x와 클릭한 위치의 차가 일정 범위 내일 때 Dragging을 감지
                    var d = Math.Abs(screenX - markerPt.X);

                    if (d < 5)
                    {
                        // Get Dragging
                        isDragOn = true;

                        // Marker Type Fixed인 경우에는 Dragging 처리 하지 않도록 설정
                        if (marker.IsFixed(i))
                        {
                            isDragOn = false;
                        }
                        marker.SelectedMarkerIndex = i;
                    }
                } // end if (markerPt != null)
            } // end for ( int i = 0; i < totalMarkerCount; i++)
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
                // Release Drag 
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

        private void openGLControl_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            // MinMax 재설정 활성화
            IsSetMinMax = true;

            // Ctrl + Wheel -> Y축 Scale 변경
            //if (System.Windows.Forms.Control.ModifierKeys == System.Windows.Forms.Keys.Control)
            //{
            //    var yScale = 5;
            //    // Ref Level 값은 고정, Min Y 만 조절하도록
            //    if (MaxY == MinY) return;

            //    if (e.Delta > 0)
            //    {
            //        // Zoom in
            //        MinY -= yScale;
            //    }
            //    else
            //    {
            //        // Zoom out
            //        MinY += yScale;
            //    }

            //    if (MinY > MaxY) MaxY = MinY;
            //}

            //// X축 Scale 변경
            //else
            //{
            //    var xScale = 10;
            //    // Center 중심으로 양 쪽으로 펼쳐지도록
            //    if (e.Delta > 0)
            //    {
            //        // Zoom in
            //        MinX -= xScale;
            //        MaxX += xScale;
            //    }
            //    else
            //    {
            //        // Zoom out
            //        MinX += xScale;
            //        MaxX -= xScale;
            //    }

            //    if (MinX > MaxX) MinX = MaxX = CenterFrequency; 
            //}
        }
        #endregion

        // 축에 표현할 최소/최대 x, y를 Spectrum Parameter(DependencyObject)에서 설정한 값으로 변환해서 적용함
        private void SetMinMaxXY()
        {
            //this.MinX = this.CenterFrequency - this.Span / 2.0;
            //this.MaxX = this.CenterFrequency + this.Span / 2.0;

            //this.MinY = this.RefLevel - this.DivScale * this.NumOfColumn;
            //this.MaxY = this.RefLevel;

            this.CenterFrequency = (this.MinX + this.MaxX) / 2.0;
            this.Span = this.MaxX - this.MinX;

            this.RefLevel = this.MaxY;
            this.DivScale = (this.MaxY - this.MinY) / this.NumOfRow;
        }

        #region Marker Interface 
        public bool ShowHideMarker(int markerIndex)
        {
            // 해당 위치에 Marker 객체가 있는 경우
            if (marker.GetPoints(markerIndex) != null)
            {
                // 제거
                marker.RemoveMarker(markerIndex);
            }

            else
            {
                // Center Frequency를 기본 Point로 추가
                var DataLength = TotalDataLength;

                var screenX = SpectrumChartUtil.GetScreenX(DataLength / 2 + 1, DataLength, CurrentControlWidth, PaddingHorizontal);
                var valueY = screenPositions.GetClosestData(screenX, ref screenX);
                marker.AddPoint(screenX, valueY, markerIndex);
            }

            return true;
        }

        public bool SetMarkerFixed(int markerIndex)
        {
            marker.SetFixedList(markerIndex);

            return true;
        }

        public bool SetMarkerDelta(int sourceIndex, int targetIndex)
        {
            marker.SetDeltaList(sourceIndex, targetIndex);

            return true;
        }
        #endregion

        #region Trace Interface
        /// <summary>
        /// Create Trace
        /// </summary>
        /// <param name="data">double array data(max length : Trace.TotalDataLength) </param>
        /// <param name="traceIndex">trace index 0~3(max count : 4)</param>
        public void MakeTrace(int traceIndex)
        {
            if (!IsLoadSample)
            {
                trace.SetData(SpectrumData, traceIndex, TotalDataLength);
            }
        }

        /// <summary>
        ///  Trace Show/Hide
        /// </summary>
        /// <param name="traceIndex">trace index 0~3(max count : 4)</param>
        /// <returns></returns>
        public bool ShowHideTrace(int traceIndex)
        {
            // 해당 Index에 대한 Trace data 유무 파악
            if (trace.GetData(traceIndex) != null)
            {
                // Trace 초기화
                trace.ClearData(traceIndex);
            }

            else
            {
                trace.SetData(new double[Trace.MaxTraceCount ,TotalDataLength], traceIndex, TotalDataLength);
            }

            return true;
        }
        #endregion

    }
}
