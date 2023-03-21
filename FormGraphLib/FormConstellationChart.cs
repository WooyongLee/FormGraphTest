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
        private bool isLoaded = false;

        public FormConstellationChart()
        {
            InitializeComponent();

            this.pictureBox1.Paint += PictureBox1_Paint;
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            // Load가 먹지 않아서 Paint Event 호출 시 초기화
            if (!isLoaded)
            {
                isLoaded = true;

                this.Initform();

                if (ConstellationComponent.IsShowSample) MakeDataForTest();
            }

            this.Draw(e.Graphics);

            pictureBox1.Invalidate();
        }

        public void Initform()
        {
            // Initialize Component
            ConstellationComponent.Init();

            // Set Size
            this.pictureBox1.Size = new System.Drawing.Size(ConstellationComponent.MaxWidth, ConstellationComponent.MaxHeight);
            
            // Set Background Color
            this.pictureBox1.BackColor = ConstellationComponent.BackgroundColor;
        }

        public void Draw(Graphics g)
        {
            // Draw Axis
            // var gridLineColor = ConstellationComponent.GridLineColor;
            // UL x, y, width, height
            //g.DrawRectangle(gridLineColor, GraphParameterDummy.Instance.ABox1_X, GraphParameterDummy.Instance.ABox_PaddingY,
            //    GraphParameterDummy.Instance.ABox1_Width - GraphParameterDummy.Instance.ABox_PaddingX * 2,
            //    GraphParameterDummy.Instance.ABox1_Height - GraphParameterDummy.Instance.ABox_PaddingY * 2);
            //DrawTitle(g, GraphParameterDummy.Instance.ABox1_X, 0,
            //    0, GraphParameterDummy.Instance.Title_Y,
            //    GraphParameterDummy.Instance.ABox1_Width, GraphParameterDummy.Instance.Title_Height, 
            //    1, 1, "I/Q Constellation");

            DrawIQConstellationAxis(g, ConstellationComponent.ABoxX, ConstellationComponent.ABoxY,
                    ConstellationComponent.ABoxWidth, ConstellationComponent.ABoxHeight,
                    ConstellationComponent.DevideX, ConstellationComponent.DevideY);

            DrawIQConstellationData(g, ConstellationComponent.ABoxX, ConstellationComponent.ABoxY,
                    ConstellationComponent.ABoxWidth, ConstellationComponent.ABoxHeight,
                    ConstellationComponent.DevideX, ConstellationComponent.DevideY,
                    ConstellationComponent.CH_X, ConstellationComponent.CH_Y, 
                    ConstellationComponent.ChannelColors, ConstellationComponent.ChannelNames,
                    ConstellationComponent.ChannelCount);
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
            double[,] CH_X, double[,] CH_Y, 
            Color[] cl, string[] cn, int ChannelCount)
        {
            Font drawFont = new Font("Arial", 10);
            SolidBrush drawBrush2 = ConstellationComponent.GridLineSolidBrush;
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = 0;
            int paddingX = ConstellationComponent.PaddingX;
            int paddingY = ConstellationComponent.PaddingY;
            int w = bw - paddingX * 2;
            int h = bh - paddingX * 2;

            int rww = w / DivideX;
            int rhh = h / DivideY;

            int ccx = paddingX + rww * ConstellationComponent.LengthInOneRow;
            int cx = ccx + BLT_X;
            int ccy = paddingY + rhh * ConstellationComponent.LengthInOneRow;
            int cy = ccy + BLT_Y;

            // Draw Point at Grid
            for (int i = 0; i < ChannelCount; i++)
            {
                // Channel Visible 허용할 시
                if (ConstellationComponent.IsVisibleChannel[i])
                {
                    var color = new SolidBrush(cl[i]);
                    int length = ConstellationComponent.MaxConstellationData;
                    for (int j = 0; j < length; j++)
                    {
                        int x2 = cx + (int)(CH_X[i, j] * w / ConstellationComponent.LengthInOneRow);
                        int y2 = cy + (int)(CH_Y[i, j] * w / ConstellationComponent.LengthInOneRow);
                        int rr2 = 2;
                        g.FillEllipse(color, x2 - rr2 / 2, y2 - rr2 / 2, rr2, rr2);
                    }
                }
            }

            // Bottom Hint Menu Visible
            if (ConstellationComponent.IsShowBottom)
            {
                Font drawFont2 = new Font("Arial", 8);
                drawFormat.FormatFlags = 0;
                int yOffset = 15;

                for (int i = 0; i < ChannelCount; i++)
                {
                    var x = BLT_X + (i % ConstellationComponent.LengthInOneRow) * 80 + 60;
                    var y = BLT_Y + bh - 35 + yOffset * (int)(i / ConstellationComponent.LengthInOneRow);
                    Rectangle ra1 = new Rectangle(x + 2, y + 2, 10, 10);
                    var drawBrush = new SolidBrush(cl[i]);
                    g.FillRectangle(drawBrush, ra1);
                    g.DrawString(cn[i], drawFont2, drawBrush, x + yOffset, y, drawFormat);
                }
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
            SolidBrush drawBrush2 = ConstellationComponent.GridLineSolidBrush;
            StringFormat drawFormat = new StringFormat();
            drawFormat.FormatFlags = 0;

            Pen pen1 = new Pen(Color.FromArgb(255, 144, 144, 144), 1);
            int paddingX = ConstellationComponent.PaddingX;
            int paddingY = ConstellationComponent.PaddingY;
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
            for (int i = 0; i <= DivideX; i++)
            {
                ssx = paddingX + rww * i;
                x = ssx + BLT_X;
                g.DrawLine(pen1, x, y, x, y + h);

                // 2칸에 하나씩 Legend String 배치
                if ((i % 2) == 0)
                {
                    // Center는 0로
                    // double ax = 2.0 * (i / 4.0) - 2.0;
                    double ax = ConstellationComponent.Xmax * (i * 2 - DivideX) / DivideX;
                    ss = string.Format("{0}", ax);
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
            for (int i = 0; i <= DivideY; i++)
            {
                // y를 변경하면서 x line을 그림
                ssy = paddingY + rhh * i;
                y = ssy + BLT_Y;
                g.DrawLine(pen1, x, y, x + w, y);
                if ((i % 2) == 0)
                {
                    double ax = ConstellationComponent.Ymax * (i * 2 - DivideY) / DivideY;
                    ax = -ax;
                    if (ax < 0)
                        ss = string.Format("{0}-", -ax);
                    else
                        ss = string.Format("{0}", ax);
                    int sx = paddingX - 4;
                    int sy = ssy - 8;
                    int x2 = sx + BLT_X;
                    int y2 = sy + BLT_Y;
                    g.DrawString(ss, drawFont, drawBrush2, x2, y2, drawFormat);
                }
            }
        }

        // Create Sample Data
        public void MakeDataForTest()
        {
            // -2 ~ 2
            for (int j = 0; j < 10; j++)
            {
                ConstellationComponent.CH_X[0, j] = -2 + j * 0.4;
                ConstellationComponent.CH_Y[0, j] = -2 + j * 0.4; 
            }
            for (int j = 0; j < 10; j++)
            {
                ConstellationComponent.CH_X[1, j] = -1.9 + j * 0.4;
                ConstellationComponent.CH_Y[1, j] = -1.9 + j * 0.4;
            }
            for (int j = 0; j < 10; j++)
            {
                ConstellationComponent.CH_X[2, j] = -1.8 + j * 0.4;
                ConstellationComponent.CH_Y[2, j] = -1.8 + j * 0.4;
            }
            for (int j = 0; j < 10; j++)
            {
                ConstellationComponent.CH_X[3, j] = -1.7 + j * 0.4;
                ConstellationComponent.CH_Y[3, j] = -1.7 + j * 0.4;
            }
        }
    }
}
