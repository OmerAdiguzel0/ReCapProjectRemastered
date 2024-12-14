using Core.Utilities.Results.Abstract;
using Entities.Concrete;

namespace Business.Abstract
{
    public interface IModelService
    {
        IDataResult<List<Model>> GetAll();
        IDataResult<Model> GetById(int modelId);
        IDataResult<List<Model>> GetByBrandId(int brandId);
        IResult Add(Model model);
        IResult Update(Model model);
        IResult Delete(Model model);
    }
} 