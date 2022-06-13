namespace longtooth.Abstractions.DTOs
{
    /// <summary>
    /// DTO with result of requesting root access
    /// </summary>
    public class RootRequestResultDto
    {
        /// <summary>
        /// If true, then device is rooted
        /// </summary>
        public bool IsRooted { get; private set; }

        /// <summary>
        /// If true, then app got root access
        /// </summary>
        public bool IsRootAccess { get; private set; }

        public RootRequestResultDto(bool isRooted, bool isRootAccess)
        {
            IsRooted = isRooted;
            IsRootAccess = isRootAccess;
        }
    }
}
