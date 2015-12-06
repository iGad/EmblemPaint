using System;
using System.Net.Mail;
using System.Windows;
using System.Windows.Media.Imaging;
using Prism.Commands;

namespace EmblemPaint.ViewModel
{
    public class SendEmailViewModel : FunctionalViewModel
    {
        private string text = string.Empty, email = string.Empty;
        public SendEmailViewModel(Configuration configuration) : base(configuration)
        {
            PreviewLostFocusCommand = new DelegateCommand<object>(PreviewLostFocus);
            Update();
        }

        private void PreviewLostFocus(object obj)
        {
            var element = obj as FrameworkElement;
            element?.Focus();
        }

        /// <summary>
        /// Расскрашенное изображение
        /// </summary>
        public BitmapSource ResultImage { get; private set; }

        /// <summary>
        /// Исходное изображение
        /// </summary>
        public BitmapSource SourceImage { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string Email
        {
            get { return this.email; }
            set
            {
                if (this.email != value)
                {
                    this.email = value;
                    try
                    {
                        MailAddress m = new MailAddress(value);
                    }
                    catch
                    {
                        throw new ArgumentException("Введен неверный адрес");
                    }
                    
                    OnPropertyChanged(nameof(Email));
                    NextCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public DelegateCommand<object> PreviewLostFocusCommand { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool AreAnimationsEnabled => true;

        /// <summary>
        /// 
        /// </summary>
        public string Text
        {
            get { return this.text; }
            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        public override void Reconfigure(Configuration newConfig)
        {
            base.Reconfigure(newConfig);
            Update();
        }

        private void Update()
        {
            if (Configuration.Painter != null)
            {
                this.email = string.Empty;
                Text = string.Empty;
                ResultImage = Configuration.Painter.FilledImage;
                SourceImage = Configuration.Painter.SourceImage;
            }
        }

        protected override void Next()
        {
            RaiseNextCommandExecuted();
        }

        protected override bool CanExecuteNextCommand()
        {
            try
            {
                MailAddress m = new MailAddress(Email);
                return true;
            }
            catch
            {
                return false;
            }
        }
        
    }
}
