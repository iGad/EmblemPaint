using System;

namespace EmblemPaint.Kernel
{
    /// <summary>
    /// Аргументы события с параметром заданного типа
    /// </summary>
    /// <typeparam name="T">Тип параметра</typeparam>
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T parameter)
        {
            Parameter = parameter;
        }

        /// <summary>
        /// Параметр события
        /// </summary>
        public T Parameter { get; }
    }
}
