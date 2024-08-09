using CommonModel;
using Microsoft.EntityFrameworkCore;
using RoomWebApi.Common.Caching;
using RoomWebApi.Common.Data;
using RoomWebApi.Common.Repository;

namespace RoomWebApi.Servies
{
    public interface ITypeRoomService
    {
        Task<ResponseService> GetRoomsByTypeId(Guid typeRoomId);
        ResponseService GetAll();
    }
    public class TypeRoomService : ITypeRoomService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICachingDbContext _cachingContext;

        public TypeRoomService(IUnitOfWork unitOfWork, ICachingDbContext cachingDbContext)
        {
            _unitOfWork = unitOfWork;
            _cachingContext = cachingDbContext;
        }

        public ResponseService GetAll()
        {
            var typeRooms = _unitOfWork.Roomtypes.FindAll();

            return new ResponseService() { Data = typeRooms};
        }

        public async Task<ResponseService> GetRoomsByTypeId(Guid typeRoomId)
        {
            var cacheRooms = _cachingContext.GetData<List<Room>>("roomsByTypeId");

            if (cacheRooms != null)
                return new ResponseService() { Data = cacheRooms };

            var typeRoom = await _unitOfWork.Roomtypes.FindByCondition(s => s.Id == typeRoomId).Include(c => c.Rooms).FirstOrDefaultAsync();
            if (typeRoom == null)
                return new ResponseService() { ErrorMessage = "Type room not found"};

            _cachingContext.SetData<IEnumerable<Room>>("roomsByTypeId", typeRoom.Rooms, DateTimeOffset.Now.AddSeconds(10));
            return new ResponseService() { Data = typeRoom.Rooms };
        }
    }
}
