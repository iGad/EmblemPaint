using System;
using System.ComponentModel;
using System.Windows.Media;

namespace WpfKb.LogicalKeys
{
    public class LogicalKeyEventArgs : EventArgs
    {
        public ILogicalKey Key { get; private set; }

        public LogicalKeyEventArgs(ILogicalKey key)
        {
            Key = key;
        }
    }

    public delegate void LogicalKeyPressedEventHandler(object sender, LogicalKeyEventArgs e);

    public abstract class LogicalKeyBase : ILogicalKey
    {
        public event LogicalKeyPressedEventHandler LogicalKeyPressed;
        public event PropertyChangedEventHandler PropertyChanged;

        private string _displayName;
        private static MediaPlayer _mediaPlayer;

        public virtual string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (value != _displayName)
                {
                    _displayName = value;
                    OnPropertyChanged("DisplayName");
                }
            }
        }

        static LogicalKeyBase()
        {
            _mediaPlayer = new MediaPlayer();
            _mediaPlayer.Open(new Uri("Sounds/clicks/keyboard.wav", UriKind.RelativeOrAbsolute));
        }

        public virtual void Press()
        {
            PlaySound();
            OnKeyPressed();
        }

        public virtual void PlaySound()
        {
            _mediaPlayer.Position = TimeSpan.Zero;
            _mediaPlayer.Play();
        }

        protected void OnKeyPressed()
        {
            if (LogicalKeyPressed != null) LogicalKeyPressed(this, new LogicalKeyEventArgs(this));
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                var args = new PropertyChangedEventArgs(propertyName);
                handler(this, args);
            }
        }
    }
}