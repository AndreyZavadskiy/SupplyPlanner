using System;
using System.Collections.Generic;
using System.Threading;

namespace SP.Service.Background
{
    public class InventoryUploadService
    {
        private readonly Guid _key;
        private readonly IBackgroundCoordinator _coordinator;

        private Dictionary<string, string> _filePaths;

        public InventoryUploadService(Guid key, Dictionary<string, string> filePaths, IBackgroundCoordinator coordinator)
        {
            _key = key;
            _filePaths = filePaths;
            _coordinator = coordinator;

            // регистрируем в координаторе
            var data = new BackgroundServiceData
            {
                Key = _key,
                Status = BackgroundServiceStatus.Created,
                Progress = -1
            };

            _coordinator.AddOrUpdate(data);
        }

        public bool Run()
        {
            foreach (var fileData in _filePaths)
            {
                decimal progress = -1;
                for (int i = 0; i < 100; i++)
                {
                    progress = i;
                    Thread.Sleep(500);

                    var progressData = new BackgroundServiceData
                    {
                        Key = _key,
                        Status = BackgroundServiceStatus.Running,
                        Step = fileData.Key,
                        Progress = progress,
                        Log = null
                    };
                    _coordinator.AddOrUpdate(progressData);

                    i++;
                }
            }

            var finalData = new BackgroundServiceData
            {
                Key = _key,
                Status = BackgroundServiceStatus.RanToCompletion,
                Step = "Загрузка завершена",
                Progress = 100,
                Log = "Finished"
            };
            _coordinator.AddOrUpdate(finalData);

            return true;
        }
    }
}
