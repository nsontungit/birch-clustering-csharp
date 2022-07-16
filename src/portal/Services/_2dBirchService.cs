using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using portal.Models;
using shared;
using shared._2d;
using shared.Exceptions;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace portal.Services
{
    public class Vector2Compare : IComparer<Vector2>
    {
        public int Compare(Vector2 x, Vector2 y)
        {
            if (x.X < y.X)
                return 1;
            else if (x.X > y.X)
                return -1;
            else if (x.Y < y.Y)
                return 1;
            else if (x.Y > y.Y)
                return -1;
            return 0;
        }
    }
    public class _2dBirchService
    {
        readonly ILogger<_2dBirchService> _logger;
        public _2dBirchService(ILogger<_2dBirchService> logger)
        {
            _logger = logger;
        }
        public IEnumerable<ScatterResult> GetResult(string path, int b, float t, int l, bool hasHeader = false, int? k = null)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = hasHeader,
            };
            using (var reader = new StreamReader(path, Encoding.UTF8))
            using (var csv = new CsvReader(reader, config))
            {
                var records = csv.GetRecords<Point2>().ToArray();

                if (records == null || records.Length == 0)
                    throw new NotFoundDateException("Không tìm thấy dữ liệu trong tệp");

                CFTree cFTree = new CFTree();

                cFTree.BranchingFactor = b;
                cFTree.Threshold = t;
                cFTree.NumberOfEntries = l;

                List<Vector2> data = new List<Vector2>();
                foreach (var i in records)
                {
                    data.Add(new Vector2(i.X, i.Y));
                }
                data.Sort(new Vector2Compare());

                foreach (var v in data)
                {
                    cFTree.Add(v);
                }

                var results = cFTree.Clustering();
                PoolResult poolResult = new PoolResult();

                // trường hợp không cho trước k
                if (k == null || k >= results.Count)
                {
                    var canClustering = results.Select(r => r.Pvalue).Any(r => r <= 0.05);
                    if (!canClustering)
                        throw new ClusteringException("Không tìm thấy số k phù hợp");

                    var zeroPvalues = results.Where(r => r.Pvalue == 0).ToArray();
                    if (zeroPvalues.Any())
                    {
                        poolResult = zeroPvalues.OrderByDescending(r => r.PseudoF).First();
                    }
                    else
                    {
                        poolResult = results.OrderBy(r => r.Pvalue).First();
                    }
                }
                // trường hợp cho trước k và k là một số hợp lệ
                else
                {
                    poolResult = results.FirstOrDefault(r => r.K == k.Value)
                        ?? throw new ClusteringException("Kết quả phân cụm không phù hợp với số K này");
                }

                if (poolResult.RawData == null)
                    throw new ClusteringException("Đã xảy ra lỗi trong quá trình phân cụm");

                var scatterResults = JsonConvert.DeserializeObject<List<Cluster>>(poolResult.RawData);

                if (scatterResults == null)
                    throw new ArgumentNullException();

                var scatters = new List<ScatterResult>();

                try
                {
                    foreach (var item in scatterResults)
                    {
                        var index = scatterResults.IndexOf(item);
                        scatters.Add(new ScatterResult()
                        {
                            backgroundColor = Constant.Colors[index],
                            data = item.Children.Select(v => new { x = v.X, y = v.Y }).ToArray(),
                            label = $"Cụm {index + 1}",
                            pointRadius = 10
                        });
                    }
                }
                catch (IndexOutOfRangeException ex)
                {
                    throw new OutOfRangeColorException("Số cụm quá lớn, không đủ màu phân phối cho từng cụm");
                }
                
                if (scatters.Count == 0) throw new ClusteringException("Không thể phân cụm với tham số này, hãy thử lại");
                return scatters;
            }
        }
    }
}
