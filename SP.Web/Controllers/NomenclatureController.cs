using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SP.Service.Services;

namespace SP.Web.Controllers
{
    public class NomenclatureController : Controller
    {
        private readonly IInventoryService _inventoryService;

        public NomenclatureController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoadListAsync()
        {
            var list = await _inventoryService.GetNomenclatureListAsync();

            return Json(new { data = list });
        }

        public async Task<IActionResult> LoadNomenclature(int group)
        {
            var list = await _inventoryService.GetNomenclatureListItemsAsync(group);

            return Json(list);
        }
    }
}