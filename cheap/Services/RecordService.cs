using cheap.Entities;
using cheap.Models;
using Microsoft.EntityFrameworkCore;

namespace cheap.Services;

public class RecordService : IBaseService<Record>
{
    private readonly Context _context;
    private readonly IBaseService<Location> _locationService;
    private readonly IBaseService<Item> _itemService;

    public RecordService(Context context, IBaseService<Location> locationService,
        IBaseService<Item> itemService)
    {
        _context = context;
        _locationService = locationService;
        _itemService = itemService;
    }

    public async Task<Response<Record?>> Get(Guid userId, Guid id)
    {
        try
        {
            return new Response<Record?>(true, await _context.Records
                .Where(x => x.Id == id)
                .Include(x => x.Item)
                .Include(x => x.Location)
                .FirstOrDefaultAsync());
        }
        catch (Exception ex)
        {
            return new Response<Record?>(false, ex.Message);
        }
    }

    public async Task<Response<IEnumerable<Record>>> List()
    {
        try
        {
            return new Response<IEnumerable<Record>>(true, await _context.Records
                .Include(x => x.Item)
                .Include(x => x.Location)
                .ToListAsync());
        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<Record>>(false, ex.Message);
        }
    }

    public async Task<Response<IEnumerable<Record>>> ListMine(Guid userId)
    {
        try
        {
            return new Response<IEnumerable<Record>>(true, await _context.Records
                .Where(x => x.UserId == userId)
                .Include(x => x.Item)
                .Include(x => x.Location)
                .ToListAsync());
        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<Record>>(false, ex.Message);
        }
    }

    public async Task<Response<Record>> Add(Guid userId, Record t)
    {
        try
        {
            var newRecord = await _context.Records.AddAsync(t);
            if (newRecord.Entity.Location != null)
                newRecord.Entity.Location.RecordId = newRecord.Entity.Id;
            if (newRecord.Entity.Item != null)
                newRecord.Entity.Item.RecordId = newRecord.Entity.Id;
            await _context.SaveChangesAsync();
            return new Response<Record>(true, newRecord.Entity);
        }
        catch (Exception e)
        {
            return new Response<Record>(false, e.ToString());
        }
    }

    public async Task<Response<Record>> Update(Guid userId, Record t)
    {
        try
        {
            var record = await Get(userId, t.Id);
            await _itemService.Update(userId, record.Data?.Item);
            await _locationService.Update(userId, record.Data?.Location);
            _context.Entry(record.Data).CurrentValues.SetValues(t);
            await _context.SaveChangesAsync();
            return await Get(userId, t.Id);
        }
        catch (Exception e)
        {
            return new Response<Record>(false, e.ToString());
        }
    }

    public async Task<Response<Record>> Delete(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
}