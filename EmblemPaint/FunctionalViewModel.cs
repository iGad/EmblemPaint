using System;
using System.Windows;
using EmblemPaint.Kernel;
using Microsoft.Practices.Prism.Commands;

namespace EmblemPaint
{
    public abstract class FunctionalViewModel : Kernel.ViewModel
    {
        protected FunctionalViewModel(Configuration configuration)
        {
            Configuration = configuration;
            BackCommand = new DelegateCommand(Back);
            NextCommand = new DelegateCommand(Next, CanExecuteNextCommand);
            HomeCommand = new DelegateCommand<bool?>(Home);
            ActionCommand = new DelegateCommand<EventInformation<RoutedEventArgs>>(Action);
            ActionHappened += OnActionHappened;
        }

        /// <summary>
        /// Настраиваемая конфигурация окон
        /// </summary>
        protected internal Configuration Configuration { get; set; }

        #region Commands

        /// <summary>
        /// Команда, выполняющаяся при каких-либо действиях пользователя
        /// </summary>
        public DelegateCommand<EventInformation<RoutedEventArgs>> ActionCommand { get; }

        /// <summary>
        /// Действие, которое выполняется при взаимодействии пользователя с окном
        /// </summary>
        /// <param name="e">Аргументы события</param>
        protected virtual void Action(EventInformation<RoutedEventArgs> e)
        {
            var handler = ActionHappened;
            handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Команда возврата к домашнему окну
        /// </summary>
        public DelegateCommand<bool?> HomeCommand { get; }

        /// <summary>
        /// Действия команды возврата к домашнему окну
        /// </summary>
        /// <param name="askUser">Требуется ли подтверждение пользователя</param>
        protected virtual void Home(bool? askUser)
        {
            RaiseHomeCommandExecuted(askUser ?? false);
        }

        /// <summary>
        /// Команда перехода к следующему окну
        /// </summary>
        public DelegateCommand NextCommand { get; }

        /// <summary>
        /// Действие перехода к следующему окну
        /// </summary>
        protected virtual void Next()
        {
            RaiseNextCommandExecuted();
        }

        /// <summary>
        /// Доступна ли команда NextCommand
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanExecuteNextCommand()
        {
            return true;
        }

        /// <summary>
        /// Команда возврата на предыдущее окно
        /// </summary>
        public DelegateCommand BackCommand { get; }

        /// <summary>
        /// Действие перехода на предыдущее окно
        /// </summary>
        protected virtual void Back()
        {
            RaiseBackCommandExecuted();
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Событие, сообщающее о том, что выполнена команда Next
        /// </summary>
        public event EventHandler NextCommandExecuted;

        /// <summary>
        /// Событие, сообщающее о том, что выполнена команда Back
        /// </summary>
        public event EventHandler BackCommandExecuted;

        /// <summary>
        /// Событие, сообщающее о том, что выполнена команда Home
        /// </summary>
        public event EventHandler<EventArgs<bool>> HomeCommandExecuted;

        /// <summary>
        /// Событие, сообщающее о том, что пользователь совершил какое-либо действие с вьюшкой
        /// </summary>
        public event EventHandler ActionHappened;

        /// <summary>
        /// Сгенерировать событие NextCommandExecuted
        /// </summary>
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
        /// Сгенерировать событие HomeCommandExecuted
        /// </summary>
        protected void RaiseHomeCommandExecuted(bool askUser)
        {
            var handle = HomeCommandExecuted;

            handle?.Invoke(this, new EventArgs<bool>(askUser));
        }
        

        protected virtual void OnActionHappened()
        {
        }

        

        /// <summary>
        /// Сгенерировать событие BackCommandExecuted
        /// </summary>
        protected void RaiseBackCommandExecuted()
        {
            var handler = BackCommandExecuted;
            handler?.Invoke(this, EventArgs.Empty);
        }

        #endregion


        #region Methods

        public virtual void Reconfigure(Configuration newConfig)
        {
            Configuration = newConfig;
        }

        #endregion

    }
}