using Microsoft.AspNetCore.Http;

namespace Core.Utilities.Helpers.FileHelper
{
    public class FileHelper : IFileHelper
    {
        public string Upload(IFormFile file, string root)
        {
            try
            {
                Console.WriteLine($"\n=== FileHelper Upload Started ===");
                Console.WriteLine($"File Name: {file?.FileName}");
                Console.WriteLine($"File Size: {file?.Length} bytes");
                Console.WriteLine($"Content Type: {file?.ContentType}");
                Console.WriteLine($"Upload Path: {root}");

                if (file == null) return null;

                if (!Directory.Exists(root))
                {
                    Console.WriteLine($"Creating directory: {root}");
                    Directory.CreateDirectory(root);
                }

                string extension = Path.GetExtension(file.FileName);
                string guid = Guid.NewGuid().ToString();
                string filePath = guid + extension;

                Console.WriteLine($"Generated File Name: {filePath}");
                Console.WriteLine($"Full Path: {Path.Combine(root, filePath)}");

                using (FileStream fileStream = File.Create(Path.Combine(root, filePath)))
                {
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                    Console.WriteLine("File successfully written to disk");
                }

                return filePath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n=== FileHelper Upload Error ===");
                Console.WriteLine($"Error Type: {ex.GetType().Name}");
                Console.WriteLine($"Error Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
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
