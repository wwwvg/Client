using System.Collections.Generic;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using Prism.Commands;
using System.Text;

namespace Client.ViewModels
{
    public class ContentViewModel : BindableBase
    {
        public const int MaxBytes = 256;
        
      
        public ContentViewModel()
        {
            Bytes.AddRange(Enumerable.Range(0, MaxBytes));
            IsStarted = false;
            IsStopped = true;
        }
        
        #region Properties
        private string _dataMask; // маска для ввода данных, генерируется в зависимости от кол-ва байт данных;
        public string DataMask { get => _dataMask; set => SetProperty(ref _dataMask, value); }

        private string _dataMaskValue; // маска для ввода данных, генерируется в зависимости от кол-ва байт данных;
        public string DataMaskValue { get => _dataMaskValue; set => SetProperty(ref _dataMaskValue, value); }

        private bool _isStarted; // нажата ли кнопка старт. Свойство нужно для блокирования/разблокирования других кнопок
        public bool IsStarted { get => _isStarted; set => SetProperty(ref _isStarted, value); }

        private bool _isStopped; // нажата ли кнопка стоп. Свойство нужно для блокирования/разблокирования других кнопок
        public bool IsStopped { get => _isStopped; set => SetProperty(ref _isStopped, value); }

        private List<int> _bytes = new(); // список для комбобоксов
        public List<int> Bytes { get => _bytes; set => SetProperty(ref _bytes, value); }

        private string _freeBytesMessage; // сообщение/предупреждение о количестве доступных байтов
        public string FreeBytesMessage { get => _freeBytesMessage; set => SetProperty(ref _freeBytesMessage, value); }


        private int _selectedIndexTrash1; // выбранный индекс в 1-м комбобоксе для подсчета доступных байт
        public int SelectedIndexTrash1 { get => _selectedIndexTrash1; set => SetProperty(ref _selectedIndexTrash1, value); }

        private int _selectedIndexTrash2; // выбранный индекс во 2-м комбобоксе для подсчета доступных байт
        public int SelectedIndexTrash2 { get => _selectedIndexTrash2; set => SetProperty(ref _selectedIndexTrash2, value); }

        private int _selectedIndexData; // выбранный индекс в 3-тьем комбобоксе для подсчета доступных байт
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

        private DelegateCommand _dataComboBoxSelected; // ВЫБОР ЭЛЕМЕНТА В КОМБОБОКСЕ 3
        public DelegateCommand DataComboBoxSelected => _dataComboBoxSelected ?? (_dataComboBoxSelected = new DelegateCommand(OnDataComboBoxSelected));

        public void OnDataComboBoxSelected()
        {
            SetFreeBytesText();
            GenerateDataMask();
        }

        private DelegateCommand _startCommand;      // СТАРТ
        public DelegateCommand StartCommand => _startCommand ?? (_startCommand = new DelegateCommand(ExecuteStartCommand))
                                                                                                            .ObservesCanExecute(() => IsStopped);

        public void ExecuteStartCommand()
        {
            IsStarted = true;
            IsStopped = false;
        }

        private DelegateCommand _stopCommand;       // СТОП
        public DelegateCommand StopCommand => _stopCommand ?? (_stopCommand = new DelegateCommand(ExecuteStopCommand))
                                                                                                            .ObservesCanExecute(() => IsStarted);

        public void ExecuteStopCommand()
        {
            IsStopped = true;
            IsStarted = false;
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
                FreeBytesMessage = $"Ошибка! Уменьшите количество байтов на: {-amount}";
            else
                FreeBytesMessage = $"Количество оставшихся байтов: {amount}";
        }

        private void GenerateDataMask()
        {
            string dataMaskValue = DataMaskValue; // сохраняем старое значение из маски данных
            int numberOfBytes = Bytes[SelectedIndexData];
            var maskBuilder = new StringBuilder();
            for (int i = 0; i < numberOfBytes; i++)
            {
                if (i != numberOfBytes - 1)
                    maskBuilder.Append(">A>A ");
                else
                    maskBuilder.Append(">A>A");
            }
            DataMask = maskBuilder.ToString();
            //DataMaskValue = dataMaskValue?.Replace(" ", "").Remove(numberOfBytes - 1);
            DataMaskValue = "AA";
        }

        #endregion

        #region Event Handlers

        #endregion  
    }
}
