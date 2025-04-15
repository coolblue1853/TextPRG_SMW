using System;
using System.Collections.Generic;
using System.Text;

namespace TextRpg
{
    public class SceneTextData
    {
        public IntroText Intro { get; set; }
        public ErrorText Error { get; set; }
    }
    public class IntroText
    {
        public string welcome { get; set; }
        public string choose_job { get; set; }
    }
    public class ErrorText
    {
        public string input_error { get; set; }

    }
}
