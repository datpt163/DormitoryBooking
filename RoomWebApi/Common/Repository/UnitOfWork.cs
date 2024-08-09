using Microsoft.EntityFrameworkCore.Storage;
using System.Security.Principal;
using System.Security;
using RoomWebApi.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace RoomWebApi.Common.Repository
{

    public  interface IUnitOfWork
    {
        IRepository<Building> Buildings { get; }
        IRepository<Room> Rooms { get; }

        IRepository<Roomtype> Roomtypes { get; }

        void SaveChanges();
        Task SaveChangesAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly dormitorybookingroomContext DbContext;
        private readonly IRepository<Room> rooms;
        private readonly IRepository<Roomtype> roomtypes;
        private readonly IRepository<Building> buildings;

        public UnitOfWork(dormitorybookingroomContext dbContext, IRepository<Room> rooms, IRepository<Roomtype> roomtypes, IRepository<Building> buildings)
        {
            DbContext = dbContext;
            this.rooms = rooms;
            this.roomtypes = roomtypes;
            this.buildings = buildings;
        }

        public IRepository<Building> Buildings => buildings;

        public IRepository<Room> Rooms => rooms;

        public IRepository<Roomtype> Roomtypes => roomtypes;

        public IDbContextTransaction BeginTransaction()
        {
            throw new NotImplementedException();
        }

        public void SaveChanges()
        {
            using (var transactionResult = DbContext.Database.BeginTransaction(System.Data.IsolationLevel.Snapshot))
            {
                try
                {
                    DbContext.SaveChanges();
                    transactionResult.Commit();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("SaveChanges: " + ex.GetBaseException());
                    transactionResult.Rollback();
                    throw;
                }
            }

        }

        public async Task SaveChangesAsync()
        {
            using (var transactionResult = await DbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Snapshot))
            {
                try
                {
                    await DbContext.SaveChangesAsync();
                    await transactionResult.CommitAsync();
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine("SaveChangesAsync: " + ex.GetBaseException());
                    await transactionResult.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
