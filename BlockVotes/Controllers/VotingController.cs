using BlockVotes.Data;
using BlockVotes.Models;
using BlockVotes.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BlockVotes.Controllers
{
    [Authorize]
    public class VotingController : Controller
    {
        private readonly BlockchainService blockchain;
        private const int Difficulty = 2;

        public VotingController(BlockchainService blockchain)
        {
            this.blockchain = blockchain;
        }

        public async Task<ActionResult> Index()
        {
            var blocks = await blockchain.GetBlockchainAsync();

            return View(blocks);
        }

        [HttpPost]
        public async Task<ActionResult> CastVote(string candidate)
        {
            string voterId = User.Identity?.Name ?? "";
            try
            {
                await blockchain.AddVoteAsync(voterId, candidate);
                TempData["Success"] = "Voted successfully.";
            }
            catch (Exception ex) { 
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Pending");
        }

        public async Task<ActionResult> PendingAsync()
        {
            List<Vote> pending = await blockchain.GetPendingVotesAsync();
            return View(pending);
        }

        [HttpPost]
        public async Task<ActionResult> Close()
        {
            try
            {
                await blockchain.MinePendingVotesAsync();
            }catch(Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
        public async Task<ActionResult> Results()
        {
            var results = await blockchain.GetVoteResultsAsync();
            return View(results);
        }
        public async Task<ActionResult> Integrity()
        {
            var blocks = await blockchain.GetBlockchainAsync();

            var issues = await blockchain.IsValid();

            ViewBag.Blocks = blocks;
            ViewBag.Issues = issues;
            ViewBag.IsValid = !issues.Any();

            return View();
        }

    }
}
