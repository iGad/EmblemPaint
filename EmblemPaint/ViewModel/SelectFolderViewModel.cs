using System.IO;
using System.Linq;
using System.Windows.Forms;
using EmblemPaint.Kernel;
using Prism.Commands;

namespace EmblemPaint.ViewModel
{
    public class SelectFolderViewModel : FunctionalViewModel
    {
        private string selectedFolderPath;
        public SelectFolderViewModel(Configuration configuration) : base(configuration)
        {
            this.selectedFolderPath = "Gerby";
            SelectFolderCommand = new DelegateCommand(SelectFolder);
        }

        public string SelectedFolderPath
        {
            get { return this.selectedFolderPath; }
            set
            {
                if (!this.selectedFolderPath.Equals(value))
                {
                    this.selectedFolderPath = value;
                    OnPropertyChanged(nameof(SelectedFolderPath));
                }
            }
        }

        public DelegateCommand SelectFolderCommand { get; }


        private void SelectFolder()
        {
            var selectFolderDialog = new FolderBrowserDialog {SelectedPath = SelectedFolderPath};
            if (selectFolderDialog.ShowDialog() == DialogResult.OK)
            {
                SelectedFolderPath = selectFolderDialog.SelectedPath;
            }
        }

        protected override void Next()
        {
            RegionsStorage storage = CreateRegionsStorage();
            base.Configuration.Storage = storage;
            base.Next();
        }

        private RegionsStorage CreateRegionsStorage()
        {
            DirectoryInfo di = new DirectoryInfo(SelectedFolderPath);
            var files = di.GetFiles("*.png");
            RegionsStorage storage = new RegionsStorage();
            storage.Regions.AddRange(files.Select(file => new Region(file.Name) {ThumbnailImageName = file.FullName}));
            return storage;
        }
    }
}
