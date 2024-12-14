using Business.Abstract;
using Business.Constants;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete;
using DataAccess.Abstract;
using Entities.Concrete;
using Microsoft.Extensions.Logging;

namespace Business.Concrete
{
    public class ModelManager : IModelService
    {
        private IModelDal _modelDal;
        private readonly ILogger<ModelManager> _logger;

        public ModelManager(IModelDal modelDal, ILogger<ModelManager> logger)
        {
            _modelDal = modelDal;
            _logger = logger;
        }

        public IResult Add(Model model)
        {
            try
            {
                _logger.LogInformation("=== ModelManager Add Operation Started ===");
                _logger.LogInformation("Adding model: {@Model}", model);

                model.ModelId = 0;

                if (string.IsNullOrEmpty(model.ModelName))
                {
                    _logger.LogWarning("Validation failed: ModelName is empty");
                    return new ErrorResult("Model adı boş olamaz");
                }

                if (model.BrandId <= 0)
                {
                    _logger.LogWarning($"Validation failed: Invalid BrandId: {model.BrandId}");
                    return new ErrorResult("Geçerli bir marka seçmelisiniz");
                }

                var existingModel = _modelDal.Get(m => m.ModelName == model.ModelName && m.BrandId == model.BrandId);
                if (existingModel != null)
                {
                    _logger.LogWarning($"Model already exists: {model.ModelName} for BrandId: {model.BrandId}");
                    return new ErrorResult("Bu markada aynı isimde model zaten var");
                }

                _logger.LogInformation("Attempting database operation...");
                _modelDal.Add(model);
                _logger.LogInformation("Model successfully added to database");

                return new SuccessResult("Model başarıyla eklendi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ModelManager.Add");
                _logger.LogError("Database State: {@DatabaseState}", 
                    _modelDal.GetContext().ChangeTracker.DebugView.LongView);
                throw;
            }
        }

        public IResult Delete(Model model)
        {
            try
            {
                _modelDal.Delete(model);
                return new SuccessResult("Model başarıyla silindi");
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Model silinirken hata oluştu: {ex.Message}");
            }
        }

        public IDataResult<List<Model>> GetAll()
        {
            try
            {
                _logger.LogInformation("\n=== GetAll Models Operation Started ===");
                _logger.LogInformation("Attempting to connect to database...");
                
                var models = _modelDal.GetAll();
                
                _logger.LogInformation($"Database query completed");
                _logger.LogInformation($"Retrieved {models?.Count ?? 0} models");
                if (models != null && models.Any())
                {
                    _logger.LogInformation("Model details:");
                    foreach (var model in models)
                    {
                        _logger.LogInformation($"- ModelId: {model.ModelId}, BrandId: {model.BrandId}, Name: {model.ModelName}");
                    }
                }
                else
                {
                    _logger.LogWarning("No models found in database");
                }
                
                _logger.LogInformation("=== GetAll Models Operation Completed Successfully ===\n");
                return new SuccessDataResult<List<Model>>(models);
            }
            catch (Exception ex)
            {
                _logger.LogError("\n=== Error in GetAll Models Operation ===");
                _logger.LogError($"Exception Type: {ex.GetType().Name}");
                _logger.LogError($"Message: {ex.Message}");
                _logger.LogError($"Stack Trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    _logger.LogError("Inner Exception Details:");
                    _logger.LogError($"Type: {ex.InnerException.GetType().Name}");
                    _logger.LogError($"Message: {ex.InnerException.Message}");
                    _logger.LogError($"Stack Trace: {ex.InnerException.StackTrace}");
                }
                _logger.LogError("=== Error Log End ===\n");
                
                return new ErrorDataResult<List<Model>>("Modeller getirilirken bir hata oluştu: " + ex.Message);
            }
        }

        public IDataResult<List<Model>> GetByBrandId(int brandId)
        {
            try
            {
                var models = _modelDal.GetAll(m => m.BrandId == brandId);
                if (!models.Any())
                {
                    return new ErrorDataResult<List<Model>>("Bu markaya ait model bulunamadı");
                }
                return new SuccessDataResult<List<Model>>(models);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<List<Model>>($"Modeller getirilirken bir hata oluştu: {ex.Message}");
            }
        }

        public IDataResult<Model> GetById(int modelId)
        {
            try
            {
                var model = _modelDal.Get(m => m.ModelId == modelId);
                if (model == null)
                    return new ErrorDataResult<Model>("Model bulunamadı");
                    
                return new SuccessDataResult<Model>(model);
            }
            catch (Exception ex)
            {
                return new ErrorDataResult<Model>($"Model getirilirken hata oluştu: {ex.Message}");
            }
        }

        public IResult Update(Model model)
        {
            try
            {
                _modelDal.Update(model);
                return new SuccessResult("Model başarıyla güncellendi");
            }
            catch (Exception ex)
            {
                return new ErrorResult($"Model güncellenirken hata oluştu: {ex.Message}");
            }
        }
    }
} 