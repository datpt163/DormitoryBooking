using Microsoft.EntityFrameworkCore.Storage;
using System.Security.Principal;
using System.Security;
using AuthenticationWebApi.Common.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationWebApi.Common.Repository
{

    public  interface IUnitOfWork
    {
        IRepository<Role> Roles { get; }
        IRepository<User> Users { get; }

        void SaveChanges();
        Task SaveChangesAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly dormitorybookinguserContext DbContext;
        private readonly IRepository<Role> roles;
        private readonly IRepository<User> users;

        public UnitOfWork(dormitorybookinguserContext dbContext, IRepository<Role> roles, IRepository<User> users)
        {
            DbContext = dbContext;
            this.roles = roles;
            this.users = users;
        }

        public IRepository<Role> Roles => roles;

        public IRepository<User> Users => users;


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
