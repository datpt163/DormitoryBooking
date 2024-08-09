using Microsoft.EntityFrameworkCore.Storage;
using System.Security.Principal;
using System.Security;
using BookingWebApi.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingWebApi.Common.Repository
{

    public  interface IUnitOfWork
    {
        IRepository<Booking> Bookings { get; }
        IRepository<Semester> Semesters { get; }

        void SaveChanges();
        Task SaveChangesAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly dormitorybookingbookingContext DbContext;
        private readonly IRepository<Booking> bookings;
        private readonly IRepository<Semester> semesters;

        public UnitOfWork(dormitorybookingbookingContext dbContext, IRepository<Booking> bookings, IRepository<Semester> semesters)
        {
            DbContext = dbContext;
            this.bookings = bookings;
            this.semesters = semesters;
        }

        public IRepository<Booking> Bookings => bookings;

        public IRepository<Semester> Semesters => semesters;


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
