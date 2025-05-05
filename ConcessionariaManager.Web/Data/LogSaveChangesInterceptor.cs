using ConcessionariaManager.Core.Interfaces.Services;
using ConcessionariaManager.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ConcessionariaManager.Web.Data
{

    public class LogSaveChangesInterceptor : SaveChangesInterceptor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LogSaveChangesInterceptor(IServiceScopeFactory scopeFactory, IHttpContextAccessor httpContextAccessor)
        {
            _scopeFactory = scopeFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;

            if (context == null)
                return await base.SavingChangesAsync(eventData, result, cancellationToken);

            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Where(e =>
                    e.Entity is ModelBase
                )
                .ToList();

            if (!entries.Any())
                return await base.SavingChangesAsync(eventData, result, cancellationToken);

            var user = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "Sistema";

            using var scope = _scopeFactory.CreateScope();
            var logService = scope.ServiceProvider.GetRequiredService<ILogService>();

            foreach (var entry in entries)
            {
                var entityName = entry.Entity.GetType().Name;

                if (entry.State == EntityState.Added)
                {
                    var newValues = entry.CurrentValues.Properties
                        .ToDictionary(p => p.Name, p => entry.CurrentValues[p]?.ToString());

                    await logService.LogCreateAsync(user, entityName, newValues);
                }
                else if (entry.State == EntityState.Modified)
                {
                    var modifiedValues = new Dictionary<string, (string OldValue, string NewValue)>();

                    foreach (var property in entry.Properties)
                    {
                        if (property.IsModified)
                        {
                            modifiedValues[property.Metadata.Name] = (
                                property.OriginalValue?.ToString(),
                                property.CurrentValue?.ToString()
                            );
                        }
                    }

                    await logService.LogEditAsync(user, entityName, modifiedValues);
                }
            }

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }

}
