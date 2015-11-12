using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace EmblemPaint.Kernel
{
    /// <summary>
    /// Данные события
    /// </summary>
    /// <typeparam name="TEventArgsType">Тип EventArgs</typeparam>
    public class EventInformation<TEventArgsType> where TEventArgsType : EventArgs
    {
        /// <summary>
        /// Посылатель события (обычно view)
        /// </summary>
        public object Sender { get; set; }
        
        /// <summary>
        /// Стандартные аргументы события
        /// </summary>
        public TEventArgsType EventArgs { get; set; }
        /// <summary>
        /// Параметры команды
        /// </summary>
        public object CommandArgument { get; set; }
    }

    /// <summary>
    /// Базовый абстрактный класс для преобразования события от внешнего источника в команду
    /// </summary>
    /// <typeparam name="TEventArgsType">Тип параметров события</typeparam>
    public abstract class EventToCommandMapBase<TEventArgsType> : TriggerAction<FrameworkElement>
    where TEventArgsType : EventArgs
    {
        
        /// <summary>
        /// Команда
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), 
                                                                                                typeof(EventToCommandMapBase<TEventArgsType>), 
                                                                                                new PropertyMetadata(null, OnCommandPropertyChanged));
        /// <summary>
        /// Параметр команды
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), 
                                                                                                         typeof(EventToCommandMapBase<TEventArgsType>), 
                                                                                                         new PropertyMetadata(null, OnCommandParameterPropertyChanged));

        /// <summary>
        /// Команда
        /// </summary>
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        /// <summary>
        /// Параметр команды
        /// </summary>
        public object CommandParameter
        {
            get
            {
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }
        private static void OnCommandParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var invokeCommand = d as EventToCommandMapBase<TEventArgsType>;
            if (invokeCommand != null)
            {
                invokeCommand.SetValue(CommandParameterProperty, e.NewValue);
            }
        }

        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var invokeCommand = d as EventToCommandMapBase<TEventArgsType>;
            if (invokeCommand != null)
            {
                invokeCommand.SetValue(CommandProperty, e.NewValue);
            }
        }


        /// <summary>
        /// Выполняет команду при возникновении события
        /// </summary>
        /// <param name="parameter"></param>
        protected override void Invoke(object parameter)
        {
            if (Command == null)
            {
                return;
            }

            var eventInfo = new EventInformation<TEventArgsType>
            {
                EventArgs = parameter as TEventArgsType,
                Sender = AssociatedObject,
                CommandArgument = GetValue(CommandParameterProperty)
            };

            if (Command.CanExecute(eventInfo))
            {

                Command.Execute(eventInfo);
            }
        }

        
    }

    /// <summary>
    /// Преобразует события движения мыши в команду
    /// </summary>
    public class RoutedEventToCommandMap : EventToCommandMapBase<RoutedEventArgs>
    {

    }

    /// <summary>
    /// Преобразует события движения мыши в команду
    /// </summary>
    public class MouseEventToCommandMap : EventToCommandMapBase<MouseEventArgs>
    {

    }

    /// <summary>
    /// Преобразует нажатия кнопок мыши в команду
    /// </summary>
    public class MouseButtonEventToCommandMap : EventToCommandMapBase<MouseButtonEventArgs>
    {

    }
}