
using Prism.Mvvm;
using System;

namespace EmblemPaint.Kernel
{
    public abstract class ViewModel : BindableBase, IDisposable
    {
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
