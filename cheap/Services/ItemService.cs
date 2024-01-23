using cheap.Entities;
using cheap.Models;

namespace cheap.Services;

public class ItemService : IBaseService<Item>
{
    public Task<Response<Record>> Get(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Response<IEnumerable<Item>>> List()
    {
        throw new NotImplementedException();
    }

    public Task<Response<IEnumerable<Item>>> ListMine(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Item>> Add(Guid userId, Item t)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Item>> Update(Guid userId, Item t)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Item>> Delete(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
}