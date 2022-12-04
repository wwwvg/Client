using System.Collections.Generic;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Documents;
using Prism.Commands;
using System.Text;
using System.Windows.Input;
using System.Net.Sockets;
using System.IO;
using System;
using Prism.Events;
using Client.Events;
using Client.Services.Interfaces;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Threading;

namespace Client.ViewModels
{
    public class ContentViewModel : BindableBase
    {
        public const int MaxBytes = 255;
        int _numberOfRequests = 0;
        int _numberOfResponses = 0;
        DispatcherTimer _timer = new DispatcherTimer(); 

        IEventAggregator _eventAggregator;          //  для отправки сообщений в статус бар
        IProcessDataService _processDataService;        // проверка вводимых данных и преобразование их в байты
        IBytesGeneratorService _bytesGeneratorService;   //  генератор случайных байтов и создатель пакета для отправки
        IHexConverterService _hexConverterService;      // преобразование байтов в строковое представление

        public ContentViewModel(IEventAggregator eventAggregator, IProcessDataService processDataService, 
                                                                  IBytesGeneratorService bytesGeneratorService,
                                                                  IHexConverterService hexConverterService)  // все сервисы зарегистрированы в App.xaml.cs
        {
            _eventAggregator = eventAggregator;
            _hexConverterService = hexConverterService;
            _processDataService = processDataService;
            _bytesGeneratorService = bytesGeneratorService;

            Bytes.AddRange(Enumerable.Range(0, MaxBytes));
            IsStarted = false;
            IsStopped = true;
            _isRandomCheckBoxEnabled = false;
            _isRandomData = false;
        }

        #region Properties
        private string _resultText;
        public string ResultText { get => _resultText; set => SetProperty(ref _resultText, value); }

        private string _packageText;
        public string PackageText { get => _packageText; set => SetProperty(ref _packageText, value); }

        private string _dataMaskValue; // поле для ввода данных
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

        private bool _isRandomCheckBoxEnabled;
        public bool IsRandomCheckBoxEnabled { get => _isRandomCheckBoxEnabled; set => SetProperty(ref _isRandomCheckBoxEnabled, value); }

        private bool _isRandomData;
        public bool IsRandomData { get => _isRandomData; set => SetProperty(ref _isRandomData, value); }

        private bool _isExtraBytesSelected;
        public bool IsExtraBytesSelected { get => _isExtraBytesSelected; set => SetProperty(ref _isExtraBytesSelected, value); }

        private bool _isIncorrectInput;
        public bool IsIncorrectInput { get => _isIncorrectInput; set => SetProperty(ref _isIncorrectInput, value); }

        private string _controlSum;
        public string ControlSum { get => _controlSum; set => SetProperty(ref _controlSum, value); }
        #endregion

        #region Commands    
        private DelegateCommand _trashComboBoxSelected1; // ВЫБОР ЭЛЕМЕНТА В КОМБОБОКСЕ 1
        public DelegateCommand TrashComboBoxSelected1 => _trashComboBoxSelected1 ?? (_trashComboBoxSelected1 = new DelegateCommand(OnTrashComboBoxSelected1));

        private DelegateCommand _trashComboBoxSelected2; // ВЫБОР ЭЛЕМЕНТА В КОМБОБОКСЕ 2
        public DelegateCommand TrashComboBoxSelected2 => _trashComboBoxSelected2 ?? (_trashComboBoxSelected2 = new DelegateCommand(OnTrashComboBoxSelected2));

        private DelegateCommand _dataComboBoxSelected; // ВЫБОР ЭЛЕМЕНТА В КОМБОБОКСЕ 3
        public DelegateCommand DataComboBoxSelected => _dataComboBoxSelected ?? (_dataComboBoxSelected = new DelegateCommand(OnDataComboBoxSelected));

        private DelegateCommand _startCommand;      // СТАРТ
        public DelegateCommand StartCommand => _startCommand ?? (_startCommand = new DelegateCommand(ExecuteStartCommand))
                                                                                                            .ObservesCanExecute(() => IsStopped);
        private DelegateCommand _stopCommand;       // СТОП
        public DelegateCommand StopCommand => _stopCommand ?? (_stopCommand = new DelegateCommand(ExecuteStopCommand))
                                                                                                          .ObservesCanExecute(() => IsStarted);

        #endregion  

