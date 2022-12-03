﻿using System.Collections.Generic;
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
        DispatcherTimer _timer = new DispatcherTimer(); 
        private IEventAggregator _eventAggregator;
        private IProcessDataService _processDataService;
        private ITrashGeneratorService _trashGeneratorService;
        private IHexConverterService _hexConverterService;

        public ContentViewModel(IEventAggregator eventAggregator, IProcessDataService processDataService, 
                                                                  ITrashGeneratorService trashGeneratorService,
                                                                  IHexConverterService hexConverterService)  // все сервисы зарегистрированы в App.xaml.cs
        {
            _eventAggregator = eventAggregator;
            _hexConverterService = hexConverterService;
            _processDataService = processDataService;
            _trashGeneratorService = trashGeneratorService;

            Bytes.AddRange(Enumerable.Range(0, MaxBytes));
            IsStarted = false;
            IsStopped = true;
            _isMaskEnabled = false;
            
        }

        #region Properties
        private string _resultText;
        public string ResultText { get => _resultText; set => SetProperty(ref _resultText, value); }

        private string _packageText;
        public string PackageText { get => _packageText; set => SetProperty(ref _packageText, value); }

        private string _dataMask; // маска для ввода данных, генерируется в зависимости от кол-ва байт данных;
        public string DataMask { get => _dataMask; set => SetProperty(ref _dataMask, value); }

        private string _dataMaskValue; // маска для ввода данных, генерируется в зависимости от кол-ва байт данных;
        public string DataMaskValue { get => _dataMaskValue; set => SetProperty(ref _dataMaskValue, value); }

        private bool _isMaskEnabled;
        public bool IsMaskEnabled { get => _isMaskEnabled; set => SetProperty(ref _isMaskEnabled, value); }

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
            //GenerateDataMask();
        }

        private DelegateCommand _startCommand;      // СТАРТ
        public DelegateCommand StartCommand => _startCommand ?? (_startCommand = new DelegateCommand(ExecuteStartCommand))
                                                                                                            .ObservesCanExecute(() => IsStopped);

        async public void ExecuteStartCommand()
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

        private DelegateCommand _stopCommand;       // СТОП
        public DelegateCommand StopCommand => _stopCommand ?? (_stopCommand = new DelegateCommand(ExecuteStopCommand))
                                                                                                            .ObservesCanExecute(() => IsStarted);
        public void ExecuteStopCommand()
        {
            _timer.Stop();
            IsStopped = true;
            IsStarted = false;
            if (DataMaskValue?.Length > 0)
                IsMaskEnabled = true;
        }
        #endregion

        #region Methods
        async Task<string> SendData(byte[] data)
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

        byte[] CreatePackageText()
        {
            PackageText = "";
            byte[] bytes1, bytes2;

            if (SelectedIndexTrash1 != 0)
            {
                bytes1 = _trashGeneratorService.GetBytes(SelectedIndexTrash1);
                PackageText = $"{_hexConverterService.ToHex(bytes1)} ";
            }

            if (_processDataService.GetBytes(DataMaskValue, out byte[] bytesData, SelectedIndexData, out string message, _hexConverterService) && SelectedIndexData != 0 && bytesData != null)
                PackageText += $"*{_hexConverterService.ToHex(bytesData)}* ";

            if (SelectedIndexTrash2 != 0)
            {
                bytes2 = _trashGeneratorService.GetBytes(SelectedIndexTrash2);
                PackageText += $"{_hexConverterService.ToHex(bytes2)}";
            }
            return bytesData;
        }

        private int FreeBytesAmount()
        {
            return MaxBytes - Bytes[SelectedIndexTrash1] - Bytes[SelectedIndexTrash2] - Bytes[SelectedIndexData] -1;
        }

        private void SetFreeBytesText()
        {
            int amount = FreeBytesAmount();
            if (amount < 0)
            {
                _timer?.Stop();
                FreeBytesMessage = $"Уменьшите количество байтов на {-amount}";
            }
            else
            {
                FreeBytesMessage = $"Количество оставшихся байтов: {amount}";
            }
        }
/*
        private void GenerateDataMask()
        {
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
        }
*/
        #endregion

        #region Event Handlers
        async void OnTimerTick(object sender, EventArgs args)
        {
            //ResultText = "";
            bool b = _processDataService.CheckData(DataMaskValue, Bytes[SelectedIndexData], out string errorMessage);
            if (b)
                _eventAggregator.GetEvent<StatusBarMessage>().Publish((false, $"{errorMessage}"));
            else
            {
                _eventAggregator.GetEvent<StatusBarMessage>().Publish((true, $"{errorMessage}"));
                return;
            }

            byte[] bytesData = CreatePackageText();
            if (bytesData is null)
                return;

            try
            {
                ResultText = await SendData(bytesData);
            }
            catch (SocketException ex)
            {
                _eventAggregator.GetEvent<StatusBarMessage>().Publish((true, $"{ex.Message}"));
            }
        }
        #endregion  
    }
}
