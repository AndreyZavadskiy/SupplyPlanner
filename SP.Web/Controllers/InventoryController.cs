using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using SP.Service.Background;
using SP.Web.ViewModels;

namespace SP.Web.Controllers
{
    public class InventoryController : Controller
    {
        private readonly IBackgroundCoordinator _coordinator;

        public InventoryController(IBackgroundCoordinator coordinator)
        {
            _coordinator = coordinator;
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
        /// <returns></returns>
        [HttpPost]
        [Route("[controller]/Upload")]
        public async Task<IActionResult> UploadAsync([FromForm]UploadInventoryViewModel model)
        {
            if (model.Files == null)
            {
                return null;
            }

            // записываем во временные файлы на сервере
            var paths = new Dictionary<string, string>();
            foreach (var formFile in model.Files)
            {
                if (formFile.Length == 0)
                {
                    continue;
                }

                string filePath = Path.GetTempFileName();
                using (var stream = System.IO.File.Create(filePath))
                {
                    await formFile.CopyToAsync(stream);
                }

                paths.Add(formFile.FileName, filePath);
            }
            // запускаем обработку
            Guid serviceKey = Guid.NewGuid();
            var service = new InventoryUploadService(serviceKey, paths, _coordinator);
            Task.Run(() => service.Run());

            return Json(new { Key = serviceKey });
        }

        public IActionResult PeekUploadStatus(Guid key)
        {
            var data = _coordinator.GetProgress(key);

            if (data.Status == BackgroundServiceStatus.NotFound)
            {
                return Json(new
                {
                    status = -1,
                    step = "Загрузка ТМЦ уже выполнена либо указан неправильный идентификатор загрузки.",
                    progress = 0
                });
            }

            return Json(new
            {
                status = data.Status,
                step = data.Step,
                progress = data.Progress
            });
        }
    }
}