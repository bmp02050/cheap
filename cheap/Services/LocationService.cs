using cheap.Entities;
using cheap.Models;
using Microsoft.EntityFrameworkCore;

namespace cheap.Services;

public class LocationService : IBaseService<Location>
{
    private readonly Context _context;

    public LocationService(Context context)
    {
        _context = context;
    }

    public async Task<Response<Location?>> Get(Guid userId, Guid id)
    {
        try
        {
            return new Response<Location?>(true, await _context.Locations
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync());
        }
        catch (Exception ex)
        {
            return new Response<Location?>(false, ex.Message);
        }
    }

    public async Task<Response<IEnumerable<Location>>> List()
    {
        try
        {
            return new Response<IEnumerable<Location>>(true, await _context.Locations
                .ToListAsync());
        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<Location>>(false, ex.Message);
        }
    }

    public async Task<Response<IEnumerable<Location>>> ListMine(Guid userId)
    {
        try
        {
            return new Response<IEnumerable<Location>>(true, await _context.Records
                .Where(x => x.UserId == userId)
                .Select(x => new Location()
                {
                    Id = x.Location.Id,
                    Latitude = x.Location.Latitude,
                    Longitude = x.Location.Longitude,
                    LocationName = x.Location.LocationName,
                    RecordId = x.Location.RecordId
                })
                .ToListAsync());
        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<Location>>(false, ex.Message);
        }
    }

    public async Task<Response<Location>> Add(Guid userId, Location t)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<Location>> Update(Guid userId, Location t)
    {
        var location = await Get(userId, t.Id);
        _context.Entry(location).CurrentValues.SetValues(t);
        await _context.SaveChangesAsync();
        return await Get(userId, t.Id);
    }

    public async Task<Response<Location>> Delete(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
}