using Ecom.Notification.Core.Abstractions.Persistence;
using Ecom.Notification.Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace Ecom.Notification.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EcomNotificationDbContext dbContext;
        private Dictionary<Type, object> _repositories = new Dictionary<Type, object>();
        private IDbContextTransaction _transaction;
        public UnitOfWork(EcomNotificationDbContext context)
        {
            dbContext = context;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            IRepository<T> repository = null;
            if (_repositories.ContainsKey(typeof(T)))
            {
                repository = _repositories[typeof(T)] as IRepository<T>;
            }
            else
            {
                repository = new Repository<T>(dbContext);
                _repositories.Add(typeof(T), repository);
            }

            return (Repository<T>)repository;
        }

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public void SaveChanges()
        {
            dbContext.SaveChanges();
        }

        // Khoi tao Transaction
        public async Task BeginTransactionAsync()
        {
            _transaction = await dbContext.Database.BeginTransactionAsync();
        }

        // Xac nhan thay doi va ket thuc Transaction
        public async Task CommitAsync()
        {
            try
            {
                await dbContext.SaveChangesAsync();
                if (_transaction != null) await _transaction.CommitAsync();
            }
            catch
            {
                await RollbackAsync();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        // Hoan tac neu co loi
        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }
}
