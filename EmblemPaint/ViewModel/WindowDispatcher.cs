using System;
using EmblemPaint.Kernel;

namespace EmblemPaint.ViewModel
{
    public class WindowDispatcher:Kernel.ViewModel
    {
        private Kernel.ViewModel activeModel;
        private readonly ScreensaverViewModel screensaverViewModel;
        private readonly SelectRegionViewModel selectRegionViewModel;
        private RegionsStorage storage;

        public WindowDispatcher(RegionsStorage storage)
        {
            this.storage = storage;
            this.screensaverViewModel = new ScreensaverViewModel();
            this.selectRegionViewModel = new SelectRegionViewModel(storage.Regions);
            Subscribe();
            ActiveModel = this.screensaverViewModel;
        }
        

        public Kernel.ViewModel ActiveModel
        {
            get { return this.activeModel; }
            set
            {
                this.activeModel = value;
                OnPropertyChanged(nameof(ActiveModel));
            }
        }


        private void Subscribe()
        {
            this.screensaverViewModel.NextCommandExecuted += ScreensaverViewModelOnNextCommandExecuted;
            this.selectRegionViewModel.HomeCommandExecuted += ViewModelOnHomeCommandExecuted;
            this.selectRegionViewModel.NextCommandExecuted += SelectRegionViewModelOnNextCommandExecuted;
        }

        #region Screensaver

        private void ScreensaverViewModelOnNextCommandExecuted(object sender, EventArgs eventArgs)
        {
            ActiveModel = this.selectRegionViewModel;
        }

        #endregion

        #region SelectRegion

        private void SelectRegionViewModelOnNextCommandExecuted(object sender, EventArgs eventArgs)
        {
            var paintViewModel = new PaintViewModel(this.selectRegionViewModel.SelectedRegion.Region);
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
                var resultViewModel = new ResultViewModel(paintViewModel.RegionSymbol, paintViewModel.SourceImage, 99);
                //TODO: получение процентов
                resultViewModel.HomeCommandExecuted += ViewModelOnHomeCommandExecuted;
                resultViewModel.Disposing += ResultViewModelOnDisposing;
                paintViewModel.Dispose();
                ActiveModel = resultViewModel;
            }
        }

        private void PaintViewModelOnBackCommandExecuted(object sender, EventArgs eventArgs)
        {
            var paintViewModel = sender as Kernel.ViewModel;
            paintViewModel?.Dispose();
            ActiveModel = this.selectRegionViewModel;
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

        private void ViewModelOnHomeCommandExecuted(object sender, EventArgs eventArgs)
        {
            var resultViewModel = sender as Kernel.ViewModel;
            if (resultViewModel != this.selectRegionViewModel)
            {
                resultViewModel?.Dispose();
            }
            ActiveModel = this.screensaverViewModel;
        }
        
    }
}
