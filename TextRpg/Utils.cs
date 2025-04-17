using System;
using System.Text;

namespace TextRpg
{
    public static class Utils
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
                if(value.Value > 0)
                    effectSB.Append($"{value.Key} + {value.Value} ");
                else
                    effectSB.Append($"{value.Key} {value.Value} ");
            }
            return effectSB.ToString();
        }

        // 패딩 관련 함수
        public static string PadRight(this string input, int totalWidth)
        {
            int len = GetKoreanAwareLength(input);
            return input + new string(' ', Math.Max(0, totalWidth - len));
        }

        public static string PadLeft(this string input, int totalWidth)
        {
            int len = GetKoreanAwareLength(input);
            return new string(' ', Math.Max(0, totalWidth - len)) + input;
        }

        private static int GetKoreanAwareLength(string input)
        {
            int len = 0;
            foreach (char c in input)
            {
                len += IsKorean(c) ? 2 : 1;
            }
            return len;
        }

        private static bool IsKorean(char c)
        {
            return (c >= 0xAC00 && c <= 0xD7A3);
        }
    }
}
