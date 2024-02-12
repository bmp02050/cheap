using cheap.Entities;
using cheap.Models;
using Microsoft.EntityFrameworkCore;

namespace cheap.Services;

public class RecordService : IBaseService<Record>
{
    private readonly Context Context;
    private readonly IBaseService<Location> LocationService;
    private readonly IBaseService<Item> ItemService;

    public RecordService(Context context, IBaseService<Location> locationService,
        IBaseService<Item> itemService)
    {
        Context = context;
        LocationService = locationService;
        ItemService = itemService;
    }

    public async Task<Response<Record?>> Get(Guid userId, Guid id)
    {
        try
        {
            return new Response<Record?>(true, await Context.Records
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

    public Task<Response<IEnumerable<Record>>> List()
    {
        throw new NotImplementedException();
    }

    public Task<Response<IEnumerable<Record>>> ListMine(Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<Record>> Add(Guid userId, Record t)
    {
        try
        {
            var newRecord = await Context.Records.AddAsync(t);
            newRecord.Entity.Location.RecordId = newRecord.Entity.Id;
            newRecord.Entity.Item.RecordId = newRecord.Entity.Id;
            await Context.SaveChangesAsync();
            return new Response<Record>(true, newRecord.Entity);
        }
        catch (Exception e)
        {
            return new Response<Record>(false, e.ToString());
        }
    }

    public async Task<Response<Record?>> Update(Guid userId, Record t)
    {
        try
        {
            var record = await Get(userId, t.Id);
            await ItemService.Update(userId, record.Data?.Item);
            await LocationService.Update(userId, record.Data?.Location);
            Context.Entry(record.Data).CurrentValues.SetValues(t);
            await Context.SaveChangesAsync();
            return await Get(userId, t.Id);
        }
        catch (Exception e)
        {
            return new Response<Record?>(false, e.ToString());
        }
    }

    public Task<Response<Record>> Delete(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
}