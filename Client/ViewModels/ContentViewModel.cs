using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using Prism.Commands;

namespace Client.ViewModels
{
    public class ContentViewModel : BindableBase
    {
        public const int MaxBytes = 256;
        public ContentViewModel()
        {
            Bytes.AddRange(Enumerable.Range(0, MaxBytes));

        }

        #region Properties
        private ObservableCollection<int> _bytes = new();
        public ObservableCollection<int> Bytes { get => _bytes; set => SetProperty(ref _bytes, value); }

        private string _freeBytes;
        public string FreeBytes { get => _freeBytes; set => SetProperty(ref _freeBytes, value); }


        private int _selectedIndexTrash1;
        public int SelectedIndexTrash1 { get => _selectedIndexTrash1; set => SetProperty(ref _selectedIndexTrash1, value); }

        private int _selectedIndexTrash2;
        public int SelectedIndexTrash2 { get => _selectedIndexTrash2; set => SetProperty(ref _selectedIndexTrash2, value); }

        private int _selectedIndexData;
        public int SelectedIndexData { get => _selectedIndexData; set => SetProperty(ref _selectedIndexData, value); }
        #endregion

        #region Commands
        private DelegateCommand _trashComboBoxSelected1; // ВЫБОР ЭЛЕМЕНТА В КОМБОБОКСЕ 1
        public DelegateCommand TrashComboBoxSelected1 => _trashComboBoxSelected1 ?? (_trashComboBoxSelected1 = new DelegateCommand(OnTrashComboBoxSelected1));

        public void OnTrashComboBoxSelected1()
        {
            SetFreeBytesText();
        }

        private DelegateCommand _trashComboBoxSelected2; // ВЫБОР ЭЛЕМЕНТА В КОМБОБОКСЕ 2
        public DelegateCommand TrashComboBoxSelected2 => _trashComboBoxSelected2 ?? (_trashComboBoxSelected2 = new DelegateCommand(OnTrashComboBoxSelected2));

        public void OnTrashComboBoxSelected2()
        {
            SetFreeBytesText();
        }

        private DelegateCommand _dataComboBoxSelected; // ВЫБОР ЭЛЕМЕНТА В КОМБОБОКСЕ 2
        public DelegateCommand DataComboBoxSelected => _dataComboBoxSelected ?? (_dataComboBoxSelected = new DelegateCommand(OnDataComboBoxSelected));

        public void OnDataComboBoxSelected()
        {
            SetFreeBytesText();
        }

        
        #endregion

        #region Methods

        private int FreeBytesAmount()
        {
            return MaxBytes - Bytes[SelectedIndexTrash1] - Bytes[SelectedIndexTrash2] - Bytes[SelectedIndexData] -1;
        }

        private void SetFreeBytesText()
        {
            int amount = FreeBytesAmount();
            if(amount < 0)
                FreeBytes = $"Ошибка! Количество оставшихся байтов: {amount}";
            else
                FreeBytes = $"Количество оставшихся байтов: {amount}";
        }
        #endregion

        #region Event Handlers

        #endregion  
    }
}
