using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using EmblemPaint.Kernel;
using Prism.Commands;
// ReSharper disable ExplicitCallerInfoArgument

namespace EmblemPaint.ViewModel
{
    public class SelectRegionViewModel : FunctionalViewModel
    {
        public readonly int VerticalItemsCount;
        public readonly int HorizontalItemsCount;
        public const double DefaultItemTopMargin = 5;
        private double visibleWidth, horizontalOffset, allWidth, allHeight, itemWidth, itemHeight, increment;
        private Thickness margin = new Thickness(5);
        private RegionViewModel selectedRegion;

        public SelectRegionViewModel(Configuration configuration):base(configuration)
        {
            this.VerticalItemsCount = configuration.VerticalItemsCount;
            this.HorizontalItemsCount = configuration.HorizontalItemsCount;
            this.allWidth = configuration.WindowWidth;
            this.allHeight = configuration.WindowHeight*(double) 5/9;
            this.visibleWidth = configuration.WindowWidth - configuration.WindowWidth/7;
           

            ComputeItemsSize();
            FillRegions(configuration.Storage.Regions);
            MoveToLeftCommand = new DelegateCommand(MoveToLeft);
            MoveToRightCommand = new DelegateCommand(MoveToRight);
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
                Configuration.SelectedRegion = this.selectedRegion.Region;
                Dispatcher.CurrentDispatcher.Invoke(() => NextCommand.RaiseCanExecuteChanged());
            }
        }

        /// <summary>
        /// Ширина видимой области
        /// </summary>
        public double VisibleWidth
        {
            get { return this.visibleWidth; }
            set
            {
                if (!this.visibleWidth.Equals(value) && value > 0)
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
        /// Горизонтальный отступ от левой границы окна
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
        /// Вся ширина области выбора региона
        /// </summary>
        public double AllWidth
        {
            get { return this.allWidth; }
            set
            {
                if (!this.allWidth.Equals(value) && value > 0)
                {
                    this.allWidth = value;
                    OnPropertyChanged(nameof(AllWidth));
                    OnPropertyChanged(nameof(CanMoveToLeft));
                    OnPropertyChanged(nameof(CanMoveToRight));
                }
            }
        }

        /// <summary>
        /// Высота области выбора региона
        /// </summary>
        public double AllHeight
        {
            get { return this.allHeight; }
            set
            {
                if (!this.allHeight.Equals(value) && value > 0)
                {
                    this.allHeight = value;
                    OnPropertyChanged(nameof(AllHeight));
                    ComputeItemsSize();
                }
            }
        }

        /// <summary>
        /// Ширина ListViewItem
        /// </summary>
        public double ItemWidth
        {
            get { return this.itemWidth; }
            set
            {
                if (!this.itemWidth.Equals(value))
                {
                    this.itemWidth = value;
                    OnPropertyChanged(nameof(ItemWidth));
                }
            }
        }

        /// <summary>
        /// Высота ListViewItem
        /// </summary>
        public double ItemHeight
        {
            get { return this.itemHeight; }
            set
            {
                if (!this.itemHeight.Equals(value))
                {
                    this.itemHeight = value;
                    OnPropertyChanged(nameof(ItemHeight));
                }
            }
        }


        /// <summary>
        /// Отступ между элементами TreeView
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
            ItemHeight = AllHeight/this.VerticalItemsCount - Margin.Top*4 - 7;
            ItemWidth = VisibleWidth/this.HorizontalItemsCount - Margin.Left *2 - 8;
           
            this.increment = ItemWidth + this.Margin.Left*2 + 6;
        }

      
        
        #region Commands
        
        /// <summary>
        /// Команда скролла влево
        /// </summary>
        public DelegateCommand MoveToLeftCommand { get; }

        /// <summary>
        /// Разрешение на выполенение команды скролла влево
        /// </summary>
        public bool CanMoveToLeft => HorizontalOffset > 0;

        /// <summary>
        /// Команда скрола вправо
        /// </summary>
        public DelegateCommand MoveToRightCommand { get; }

        /// <summary>
        /// Разрешение на выполенение команды скролла вправо
        /// </summary>
        public bool CanMoveToRight => HorizontalOffset < AllWidth - VisibleWidth;


        private void MoveToLeft()
        {
            if (HorizontalOffset < Constants.DefaultRegionSize)
            {
                HorizontalOffset = 0;
            }
            else
            {
                HorizontalOffset -= this.increment;
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
                HorizontalOffset += this.increment;
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
        
        /// <summary>
        /// Блокировка кнопки Далее, если не выбран регион
        /// </summary>
        /// <returns></returns>
        protected override bool CanExecuteNextCommand()
        {
            return SelectedRegion != null;
        }
        
        #endregion
    }
}
