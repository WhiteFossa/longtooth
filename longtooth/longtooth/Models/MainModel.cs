using longtooth.Abstractions.Interfaces.Models;

namespace longtooth.Models
{
    public class MainModel : IMainModel
    {
        /// <summary>
        /// If true, then we have root access
        /// </summary>
        public bool IsRootAccess { get; set; }
    }
}
