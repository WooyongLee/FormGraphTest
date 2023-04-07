using System.Windows;

// 해당 파일은 Dependency Object들을 정의
namespace GLGraphLib_DotNet6
{
    // ConstellationChart의 Dependency Object들을 정의
    public partial class ConstellationChart
    {
        #region Properties
        /// <summary>
        /// Sample Load 여부
        /// </summary>
        public bool IsLoadSample
        {
            get { return (bool)GetValue(IsLoadSampleProperty); }
            set { SetValue(IsLoadSampleProperty, value); }
        }

        /// <summary>
        /// Minumum X Axis Value
        /// </summary>
        public double MinX
        {
            get { return (double)GetValue(MinXProperty); }
            set { SetValue(MinXProperty, value); }
        }

        /// <summary>
        /// Minumum Y Axis Value
        /// </summary>
        public double MinY
        {
            get { return (double)GetValue(MinYProperty); }
            set { SetValue(MinYProperty, value); }
        }

        /// <summary>
        /// Maximum X Axis Value
        /// </summary>
        public double MaxX
        {
            get { return (double)GetValue(MaxXProperty); }
            set { SetValue(MaxXProperty, value); }
        }

        /// <summary>
        /// Maximum Y Axis Value
        /// </summary>
        public double MaxY
        {
            get { return (double)GetValue(MaxYProperty); }
            set { SetValue(MaxYProperty, value); }
        }

        /// <summary>
        /// 표현할 Constellation 행의 수
        /// </summary>
        public int NumOfRow
        {
            get { return (int)GetValue(NumOfRowProperty); }
            set { SetValue(NumOfRowProperty, value); }
        }

        /// <summary>
        /// 표현할 Constellation 열의 수
        /// </summary>
        public int NumOfColumn
        {
            get { return (int)GetValue(NumOfColumnProperty); }
            set { SetValue(NumOfColumnProperty, value); }
        }

        /// <summary>
        /// 수평 패딩
        /// </summary>
        public float PaddingHorizontal
        {
            get { return (float)GetValue(PaddingHorizontalProperty); }
            set { SetValue(PaddingHorizontalProperty, value); }
        }

        /// <summary>
        /// 수직 패딩
        /// </summary>
        public float PaddingVertical
        {
            get { return (float)GetValue(PaddingVerticalProperty); }
            set { SetValue(PaddingVerticalProperty, value); }
        }

        /// <summary>
        /// 현재 컨트롤의 너비
        /// </summary>
        public double CurrentControlWidth
        {
            get { return (double)GetValue(CurrentControlWidthProperty); }
            set { SetValue(CurrentControlWidthProperty, value); }
        }

        /// <summary>
        /// 현재 컨트롤의 높이
        /// </summary>
        public double CurrentControlHeight
        {
            get { return (double)GetValue(CurrentControlHeightProperty); }
            set { SetValue(CurrentControlHeightProperty, value); }
        }
        #endregion

        #region Define DependencyProperty from Properties
        public static readonly DependencyProperty IsLoadSampleProperty = DependencyProperty.Register(
            "IsLoadSample",
            typeof(bool),
            typeof(ConstellationChart),
            null
            );

        public static readonly DependencyProperty MinXProperty = DependencyProperty.Register(
            "MinX",
            typeof(double),
            typeof(ConstellationChart),
            null
            );

        public static readonly DependencyProperty MinYProperty = DependencyProperty.Register(
            "MinY",
            typeof(double),
            typeof(ConstellationChart),
            null
            );

        public static readonly DependencyProperty MaxXProperty = DependencyProperty.Register(
            "MaxX",
            typeof(double),
            typeof(ConstellationChart),
            null
            );

        public static readonly DependencyProperty MaxYProperty = DependencyProperty.Register(
            "MaxY",
            typeof(double),
            typeof(ConstellationChart),
            null
            );

        public static readonly DependencyProperty NumOfRowProperty = DependencyProperty.Register(
            "NumOfRow",
            typeof(int),
            typeof(ConstellationChart),
            null
            );

        public static readonly DependencyProperty NumOfColumnProperty = DependencyProperty.Register(
            "NumOfColumn",
            typeof(int),
            typeof(ConstellationChart),
            null
            );

        public static readonly DependencyProperty PaddingHorizontalProperty = DependencyProperty.Register(
            "PaddingHorizontal",
            typeof(float),
            typeof(ConstellationChart),
            null
            );

        public static readonly DependencyProperty PaddingVerticalProperty = DependencyProperty.Register(
            "PaddingVertical",
            typeof(float),
            typeof(ConstellationChart),
            null
            );

        public static readonly DependencyProperty CurrentControlWidthProperty = DependencyProperty.Register(
            "CurrentControlWidth",
            typeof(double),
            typeof(ConstellationChart),
            null
            );

        public static readonly DependencyProperty CurrentControlHeightProperty = DependencyProperty.Register(
            "CurrentControlHeight",
            typeof(double),
            typeof(ConstellationChart),
            null
            );
        #endregion

