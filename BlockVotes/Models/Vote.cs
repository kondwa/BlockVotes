namespace BlockVotes.Models
{
    public class Vote
    {
        public int Id { get; set; }                // EF Primary Key
        public string VoterId { get; set; } = string.Empty;
        public string Candidate { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public bool IsCommitted { get; set; } = false;

        public int? BlockId { get; set; }
        public Block? Block { get; set; }           // Navigation property
    }
}
