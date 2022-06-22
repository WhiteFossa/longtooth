namespace longtooth.Common.Abstractions.DTOs.ClientService
{
    /// <summary>
    /// Information about file
    /// </summary>
    public class FileMetadata
    {
        /// <summary>
        /// Returns true if file exist
        /// </summary>
        public bool IsExist { get; private set; }

        /// <summary>
        /// Filename
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Full path to file
        /// </summary>
        public string Path { get; private set; }

        public FileMetadata(bool isExist, string name, string path)
        {
            IsExist = isExist;
            Name = name;
            Path = path;
        }
    }
}
