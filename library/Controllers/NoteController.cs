
using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;

namespace library.Controllers;

public class NotesController : Controller
{
    private readonly NoteRepository _repository;

    public NotesController(NoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<IActionResult> Index()
    {
        var notes = await _repository.GetAllAsync();
        return View(notes);
    }

    public IActionResult Create()
    {
        return View(new Note { CreatedOn = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Note note)
    {
        if (!ModelState.IsValid)
            return View(note);

        await _repository.AddAsync(note);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(long id)
    {
        var note = await _repository.GetByIdAsync(id);

        if (note == null)
            return NotFound();

        return View(note);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, Note note)
    {
        if (id != note.NoteId)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(note);

        if (await _repository.GetByIdAsync(id) == null)
            return NotFound();

        await _repository.UpdateAsync(note);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(long id)
    {
        var note = await _repository.GetByIdAsync(id);

        if (note == null)
            return NotFound();

        return View(note);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        if (await _repository.GetByIdAsync(id) == null)
            return NotFound();

        await _repository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
