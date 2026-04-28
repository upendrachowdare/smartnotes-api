using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmartNotes.Api.Models;
using SmartNotes.Api.Services;

namespace SmartNotes.Api.Controllers
{
    [ApiController]
    [Route("api/notes")]
    public class NotesController : ControllerBase
    {
        private static readonly ConcurrentDictionary<System.Guid, Note> _store = new();
        private readonly IOpenAiService _ai;

        public NotesController(IOpenAiService ai)
        {
            _ai = ai;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_store.Values);
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetById(System.Guid id)
        {
            if (_store.TryGetValue(id, out var note))
            {
                return Ok(note);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Note note)
        {
            if (string.IsNullOrWhiteSpace(note.Content))
            {
                return BadRequest("Content is required.");
            }

            note.Summary = await _ai.SummarizeAsync(note.Content);
            _store[note.Id] = note;

            return CreatedAtAction(nameof(GetById), new { id = note.Id }, note);
        }
    }
}
