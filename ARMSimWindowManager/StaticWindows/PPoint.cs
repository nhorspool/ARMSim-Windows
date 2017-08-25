using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Text.RegularExpressions;

namespace StaticWindows
{
    public static class PPoint
    {
        //{X=1094,Y=265}
        public static bool TryParse(string str, out Point point)
        {
            point = new Point(0, 0);
            if (string.IsNullOrEmpty(str))
                return false;

            Regex reg = new Regex(@"^{X=(?<xpos>\d+),Y=(?<ypos>\d+)}$", RegexOptions.IgnoreCase);
            Match match = reg.Match(str);
            string val1 = match.Result("${xpos}");
            string val2 = match.Result("${ypos}");

            int xpos, ypos;
            if (!int.TryParse(val1, out xpos) ||
                !int.TryParse(val2, out ypos))
                return false;

            point = new Point(xpos, ypos);
            return true;
        }
    }
}
