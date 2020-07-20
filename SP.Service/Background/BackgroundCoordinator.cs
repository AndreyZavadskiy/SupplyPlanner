using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace SP.Service.Background
{
    public interface IBackgroundCoordinator
    {
        void AddOrUpdate(BackgroundServiceProgress progress);
        BackgroundServiceProgress Get(Guid key);
        bool Remove(Guid key);
        (BackgroundServiceStatus Status, string Step, decimal Progress) GetProgress(Guid key);
    }

    /// <summary>
    /// Координатор фоновых процессов
    /// </summary>
    public class BackgroundCoordinator : IBackgroundCoordinator
    {
        private readonly ConcurrentDictionary<Guid, BackgroundServiceProgress> _services = new ConcurrentDictionary<Guid, BackgroundServiceProgress>();

        public void AddOrUpdate(BackgroundServiceProgress progress)
        {
            Debug.WriteLine(progress.Progress);
            _services.AddOrUpdate(progress.Key, progress, (key, oldData) => progress);
        }

        public BackgroundServiceProgress Get(Guid key)
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
                return (BackgroundServiceStatus.NotFound, null, 0);
            }

            return (data.Status, data.Step, data.Progress);
        }
    }
}
