using cheap.Models;

namespace cheap.Services;

public interface IBaseService<T>
{
    Task<Response<T?>> Get(Guid userId, Guid id);
    Task<Response<IEnumerable<T>>> List();
    Task<Response<IEnumerable<T>>> ListMine(Guid userId);
    Task<Response<T>> Add(Guid userId, T t);
    Task<Response<T>> Update(Guid userId, T t);
    Task<Response<T>> Delete(Guid userId, Guid id);
}