using FilesShareApi.FilesCleaner;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FilesShareApi
{
    public class ScheduledServices : HostedService
    {
        private readonly FileManager filesManager;

        public ScheduledServices(FileManager filesManager)
        {
            this.filesManager = filesManager;
        }

        /// <summary>
        /// Method calling FilesManager every 10 minutes
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await filesManager.DeleteUselessFiles(cancellationToken);

                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            }
        }
    }
}
