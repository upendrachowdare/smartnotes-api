using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartNotes.Api.Data;
using SmartNotes.Api.Models;

namespace SmartNotes.Api.Controllers
{
    [ApiController]
    [Route("api/applications")]
    public class ApplicationsController : ControllerBase
    {
        private readonly SmartNotesContext _db;
        private readonly ILogger<ApplicationsController> _logger;

        public ApplicationsController(SmartNotesContext db, ILogger<ApplicationsController> logger)
        {
            _db = db;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.ApplicationAttempts.AsNoTracking().ToListAsync();
            return Ok(list);
        }

        [HttpPost("approve/{id}")]
        public async Task<IActionResult> Approve(int id)
        {
            var attempt = await _db.ApplicationAttempts.FindAsync(id);
            if (attempt == null) return NotFound();

            if (attempt.Success)
            {
                return BadRequest("Already approved/processed.");
            }

            attempt.Success = true;
            attempt.Details = "Approved manually via API";
            await _db.SaveChangesAsync();

            _logger.LogInformation("Application attempt {id} approved manually.", id);

            return Ok(attempt);
        }
    }
}
