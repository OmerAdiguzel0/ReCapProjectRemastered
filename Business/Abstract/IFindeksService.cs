using Core.Utilities.Results.Abstract;

namespace Business.Abstract
{
    public interface IFindeksService
    {
        IDataResult<int> CalculateFindeksScore(string tckn, string birthYear);
        IResult UpdateUserFindeksScore(int userId, int findeksScore);
    }
} 