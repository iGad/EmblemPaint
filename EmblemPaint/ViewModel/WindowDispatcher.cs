using System;
using System.Timers;
using System.Windows;
using EmblemPaint.Kernel;
using EmblemPaint.View;

namespace EmblemPaint.ViewModel
{
    public class WindowDispatcher: FunctionalViewModel
    {
        private double windowHeight, windowWidth;
        private readonly Timer timer;
        private FunctionalViewModel activeModel;
        private readonly ScreensaverViewModel screensaverViewModel;
        private readonly SelectRegionViewModel selectRegionViewModel;

        public WindowDispatcher(Configuration configuration):base(configuration)
        {
            Configuration = configuration;
            WindowHeight = configuration.WindowHeight;
            WindowWidth = configuration.WindowWidth;
            this.timer = new Timer(configuration.WaitUserActionInterval * 1000);
            this.timer.Elapsed += OnTimerElapsed;
            this.screensaverViewModel = new ScreensaverViewModel(configuration);
            this.selectRegionViewModel = new SelectRegionViewModel(configuration);
            Subscribe();
            ActiveModel = this.screensaverViewModel;
        }

        #region Properties

        public FunctionalViewModel ActiveModel
        {
            get { return this.activeModel; }
            set
            {
                this.activeModel = value;
                if (this.activeModel != this.screensaverViewModel)
                {
                    ResetTimer();
                }
                OnPropertyChanged(nameof(ActiveModel));
            }
        }

        public double WindowHeight
        {
            get { return this.windowHeight; }
            set
            {
                if (!this.windowHeight.Equals(value))
                {
                    this.windowHeight = value;
                    OnPropertyChanged(nameof(WindowHeight));
                }
            }
        }

        public double WindowWidth
        {
            get { return this.windowWidth; }
            set
            {
                if (!this.windowWidth.Equals(value))
                {
                    this.windowWidth = value;
                    OnPropertyChanged(nameof(WindowWidth));
                }
            }
        }

        #endregion

        private void Subscribe()
        {
            this.screensaverViewModel.NextCommandExecuted += ScreensaverViewModelOnNextCommandExecuted;
            this.selectRegionViewModel.HomeCommandExecuted += ViewModelOnHomeCommandExecuted;
            this.selectRegionViewModel.NextCommandExecuted += SelectRegionViewModelOnNextCommandExecuted;
        }

        #region Event handlers

        #region Screensaver

        private void ScreensaverViewModelOnNextCommandExecuted(object sender, EventArgs eventArgs)
        {
            ActiveModel = this.selectRegionViewModel;
        }

        #endregion

        #region SelectRegion

        private void SelectRegionViewModelOnNextCommandExecuted(object sender, EventArgs eventArgs)
        {
            var paintViewModel = new PaintViewModel(Configuration);
            SubscribeToPaintViewModel(paintViewModel);
            ActiveModel = paintViewModel;
        }

        #endregion

        #region Paint

        private void SubscribeToPaintViewModel(PaintViewModel paintViewModel)
        {
            paintViewModel.BackCommandExecuted += PaintViewModelOnBackCommandExecuted;
            paintViewModel.NextCommandExecuted += PaintViewModelOnNextCommandExecuted;
            paintViewModel.HomeCommandExecuted += ViewModelOnHomeCommandExecuted;
            paintViewModel.Disposing += PaintViewModelOnDisposing;
        }

        private void PaintViewModelOnDisposing(object sender, EventArgs eventArgs)
        {
            var paintViewModel = sender as PaintViewModel;
            if (paintViewModel != null)
            {
                UnsubscribeFromPaintViewModel(paintViewModel);
            }
        }

        private void UnsubscribeFromPaintViewModel(PaintViewModel paintViewModel)
        {
            paintViewModel.BackCommandExecuted -= PaintViewModelOnBackCommandExecuted;
            paintViewModel.NextCommandExecuted -= PaintViewModelOnNextCommandExecuted;
            paintViewModel.HomeCommandExecuted -= ViewModelOnHomeCommandExecuted;
            paintViewModel.Disposing -= PaintViewModelOnDisposing;
        }

        private void PaintViewModelOnNextCommandExecuted(object sender, EventArgs eventArgs)
        {
            var paintViewModel = sender as PaintViewModel;
            if (paintViewModel != null)
            {
                var resultViewModel = new ResultViewModel(paintViewModel.RegionSymbol, paintViewModel.SourceImage, paintViewModel.CalculateFillAccuracy(), Configuration);
                resultViewModel.HomeCommandExecuted += ViewModelOnHomeCommandExecuted;
                resultViewModel.Disposing += ResultViewModelOnDisposing;
                paintViewModel.Dispose();
                ActiveModel = resultViewModel;
            }
        }

        private void PaintViewModelOnBackCommandExecuted(object sender, EventArgs eventArgs)
        {
            if (AskUserConfirmation(true))
            {
                var paintViewModel = sender as Kernel.ViewModel;
                paintViewModel?.Dispose();
                ActiveModel = this.selectRegionViewModel;
            }
        }

        #endregion

        #region Result

        private void ResultViewModelOnDisposing(object sender, EventArgs eventArgs)
        {
            var resultViewModel = sender as ResultViewModel;
            if (resultViewModel != null)
            {
                resultViewModel.HomeCommandExecuted -= ViewModelOnHomeCommandExecuted;
                resultViewModel.Disposing -= ResultViewModelOnDisposing;
            }
        }

        #endregion

        private bool AskUserConfirmation(bool needAsking)
        {
            if (needAsking)
            {
                var askUserView = new ConfirmView {Owner = Application.Current.MainWindow};
                return askUserView.ShowDialog() ?? false;
            }
            return true;
        }

        private void ViewModelOnHomeCommandExecuted(object sender, EventArgs<bool> eventArgs)
        {
            
            if (AskUserConfirmation(eventArgs.Parameter))
            {
                var resultViewModel = sender as Kernel.ViewModel;
                if (resultViewModel != this.selectRegionViewModel)
                {
                    resultViewModel?.Dispose();
                }
                ActiveModel = this.screensaverViewModel;
            }
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.timer.Stop();
            bool result = false;
            Application.Current.Dispatcher.Invoke(() => result = WaitUserAction());
            if (result)
            {
                Application.Current.Dispatcher.Invoke(() => ActiveModel.HomeCommand.Execute(false));
            }
            else
            {
                this.timer.Start();
            }
        }

        private bool WaitUserAction()
        {
            PleaseReturnView view = new PleaseReturnView();
            view.ShowDialog();
            var dialogResult = view.DialogResult;
            return (dialogResult.HasValue && dialogResult.Value);
        }

        public void ResetTimer()
        {
            this.timer.Stop();
            this.timer.Start();
        }

        protected override void OnActionHappened()
        {
            if (this.timer.Enabled)
            {
                ResetTimer();
            }
        }

        #endregion
    }
}
