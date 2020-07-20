using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

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
