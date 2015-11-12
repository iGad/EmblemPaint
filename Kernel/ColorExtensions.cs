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
            return Math.Sqrt(Math.Pow(color.A - secondColor.A, 2)+ 
                    Math.Pow(color.R - secondColor.R, 2)+
                    Math.Pow(color.G - secondColor.G, 2)+
                    Math.Pow(color.B - secondColor.B, 2)) <= precision;
        }
    }
}
