using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmblemPaint.Kernel
{
    public interface IConfiguration
    {
        RegionsStorage Storage { get; }

        bool Save(Stream stream);
    }
}
