using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EmblemPaint.Kernel
{
    public class RegionSuffixRegexes
    {

        public RegionSuffixRegexes():this('_',"thumb","ideal","pattern")
        {
        }

        public RegionSuffixRegexes(char separator, string thumbnailPattern, string sourceImagePattern, string patternImagePattern)
        {
            Separator = separator;
            ThumbnailRegex = new Regex(thumbnailPattern);
            SourceImageRegex = new Regex(sourceImagePattern);
            PatternImageRegex = new Regex(patternImagePattern);
        }

        public char Separator { get; }

        public Regex ThumbnailRegex { get; }

        public Regex SourceImageRegex { get; }

        public Regex PatternImageRegex { get; }
    }
}
