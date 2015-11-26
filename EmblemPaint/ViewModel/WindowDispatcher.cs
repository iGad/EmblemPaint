using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using EmblemPaint.Kernel;
using EmblemPaint.View;
using Microsoft.Practices.Prism.Commands;

namespace EmblemPaint.ViewModel
{
    public class WindowDispatcher: FunctionalViewModel
    {
        private double windowHeight, windowWidth;
        private readonly Timer timer;
        private FunctionalViewModel activeModel;


        public WindowDispatcher(Configuration configuration, IList<FunctionalViewModel> models):base(configuration)
        {
            CloseCommand = new DelegateCommand(Close);
            Configuration = configuration;
            WindowHeight = configuration.WindowHeight;
            WindowWidth = configuration.WindowWidth;
            this.timer = new Timer(configuration.WaitUserActionInterval * 1000);
            this.timer.Elapsed += OnTimerElapsed;
            Models = models;
            Subscribe();
            ActiveModel = Models.First();
        }

        #region Properties

        /// <summary>
        /// Список моделей окон
        /// </summary>
        public IList<FunctionalViewModel> Models { get; } 

        /// <summary>
        /// Текущая модель окна
        /// </summary>
        public FunctionalViewModel ActiveModel
        {
            get { return this.activeModel; }
            set
            {
                this.activeModel = value;
                if (this.activeModel != Models.First())
                {
                    ResetTimer();
                }
                else
                {
                    this.timer.Stop();
                }
                OnPropertyChanged(nameof(ActiveModel));
            }
        }

        /// <summary>
        /// Высота окна
        /// </summary>
        public double WindowHeight
        {
            get { return this.windowHeight; }
            set
            {
                if (!this.windowHeight.Equals(value))
                {
                    this.windowHeight = value;
                    Configuration.WindowHeight = Convert.ToInt32(value);
                    OnPropertyChanged(nameof(WindowHeight));
                }
            }
        }

        /// <summary>
        /// Ширина окна
        /// </summary>
        public double WindowWidth
        {
            get { return this.windowWidth; }
            set
            {
                if (!this.windowWidth.Equals(value))
                {
                    this.windowWidth = value;
                    Configuration.WindowWidth = Convert.ToInt32(value);
                    OnPropertyChanged(nameof(WindowWidth));
                }
            }
        }

        /// <summary>
        /// Команда закрытия окна
        /// </summary>
        public DelegateCommand CloseCommand { get; }


        #endregion

        private void Subscribe()
        {
            foreach (var functionalViewModel in Models)
            {
                functionalViewModel.BackCommandExecuted += FunctionalViewModelOnBackCommandExecuted;
                functionalViewModel.NextCommandExecuted += FunctionalViewModelOnNextCommandExecuted;
                functionalViewModel.HomeCommandExecuted += FunctionalViewModelOnHomeCommandExecuted;
            }
        }

        private void FunctionalViewModelOnHomeCommandExecuted(object sender, EventArgs<bool> eventArgs)
        {
            if (AskUserConfirmation(eventArgs.Parameter))
            {
                ActiveModel = Models.First();
            }
        }

        private void FunctionalViewModelOnNextCommandExecuted(object sender, EventArgs eventArgs)
        {
            int index = Models.IndexOf(ActiveModel);
            if (index < Models.Count - 1)
            {
                Models[index + 1].Reconfigure(ActiveModel.Configuration);
                ActiveModel = Models[index + 1];
            }
        }

        private void FunctionalViewModelOnBackCommandExecuted(object sender, EventArgs eventArgs)
        {
            int index = Models.IndexOf(ActiveModel);
            if (index > 0)
            {
                Models[index - 1].Reconfigure(ActiveModel.Configuration);
                ActiveModel = Models[index - 1];
            }
        }

     
        private bool AskUserConfirmation(bool needAsking)
        {
            if (needAsking)
            {
                var askUserView = new ConfirmView {Owner = Application.Current.MainWindow};
                return askUserView.ShowDialog() ?? false;
            }
            return true;
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

        private void Close()
        {
            SaveConfiguration();
        }

        private void SaveConfiguration()
        {
            string tempFileName = "Temp" + Constants.DefaultConfigurationName;
            try
            {
                if (File.Exists(Constants.DefaultConfigurationName))
                {
                    File.Copy(Constants.DefaultConfigurationName, tempFileName);
                }
                using (Stream sw = new FileStream(Constants.DefaultConfigurationName, FileMode.Create, FileAccess.Write))
                {
                    Configuration.Save(sw);
                }
            }
            catch
            {
                if (File.Exists(tempFileName))
                {
                    if (File.Exists(Constants.DefaultConfigurationName))
                    {
                        File.Delete(Constants.DefaultConfigurationName);
                    }
                    File.Move(tempFileName, Constants.DefaultConfigurationName);
                }
            }
        }

        protected override void OnActionHappened()
        {
            if (this.timer.Enabled)
            {
                ResetTimer();
            }
        }
        
    }
}
