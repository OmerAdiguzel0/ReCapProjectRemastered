using Microsoft.AspNetCore.Http;

namespace Core.Utilities.Helpers.FileHelper
{
    public class FileHelper : IFileHelper
    {
        public void Delete(string filePath)
        {
            try
            {
                if (string.IsNullOrEmpty(filePath))
                {
                    throw new ArgumentException("Dosya yolu boş olamaz.");
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Dosya silinirken hata oluştu: {ex.Message}");
            }
        }

        public string Update(IFormFile file, string filePath, string root)
        {
            try
            {
                if (file == null)
                {
                    throw new ArgumentNullException(nameof(file), "Dosya boş olamaz.");
                }

                if (string.IsNullOrEmpty(filePath))
                {
                    throw new ArgumentException("Dosya yolu boş olamaz.");
                }

                if (string.IsNullOrEmpty(root))
                {
                    throw new ArgumentException("Kök dizin yolu boş olamaz.");
                }

                Delete(filePath);
                return Upload(file, root);
            }
            catch (Exception ex)
            {
                throw new Exception($"Dosya güncellenirken hata oluştu: {ex.Message}");
            }
        }

        public string Upload(IFormFile file, string root)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("Dosya seçilmedi veya boş.");
                }

                if (string.IsNullOrEmpty(root))
                {
                    throw new ArgumentException("Kök dizin yolu boş olamaz.");
                }

                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }

                string fileExtension = Path.GetExtension(file.FileName).ToLower();
                if (!IsValidFileExtension(fileExtension))
                {
                    throw new ArgumentException("Geçersiz dosya uzantısı. Sadece resim dosyaları (.jpg, .jpeg, .png, .gif) yüklenebilir.");
                }

                string fileName = Guid.NewGuid().ToString() + fileExtension;
                string filePath = Path.Combine(root, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                throw new Exception($"Dosya yüklenirken hata oluştu: {ex.Message}");
            }
        }

        private bool IsValidFileExtension(string extension)
        {
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            return allowedExtensions.Contains(extension.ToLower());
        }
    }
}
