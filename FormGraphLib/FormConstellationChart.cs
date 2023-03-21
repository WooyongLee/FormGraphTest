using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormGraphLib
{
    // Constellation Chart 표현
    public partial class FormConstellationChart : UserControl
    {

        // Max ChartView Size
        static int MaxWidth = 300;
        static int MaxHeight = 300;

        public FormConstellationChart()
        {
            InitializeComponent();

            this.Initform();

            pictureBox1.Paint += PictureBox1_Paint;
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            this.Draw(e.Graphics);

            pictureBox1.Invalidate();
        }

        public void Initform()
        {
            // Set Background Color
            this.pictureBox1.BackColor = GraphParameterDummy.Instance.BackgroundColor;
        }

        public void Draw(Graphics g)
        {
            var gridLineColor = GraphParameterDummy.Instance.GridLineColor;

            // Draw Axis
            // UL x, y, width, height
            g.DrawRectangle(gridLineColor, GraphParameterDummy.Instance.ABox1_X, GraphParameterDummy.Instance.ABox_PaddingY,
                GraphParameterDummy.Instance.ABox1_Width - GraphParameterDummy.Instance.ABox_PaddingX * 2,
                GraphParameterDummy.Instance.ABox1_Height - GraphParameterDummy.Instance.ABox_PaddingY * 2);
            //DrawTitle(g, GraphParameterDummy.Instance.ABox1_X, 0,
            //    0, GraphParameterDummy.Instance.Title_Y,
            //    GraphParameterDummy.Instance.ABox1_Width, GraphParameterDummy.Instance.Title_Height, 
            //    1, 1, "I/Q Constellation");

            DrawIQConstellationAxis(g, GraphParameterDummy.Instance.ABox1_X, 0,
                    GraphParameterDummy.Instance.ABox1_Width, GraphParameterDummy.Instance.ABox1_Height,
                    GraphParameterDummy.Instance.DevideX, GraphParameterDummy.Instance.DevideY);

            DrawIQConstellationData(g, GraphParameterDummy.Instance.ABox1_X, 0,
                    GraphParameterDummy.Instance.ABox1_Width, GraphParameterDummy.Instance.ABox1_Height,
                    GraphParameterDummy.Instance.DevideX, GraphParameterDummy.Instance.DevideY,
                    GraphParameterDummy.Instance.CH_X, GraphParameterDummy.Instance.CH_Y, GraphParameterDummy.Instance.CH_R,
                    GraphParameterDummy.Instance.ChannelColor, GraphParameterDummy.Instance.ChannelName,
                    GraphParameterDummy.Instance.ChannelCount);

        }

        /// <summary>
        ///  Constellation Data를 표현함
        /// </summary>
        /// <param name="g">graphics object</param>
        /// <param name="BLT_X">Margin Right</param>
        /// <param name="BLT_Y">Margin Top</param>
        /// <param name="bw">Chart Total Width</param>
        /// <param name="bh">Chart Total Height</param>
        /// <param name="DivideX">분할된 X 개수</param>
        /// <param name="DivideY">분할된 Y 개수</param>
        /// <param name="CH_X">Channel - X Data Pair</param>
        /// <param name="CH_Y">Channel - Y Data Pair</param>
        /// <param name="CH_R">Channel - R Data Pair</param>
        /// <param name="cl">Color Array</param>
        /// <param name="cn">Channel Name Array</param>
        /// <param name="ChannelCount">Channel Count</param>
        private void DrawIQConstellationData(Graphics g, int BLT_X, int BLT_Y,
            int bw, int bh, int DivideX, int DivideY,
            double[,] CH_X, double[,] CH_Y, double[,] CH_R,
            Color[] cl, string[] cn, int ChannelCount)
        {
            Font drawFont = new Font("Arial", 10);
            SolidBrush drawBrush2 = GraphParameterDummy.Instance.GridLineSolidBrush;
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = 0;
            int paddingX = GraphParameterDummy.Instance.PaddingX;
            int paddingY = GraphParameterDummy.Instance.PaddingY;
            int w = bw - paddingX * 2;
            int h = bh - paddingX * 2;

            int rww = w / DivideX;
            int rhh = h / DivideY;

            int ccx = paddingX + rww * 4;
            int cx = ccx + BLT_X;
            int ccy = paddingY + rhh * 4;
            int cy = ccy + BLT_Y;

            // Draw Point at Grid
            for (int i = 0; i < ChannelCount; i++)
            {
                var color = new SolidBrush(cl[i]);
                int length = GraphParameterDummy.MaxConstellationData;
                for ( int j = 0; j < length; j++ )
                {
                    int x2 = cx + (int)(CH_X[i, j] * w / 4);
                    int y2 = cy + (int)(CH_Y[i, j] * w / 4);
                    int rr2 = 2;
                    g.FillEllipse(color, x2 - rr2 / 2, y2 - rr2 / 2, rr2, rr2);
                }
            }

            // Draw Legend 
            Font drawFont2 = new Font("Arial", 8);
            drawFormat.FormatFlags = 0;
            var ss = "";

            for (int i = 0; i < ChannelCount; i++)
            {
                //ss = "nnn" + String.Format("{0}", i); 
                var x = BLT_X + (i % 4) * 80 + 60;
                var y = BLT_Y + bh - 35 + 15 * (int)(i / 4);
                Rectangle ra1 = new Rectangle(x + 2, y + 2, 10, 10);
                drawBrush32 = new SolidBrush(cl[i]);
                g.FillRectangle(drawBrush32, ra1);
                g.DrawString(cn[i], drawFont2, drawBrush31, x + 15, y, drawFormat);
            }
        }

        /// <summary>
        /// Constellation Axis를 생성함
        /// </summary>
        /// <param name="g">graphics object</param>
        /// <param name="BLT_X">Margin Right</param>
        /// <param name="BLT_Y">Margin Top</param>
        /// <param name="bw">Chart Total Width</param>
        /// <param name="bh">Chart Total Height</param>
        /// <param name="DivideX">분할된 X 개수</param>
        /// <param name="DivideY">분할된 Y 개수</param>
        private void DrawIQConstellationAxis(Graphics g, int BLT_X, int BLT_Y, int bw, int bh, int DivideX, int DivideY)
        {
            Font drawFont = new Font("Arial", 10);
            SolidBrush drawBrush2 = GraphParameterDummy.Instance.GridLineSolidBrush;
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = 0;

            Pen pen1 = new Pen(Color.FromArgb(255, 144, 144, 144), 1);
            int paddingX = GraphParameterDummy.Instance.PaddingX;
            int paddingY = GraphParameterDummy.Instance.PaddingY;
            int w = bw - paddingX * 2;
            int h = bh - paddingX * 2;
            int rww = w / DivideX;
            int rhh = h / DivideY;

            int ssx = paddingX;
            int ssy = paddingY;

            int x = ssx + BLT_X;
            int y = ssy + BLT_Y;

            string ss = "";

            // Draw Vertical Line
            SolidBrush drawBrush1 = new SolidBrush(Color.FromArgb(255, 222, 30, 30));
            ssy = paddingY;
            y = ssy + BLT_Y;
            for (int i = 0; i < DivideX + 1; i++)
            {
                ssx = paddingX + rww * i;
                x = ssx + BLT_X;
                g.DrawLine(pen1, x, y, x, y + h);
                if ((i % 2) == 0)
                {
                    double ax = 2.0 * (i / 4.0) - 2.0;
                    ss = String.Format("{0}", ax);
                    int sx = ssx - 4;
                    int sy = paddingY + h + 4;
                    int x2 = sx + BLT_X;
                    int y2 = sy + BLT_Y;
                    g.DrawString(ss, drawFont, drawBrush2, x2, y2, drawFormat);
                }
            }

            // Draw Horizontal Line
            drawFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            ssx = paddingX;
            x = ssx + BLT_X;
            for (int i = 0; i < DivideY + 1; i++)
            {
                // y를 변경하면서 x line을 그림
                ssy = paddingY + rhh * i;
                y = ssy + BLT_Y;
                g.DrawLine(pen1, x, y, x + w, y);
                if ((i % 2) == 0)
                {
                    double ax = 2.0 * (i / 4.0) - 2.0;
                    ax = -ax;
                    if (ax < 0)
                        ss = String.Format("{0}-", -ax);
                    else
                        ss = String.Format("{0}", ax);
                    int sx = paddingX - 4;
                    int sy = ssy - 8;
                    int x2 = sx + BLT_X;
                    int y2 = sy + BLT_Y;
                    g.DrawString(ss, drawFont, drawBrush2, x2, y2, drawFormat);
                }
            }
        }

        public static void DrawTitle(Graphics g, int BLT_X, int BLT_Y, int x, int y, int w, int h, int px, int py, string ss)
        {
            // 이거 시작점과 크기만 파라미터로 바는다.

            // 타이틀을 그리는 것, 이걱 각 상자마다 약간씩 크기가 다른다.
            //Pen pen1 = new Pen(Color.Green, 2);
            //g.DrawRectangle(pen1, px, py, w - x * 2, h -py* 2);
            int ww = w * 8 / 10;
            SolidBrush drawBrush1 = new SolidBrush(Color.FromArgb(255, 30, 30, 30));
            Rectangle ra1 = new Rectangle(BLT_X + (w - ww) / 2, BLT_Y + y, ww, h);
            g.FillRectangle(drawBrush1, ra1);

            Font drawFont = new Font("Arial", 12, FontStyle.Bold);
            SolidBrush drawBrush2 = new SolidBrush(Color.FromArgb(255, 211, 211, 211));
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = 0;
            int ssx = BLT_X + w / 2 - 66;
            int ssy = BLT_Y + y + 6;
            g.DrawString(ss, drawFont, drawBrush2, ssx, ssy, drawFormat);

        }
    }
}
