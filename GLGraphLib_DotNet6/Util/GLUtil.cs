using SharpGL;
using SharpGL.Enumerations;
using System;
using System.Drawing;

namespace GLGraphLib
{
    public static class GLUtil
    {
        // Text 폰트
        // public static readonly string FONT = "Microsoft Sans Serif";
        public static readonly string FONT = "verdana";

        /// <summary>
        /// LineWidth를 적용한 BoundingBox를 그림
        /// </summary>
        /// <param name="gl">OpenGL Object</param>
        /// <param name="minX">x 최소 위치</param>
        /// <param name="minY">y 최소 위치</param>
        /// <param name="maxX">x 최대 위치</param>
        /// <param name="maxY">y 최대 위치</param>
        /// <param name="c">색상</param>
        /// <param name="lineWidth">두께, 반복해서 그려서 두께를 늘림</param>
        public static void DrawThickBoundingBox(OpenGL gl, float minX, float minY, float maxX, float maxY, RGBcolor c, int lineWidth = 1)
        {
            for (double iter_offset = -lineWidth / 2.0; iter_offset < lineWidth / 2.0; ++iter_offset)
            {
                // Draw Bounding Box
                gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
                gl.Begin(BeginMode.Quads);
                gl.Color(c.R, c.G, c.B);
                gl.Vertex(minX + iter_offset, minY + iter_offset, 0.0f);
                gl.Vertex(maxX - iter_offset, minY+ iter_offset, 0.0f);
                gl.Vertex(maxX - iter_offset, maxY - iter_offset, 0.0f);
                gl.Vertex(minX + iter_offset, maxY - iter_offset, 0.0f);
                gl.End();
            }
        }

        /// <summary>
        /// LineWidth를 적용한 두 점을 잇는 Line을 그림
        /// </summary>
        /// <param name="gl">OpenGL Object</param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="c">색상</param>
        /// <param name="lineWidth">두께</param>
        public static void DrawThickLine(OpenGL gl, float x1, float y1, float x2, float y2, RGBcolor c, int lineWidth = 1)
        {
            for (double iter_offset = -lineWidth / 2.0; iter_offset < lineWidth / 2.0; ++iter_offset)
            {
                gl.Begin(OpenGL.GL_LINE_STRIP);
                gl.Color(c.R, c.G, c.B);
                gl.Vertex(x1 + iter_offset, y1 + iter_offset);
                gl.Vertex(x2 + iter_offset, y2 + iter_offset);
                gl.End();
            }
        }

        /// <summary>
        /// 지정된 소수점 만큼의 소수 형태의 문자열을 도시함
        /// </summary>
        /// <param name="gl">OpenGL Object</param>
        /// <param name="value">표현하려는 값</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="c">색상</param>
        /// <param name="decPlace">소수점 자리수</param>
        /// <param name="size">크기</param>
        public static void DrawFormattedText(OpenGL gl, double value, int x, int y, RGBcolor c, int decPlace = 2, float size = 12.0f)
        {
            var truncatedValue = Math.Round(value, decPlace);
            string formattedString = "{0," + (decPlace + 3).ToString() + "}";
            string strValue = string.Format(formattedString, truncatedValue);

            gl.DrawText(x, y, c.R, c.G, c.B, FONT, size, strValue);
        }

        public static void DrawText(OpenGL gl, string strText, float x, float y, RGBcolor c, float size = 12.0f, bool isBold = false)
        {
            var encodingBytes = System.Text.Encoding.ASCII.GetBytes(strText);

            var encodingStr = System.Text.Encoding.UTF8.GetString(encodingBytes);

            // Draw Text
            if (isBold)
            {
                gl.DrawText((int)x, (int)y, c.R, c.G, c.B, "verdana bold", size, encodingStr);
            }

            else
            {
                // Draw Text -> Font는 vendana로 고정, MS. San Sarif 사용 시 의도하지 않은 문자열을 도시함
                gl.DrawText((int)x, (int)y, c.R, c.G, c.B, "verdana", size, encodingStr);
            }

        }
    }
}
