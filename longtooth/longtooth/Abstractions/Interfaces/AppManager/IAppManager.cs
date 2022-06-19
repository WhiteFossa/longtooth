namespace longtooth.Abstractions.Interfaces.AppManager
{
    /// <summary>
    /// Interface to work with mobile application
    /// </summary>
    public interface IAppManager
    {
        /// <summary>
        /// Gracefully closes app
        /// </summary>
        void CloseApp();
    }
}
