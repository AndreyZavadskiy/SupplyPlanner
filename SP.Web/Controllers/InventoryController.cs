﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using SP.Data;
using SP.Service.Background;
using SP.Service.DTO;
using SP.Web.ViewModels;

namespace SP.Web.Controllers
{
    public class InventoryController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public InventoryController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Отобразить форму для загрузки остатков ТМЦ
        /// </summary>
        /// <returns></returns>
        public IActionResult Upload()
        {
            var model = new UploadInventoryViewModel
            {
                ProcessingDate = DateTime.Now
            };

            return View("Upload", model);
        }

        /// <summary>
        /// Запустить загрузку остатков ТМЦ
        /// </summary>
        /// <param name="model"></param>
        /// <param name="coordinator"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("[controller]/Upload")]
        public async Task<IActionResult> UploadAsync([FromForm]UploadInventoryViewModel model,
            [FromServices] IBackgroundCoordinator coordinator)
        {
            if (model.Files == null)
            {
                return null;
            }

            // записываем во временные файлы на сервере
            var uploadedFiles = new List<UploadedFile>();
            foreach (var formFile in model.Files)
            {
                if (formFile.Length == 0 || !CheckExcelFile(formFile.FileName))
                {
                    continue;
                }

                string filePath = Path.GetTempFileName();
                using (var stream = System.IO.File.Create(filePath))
                {
                    await formFile.CopyToAsync(stream);
                }

                uploadedFiles.Add(new UploadedFile
                {
                    FileName = formFile.FileName,
                    FileInfo = new FileInfo(filePath)
                });
            }

            Guid serviceKey = Guid.NewGuid();
            var user = await _userManager.GetUserAsync(User);
            StartBackgroundUpload(coordinator, serviceKey, uploadedFiles, user.Id);

            return Json(new { Key = serviceKey });
        }

        /// <summary>
        /// Отобразить форму для автоматического объединения ТМЦ с Номенклатурой
        /// </summary>
        /// <returns></returns>
        public IActionResult AutoMerge()
        {
            var model = new AutoMergeInventoryViewModel
            {
                ProcessingDate = DateTime.Now
            };

            return View("AutoMerge", model);
        }

        [HttpPost]
        [Route("[controller]/AutoMerge")]
        public async Task<IActionResult> AutoMergeAsync([FromForm] AutoMergeInventoryViewModel model,
            [FromServices] IBackgroundCoordinator coordinator)
        {
            Guid serviceKey = Guid.NewGuid();
            var user = await _userManager.GetUserAsync(User);
            StartBackgroundAutoMerge(coordinator, serviceKey, user.Id);

            return Json(new { Key = serviceKey });
        }

        /// <summary>
        /// Получить статус выполнения задания
        /// </summary>
        /// <param name="key"></param>
        /// <param name="coordinator"></param>
        /// <returns></returns>
        public IActionResult PeekStatus(Guid key, [FromServices] IBackgroundCoordinator coordinator)
        {
            var data = coordinator.GetProgress(key);

            if (data.Status == BackgroundServiceStatus.NotFound)
            {
                return Json(new
                {
                    status = 0,
                    step = "Загрузка ТМЦ уже выполнена либо указан неправильный идентификатор загрузки.",
                    progress = 0
                });
            }

            return Json(new
            {
                status = data.Status,
                step = data.Step,
                progress = data.Progress,
                log = data.Log
            });
        }

        private bool CheckExcelFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            string fileExtension = Path.GetExtension(fileName);
            switch (fileExtension.ToLower())
            {
                case ".xlsx":
                    return true;
            }

            return false;
        }

        private void StartBackgroundUpload(IBackgroundCoordinator coordinator, Guid serviceKey, List<UploadedFile> files, string aspNetUserId)
        {
            Task.Run(async () => 
            {
                var service = new BackgroundInventoryService(coordinator);
                await service.UploadAsync(serviceKey, files, aspNetUserId);
            });
        }

        private void StartBackgroundAutoMerge(IBackgroundCoordinator coordinator, Guid serviceKey, string aspNetUserId)
        {
            Task.Run(async () =>
            {
                var service = new BackgroundInventoryService(coordinator);
                await service.AutoMergeAsync(serviceKey, aspNetUserId);
            });
        }
    }
}