        #region Methods
        async Task<string> SendData(byte[] data)            //  отправка данных
        {
            using TcpClient tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("localhost", 55555);

            var stream = tcpClient.GetStream();
            await stream.WriteAsync(data);
            int lowByte = stream.ReadByte();
            int highByte = stream.ReadByte() * 256;
            if (lowByte == -1 || highByte == -1)
                return "";
            return (lowByte + highByte).ToString("X");
        }

        private int FreeBytesAmount()
        {
            return MaxBytes - SelectedIndexTrash1 - SelectedIndexTrash2 - SelectedIndexData -1;
        }

        private void SetFreeBytesText()         // счетчик оставшихся байтов
        {
            int amount = FreeBytesAmount();
            if (amount < 0)
            {
                ExecuteStopCommand();
                FreeBytesMessage = $"Уменьшите количество байтов на {-amount}";
                IsExtraBytesSelected = true;
            }
            else
            {
                FreeBytesMessage = $"Количество оставшихся байтов: {amount}";
                IsExtraBytesSelected = false;
            }
        }

        #endregion

        #region Event Handlers
        public void ExecuteStartCommand()                               //    СТАРТ
        {
            if (FreeBytesAmount() < 1)
                return;

            IsStarted = true;
            IsStopped = false;
            ResultText = "";

            _timer.Tick += OnTimerTick;
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();
        }

        public void ExecuteStopCommand()                                //    СТОП
        {
            _timer.Stop();
            IsStopped = true;
            IsStarted = false;
        }

        async void OnTimerTick(object sender, EventArgs args)           //    ТАЙМЕР 
        {
            if (IsRandomData && !IsStopped)
            {
                while (_numberOfRequests != _numberOfResponses)
                    await Task.Delay(1000);
                var result = GetRandomData();
                DataMaskValue = result.Item1;
                ControlSum = result.Item2;
            }

            bool isDataChecked = _processDataService.CheckData(DataMaskValue, SelectedIndexData, out string errorMessage); // проверка вводимых данных
            if (isDataChecked)
            {
                _eventAggregator.GetEvent<StatusBarMessage>().Publish((false, $"{errorMessage}"));
                IsIncorrectInput = false;
            }
            else
            {
                _eventAggregator.GetEvent<StatusBarMessage>().Publish((true, $"{errorMessage}"));
                IsIncorrectInput = true;
                return;
            }
            byte[] data = new byte[SelectedIndexData];
            _processDataService.GetBytes(DataMaskValue, out data, SelectedIndexData, out string message, _hexConverterService); // поручение байтов данных

            if(data == null)
            {
                return;
            }

            byte[] package = _bytesGeneratorService.GetBytes(SelectedIndexTrash1, data, SelectedIndexTrash2); // получение байтов посылки целиком
            PackageText = _hexConverterService.ToHex(package);

            if (package is null)            // если пустой пакет, то выход
            {
                return;
            }

            try
            {
                _numberOfRequests++;
                ResultText = await SendData(package);       // вывод полученного от сервера результата
                _numberOfResponses++;

            }
            catch (Exception ex)
            {
                _eventAggregator.GetEvent<StatusBarMessage>().Publish((true, $"{ex.Message}")); // отправка сообщения об ошибки в статусбар
            }
        }

        public void OnTrashComboBoxSelected1()          // ВЫБОР В КОМБОБОКСЕ 1
        {
            SetFreeBytesText();
        }

        public void OnTrashComboBoxSelected2()          // ВЫБОР В КОМБОБОКСЕ 2
        {
            SetFreeBytesText();
        }

        public void OnDataComboBoxSelected()            // ВЫБОР В КОМБОБОКСЕ 3
        {
            SetFreeBytesText();
            if(SelectedIndexData >= 3)
                IsRandomCheckBoxEnabled = true;
            else
                IsRandomCheckBoxEnabled = false;
        }

        (string, string) GetRandomData()
        {
            int lenght = SelectedIndexData - 2;
            byte[] data = new byte[lenght];
            data = _bytesGeneratorService.GetBytes(lenght);
            string hex = _hexConverterService.ToHex(data);
            return ("0a " + hex + " 0b", GetSum(data));
        }

        string GetSum(byte[] data)
        {
            int sum = 0;
            for (int i = 0; i < data.Length; i++) // подсчет
            {
                sum += data[i];
            }
            return sum.ToString("X");
        }

        #endregion  
    }
}
