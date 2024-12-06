using Microsoft.AspNetCore.Http;

namespace Core.Utilities.Helpers.FileHelper
{
    public class FileHelper : IFileHelper
    {
        public string Upload(IFormFile file, string root)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("Dosya seçilmedi veya boş.");

                if (!Directory.Exists(root))
                    Directory.CreateDirectory(root);

                string extension = Path.GetExtension(file.FileName).ToLower();
                string fileName = Guid.NewGuid().ToString() + extension;
                string filePath = Path.Combine(root, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return fileName;  // Sadece dosya adını döndür
            }
            catch (Exception ex)
            {
                throw new Exception($"Dosya yükleme hatası: {ex.Message}");
            }
        }

        public void Delete(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch (Exception ex)
            {
                throw new Exception($"Dosya silme hatası: {ex.Message}");
            }
        }

        public string Update(IFormFile file, string filePath, string root)
        {
            try
            {
                Delete(filePath);
                return Upload(file, root);
            }
            catch (Exception ex)
            {
                throw new Exception($"Dosya güncelleme hatası: {ex.Message}");
            }
        }
    }
}
