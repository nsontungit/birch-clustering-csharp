using CsvHelper.Configuration.Attributes;

namespace portal.Models
{
    public class Point2
    {
        [Index(0)]
        public float X { get; set; }
        [Index(1)]
        public float Y { get; set; }
    }
}
