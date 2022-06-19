using longtooth.Abstractions.Interfaces.Models;
using longtooth.ViewModels;

namespace longtooth.Models
{
    public class MainModel : IMainModel
    {
        /// <summary>
        /// If true, then we have root access
        /// </summary>
        public bool IsRootAccess { get; set; }

        /// <summary>
        /// Main page view model (to update it's properties from other pages)
        /// </summary>
        public MainPageViewModel MainPageViewModel;
    }
}
