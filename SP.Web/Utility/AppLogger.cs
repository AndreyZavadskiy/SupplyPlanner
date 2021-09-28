using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SP.Core.Log;
using SP.Data;
using SP.Service.Services;

namespace SP.Web.Utility
{
    public interface IAppLogger
    {
        Task<bool> SaveActionAsync(string name, DateTime date, string category, string description);
        Task<bool> SaveLogRecord(ChangeLog changeLogRecord);
    }

    /// <summary>
    /// Логирование действий
    /// </summary>
    public class AppLogger : IAppLogger
    {
        private readonly ILogService _logService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AppLogger(ILogService logService, UserManager<ApplicationUser> userManager)
        {
            _logService = logService;
            _userManager = userManager;
        }

        /// <summary>
        /// Сохранить действие пользователя
        /// </summary>
        /// <param name="name"></param>
        /// <param name="date"></param>
        /// <param name="category"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public async Task<bool> SaveActionAsync(string name, DateTime date, string category, string description)
        {
            var user = await _userManager.FindByNameAsync(name);
            return await _logService.SaveActionAsync(user.Id, date, category, description);
        }

        public async Task<bool> SaveLogRecord(ChangeLog changeLogRecord)
        {
            return await _logService.SaveLogRecordAsync(changeLogRecord);
        }
    }
}
