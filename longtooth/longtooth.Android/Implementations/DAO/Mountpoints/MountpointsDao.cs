using longtooth.Common.Abstractions.DAO.Mountpoints;
using longtooth.Common.Abstractions.DTOs;
using longtooth.Droid.Implementations.Mountpoints.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Android.Provider.ContactsContract.CommonDataKinds;
using Environment = System.Environment;

namespace longtooth.Droid.Implementations.DAO.Mountpoints
{
    public class MountpointsDao : IMountpointsDao
    {
        private readonly string DbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Mountpoints.db3");

        readonly SQLiteAsyncConnection _connection;

        public MountpointsDao()
        {
            _connection = new SQLiteAsyncConnection(DbPath);

            _connection.CreateTableAsync<MountpointModel>().Wait();
        }

        public async Task AddMountpointAsync(MountpointDto mountpoint)
        {
            _ = mountpoint ?? throw new ArgumentNullException(nameof(mountpoint));

            // Maybe do we already have this mountpoint?
            var possibleExistingMountpoint = await GetMountpointAsync(mountpoint.Name, mountpoint.ServerSidePath);
            if (possibleExistingMountpoint != null)
            {
                throw new ArgumentException("This mountpoint already exist!", nameof(mountpoint));
            }

            var mountpointModel = Map(mountpoint);

            var affected = await _connection.InsertAsync(mountpointModel);
            if (affected != 1)
            {
                throw new InvalidOperationException("Failed to add new mountpoint to database!");
            }
        }

        public async Task DeleteMountpointsAsync(MountpointDto mountpoint)
        {
            _ = mountpoint ?? throw new ArgumentNullException(nameof(mountpoint));

            var mountpointToDelete = await GetMountpointModelAsync(mountpoint.Name, mountpoint.ServerSidePath);
            if (mountpointToDelete == null)
            {
                throw new ArgumentException("Attempt to delete nonexistent mountpoint!");
            }

            if (await _connection.DeleteAsync(mountpointToDelete) != 1)
            {
                throw new ArgumentException("Mountpoint removal failed!");
            }
        }

        public async Task<IReadOnlyCollection<MountpointDto>> GetAllMountpointsAsync()
        {
            return (await _connection
                .Table<MountpointModel>()
                .ToListAsync())
                .Select(mp => Map(mp))
                .ToList();
        }

        private async Task<MountpointModel> GetMountpointModelAsync(string name, string serverSidePath)
        {
            return await _connection
                .Table<MountpointModel>()
                .Where(mp => mp.Name == name && mp.ServerSidePath == serverSidePath)
                .FirstOrDefaultAsync();
        }

        public async Task<MountpointDto> GetMountpointAsync(string name, string serverSidePath)
        {
            var mountpoint = await GetMountpointModelAsync(name, serverSidePath);

            return Map(mountpoint);
        }

        private MountpointModel Map(MountpointDto mountpoint)
        {
            if (mountpoint == null)
            {
                return null;
            }

            return new MountpointModel()
            {
                Id = 0,
                Name = mountpoint.Name,
                ServerSidePath = mountpoint.ServerSidePath
            };
        }

        private MountpointDto Map(MountpointModel mountpoint)
        {
            if (mountpoint == null)
            {
                return null;
            }

            return new MountpointDto(mountpoint.Name, mountpoint.ServerSidePath);
        }
    }
}