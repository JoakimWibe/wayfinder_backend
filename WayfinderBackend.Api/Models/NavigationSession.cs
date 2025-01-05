using System;

namespace WayfinderBackend.Api.Models
{
    public class NavigationSession
    {
        public string SessionId { get; set; } = Guid.NewGuid().ToString();
        public required string StartPoint { get; set; }
        public required string EndPoint { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; set; }
        public List<NavigationStep> Steps { get; set; } = new List<NavigationStep>();
    }

    public class NavigationStep
    {
        public string NavigationSessionId { get; set; } = null!;
        public int Order { get; set; }
        public required string Instruction { get; set; }
        public required string Direction { get; set; }
        public double? Distance { get; set; }
    }
}
