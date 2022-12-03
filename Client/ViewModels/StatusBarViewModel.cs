using Client.Events;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Client.ViewModels
{
    public class StatusBarViewModel : BindableBase
    {
        public StatusBarViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<ConnectionStatusChanged>().Subscribe(SetConnectionStatus);
            eventAggregator.GetEvent<StatusBarMessage>().Subscribe(SetMessage);

            // подписка на уведомления
            // теперь любая часть программы, посредством IEventAggregator, может посылать уведомления в статусбар, при этом не имея на него прямых ссылок
        }

        #region Properties

        private string _message;
        public string Message { get => _message; set => SetProperty(ref _message, value); }

        private string _connectionStatus;
        public string ConnectionStatus { get => _connectionStatus; set => SetProperty(ref _connectionStatus, value); }

        private ImageSource _warningIcon;
        public ImageSource WarningIcon { get => _warningIcon; set => SetProperty(ref _warningIcon, value); }

        private bool _isError;
        public bool IsError { get => _isError; set => SetProperty(ref _isError, value); }

        #endregion


        #region Methods      
        private void SetWarningIcon(bool isError)
        {
            if (isError)
            {
                WarningIcon = new BitmapImage(new Uri("/Client;component/Icons/Warning.png", UriKind.Relative));
                IsError = true;
            }
            else
            {
                WarningIcon = new BitmapImage();
                IsError = false;
            }
        }
        #endregion

        #region Event Handlers
        private void SetMessage((bool, string) message)
        {
            SetWarningIcon(message.Item1);
            Message = message.Item2;
        }

        private void SetConnectionStatus(string status)
        {
            ConnectionStatus = status;
        }
        #endregion
    }
}
