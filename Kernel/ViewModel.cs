
using Prism.Mvvm;
using System;
using System.Windows;
using Microsoft.Practices.Prism.Commands;

namespace EmblemPaint.Kernel
{
    public abstract class ViewModel : BindableBase, IDisposable
    {
        protected ViewModel()
        {
            NextCommand = new DelegateCommand(Next);
            ActionCommand = new DelegateCommand<EventInformation<RoutedEventArgs>>(Action);
            ActionHappened += OnActionHappened;
        }

        protected virtual void Next()
        {
            RaiseNextCommandExecuted();
        }

        protected void RaiseNextCommandExecuted()
        {
            var handler = NextCommandExecuted;
            handler?.Invoke(this, EventArgs.Empty);
        }

        private void OnActionHappened(object sender, EventArgs e)
        {
            OnActionHappened();
        }

        /// <summary>
        /// Команда, выполняющаяся при каких-либо действиях пользователя
        /// </summary>
        public DelegateCommand<EventInformation<RoutedEventArgs>> ActionCommand { get; }


        public DelegateCommand NextCommand { get; }

        public event EventHandler NextCommandExecuted;

        protected virtual void Action(EventInformation<RoutedEventArgs> e)
        {
            var handler = ActionHappened;
            handler?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnActionHappened()
        {
            
        }

        /// <summary>
        /// Событие, сообщеющее о том, что пользователь совершил какое-либо действие с вьюшкой
        /// </summary>
        public event EventHandler ActionHappened;

        #region Dispose

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler Disposing;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                var handler = Disposing;
                handler?.Invoke(this, EventArgs.Empty);
                DoDispose();
            }
        }

        protected virtual void DoDispose()
        {
        }

        ~ViewModel()
        {
            Dispose(false);
        }

        #endregion
    }
}
