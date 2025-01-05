using System;
using Microsoft.EntityFrameworkCore;
using WayfinderBackend.Api.Data;
using WayfinderBackend.Api.Models;

namespace WayfinderBackend.Api.Services
{
    public interface INavigationService
    {
        Task<NavigationSession> CreateSessionAsync(string startPoint, string endPoint, List<NavigationStep> steps);
        Task<NavigationSession?> GetSessionAsync(string sessionId);
        Task CleanExpiredSessionsAsync();
    }

    public class NavigationService : INavigationService
    {
        private readonly WayfinderDbContext _context;

        public NavigationService(WayfinderDbContext context)
        {
            _context = context;
        }

        public async Task<NavigationSession> CreateSessionAsync(string startPoint, string endPoint, List<NavigationStep> steps)
        {
            var session = new NavigationSession
            {
                StartPoint = startPoint,
                EndPoint = endPoint,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };

            // Set the session ID for each step
            foreach (var step in steps)
            {
                step.NavigationSessionId = session.SessionId;
            }
            session.Steps = steps;

            _context.NavigationSessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<NavigationSession?> GetSessionAsync(string sessionId)
        {
            var session = await _context.NavigationSessions
                .Include(s => s.Steps)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);

            if (session == null || session.ExpiresAt < DateTime.UtcNow)
            {
                if (session != null)
                {
                    _context.NavigationSessions.Remove(session);
                    await _context.SaveChangesAsync();
                }
                return null;
            }

            return session;
        }

        public async Task CleanExpiredSessionsAsync()
        {
            var expiredSessions = await _context.NavigationSessions
                .Where(s => s.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();

            if (expiredSessions.Any())
            {
                _context.NavigationSessions.RemoveRange(expiredSessions);
                await _context.SaveChangesAsync();
            }
        }
    }
}
