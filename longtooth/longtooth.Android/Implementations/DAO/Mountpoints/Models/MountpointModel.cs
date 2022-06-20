using SQLite;

namespace longtooth.Droid.Implementations.Mountpoints.Models
{
    /// <summary>
    /// Mountpoint
    /// </summary>
    public class MountpointModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// Mountpoint name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Mountpoint server side path
        /// </summary>
        public string ServerSidePath { get; set; }
    }
}