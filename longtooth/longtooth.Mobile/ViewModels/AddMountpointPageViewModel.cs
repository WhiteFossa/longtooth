using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace longtooth.Mobile.ViewModels
{
    public class AddMountpointPageViewModel : BindableObject
    {
        #region Commands

        /// <summary>
        /// Command, executed when user selects path
        /// </summary>
        public ICommand SelectPathCommand { get; }

        /// <summary>
        /// Command, executed when user press "Accept"
        /// </summary>
        public ICommand AcceptCommand { get; }

        /// <summary>
        /// Command, executed when user press "Cancel"
        /// </summary>
        public ICommand CancelCommand { get; }

        #endregion

        private string _name;

        /// <summary>
        /// Mountpoint name
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public AddMountpointPageViewModel()
        {
            SelectPathCommand = new Command(async () => await OnSelectPathAsync());
            AcceptCommand = new Command(async () => await OnAcceptAsync());
            CancelCommand = new Command(async () => await OnCancelAsync());
        }

        public async Task OnSelectPathAsync()
        {

        }

        public async Task OnAcceptAsync()
        {

        }

        public async Task OnCancelAsync()
        {

        }

    }
}
