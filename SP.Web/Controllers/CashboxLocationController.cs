﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SP.Core.Master;
using SP.Service.Models;
using SP.Service.Services;

namespace SP.Web.Controllers
{
    public class CashboxLocationController : Controller
    {
        private readonly IMasterService _masterService;
        private readonly string _title = "Расчетно-кассовый узел";
        private readonly string _className = "CashboxLocation";

        public CashboxLocationController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = _title;
            ViewData["ClassName"] = _className;
            return View("MasterDictionary");
        }

        [HttpPost]
        [Route("[controller]/LoadList")]
        public async Task<IActionResult> LoadListAsync()
        {
            var list = await _masterService.GetDictionaryListAsync<CashboxLocation>();

            return Json(new { data = list });
        }

        public async Task<IActionResult> EditAsync(int id)
        {
            var model = await _masterService.GetDictionaryModelAsync<CashboxLocation>(id);
            if (model == null)
            {
                return NotFound();
            }

            ViewData["Title"] = _title;
            ViewData["ClassName"] = _className;

            return View("_MasterDictionaryEdit", model);
        }

        public IActionResult Create()
        {
            var model = new DictionaryModel();

            ViewData["Title"] = _title;
            ViewData["ClassName"] = _className;

            return View("_MasterDictionaryEdit", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditAsync([FromForm] DictionaryModel model)
        {
            string errorMessage;
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(ms => ms.Errors)
                    .Select(e => e.ErrorMessage);
                errorMessage = string.Join(" ", errors);
                return Content(errorMessage);
            }

            var result = await _masterService.SaveDictionaryAsync<CashboxLocation>(model);
            if (result.Success)
            {
                return Content(result.Id.ToString());
            }

            errorMessage = string.Join(" ", result.Errors);

            return Content(errorMessage);
        }
    }
}