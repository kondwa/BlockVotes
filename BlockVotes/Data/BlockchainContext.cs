using BlockVotes.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

namespace BlockVotes.Data
{

    public class BlockchainContext : IdentityDbContext<AppUser>
    {
        public DbSet<Block> Blocks { get; set; }
        public DbSet<Vote> Votes { get; set; }

        public BlockchainContext(DbContextOptions<BlockchainContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Block>()
                .HasMany(b => b.Votes)
                .WithOne(v => v.Block)
                .HasForeignKey(v => v.BlockId);
            modelBuilder.Entity<Vote>()
              .HasIndex(v => v.VoterId)
              .IsUnique();  // ensures only one vote per voter
        }
    }
}
