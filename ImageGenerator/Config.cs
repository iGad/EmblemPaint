using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmblemPaint.Kernel;

namespace EmblemPaint.ImageGenerator
{
    public class Config : Configuration
    {
        public Config()
        {
            var defaultConfig = Configuration.DefaultConfiguration;
            WaitUserActionInterval = defaultConfig.WaitUserActionInterval;
            FileNameSeparator = defaultConfig.FileNameSeparator;
            WaitAnswerInterval = defaultConfig.WaitAnswerInterval;
            HorizontalItemsCount = defaultConfig.HorizontalItemsCount;
            VerticalItemsCount = defaultConfig.VerticalItemsCount;
        }

        /// <summary>
        /// 
        /// </summary>
        public string FolderPath { get; set; } = "Gerby";
        
        
    }
}
