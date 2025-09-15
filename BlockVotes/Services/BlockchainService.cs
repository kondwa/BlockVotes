using BlockVotes.Data;
using BlockVotes.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlockVotes.Services
{
    public class BlockchainService
    {
        private const int Difficulty = 2;
        private readonly BlockchainContext context;
        public BlockchainService(BlockchainContext context) { 
            this.context = context;
            InitializeBlockchain();
        }
        public void InitializeBlockchain()
        {
            if (!context.Blocks.Any())
            {
                Block genesisBlock = new()
                {
                    Index = 0,
                    PreviousHash = "GENESIS",
                    Timestamp = DateTime.UtcNow
                };
                genesisBlock.MineBlock(Difficulty);
                context.Blocks.Add(genesisBlock);
                context.SaveChanges();
            }
        }
        public async Task MinePendingVotesAsync()
        {
            List<Vote> pendingVotes = await GetPendingVotesAsync();
            if (!pendingVotes.Any())
            {
                throw new InvalidOperationException("No votes to mine block.");
            }
            var lastBlock = await context.Blocks.OrderByDescending(b => b.Index).FirstOrDefaultAsync();
            int nextIndex = lastBlock?.Index + 1 ?? 0;
            string prevHash = lastBlock?.Hash ?? "0";

            Block block = new()
            {
                Index = nextIndex,
                Votes = [..pendingVotes],
                PreviousHash = prevHash
            };
            block.MineBlock(Difficulty);

            context.Blocks.Add(block);

            pendingVotes.ForEach(v => v.IsCommitted = true);

            await context.SaveChangesAsync();
        }
        public async Task<List<string>> IsValid()
        {
            List<Block> blocks = await GetBlockchainAsync();
            List<string> issues = new();
            for(int i =0; i < blocks.Count; i++)
            {
                var current = blocks[i];
                var previous = i > 0 ? blocks[i - 1] : null;
                if (current.Hash != current.CalculateHash())
                {
                    issues.Add($"Block {current.Index} has been tampered with (stored hash does not match calculated hash).");
                }
                if (previous != null && current.PreviousHash != previous.Hash)
                {
                    issues.Add($"Block {current.Index} previous hash does not match Block {previous.Index} hash.");
                }
            }
            return issues;
        }
        public async Task AddVoteAsync(string voterId, string candidate)
        {
            if(string.IsNullOrEmpty(voterId))
                throw new ArgumentException("Voter ID cannot be null or empty", nameof(voterId));
            if(string.IsNullOrEmpty(candidate))
                throw new ArgumentException("Candidate cannot be null or empty", nameof(candidate));
            if (context.Votes.Any(v => v.VoterId == voterId))
            {
                throw new InvalidOperationException("This voter has already cast a vote.");
            }
            context.Votes.Add(new Vote { VoterId = voterId, Candidate = candidate, Timestamp = DateTime.UtcNow, IsCommitted = false });
            await context.SaveChangesAsync();
        }
        public async Task<List<Block>> GetBlockchainAsync()
        {
            var blocks = await context.Blocks
                .Include(b => b.Votes)
                .OrderBy(b => b.Index)
                .ToListAsync();
            return blocks;
        }
        public async Task<List<Vote>> GetPendingVotesAsync()
        {
            return await context.Votes.Where(v => !v.IsCommitted).ToListAsync();
        }
        public async Task<List<VoteResult>> GetVoteResultsAsync()
        {
            var results = await context.Votes
                .Where(v => v.IsCommitted)
                .GroupBy(v => v.Candidate)
                .Select(g => new VoteResult
                {
                    Candidate = g.Key,
                    TotalVotes = g.Count()
                })
                .OrderByDescending(r => r.TotalVotes)
                .ToListAsync();
            return results;
        }
    }
}
