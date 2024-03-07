using GLGraphLib_DotNet6;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Transactions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Markup.Primitives;
using System.Windows.Media.Animation;

// 해당 파일은 Dependency Object들을 정의
namespace GLGraphLib
{
    // ConstellationChart의 Dependency Object들을 정의
    public partial class ConstellationChart
    {
        #region Properties
        public ConstellationData Data
        {
            get { return (ConstellationData)GetValue(ConstellationDataProperty); }
            set { SetValue(ConstellationDataProperty, value); }
        }

        public bool IsShowLegend
        {
            get { return (bool)GetValue(IsShowLegendProperty); }
            set { SetValue(IsShowLegendProperty, value); }
        }

        public List<string> StrLegend
        {
            get { return (List<string>)GetValue(StrLegendProperty); }
            set { SetValue(StrLegendProperty, value); }
        }

        #endregion

        #region Define DependencyProperty from Properties
        public static readonly DependencyProperty ConstellationDataProperty = DependencyProperty.Register(
            "Data",
            typeof(ConstellationData),
            typeof(ConstellationChart),
            null
        );

        //public static readonly DependencyProperty ConstellationDataProperty = DependencyProperty.Register(
        //    "ConstellationData",
        //    typeof(double[,,]),
        //    typeof(ConstellationChart),
        //    null
        //);
        public static readonly DependencyProperty IsShowLegendProperty = DependencyProperty.Register(
            "IsShowLegend",
            typeof(bool),
            typeof(ConstellationChart),
            null
        );

        public static readonly DependencyProperty StrLegendProperty = DependencyProperty.Register(
            "StrLegend",
            typeof(List<string>),
            typeof(ConstellationChart),
            null
        );
        #endregion

        override public void InitProperty()
        {
            if (this.MinX <= 0 && this.MinY <= 0) this.MinY = this.MinX = -2.0;
            if (this.MaxX <= 0 && this.MaxY <= 0) this.MaxY = this.MaxX = 2.0;

            if (this.PaddingHorizontal <= 0) this.PaddingHorizontal = 30;
            if (this.PaddingVertical <= 0) this.PaddingVertical = 30;

            if (this.NumOfRow <= 0) this.NumOfRow = 8; // 한 행의 개수
            if (this.NumOfColumn <= 0) this.NumOfColumn = 8; // 한 열의 개수

            // 현재 컨트롤의 너비/높이
            if (this.CurrentControlHeight <= 0) this.CurrentControlWidth = 300;
            if (this.CurrentControlWidth <= 0) this.CurrentControlHeight = 300;

            this.BackgroundColor = new RGBcolor(Color.Black);
            this.AxisColor = new RGBcolor(Color.White);
        }

        public override void UpdateTheme()
        {
            if (this.BackgroundTheme == ETheme.Black)
            {
                BackgroundColor = new RGBcolor(Color.Black);
                AxisColor = new RGBcolor(Color.White);
            }

            else if(this.BackgroundTheme == ETheme.White)
            {
                BackgroundColor = new RGBcolor(Color.White);
                AxisColor = new RGBcolor(Color.Black);
            }
        }
    }

    // SpectrumChart의 Dependency Object들을 정의
    public partial class SpectrumChart
    {
        #region Properties
        /// <summary>
        /// Trace 데이터
        /// </summary>
        public Trace TraceData
        {
            get { return (Trace)GetValue(TraceDataProperty); }
            set { SetValue(TraceDataProperty, value); }
        }

        /// <summary>
        /// Spectrum 보임/숨김
        /// </summary>
        public bool[] IsVisibleSpectrum
        {
            get { return (bool[])GetValue(IsVisibleSpectrumProperty); }
            set { SetValue(IsVisibleSpectrumProperty, value); }
        }

        /// <summary>
        /// Marker 정도
        /// </summary>
        public Marker MarkerInfo
        {
            get { return (Marker)GetValue(MarkerInfoProperty); }
            set { SetValue(MarkerInfoProperty, value); }
        }

        /// <summary>
        /// 전체 스펙트럼 크기
        /// </summary>
        public int TotalDataLength
        {
            get { return (int)GetValue(TotalDataLengthProperty); }
            set { SetValue(TotalDataLengthProperty, value); }
        }

        /// <summary>
        /// 현재 선택된 Marker의 Trace 상에서 위치
        /// </summary>
        public int CurrentDataPosition
        {
            get { return (int)GetValue(CurrentDataPositionProperty); }
            set { SetValue(CurrentDataPositionProperty, value); }
        }

