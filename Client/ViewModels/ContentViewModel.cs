using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;

namespace Client.ViewModels
{
    public class ContentViewModel : BindableBase
    {
        private byte _bytesLeft = 255;
        public ContentViewModel()
        {
            TrashBytesAmount1.AddRange(Enumerable.Range(1, 255));
            TrashBytesAmount2.AddRange(Enumerable.Range(1, 255));
            DataAmount.AddRange(Enumerable.Range(1, 255));
        }

        #region Properties
        private ObservableCollection<int> _trashBytesAmount1 = new();
        public ObservableCollection<int> TrashBytesAmount1 { get => _trashBytesAmount1; set => SetProperty(ref _trashBytesAmount1, value); }

        private ObservableCollection<int> _trashBytesAmount2 = new();
        public ObservableCollection<int> TrashBytesAmount2 { get => _trashBytesAmount2; set => SetProperty(ref _trashBytesAmount2, value); }

        private ObservableCollection<int> _dataAmount = new();
        public ObservableCollection<int> DataAmount { get => _dataAmount; set => SetProperty(ref _dataAmount, value); }
        #endregion

        #region Commands
        private DelegateCommand _trashComboBoxOpenned1;
        public DelegateCommand TrashComboBoxOpenned1 => _trashComboBoxOpenned1 ?? (_trashComboBoxOpenned1 = new DelegateCommand(OnTrashComboBoxOpenned1));

        public void OnTrashComboBoxOpenned1()
        {
            
        }
        #endregion

        #region Methods

        #endregion

        #region Event Handlers

        #endregion  
    }
}
