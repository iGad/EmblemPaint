using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using EmblemPaint.Kernel;
using Prism.Commands;

namespace EmblemPaint.ViewModel
{
    public class SelectRegionViewModel : FunctionalViewModel
    {
        public const int ItemsCountByHeight = 3;
        public const int ItemsCountByWidth = 3;
        public const double DefaultItemTopMargin = 5;
        private double visibleWidth, horizontalOffset, allWidth, allHeight, itemSize = Constants.DefaultRegionSize, increment;
        private Thickness margin;
        private RegionViewModel selectedRegion;
        private View.PaintViewOld paintViewOld;

        public SelectRegionViewModel(IEnumerable<Region> regions)
        {
            //заполняем коллекцию регионов
            FillRegions(regions);
            MoveToLeftCommand = new DelegateCommand(MoveToLeft);
            MoveToRightCommand = new DelegateCommand(MoveToRight);
            //ScreensaverWindow = new View.ScreensaverView(new ScreensaverViewModel());
            //ScreensaverWindow.Closed += ScreensaverWindow_Closed;
            //ScreensaverWindow.IsVisibleChanged += ScreensaverWindow_IsVisibleChanged;
            //ShowScreensaverView();
        }

        private void ScreensaverWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (ScreensaverWindow.Visibility == Visibility.Hidden)
            {
                StartTimer();
            }
        }

        private void ScreensaverWindowOnStateChanged(object sender, EventArgs eventArgs)
        {
            if(ScreensaverWindow.Visibility == Visibility.Hidden)
            StartTimer();
        }

        private void FillRegions(IEnumerable<Region> regions)
        {
            foreach (var region in regions)
            {
                Regions.Add(new RegionViewModel(region));
            }
        }

        #region Properties

        public ObservableCollection<RegionViewModel> Regions { get; } = new ObservableCollection<RegionViewModel>();

        public RegionViewModel SelectedRegion
        {
            get { return this.selectedRegion; }
            set
            {
                this.selectedRegion = value;
                OnPropertyChanged(nameof(SelectedRegion));
                Dispatcher.CurrentDispatcher.Invoke(() => GoNextCommand.RaiseCanExecuteChanged());
            }
        }

