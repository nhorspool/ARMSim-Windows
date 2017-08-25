using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Text.RegularExpressions;

namespace StaticWindows
{
    public static class PSize
    {
        //{Width=300, Height=300}" 
        public static bool TryParse(string str, out Size size)
        {
            size = new Size(0, 0);

            if (string.IsNullOrEmpty(str))
                return false;

            Regex reg = new Regex(@"^{Width=(?<width>\d+), Height=(?<height>\d+)}$", RegexOptions.IgnoreCase);
            Match match = reg.Match(str);
            string val1 = match.Result("${width}");
            string val2 = match.Result("${height}");

            int width, height;
            if (!int.TryParse(val1, out width) ||
                !int.TryParse(val2, out height))
                return false;

            size = new Size(width, height);
            return true;
        }
    }
}
