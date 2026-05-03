using ClinicManagement.Data;
using ClinicManagement.Models.Entities;

namespace ClinicManagement.Helpers
{
    public class AuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>Logs an audit entry for create, update, or delete operations.</summary>
        public async Task LogAsync(string action, string entity, string? entityId, string? details = null)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();

            _context.AuditLogs.Add(new AuditLog
            {
                UserId = userId,
                Action = action,
                Entity = entity,
                EntityId = entityId,
                Details = details,
                IPAddress = ip,
                Timestamp = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
        }
    }
}
