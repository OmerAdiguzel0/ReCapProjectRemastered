using Business.Abstract;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Microsoft.Extensions.Logging;

namespace Business.Concrete
{
    public class FindeksManager : IFindeksService
    {
        private readonly IUserService _userService;
        private readonly ILogger<FindeksManager> _logger;

        public FindeksManager(IUserService userService, ILogger<FindeksManager> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public IDataResult<int> CalculateFindeksScore(string tckn, string birthYear)
        {
            try
            {
                _logger.LogInformation("\n=== FindeksManager.CalculateFindeksScore Started ===");
                _logger.LogInformation($"Input - TCKN: {tckn?.Substring(0, 3)}***{tckn?.Substring(7)}, BirthYear: {birthYear}");

                // TCKN validasyonu
                if (string.IsNullOrEmpty(tckn))
                {
                    _logger.LogError("TCKN is null or empty");
                    return new ErrorDataResult<int>("TCKN boş olamaz");
                }

                if (tckn.Length != 11)
                {
                    _logger.LogError($"Invalid TCKN length: {tckn.Length}");
                    return new ErrorDataResult<int>("TCKN 11 haneli olmalıdır");
                }

                if (!tckn.All(char.IsDigit))
                {
                    _logger.LogError("TCKN contains non-digit characters");
                    return new ErrorDataResult<int>("TCKN sadece rakamlardan oluşmalıdır");
                }

                // Doğum yılı validasyonu
                if (string.IsNullOrEmpty(birthYear))
                {
                    _logger.LogError("Birth year is null or empty");
                    return new ErrorDataResult<int>("Doğum yılı boş olamaz");
                }

                if (birthYear.Length != 4)
                {
                    _logger.LogError($"Invalid birth year length: {birthYear.Length}");
                    return new ErrorDataResult<int>("Doğum yılı 4 haneli olmalıdır");
                }

                if (!birthYear.All(char.IsDigit))
                {
                    _logger.LogError("Birth year contains non-digit characters");
                    return new ErrorDataResult<int>("Doğum yılı sadece rakamlardan oluşmalıdır");
                }

                // Yaş hesaplama
                _logger.LogInformation("Calculating age...");
                int birthYearValue = int.Parse(birthYear);
                int currentYear = DateTime.Now.Year;
                int age = currentYear - birthYearValue;
                _logger.LogInformation($"Calculated age: {age}");

                if (age < 18)
                {
                    _logger.LogError($"User is underage: {age}");
                    return new ErrorDataResult<int>("18 yaşından küçükler için findeks puanı hesaplanamaz");
                }

                // Findeks puanı hesaplama
                _logger.LogInformation("Calculating findeks score...");
                int baseScore = int.Parse(tckn.Substring(7, 4));
                int findeksScore = (baseScore % 900) + 500;
                int ageBonus = Math.Min((age - 18) * 10, 100);
                findeksScore += ageBonus;

                _logger.LogInformation($"Calculation details - Base Score: {baseScore}, Age Bonus: {ageBonus}, Final Score: {findeksScore}");
                _logger.LogInformation("=== FindeksManager.CalculateFindeksScore Completed Successfully ===\n");

                return new SuccessDataResult<int>(findeksScore, "Findeks puanı başarıyla hesaplandı");
            }
            catch (Exception ex)
            {
                _logger.LogError("\n=== FindeksManager.CalculateFindeksScore Error ===");
                _logger.LogError($"Error Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}");
                _logger.LogError($"Input Parameters - TCKN: {tckn?.Substring(0, 3)}***{tckn?.Substring(7)}, BirthYear: {birthYear}");
                _logger.LogError($"Error Type: {ex.GetType().Name}");
                _logger.LogError($"Error Message: {ex.Message}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner Exception: {ex.InnerException.Message}");
                    _logger.LogError($"Inner Exception Stack Trace: {ex.InnerException.StackTrace}");
                }
                
                _logger.LogError("=== Error Log End ===\n");

                return new ErrorDataResult<int>($"Findeks hesaplanırken bir hata oluştu: {ex.Message}");
            }
        }

        public IResult UpdateUserFindeksScore(int userId, int findeksScore)
        {
            try
            {
                _logger.LogInformation("\n=== Update Findeks Score Started ===");
                _logger.LogInformation($"Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                _logger.LogInformation($"User ID: {userId}");
                _logger.LogInformation($"New Score: {findeksScore}");

                if (findeksScore < 0)
                {
                    _logger.LogWarning($"Invalid findeks score (negative): {findeksScore}");
                    return new ErrorResult("Findeks puanı 0'dan küçük olamaz");
                }

                if (findeksScore > 1900)
                {
                    _logger.LogWarning($"Invalid findeks score (too high): {findeksScore}");
                    return new ErrorResult("Findeks puanı 1900'den büyük olamaz");
                }

                var result = _userService.UpdateFindeksScore(userId, findeksScore);
                
                if (result.Success)
                {
                    _logger.LogInformation("Findeks score updated successfully");
                    _logger.LogInformation("=== Update Findeks Score Completed ===\n");
                }
                else
                {
                    _logger.LogWarning($"Failed to update findeks score: {result.Message}");
                    _logger.LogInformation("=== Update Findeks Score Failed ===\n");
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("\n=== Update Findeks Score Error ===");
                _logger.LogError($"Error Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                _logger.LogError($"Error Type: {ex.GetType().Name}");
                _logger.LogError($"Error Message: {ex.Message}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner Exception: {ex.InnerException.Message}");
                }
                _logger.LogError("=== Error Log End ===\n");

                return new ErrorResult($"Findeks puanı güncellenirken bir hata oluştu: {ex.Message}");
            }
        }
    }
} 