using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace GLGraphLib_DotNet6
{
    public abstract partial class ChartUserControlBase : UserControl
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

        public RGBcolor BackgroundColor
        {
            get { return (RGBcolor )GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        } 
        
        public RGBcolor AxisColor
        {
            get { return ( RGBcolor )GetValue(AxisColorProperty); }
            set { SetValue(AxisColorProperty, value);}
        } 
        #endregion

        #region Define DependencyProperty from Properties
        public static readonly DependencyProperty IsLoadSampleProperty = DependencyProperty.Register(
            "IsLoadSample",
            typeof(bool),
            typeof(ChartUserControlBase),
            null
            );

        public static readonly DependencyProperty MinXProperty = DependencyProperty.Register(
            "MinX",
            typeof(double),
            typeof(ChartUserControlBase),
            null
            );

        public static readonly DependencyProperty MinYProperty = DependencyProperty.Register(
            "MinY",
            typeof(double),
            typeof(ChartUserControlBase),
            null
            );

        public static readonly DependencyProperty MaxXProperty = DependencyProperty.Register(
            "MaxX",
            typeof(double),
            typeof(ChartUserControlBase),
            null
            );

        public static readonly DependencyProperty MaxYProperty = DependencyProperty.Register(
            "MaxY",
            typeof(double),
            typeof(ChartUserControlBase),
            null
            );

        public static readonly DependencyProperty NumOfRowProperty = DependencyProperty.Register(
            "NumOfRow",
            typeof(int),
            typeof(ChartUserControlBase),
            null
            );

        public static readonly DependencyProperty NumOfColumnProperty = DependencyProperty.Register(
            "NumOfColumn",
            typeof(int),
            typeof(ChartUserControlBase),
            null
            );

        public static readonly DependencyProperty PaddingHorizontalProperty = DependencyProperty.Register(
            "PaddingHorizontal",
            typeof(float),
            typeof(ChartUserControlBase),
            null
            );

        public static readonly DependencyProperty PaddingVerticalProperty = DependencyProperty.Register(
            "PaddingVertical",
            typeof(float),
            typeof(ChartUserControlBase),
            null
            );

        public static readonly DependencyProperty CurrentControlWidthProperty = DependencyProperty.Register(
            "CurrentControlWidth",
            typeof(double),
            typeof(ChartUserControlBase),
            null
            );

        public static readonly DependencyProperty CurrentControlHeightProperty = DependencyProperty.Register(
            "CurrentControlHeight",
            typeof(double),
            typeof(ChartUserControlBase),
            null
            );

        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register(
            "BackgroundColor",
            typeof(RGBcolor),
            typeof(ChartUserControlBase),
            null
        );

        public static readonly DependencyProperty AxisColorProperty = DependencyProperty.Register(
            "AxisColor",
            typeof(RGBcolor),
            typeof(ChartUserControlBase),
            null
        );
        #endregion


        // 초기 Dependency Property 값 들을 할당
        public abstract void InitProperty();
    }
}
