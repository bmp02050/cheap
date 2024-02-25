using cheap.Entities;
using cheap.Models;
using Microsoft.EntityFrameworkCore;

namespace cheap.Services;

public class ItemService : IBaseService<Item>
{
    private readonly Context _context;

    public ItemService(Context context)
    {
        _context = context;
    }

    public async Task<Response<Item?>> Get(Guid userId, Guid id)
    {
        try
        {
            return new Response<Item?>(true, await _context.Items
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync());
        }
        catch (Exception ex)
        {
            return new Response<Item?>(false, ex.Message);
        }
    }

    public async Task<Response<IEnumerable<Item>>> List()
    {
        try
        {
            return new Response<IEnumerable<Item>>(true, await _context.Items
                .ToListAsync());
        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<Item>>(false, ex.Message);
        }
    }

    public async Task<Response<IEnumerable<Item>>> ListMine(Guid userId)
    {
        try
        {
            return new Response<IEnumerable<Item>>(true, await _context.Records
                .Where(x => x.UserId == userId)
                .Select(x => new Item()
                {
                    Barcode = x.Item.Barcode,
                    Cost = x.Item.Cost,
                    Description = x.Item.Description,
                    Id = x.Item.Id,
                    ImageData = x.Item.ImageData,
                    ImageText = x.Item.ImageText,
                    Name = x.Item.Name,
                    Quantity = x.Item.Quantity,
                    RecordId = x.Item.RecordId,
                    UnitPrice = x.Item.UnitPrice
                })
                .ToListAsync());
        }
        catch (Exception ex)
        {
            return new Response<IEnumerable<Item>>(false, ex.Message);
        }
    }

    public Task<Response<Item>> Add(Guid userId, Item t)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<Item?>> Update(Guid userId, Item t)
    {
        var item = await Get(userId, t.Id);
        _context.Entry(item).CurrentValues.SetValues(t);
        await _context.SaveChangesAsync();
        return await Get(userId, t.Id);
    }

    public async Task<Response<Item>> Delete(Guid userId, Guid id)
    {
        throw new NotImplementedException();
    }
}