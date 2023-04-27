using System;
using System.Drawing;
using System.Windows;

// 해당 파일은 Dependency Object들을 정의
namespace GLGraphLib_DotNet6
{
    // ConstellationChart의 Dependency Object들을 정의
    public partial class ConstellationChart
    {
        #region Properties

        #endregion

        #region Define DependencyProperty from Properties

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

            this.BackgroundColor = new(Color.Black);
            this.AxisColor = new(Color.White);
        }
    }

    // SpectrumChart의 Dependency Object들을 정의
    public partial class SpectrumChart
    {
        #region Properties
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

        // To Do :: Color 관련하여 Dependency Object 추가하기
        RGBcolor spectrumColor1 = new(Color.Yellow); // trace 1
        RGBcolor spectrumColor2 = new(Color.Pink); // trace 2
        RGBcolor spectrumColor3 = new(Color.Purple); // trace 3
        RGBcolor spectrumColor4 = new(Color.Violet); // trace 4

        /// <summary>
        ///  x 축의 Text의 도시 여부
        /// </summary>
        public bool IsShowXaxisText
        {
            get { return (bool)GetValue(IsShowXaxisTextProperty); }
            set { SetValue(IsShowXaxisTextProperty, value); }
        }

        #endregion

        #region Define DependencyProperty from Properties
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

        public static readonly DependencyProperty IsShowXaxisTextProperty = DependencyProperty.Register(
            "IsShowXaxisText",
            typeof(bool),
            typeof(SpectrumChart),
            null
            );

        #endregion

        override public void InitProperty()
        {
            // Min, Max X to Freq/Span
            if (this.MinX > 0 && this.MaxX > 0)
            {
                this.CenterFrequency = (this.MinX + this.MaxX) / 2.0;
                this.Span = Math.Abs(this.MaxX - this.MinX);
            }

            // Set default Center Freq
            else
            {
                this.CenterFrequency = 3650.01;
                this.Span = 150.0;
            }

            // Min, Max Y to RefLv/DicScale
            if (this.MinY > 0 && this.MaxY > 0)
            {
                this.RefLevel = this.MaxY;
                this.DivScale = (this.MaxY - this.MinY) / (double)this.NumOfRow;
            }

            else
            {
                this.RefLevel = 0;
                this.DivScale = 10;
            }

            if (this.NumOfRow <= 0 && this.NumOfColumn <= 0) this.NumOfRow = this.NumOfColumn = 10;
            if (this.PaddingHorizontal <= 0) this.PaddingHorizontal = 40;
            if (this.PaddingVertical <= 0) this.PaddingVertical = 30;

            if (this.CurrentControlHeight <= 0) this.CurrentControlHeight = 300;
            if (this.CurrentControlWidth <= 0 ) this.CurrentControlWidth = 800;

            this.BackgroundColor = new(Color.Black);
            this.AxisColor = new(Color.White);

            this.IsShowXaxisText = true;
        }
    }

    public partial class BarGraphChart
    {
        #region Properties
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

            if (this.BackgroundColor == null) this.BackgroundColor = new(Color.Black);
            if (this.AxisColor == null) this.AxisColor = new(Color.White);
            if (this.BarColor == null) this.BarColor = new(Color.Red);
        }
    }
}