        /// <summary>
        /// X : Center Frequency (Center X Value)
        /// </summary>
        public double CenterFrequency
        {
            get { return (double)GetValue(CenterFrequencyProperty); }
            set { SetValue(CenterFrequencyProperty, value); }
        }

        /// <summary>
        /// X :: Span (Space of Column)
        /// </summary>
        public double Span
        {
            get { return (double)GetValue(SpanProperty); }
            set { SetValue(SpanProperty, value); }
        }

        /// <summary>
        /// Y :: Reference Level (Top of Y Value)
        /// </summary>
        public double RefLevel
        {
            get { return (double)GetValue(RefLevelProperty); }
            set { SetValue(RefLevelProperty, value); }
        }

        /// <summary>
        /// Y : Scale/Div (Space of Row)
        /// </summary>
        public double DivScale
        {
            get { return (double)GetValue(DivScaleProperty); }
            set { SetValue(DivScaleProperty, value); }
        }

        public IList<RGBcolor> TraceColors
        {
            get { return (IList<RGBcolor>)GetValue(TraceColorsProperty); } 
            set { SetValue(TraceColorsProperty, value); }
        }

        // To Do :: Color 관련하여 Dependency Object 추가하기
        RGBcolor spectrumColor1 = new RGBcolor(Color.Yellow); // trace 1
        RGBcolor spectrumColor2 = new RGBcolor(Color.Pink); // trace 2
        RGBcolor spectrumColor3 = new RGBcolor(Color.Purple); // trace 3
        RGBcolor spectrumColor4 = new RGBcolor(Color.Violet); // trace 4

        /// <summary>
        ///  x 축의 Text의 도시 여부
        /// </summary>
        public bool IsShowXaxisText
        {
            get { return (bool)GetValue(IsShowXaxisTextProperty); }
            set { SetValue(IsShowXaxisTextProperty, value); }
        }

        public bool IsShowMarkerInfo
        {
            get { return (bool)GetValue(IsShowMarkerInfoProperty); }
            set { SetValue(IsShowMarkerInfoProperty, value);}
        }

        public ESpectrumChartMode ChartMode
        {
            get { return (ESpectrumChartMode)GetValue(ChartModeProperty); }
            set { SetValue(ChartModeProperty, value);}
        }
        #endregion

        #region Define DependencyProperty from Properties
        public static readonly DependencyProperty TraceDataProperty = DependencyProperty.Register(
            "TraceData",
            typeof(Trace),
            typeof(SpectrumChart),
            null
            );

        public static readonly DependencyProperty IsVisibleSpectrumProperty = DependencyProperty.Register(
            "IsVisibleSpectrum",
            typeof(bool[]),
            typeof(SpectrumChart),
            null
            );
        
        public static readonly DependencyProperty MarkerInfoProperty = DependencyProperty.Register(
            "MarkerInfo",
            typeof(Marker),
            typeof(SpectrumChart),
            null
            );

        public static readonly DependencyProperty TotalDataLengthProperty = DependencyProperty.Register(
            "TotalDataLength",
            typeof(int),
            typeof(SpectrumChart),
            null
            );

        public static readonly DependencyProperty CurrentDataPositionProperty = DependencyProperty.Register(
            "CurrentDataPosition",
            typeof(int),
            typeof(SpectrumChart),
            null
            );

