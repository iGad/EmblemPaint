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
        /// ������������� ������������ ����
        /// </summary>
        protected internal Configuration Configuration { get; set; }

        #region Commands

        /// <summary>
        /// �������, ������������� ��� �����-���� ��������� ������������
        /// </summary>
        public DelegateCommand<EventInformation<RoutedEventArgs>> ActionCommand { get; }

        /// <summary>
        /// ��������, ������� ����������� ��� �������������� ������������ � �����
        /// </summary>
        /// <param name="e">��������� �������</param>
        protected virtual void Action(EventInformation<RoutedEventArgs> e)
        {
            var handler = ActionHappened;
            handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// ������� �������� � ��������� ����
        /// </summary>
        public DelegateCommand<bool?> HomeCommand { get; }

        /// <summary>
        /// �������� ������� �������� � ��������� ����
        /// </summary>
        /// <param name="askUser">��������� �� ������������� ������������</param>
        protected virtual void Home(bool? askUser)
        {
            RaiseHomeCommandExecuted(askUser ?? false);
        }

        /// <summary>
        /// ������� �������� � ���������� ����
        /// </summary>
        public DelegateCommand NextCommand { get; }

        /// <summary>
        /// �������� �������� � ���������� ����
        /// </summary>
        protected virtual void Next()
        {
            RaiseNextCommandExecuted();
        }

        /// <summary>
        /// �������� �� ������� NextCommand
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanExecuteNextCommand()
        {
            return true;
        }

        /// <summary>
        /// ������� �������� �� ���������� ����
        /// </summary>
        public DelegateCommand BackCommand { get; }

        /// <summary>
        /// �������� �������� �� ���������� ����
        /// </summary>
        protected virtual void Back()
        {
            RaiseBackCommandExecuted();
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// �������, ���������� � ���, ��� ��������� ������� Next
        /// </summary>
        public event EventHandler NextCommandExecuted;

        /// <summary>
        /// �������, ���������� � ���, ��� ��������� ������� Back
        /// </summary>
        public event EventHandler BackCommandExecuted;

        /// <summary>
        /// �������, ���������� � ���, ��� ��������� ������� Home
        /// </summary>
        public event EventHandler<EventArgs<bool>> HomeCommandExecuted;

        /// <summary>
        /// �������, ���������� � ���, ��� ������������ �������� �����-���� �������� � �������
        /// </summary>
        public event EventHandler ActionHappened;

        /// <summary>
        /// ������������� ������� NextCommandExecuted
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
        /// ������������� ������� HomeCommandExecuted
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
        /// ������������� ������� BackCommandExecuted
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