using BlockVotes.Data;
using BlockVotes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlockVotes.Controllers
{
    [Authorize]
    public class VotingController : Controller
    {
        private readonly BlockchainContext _context;
        private const int Difficulty = 2;

        public VotingController(BlockchainContext context)
        {
            _context = context;
        }

        public ActionResult Index()
        {
            var blocks = _context.Blocks
                .Include(b => b.Votes)
                .OrderBy(b => b.Index)
                .ToList();

            return View(blocks);
        }

        [HttpPost]
        public async Task<ActionResult> CastVote(string candidate)
        {
            string? voterId = User.Identity?.Name;
            if (string.IsNullOrEmpty(voterId))
            {
                TempData["Error"] = "The voter needs to login to vote.";
                return RedirectToAction("Pending");
            }

            // Check DB for existing vote by this voter
            bool alreadyVoted = _context.Votes.Any(v => v.VoterId == voterId);

            if (alreadyVoted)
            {
                TempData["Error"] = "This voter has already cast a vote.";
                return RedirectToAction("Pending");
            }

            _context.Votes.Add(new Vote { VoterId = voterId, Candidate = candidate,Timestamp = DateTime.UtcNow,IsCommitted = false });
            await _context.SaveChangesAsync();
            TempData["Success"] = "Voted successfully.";
            return RedirectToAction("Pending");
        }

        public ActionResult Pending()
        {
            List<Vote> pending = _context.Votes.Where(v=>!v.IsCommitted).ToList();
            return View(pending);
        }

        [HttpPost]
        public async Task<ActionResult> Close()
        {
            List<Vote> pendings = await _context.Votes.Where(v => !v.IsCommitted).ToListAsync();
            if (!pendings.Any())
            {
                TempData["Error"] = "No votes to mine block";
                return RedirectToAction("Index");
            }
            var lastBlock = _context.Blocks.OrderByDescending(b => b.Index).FirstOrDefault();
            int nextIndex = lastBlock?.Index + 1 ?? 0;
            string prevHash = lastBlock?.Hash ?? "0";

            Block block = new Block
            {
                Index = nextIndex,
                PreviousHash = prevHash,
                Votes = [..pendings]
            };

            block.MineBlock(Difficulty);
            
            _context.Blocks.Add(block);
            
            pendings.ForEach(v => v.IsCommitted = true);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Vote closed and Block mined successfully";
            return RedirectToAction("Index");
        }
        public ActionResult Results()
        {
            var results = _context.Votes
                .Where(v=>v.IsCommitted)
                .GroupBy(v => v.Candidate)
                .Select(g => new VoteResult
                {
                    Candidate = g.Key,
                    TotalVotes = g.Count()
                })
                .OrderByDescending(r => r.TotalVotes)
                .ToList();

            return View(results);
        }
        public ActionResult Integrity()
        {
            var blocks = _context.Blocks
                .Include(b => b.Votes)
                .OrderBy(b => b.Index)
                .ToList();

            var issues = new List<string>();

            for (int i = 0; i < blocks.Count; i++)
            {
                var current = blocks[i];
                string calculatedHash = current.CalculateHash();

                if (current.Hash != calculatedHash)
                {
                    issues.Add($"Block {current.Index} has been tampered with (stored hash does not match calculated hash).");
                }

                if (i > 0)
                {
                    var prev = blocks[i - 1];
                    if (current.PreviousHash != prev.Hash)
                    {
                        issues.Add($"Block {current.Index} previous hash does not match Block {prev.Index} hash.");
                    }
                }
            }

            ViewBag.Blocks = blocks;
            ViewBag.Issues = issues;
            ViewBag.IsValid = !issues.Any();

            return View();
        }

    }
}
