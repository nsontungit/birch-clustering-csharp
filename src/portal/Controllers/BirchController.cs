using CsvHelper.TypeConversion;
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
                string exceptionKey = "Exception";
                try
                {
                    string path = _cachedDataService.ImportedFilePath = _fileUploadService.Upload(model.DataCollection);
                    string ext = System.IO.Path.GetExtension(path);
                    if (ext.ToLower() != ".csv")
                        throw new InvalidTypeFileException("Tệp không phù hợp");

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
                    ViewData[exceptionKey] = $"{ex.Message}";
                }
                catch (InvalidTypeFileException ex)
                {
                    _logger.LogWarning(ex, "Imported file extension is not .csv");
                    ViewData[exceptionKey] = $"{ex.Message}";
                }
                catch (CsvHelper.MissingFieldException ex)
                {
                    _logger.LogWarning(ex, "Imported file missing field");
                    ViewData[exceptionKey] = "Tệp đầu vào đã không đủ số lượng trường";
                }
                catch (NotFoundDateException ex)
                {
                    _logger.LogWarning(ex, "Imported file not contain any records");
                    ViewData[exceptionKey] = $"{ex.Message}";
                }
                catch (TypeConverterException ex)
                {
                    _logger.LogWarning(ex, "Config data file is incorrect or value invalid");
                    ViewData[exceptionKey] = "Không thể chuyển đổi dữ liệu từ file csv";
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    _logger.LogWarning(ex, "The number of clusters is too many to represent on the graph, try again with less data");
                    ViewData[exceptionKey] = "Số cụm quá nhiều không thể biểu diễn trên biểu đồ, hãy thử lại với dữ liệu ít bản ghi hơn";
                }
                catch (OutOfRangeColorException ex)
                {
                    _logger.LogWarning(ex, "The number of colors not enough");
                    ViewData[exceptionKey] = $"{ex.Message}";
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Unknow exception");
                    ViewData[exceptionKey] = "Lỗi không xác định";
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
