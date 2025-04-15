using System;
using System.Collections.Generic;
using System.Text;

namespace TextRpg
{
    internal class Utils
    {
        static StringBuilder sb = new StringBuilder();
        public static int GetEnumIndex(Enum inputEnum)
        {
            Array values = Enum.GetValues(inputEnum.GetType());
            return Array.IndexOf(values, inputEnum);
        }
        public static void UpdateStringBuilder(string text, bool isShow = false, bool isClear = false)
        {
            if (isClear)
                ClearStringBuilder();

            sb.Append(text);
            if (isShow)
                ShowStringBuilder();
        }
        public static void ClearStringBuilder()
        {
            Console.Clear();
            sb.Clear();
        }
        public static void ShowStringBuilder()
        {
            Console.WriteLine(sb);
        }
        public static void ReadLine(out string line)
        {
            line = Console.ReadLine();
        }
        public static string AddEffectText(Item item)
        {
            StringBuilder effectSB = new StringBuilder();
            foreach (var value in item.GetEffect())
            {
                effectSB.Append($"{value.Key} + {value.Value} ");
            }

            return effectSB.ToString();
        }


        

    }
}
