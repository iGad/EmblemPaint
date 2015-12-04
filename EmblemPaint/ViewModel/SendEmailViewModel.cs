using System;
using System.Net.Mail;

namespace EmblemPaint.ViewModel
{
    public class SendEmailViewModel : FunctionalViewModel
    {
        private string text = string.Empty, email = string.Empty;
        public SendEmailViewModel(Configuration configuration) : base(configuration)
        {
        }

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
                    try
                    {
                        MailAddress m = new MailAddress(value);
                    }
                    catch
                    {
                        throw  new ArgumentException("Введен неверный адрес");
                    }
                    this.email = value;
                    OnPropertyChanged(nameof(Email));
                    NextCommand.RaiseCanExecuteChanged();
                }
            }
        }

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

        protected override void Next()
        {
            Home(false);
        }
    }
}
