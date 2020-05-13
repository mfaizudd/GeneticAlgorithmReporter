using System;
using System.Collections.Generic;
using System.Text;

namespace GeneticAlgorithmReporter.Utility
{
    public static class Extension
    {
        public static string CenteredString(this string s, int width)
        {
            if (s.Length >= width)
            {
                return s;
            }

            int leftPadding = (width - s.Length) / 2;
            int rightPadding = width - s.Length - leftPadding;

            return new string(' ', leftPadding) + s + new string(' ', rightPadding);
        }
    }
}
