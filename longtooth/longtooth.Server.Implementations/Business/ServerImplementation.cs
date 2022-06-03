using longtooth.Server.Abstractions.Interfaces;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace longtooth.Server.Implementations.Business
{
    public class ServerImplementation : IServer
    {
        public async Task StartAsync()
        {
            Debug.WriteLine("Server started");
        }
    }
}
