using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace EmblemPaint.Kernel
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Проверка на приблизительное равенство цветов (по евклидову расстоянию)
        /// </summary>
        /// <param name="color"></param>
        /// <param name="secondColor">С каким цветом сравнивается</param>
        /// <param name="precision">Точность сравнения</param>
        /// <returns></returns>
        public static bool IsNearEqualTo(this Color color, Color secondColor, double precision)
        {
            return color.DistanceTo(secondColor) <= precision;
        }

        /// <summary>
        /// Подсчет евклидова расстояния между цветами
        /// </summary>
        /// <param name="color"></param>
        /// <param name="secondColor"></param>
        /// <returns></returns>
        public static double DistanceTo(this Color color, Color secondColor)
        {
            return Math.Sqrt(Math.Pow(color.A - secondColor.A, 2) +
                             Math.Pow(color.R - secondColor.R, 2) +
                             Math.Pow(color.G - secondColor.G, 2) +
                             Math.Pow(color.B - secondColor.B, 2));
        }

        /// <summary>
        /// Преобразование цвета в строку в 16-ом представлении
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToHexString(this Color color)
        {
            string a = color.A < 16 ? "0" + Convert.ToString(color.A, 16) : Convert.ToString(color.A, 16);
            string r = color.R < 16 ? "0" + Convert.ToString(color.R, 16) : Convert.ToString(color.R, 16);
            string g = color.G < 16 ? "0" + Convert.ToString(color.G, 16) : Convert.ToString(color.G, 16);
            string b = color.B < 16 ? "0" + Convert.ToString(color.B, 16) : Convert.ToString(color.B, 16);
            return a + r + g + b;
        }


    }
}
