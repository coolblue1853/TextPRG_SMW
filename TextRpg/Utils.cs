using System;
using System.Collections.Generic;
using System.Text;

namespace TextRpg
{
    internal class Utils
    {
        public static int GetEnumIndex(Enum inputEnum)
        {
            Array values = Enum.GetValues(inputEnum.GetType());
            return Array.IndexOf(values, inputEnum);
        }
        public static void UpdateStringBuilder(StringBuilder sb, string text)
        {
            sb.Append(text);
        }
        public static void ClearStringBuilder(StringBuilder sb)
        {
            Console.Clear();
            sb.Clear();
        }
        public static void ShowStringBuilder(StringBuilder sb)
        {
            Console.WriteLine(sb);
        }
    }
}
