namespace BlockVotes.Models
{
    public class Blockchain
    {
        public List<Block> Chain { get; set; } = new();
        public List<Vote> PendingVotes { get; set; } = new();
        public int Difficulty { get; set; } = 2;

        public Blockchain()
        {
            // Create genesis block
            Block genesis = new Block { Index = 0, PreviousHash = "0" };
            genesis.MineBlock(Difficulty);
            //genesis.Hash = genesis.CalculateHash();
            Chain.Add(genesis);
        }

        public void AddVote(Vote vote)
        {
            PendingVotes.Add(vote);
        }

        public void MinePendingVotes()
        {
            Block block = new Block
            {
                Index = Chain.Count,
                Votes = [..PendingVotes],
                PreviousHash = Chain.Last().Hash
            };

            block.MineBlock(Difficulty);
            Chain.Add(block);
            PendingVotes.Clear();
        }

        public bool IsValid()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                var current = Chain[i];
                var previous = Chain[i - 1];

                if (current.Hash != current.CalculateHash()) return false;
                if (current.PreviousHash != previous.Hash) return false;
            }
            return true;
        }
    }
}
