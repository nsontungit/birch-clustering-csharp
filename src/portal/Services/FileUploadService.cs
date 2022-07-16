namespace portal.Services
{
    public class FileUploadService
    {
        private readonly IWebHostEnvironment env;

        public FileUploadService(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public string Upload(IFormFile file)
        {
            var uploadDirecotroy = "uploads\\";
            var uploadPath = Path.Combine(env.WebRootPath, uploadDirecotroy);

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath2 = uploadPath + fileName;

            using (var strem = File.Create(filePath2))
            {
                file.CopyTo(strem);
            }
            return filePath2;
        }
    }
}
