using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using portal.Models;
using portal.Services;
using shared.Exceptions;

namespace portal.Controllers
{
    public class BirchController : Controller
    {
        readonly ILogger<BirchController> _logger;
        readonly _2dBirchService __2dBirch;
        readonly FileUploadService _fileUploadService;
        public BirchController(ILogger<BirchController> logger, _2dBirchService _2dBirch, FileUploadService fileUploadService)
        {
            _logger = logger;
            __2dBirch = _2dBirch;
            _fileUploadService = fileUploadService;
        }

        [HttpGet]
        public IActionResult Input()
        {
            var birchFormModel = new BirchFormModel();
            return View(birchFormModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Output(BirchFormModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string path = _fileUploadService.Upload(model.DataCollection);
                    if (model.B != null && model.L != null && model.T != null)
                    {
                        var result = __2dBirch.GetResult(path, model.B.Value,
                            model.T.Value, model.L.Value, false, model.K);

                        ViewData["scatter"] = JsonConvert.SerializeObject(result);
                        System.IO.File.Delete(path);
                        return View("Output");
                    }
                }
                catch (ClusteringException ex)
                {
                    _logger.LogWarning(ex, "Xảy ra lỗi trong quá trình phân cụm");
                    ViewData["Exception"] = $"{ex.Message}";
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    _logger.LogWarning(ex,"Số cụm quá nhiều không thể biểu diễn trên biểu đồ, hãy thử lại với dữ liệu ít bản ghi hơn");
                    ViewData["Exception"] = $"Số cụm quá nhiều không thể biểu diễn trên biểu đồ, hãy thử lại với dữ liệu ít bản ghi hơn";
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Lỗi không xác định");
                    ViewData["Exception"] = $"Lỗi không xác định";
                }
            }
            _logger.LogWarning("Tham số không phù hợp");
            return View("Input", model);
        }
    }
}
