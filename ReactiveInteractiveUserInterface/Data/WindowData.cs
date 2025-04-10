using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.Data.WindowData
{
    public class WindowData
    {
        public double ScreenWidth { get; }
        public double ScreenHeight { get; }
        public double BorderWidth { get; }

        public WindowData(double screenWidth, double screenHeight, double borderWidth)
        {
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            BorderWidth = borderWidth;
        }
    }
}
