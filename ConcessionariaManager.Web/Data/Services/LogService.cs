using ConcessionariaManager.Core.Interfaces.Services;
using ConcessionariaManager.Core.Models.AppLog;
using System;

namespace ConcessionariaManager.Web.Data.Services
{
    public class LogService : ILogService
    {
        private readonly ApplicationDbContext _context;

        public LogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogCreateAsync(string user, string screen, Dictionary<string, string> newValues)
        {
            var log = new Log
            {
                Date = DateTime.UtcNow,
                User = user,
                Action = "Criação",
                Screen = screen
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();

            var logDetails = newValues.Select(kvp => new LogDetail
            {
                LogId = log.Id,
                Field = kvp.Key,
                NewValue = kvp.Value,
                Log = log
            }).ToList();

            _context.LogDetails.AddRange(logDetails);
            await _context.SaveChangesAsync();
        }

        public async Task LogEditAsync(string user, string screen, Dictionary<string, (string OldValue, string NewValue)> modifiedValues)
        {
            var log = new Log
            {
                Date = DateTime.UtcNow,
                User = user,
                Action = "Edição",
                Screen = screen
            };

            _context.Logs.Add(log);
            await _context.SaveChangesAsync();

            var logDetails = modifiedValues.Select(kvp => new LogDetail
            {
                LogId = log.Id,
                Field = kvp.Key,
                OldValue = kvp.Value.OldValue,
                NewValue = kvp.Value.NewValue,
                Log = log
            }).ToList();

            _context.LogDetails.AddRange(logDetails);
            await _context.SaveChangesAsync();
        }
    }
}
