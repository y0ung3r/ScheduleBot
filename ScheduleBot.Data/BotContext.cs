using Microsoft.EntityFrameworkCore;
using ScheduleBot.Data.Models;

namespace ScheduleBot.Data
{
    public class BotContext : DbContext
    {
        public DbSet<UserSchedule> UserSchedules { get; }

        public BotContext(DbContextOptions<BotContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
