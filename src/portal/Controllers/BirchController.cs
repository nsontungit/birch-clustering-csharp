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
        readonly CachedDataService _cachedDataService;
        public BirchController(ILogger<BirchController> logger, _2dBirchService _2dBirch,
            FileUploadService fileUploadService, CachedDataService cachedDataService)
        {
            _logger = logger;
            __2dBirch = _2dBirch;
            _fileUploadService = fileUploadService;
            _cachedDataService = cachedDataService;
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
                    string path = _cachedDataService.ImportedFilePath = _fileUploadService.Upload(model.DataCollection);
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
                    _logger.LogWarning(ex, "Occur error while clustering");
                    ViewData["Exception"] = $"{ex.Message}";
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    _logger.LogWarning(ex, "The number of clusters is too many to represent on the graph, try again with less data");
                    ViewData["Exception"] = $"Số cụm quá nhiều không thể biểu diễn trên biểu đồ, hãy thử lại với dữ liệu ít bản ghi hơn";
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Unknow exception");
                    ViewData["Exception"] = $"Lỗi không xác định";
                }
                finally
                {
                    // Xóa file khi import xong (thất bại và khi thành công)
                    if (_cachedDataService.ImportedFilePath != null)
                    {
                        System.IO.File.Delete(_cachedDataService.ImportedFilePath);
                        _logger.LogInformation("Deleted file successfully!");
                    }    
                }
            }
            _logger.LogWarning("Parameter does not match");
            return View("Input", model);
        }
    }
}
