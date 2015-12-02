using System.Windows.Forms;

namespace WpfKb
{
    public static class InputMethodSwitcher
    {
        public static void ToRus()
        {
            SetInputMethod("Ru");
        }

        public static void ToEng()
        {
            SetInputMethod("En");
        }

        public static void SetInputMethod(string method)
        {
            string cname = "";
            foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
            {
                cname = lang.Culture.EnglishName.ToString();

                if (cname.StartsWith(method))
                {
                    InputLanguage.CurrentInputLanguage = lang;
                }
            }
        }
    }
}