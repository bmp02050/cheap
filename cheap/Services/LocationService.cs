using cheap.Entities;
using cheap.Models;

namespace cheap.Services;

public class LocationService : IBaseService<Location>
{
    public Task<Response<Record?>> Get(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Response<IEnumerable<Location>>> List()
    {
        throw new NotImplementedException();
    }

    public Task<Response<IEnumerable<Location>>> ListMine(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Location>> Add(Guid userId, Location t)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Record>> Update(Guid userId, Location t)
    {
        throw new NotImplementedException();
    }

    public Task<Response<Location>> Delete(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
}