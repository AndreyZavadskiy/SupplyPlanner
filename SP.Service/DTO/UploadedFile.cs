using System.IO;

namespace SP.Service.DTO
{
    /// <summary>
    /// Данные по загруженным файлам
    /// </summary>
    public class UploadedFile
    {
        public string FileName { get; set; }
        public FileInfo FileInfo { get; set; }
    }
}
