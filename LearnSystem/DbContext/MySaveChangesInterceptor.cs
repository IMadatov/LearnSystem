using BaseCrud.Entities;
using LearnSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;

namespace LearnSystem.DbContext
{
    public class MySaveChangesInterceptor : SaveChangesInterceptor
    {

        IServiceProvider _sp;
        public MySaveChangesInterceptor(IServiceProvider serviceProvider)
        {
            _sp=serviceProvider;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            var dbContext = eventData.Context;

            var userCurrent = _sp.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value;


            var changes = dbContext!.ChangeTracker.Entries().ToList();

            foreach (var entry in dbContext!.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                if (entry.Entity is IEntity iEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        iEntity.CreatedDate = DateTime.Now;

                        iEntity.CreatedBy = userCurrent; //
                    }
                    else if (entry.State == EntityState.Modified)
                    {
                        iEntity.LastModifiedDate = DateTime.Now;

                        iEntity.LastModifiedBy = userCurrent;//
                    }

                    iEntity.LastModifiedDate = DateTime.Now;

                    iEntity.LastModifiedBy = userCurrent;//
                }
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
     
  
    }
}
