using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WABot.Helpers.Json;

namespace WABot.Data
{
    public class WABotContext : DbContext
    {
        public WABotContext (DbContextOptions<WABotContext> options)
            : base(options)
        {
        }

        public DbSet<WABot.Helpers.Json.Message> Message { get; set; }
    }
}