        public double VisibleWidth
        {
            get { return this.visibleWidth; }
            set
            {
                if (!this.visibleWidth.Equals(value))
                {
                    this.visibleWidth = value;
                    OnPropertyChanged(nameof(VisibleWidth));
                    OnPropertyChanged(nameof(CanMoveToLeft));
                    OnPropertyChanged(nameof(CanMoveToRight));
                    ComputeItemsSize();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double HorizontalOffset
        {
            get { return this.horizontalOffset; }
            set
            {
                if (!this.horizontalOffset.Equals(value))
                {
                    this.horizontalOffset = value;
                    OnPropertyChanged(nameof(HorizontalOffset));
                    OnPropertyChanged(nameof(CanMoveToLeft));
                    OnPropertyChanged(nameof(CanMoveToRight));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double AllWidth
        {
            get { return this.allWidth; }
            set
            {
                this.allWidth = value;
                OnPropertyChanged(nameof(AllWidth));
                OnPropertyChanged(nameof(CanMoveToLeft));
                OnPropertyChanged(nameof(CanMoveToRight));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double AllHeight
        {
            get { return this.allHeight; }
            set
            {
                this.allHeight = value;
                OnPropertyChanged(nameof(AllHeight));
                ComputeItemsSize();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public double ItemSize
        {
            get { return this.itemSize; }
            set
            {
                if (!this.itemSize.Equals(value))
                {
                    this.itemSize = value;
                    OnPropertyChanged(nameof(ItemSize));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Thickness Margin
        {
            get { return this.margin; }
            set
            {
                if (!this.margin.Equals(value))
                {
                    this.margin = value;
                    OnPropertyChanged(nameof(Margin));
                }
            }
        }

        #endregion

        private void ComputeItemsSize()
        {
            //TODO: сделать формирование заданного количества (margin зафиксировать)
            ItemSize = AllHeight/(ItemsCountByHeight) - ItemsCountByHeight* 2 *DefaultItemTopMargin;
            
            int itemsAtHorizontal = (int) Math.Truncate(VisibleWidth/ItemSize);
            double sideMargin = (VisibleWidth -itemsAtHorizontal*ItemSize)/itemsAtHorizontal - 1;
            this.increment = sideMargin + ItemSize;
            Margin = new Thickness(sideMargin/2, DefaultItemTopMargin, sideMargin/2, DefaultItemTopMargin);
        }

        public void ShowScreensaverView()
        {
            SelectedRegion = null;
            StopTimer();
            ScreensaverWindow.Dispatcher.Invoke(() => ScreensaverWindow.ShowDialog());
        }

        private void ScreensaverWindow_Closed(object sender, EventArgs e)
        {
            Close(null);
        }
        
        #region Commands
        

        public DelegateCommand MoveToLeftCommand { get; }

        public bool CanMoveToLeft => HorizontalOffset > 0;

        public bool CanMoveToRight => HorizontalOffset < AllWidth - VisibleWidth;

        public DelegateCommand MoveToRightCommand { get; }
        

        

        

        private void MoveToLeft()
        {
            if (HorizontalOffset < Constants.DefaultRegionSize)
            {
                HorizontalOffset = 0;
            }
            else
            {
                HorizontalOffset -= this.increment; //ItemSize + Margin.Left + Margin.Right;
            }
        }
        

        private void MoveToRight()
        {
            if (HorizontalOffset > AllWidth - VisibleWidth)
            {
                HorizontalOffset = AllWidth - VisibleWidth;
            }
            else
            {
                HorizontalOffset += this.increment; //ItemSize + Margin.Left + Margin.Right;
            }
        }

        #endregion


        private void FillRegionsTest(int count)
        {
            for(int i=0;i< count;i++)
            {
                Regions.Add(new RegionViewModel(new Region("Region " + i)));
            }
        }
        #region Overrides

        //protected override void Home(bool? askUser)
        //{
        //    base.Home(askUser);
        //    //ShowScreensaverView();
        //}

        protected override bool CanExecuteGoNext()
        {
            return SelectedRegion != null;
        }

        protected override void GoNext()
        {
            StopTimer();
            //var paintViewModel = new PaintViewModel(SelectedRegion.Region);
            //paintViewModel.HomeCommandExecuted += PaintViewModelHomeCommandExecuted;
            //this.paintViewOld = new View.PaintViewOld(paintViewModel);
            //this.paintViewOld.Closing += PaintViewOldModelClosing;
            //this.paintViewOld.ShowDialog();
        }

        private void PaintViewOldModelClosing(object sender, EventArgs e)
        {
            //var paintViewModel = this.paintViewOld.DataContext as PaintViewModel;
            //if (paintViewModel != null)
            //{
            //    paintViewModel.HomeCommandExecuted -= PaintViewModelHomeCommandExecuted;
            //    this.paintViewOld.Closing -= PaintViewOldModelClosing;
            //}
            //this.paintViewOld.Close();
            //StartTimer();
        }

        private void PaintViewModelHomeCommandExecuted(object sender, EventArgs e)
        {
            //var paintViewModel = sender as PaintViewModel;
            //if (paintViewModel != null)
            //{
            //    this.paintViewOld.Closing -= PaintViewOldModelClosing;
            //    paintViewModel.HomeCommandExecuted -= PaintViewModelHomeCommandExecuted;
            //}
            //this.paintViewOld.Close();
            //ShowScreensaverView();
        }

        //protected override void GoBack(object parameter)
        //{
        //    base
        //}

        protected override void Close(Window window)
        {
            //ScreensaverWindow.Closed -= ScreensaverWindow_Closed;
            Application.Current.Shutdown();
        }

        #endregion
    }
}
