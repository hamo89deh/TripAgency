using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripAgency.Service.ServiceBases

{
    public interface IGenericServices<T>
    {
        public IQueryable<T> GetAll();
        public Task<T?> GetByIdAsync(int id);
        public Task AddAsync(T entity);
        public Task UpdateAsync(T entity);
        public Task DeleteAsync(T entity);
    }
}
