using AutoMapper;
using cheap.Entities;
using cheap.Models;
using cheap.Models.Users;

namespace cheap
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();
            CreateMap<User, UserModel>();
            CreateMap<UserModel, User>();
            CreateMap<Record, RecordModel>();
            CreateMap<RecordModel, Record>();
            CreateMap<Item, ItemModel>();
            CreateMap<ItemModel, Item>();
            CreateMap<Location, LocationModel>();
            CreateMap<LocationModel, Location>();
            CreateMap<IEnumerable<Record>, IEnumerable<RecordModel>>();
            CreateMap<IEnumerable<RecordModel>, IEnumerable<Record>>();
        }
    }
}