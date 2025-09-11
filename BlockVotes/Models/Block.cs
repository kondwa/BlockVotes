using System.Security.Cryptography;
using System.Text;

namespace BlockVotes.Models
{
    public class Block
    {
        public int Id { get; set; }
        public int Index { get; set; }
        public DateTime Timestamp { get; set; }
        public string PreviousHash { get; set; } = string.Empty;
        public string Hash { get; set; } = string.Empty;
        public int Nonce { get; set; }
        public double MiningTimeSeconds { get; set; }   // ⏱ new
        public List<Vote> Votes { get; set; } = new();

        public string CalculateHash()
        {
            string voteData = string.Join("|", Votes.Select(v => v.VoterId + v.Candidate + v.Timestamp.ToString("s")));
            string blockData = Index + Timestamp.ToString("s") + voteData + PreviousHash + Nonce;
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(blockData));
            return Convert.ToHexString(bytes);
        }

        public void MineBlock(int difficulty)
        {
            Timestamp = DateTime.UtcNow;

            var start = DateTime.UtcNow;

            string target = new string('0', difficulty);
            while (Hash == null || !Hash.StartsWith(target))
            {
                Nonce++;
                Hash = CalculateHash();
            }

            var end = DateTime.UtcNow;
            MiningTimeSeconds = (end - start).TotalSeconds;
        }
    }

}