        public static readonly DependencyProperty CenterFrequencyProperty = DependencyProperty.Register(
            "CenterFrequency",
            typeof(double),
            typeof(SpectrumChart),
            new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnFreqChanged))
            );

        public static readonly DependencyProperty SpanProperty = DependencyProperty.Register(
            "Span",
            typeof(double),
            typeof(SpectrumChart),
            new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnSpanChanged))
            );

        public static readonly DependencyProperty RefLevelProperty = DependencyProperty.Register(
            "RefLevel",
            typeof(double),
            typeof(SpectrumChart),
            new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnRefLvChanged))
            );

        public static readonly DependencyProperty DivScaleProperty = DependencyProperty.Register(
            "DivScale",
            typeof(double),
            typeof(SpectrumChart),
            new FrameworkPropertyMetadata(0.0, new PropertyChangedCallback(OnDivScaleChanged))
            );

        public static readonly DependencyProperty IsShowXaxisTextProperty = DependencyProperty.Register(
            "IsShowXaxisText",
            typeof(bool),
            typeof(SpectrumChart),
            null
            );

        public static readonly DependencyProperty TraceColorsProperty = DependencyProperty.Register(
            "TraceColors",
            typeof(IList<RGBcolor>),
            typeof(SpectrumChart),
            null
            );
        public static readonly DependencyProperty IsShowMarkerInfoProperty = DependencyProperty.Register(
            "IsShowMarkerInfo",
            typeof(bool),
            typeof(SpectrumChart),
            null
            );
        public static readonly DependencyProperty ChartModeProperty = DependencyProperty.Register(
            "ChartMode",
            typeof(ESpectrumChartMode),
            typeof(SpectrumChart),
            new FrameworkPropertyMetadata(ESpectrumChartMode.DefaultSpecturm, new PropertyChangedCallback(OnChartModeChanged))
            );
        #endregion

        private static void OnChartModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var spectrumChartControl = d as SpectrumChart;
            ESpectrumChartMode newChartMode = (ESpectrumChartMode)e.NewValue;

            if (spectrumChartControl != null )
            {
                if (newChartMode == ESpectrumChartMode.IQ)
                {
                    spectrumChartControl.IsShowXaxisText = false;
                }
                else
                {
                    spectrumChartControl.IsShowXaxisText = true;
                }
            }
        }

        private static void OnFreqChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var spectrumChartControl = d as SpectrumChart;

            // New Center Freq Value
            double newValue = (double)e.NewValue;

            if (spectrumChartControl != null )
            {
                spectrumChartControl.CenterFrequency = newValue;

                spectrumChartControl.MinX = spectrumChartControl.CenterFrequency - spectrumChartControl.Span / 2.0;
                spectrumChartControl.MaxX = spectrumChartControl.CenterFrequency + spectrumChartControl.Span / 2.0;
            }
        }

        private static void OnSpanChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var spectrumChartControl = d as SpectrumChart;

            // New Span Value
            double newValue = (double)e.NewValue;

            if (spectrumChartControl != null)
            {
                spectrumChartControl.Span = newValue;

                spectrumChartControl.MinX = spectrumChartControl.CenterFrequency - spectrumChartControl.Span / 2.0;
                spectrumChartControl.MaxX = spectrumChartControl.CenterFrequency + spectrumChartControl.Span / 2.0;
            }
        }

        private static void OnRefLvChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var spectrumChartControl = d as SpectrumChart;

            // New Ref Level Value
            double newValue = (double)e.NewValue;

            if (spectrumChartControl != null)
            {
                spectrumChartControl.RefLevel = newValue;

                spectrumChartControl.MaxY = spectrumChartControl.RefLevel;
                spectrumChartControl.MinY = spectrumChartControl.RefLevel - 10 * spectrumChartControl.DivScale;
            }
        }

        private static void OnDivScaleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var spectrumChartControl = d as SpectrumChart;

            // New Ref Level Value
            double newValue = (double)e.NewValue;

            if (spectrumChartControl != null)
            {
                spectrumChartControl.DivScale = newValue;

                spectrumChartControl.MaxY = spectrumChartControl.RefLevel;
                spectrumChartControl.MinY = spectrumChartControl.RefLevel - 10 * spectrumChartControl.DivScale;
            }
        }

        //protected static void OnMinXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var spectrumChartControl = d as SpectrumChart;

        //    double newValue = (double)e.NewValue;

        //    if (spectrumChartControl != null)
        //    {
        //        spectrumChartControl.MinX = newValue;

        //        spectrumChartControl.CenterFrequency = (spectrumChartControl.MinX + spectrumChartControl.MaxX) / 2.0;
        //        spectrumChartControl.Span = spectrumChartControl.MaxX - spectrumChartControl.MinX;
        //    }
        //}

        //protected static void OnMaxXChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    var spectrumChartControl = d as SpectrumChart;

        //    double newValue = (double)e.NewValue;

        //    if (spectrumChartControl != null)
        //    {
        //        spectrumChartControl.MaxX = newValue;

        //        spectrumChartControl.CenterFrequency = (spectrumChartControl.MinX + spectrumChartControl.MaxX) / 2.0;
        //        spectrumChartControl.Span = spectrumChartControl.MaxX - spectrumChartControl.MinX;
        //    }
        //}

        private void SpectrumChart_MinXChanged(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                this.MinX = (double)sender;

                this.CenterFrequency = (this.MinX + this.MaxX) / 2.0;
                this.Span = this.MaxX - this.MinX;
            }
        }

        private void SpectrumChart_MaxXChanged(object? sender, EventArgs e)
        {
            if (sender != null)
            {
                this.MaxX = (double)sender;

                this.CenterFrequency = (this.MinX + this.MaxX) / 2.0;
                this.Span = this.MaxX - this.MinX;
            }
        }

        override public void InitProperty()
        {
            if (this.NumOfRow <= 0 && this.NumOfColumn <= 0) this.NumOfRow = this.NumOfColumn = 10;
            if (this.PaddingHorizontal <= 0) this.PaddingHorizontal = 40;
            if (this.PaddingVertical <= 0) this.PaddingVertical = 30;

            if (this.CurrentControlHeight <= 0) this.CurrentControlHeight = 300;
            if (this.CurrentControlWidth <= 0) this.CurrentControlWidth = 800;

            // Min, Max X to Freq/Span
            if (this.MinX == this.MaxX && this.MaxX == 0)
            {
                this.MinX = this.CenterFrequency - this.Span / 2.0;
                this.MaxX = this.CenterFrequency + this.Span / 2.0;
            }

            // Set default Center Freq
            else
            {
                this.CenterFrequency = (this.MinX + this.MaxX) / 2.0;
                this.Span = Math.Abs(this.MaxX - this.MinX);
            }

            // DivScale Default
            if (this.DivScale == 0) this.DivScale = 10;

            // Min, Max Y to RefLv/DicScale
            if (this.MinY == this.MaxY && this.MaxY == 0)
            {
                // Default Min, Max Y
                this.MinY = this.RefLevel - this.DivScale * this.NumOfRow;
                this.MaxY = this.RefLevel;
            }

            else
            {
                // Max, Min Y가 적절하게 설정되어 있을 때
                this.RefLevel = this.MaxY;
                this.DivScale = (this.MaxY - this.MinY) / (double)this.NumOfRow;
            }

            this.BackgroundColor = new RGBcolor(Color.Black);
            this.AxisColor = new RGBcolor(Color.White);

            this.IsShowXaxisText = true;
            this.TotalDataLength = 1001;

            IsVisibleSpectrum = new bool[Trace.MaxTraceCount];

            IsVisibleSpectrum[0] = true; // 첫 trace는 Visible

            //MinXChanged += SpectrumChart_MinXChanged;
            //MaxXChanged += SpectrumChart_MaxXChanged;
        }

        public override void UpdateTheme()
        {
            if (this.BackgroundTheme == ETheme.Black)
            {
                BackgroundColor = new RGBcolor(Color.Black);
                AxisColor = new RGBcolor(Color.White);
            }

            else if (this.BackgroundTheme == ETheme.White)
            {
                BackgroundColor = new RGBcolor(Color.White);
                AxisColor = new RGBcolor(Color.Black);
            }
        }
    }

    public partial class BarGraphChart
    {
        #region Properties
        public List<double> BarData
        {
            get { return (List<double>)GetValue(BarDataProperty); }
            set { SetValue(BarDataProperty, value); }
        }

        public List<string> BarLegend
        {
            get { return (List<string>)GetValue(BarLegendProperty); }
            set { SetValue(BarLegendProperty, value); }
        }

        /// <summary>
        /// X : Center Frequency (Center X Value)
        /// </summary>
        public RGBcolor BarColor
        {
            get { return (RGBcolor)GetValue(BarColorProperty); }
            set { SetValue(BarColorProperty, value); }
        }
        #endregion

        #region Define DependencyProperty from Properties
        public static readonly DependencyProperty BarDataProperty = DependencyProperty.Register(
            "BarData",
            typeof(List<double>),
            typeof(BarGraphChart),
            null
        );

        public static readonly DependencyProperty BarLegendProperty = DependencyProperty.Register(
            "BarLegend",
            typeof(List<string>),
            typeof(BarGraphChart),
            null
        );

        public static readonly DependencyProperty BarColorProperty = DependencyProperty.Register(
            "BarColor",
            typeof(RGBcolor),
            typeof(BarGraphChart),
            null
        );
        #endregion

        override public void InitProperty()
        {
            if (this.NumOfRow <= 0) this.NumOfRow = 5;
            if (this.NumOfColumn <= 0) this.NumOfColumn = 8;

            if (this.PaddingHorizontal <= 0) this.PaddingHorizontal = 40;
            if (this.PaddingVertical <= 0) this.PaddingVertical = 30;

            if (this.CurrentControlHeight <= 0) this.CurrentControlHeight = 300;
            if (this.CurrentControlWidth <= 0) this.CurrentControlWidth = 800;

            if (this.BackgroundColor == null) this.BackgroundColor = new RGBcolor(Color.Black);
            if (this.AxisColor == null) this.AxisColor = new RGBcolor(Color.White);
            if (this.BarColor == null) this.BarColor = new RGBcolor(Color.Red);
        }

        public override void UpdateTheme()
        {

        }
    }

}
