using System;
using System.Windows.Controls;

namespace WpfKb.LogicalKeys
{
    public class SwitchLanguageKey : LogicalKeyBase
    {
        private readonly string _targetLangPrefix;
        private readonly Grid _keyboardControl;

        public SwitchLanguageKey(string displayName, string targetLangPrefix, Grid keyboardControl)
        {
            _targetLangPrefix = targetLangPrefix;
            _keyboardControl = keyboardControl;
            DisplayName = displayName;
        }

        public override void Press()
        {
            InputMethodSwitcher.SetInputMethod(_targetLangPrefix);
            Panel.SetZIndex(_keyboardControl, Panel.GetZIndex(_keyboardControl) - 1);
        }
    }
}