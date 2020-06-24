using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace SP.Service.Background
{
    public interface IBackgroundCoordinator
    {
        void AddOrUpdate(BackgroundServiceData data);
        BackgroundServiceData Get(Guid key);
        bool Remove(Guid key);
        (BackgroundServiceStatus Status, string Step, decimal Progress) GetProgress(Guid key);
    }

    /// <summary>
    /// Координатор фоновых процессов
    /// </summary>
    public class BackgroundCoordinator : IBackgroundCoordinator
    {
        private readonly ConcurrentDictionary<Guid, BackgroundServiceData> _services = new ConcurrentDictionary<Guid, BackgroundServiceData>();

        public void AddOrUpdate(BackgroundServiceData data)
        {
            Debug.WriteLine(data.Progress);
            _services.AddOrUpdate(data.Key, data, (key, oldData) => data);
        }

        public BackgroundServiceData Get(Guid key)
        {
            if (_services.TryGetValue(key, out var data))
            {
                return data;
            }

            return null;
        }

        public bool Remove(Guid key)
        {
            return _services.TryRemove(key, out var data);
        }

        public (BackgroundServiceStatus Status, string Step, decimal Progress) GetProgress(Guid key)
        {
            if (!_services.TryGetValue(key, out var data))
            {
                return (BackgroundServiceStatus.NotFound, null, -1);
            }

            return (data.Status, data.Step, data.Progress);
        }
    }
}