        void InitProperty()
        {
            this.MinY = this.MinX = -2.0;
            this.MaxY = this.MaxX = 2.0;

            this.PaddingHorizontal = 30;
            this.PaddingVertical = 30;

            this.NumOfRow = 8; // 한 행의 개수
            this.NumOfColumn = 8; // 한 열의 개수

            // 현재 컨트롤의 너비/높이
            this.CurrentControlWidth = 300;
            this.CurrentControlHeight = 300;
        }
    }

    // SpectrumChart의 Dependency Object들을 정의
    public partial class SpectrumChart
    {
        #region Properties
        /// <summary>
        /// Sample Load 여부
        /// </summary>
        public bool IsLoadSample
        {
            get { return (bool)GetValue(IsLoadSampleProperty); }
            set { SetValue(IsLoadSampleProperty, value); }
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

        /// <summary>
        /// 수평 구분 칸 개수
        /// </summary>
        public int HorizontalSpaceCount
        {
            get { return (int)GetValue(HorizontalSpaceCountProperty); }
            set { SetValue(HorizontalSpaceCountProperty, value); }
        }

        /// <summary>
        /// 수직 구분 칸 개수
        /// </summary>
        public int VerticalSpaceCount
        {
            get { return (int)GetValue(VerticalSpaceCountProperty); }
            set { SetValue(VerticalSpaceCountProperty, value); }
        }

        /// <summary>
        /// 수평 패딩
        /// </summary>
        public float PaddingHorizontal
        {
            get { return (float)GetValue(PaddingHorizontalProperty); }
            set { SetValue(PaddingHorizontalProperty, value); }
        }

        /// <summary>
        /// 수직 패딩
        /// </summary>
        public float PaddingVertical
        {
            get { return (float)GetValue(PaddingVerticalProperty); }
            set { SetValue(PaddingVerticalProperty, value); }
        }

        /// <summary>
        /// 현재 컨트롤의 너비
        /// </summary>
        public double CurrentControlWidth
        {
            get { return (double)GetValue(CurrentControlWidthProperty); }
            set { SetValue(CurrentControlWidthProperty, value); }
        }

        /// <summary>
        /// 현재 컨트롤의 높이
        /// </summary>
        public double CurrentControlHeight
        {
            get { return (double)GetValue(CurrentControlHeightProperty); }
            set { SetValue(CurrentControlHeightProperty, value); }
        }
        #endregion

        #region Define DependencyProperty from Properties
        public static readonly DependencyProperty IsLoadSampleProperty = DependencyProperty.Register(
            "IsLoadSample",
            typeof(bool),
            typeof(SpectrumChart),
            null
            );

        public static readonly DependencyProperty CenterFrequencyProperty = DependencyProperty.Register(
            "CenterFrequency",
            typeof(double),
            typeof(SpectrumChart),
            null
            );

        public static readonly DependencyProperty SpanProperty = DependencyProperty.Register(
            "Span",
            typeof(double),
            typeof(SpectrumChart),
            null
            );

        public static readonly DependencyProperty RefLevelProperty = DependencyProperty.Register(
            "RefLevel",
            typeof(double),
            typeof(SpectrumChart),
            null
            );

        public static readonly DependencyProperty DivScaleProperty = DependencyProperty.Register(
            "DivScale",
            typeof(double),
            typeof(SpectrumChart),
            null
            );

        public static readonly DependencyProperty HorizontalSpaceCountProperty = DependencyProperty.Register(
            "HorizontalSpaceCount",
            typeof(int),
            typeof(SpectrumChart),
            null
            );

        public static readonly DependencyProperty VerticalSpaceCountProperty = DependencyProperty.Register(
            "VerticalSpaceCount",
            typeof(int),
            typeof(SpectrumChart),
            null
            );

        public static readonly DependencyProperty PaddingHorizontalProperty = DependencyProperty.Register(
            "PaddingHorizontal",
            typeof(float),
            typeof(SpectrumChart),
            null
            );

        public static readonly DependencyProperty PaddingVerticalProperty = DependencyProperty.Register(
            "PaddingVertical",
            typeof(float),
            typeof(SpectrumChart),
            null
            );

        public static readonly DependencyProperty CurrentControlWidthProperty = DependencyProperty.Register(
            "CurrentControlWidth",
            typeof(double),
            typeof(SpectrumChart),
            null
            );

        public static readonly DependencyProperty CurrentControlHeightProperty = DependencyProperty.Register(
            "CurrentControlHeight",
            typeof(double),
            typeof(SpectrumChart),
            null
            );
        #endregion

        void InitProperty()
        {
            this.CenterFrequency = 3650.01;
            this.Span = 150.0;

            this.RefLevel = 0;
            this.DivScale = 10;

            this.HorizontalSpaceCount = this.VerticalSpaceCount = 10;

            this.PaddingHorizontal = 40;
            this.PaddingVertical = 30;

            this.CurrentControlHeight = 300;
            this.CurrentControlWidth = 800;
        }
    }
}
