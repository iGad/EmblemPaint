using System;
using System.Timers;
using System.Windows;
using Microsoft.Practices.Prism.Commands;

namespace EmblemPaint.ViewModel
{
    public abstract class FunctionalViewModel : Kernel.ViewModel
    {
        private readonly Timer timer;
        protected FunctionalViewModel()
        {
            this.timer = new Timer(2*5*1000);
            this.timer.Elapsed += OnTimerElapsed;
            HomeCommand = new DelegateCommand<bool?>(Home);
            BackCommand = new DelegateCommand(Back);
            GoNextCommand = new DelegateCommand(GoNext, CanExecuteGoNext);
            GoBackCommand = new DelegateCommand<object>(GoBack,CanExecuteGoBack);
            CloseCommand = new DelegateCommand<Window>(Close);
        }

        

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            StopTimer();
            bool result = false;
            Application.Current.Dispatcher.Invoke(() => result = WaitUserAction());
            if (result)
            {
                Application.Current.Dispatcher.Invoke(() => Home(false));
            }
            else
            {
                StartTimer();
            }
        }

        private bool WaitUserAction()
        {
            View.PleaseReturnView view = new View.PleaseReturnView();
            var dialogResult = view.ShowDialog();
            return (dialogResult.HasValue && dialogResult.Value);
        }

        public void ResetTimer()
        {
            StopTimer();
            StartTimer();
        }

        protected void StopTimer()
        {
            this.timer.Stop();
        }

        protected void StartTimer()
        {
            this.timer.Start();
        }

        protected override void OnActionHappened()
        {
            if (this.timer.Enabled)
            {
                ResetTimer();
            }
        }

        public WindowStartupLocation StartupLocation { get; } = WindowStartupLocation.Manual;

        public DelegateCommand GoNextCommand { get; }

        public DelegateCommand<object> GoBackCommand { get; }

        public DelegateCommand BackCommand { get; }

        public event EventHandler BackCommandExecuted;

        public DelegateCommand<Window> CloseCommand { get; }

        public DelegateCommand<bool?> HomeCommand { get; protected set; }

        public Window ScreensaverWindow { get; protected set; }

        public event EventHandler HomeCommandExecuted;

        public event EventHandler Closing;

        protected virtual void Home(bool? askUser)
        {
            RaiseHomeCommandExecuted();
        }

        protected void RaiseHomeCommandExecuted()
        {
            var handle = HomeCommandExecuted;
            handle?.Invoke(this, EventArgs.Empty);
        }

        protected void RaiseClosing()
        {
            var handle = Closing;
            handle?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void Back()
        {
            RaiseBackCommandExecuted();
        }

        protected void RaiseBackCommandExecuted()
        {
            var handler = BackCommandExecuted;
            handler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual bool CanExecuteGoNext()
        {
            return true;
        }

        protected virtual void GoNext()
        {
        }

        protected virtual bool CanExecuteGoBack(object parameter)
        {
            return true;
        }

        protected virtual void GoBack(object parameter)
        {
            
        }

        protected virtual void Close(Window window)
        {
            RaiseClosing();
            window?.Close();
        }
    }
}