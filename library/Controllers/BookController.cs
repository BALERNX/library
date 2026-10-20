using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;
namespace library.Controllers;

public class BooksController : Controller
{
    private readonly BookRepository _books;

    // ASP.NET Core hands in the repository, because Program.cs registered it.
    public BooksController(BookRepository books)
    {
        _books = books;
    }
    // GET /Books : the list of books.
    public async Task<IActionResult> Index()
    {
        var books = await _books.GetAllAsync();
        return View(books);                             // Views/Books/Index.cshtml
    }
    // GET /Books/Create : an empty form.
    public IActionResult Create()
    {
        return View(new Book());
    }
    // POST /Books/Create : the form was submitted.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book book)
    {
        if (!ModelState.IsValid)                        // a required field was left empty
        {
            return View(book);
        }

        await _books.AddAsync(book);
        return RedirectToAction(nameof(Index));
    }
    // GET /Books/Edit/5 : the form, filled with book 5.
    public async Task<IActionResult> Edit(long id)
    {
        var book = await _books.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }
    // POST /Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, Book book)
    {
        if (id != book.BookId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(book);
        }

        if (await _books.GetByIdAsync(id) == null)
        {
            return NotFound();
        }

        await _books.UpdateAsync(book);
        return RedirectToAction(nameof(Index));
    }

    // GET /Books/Delete/5 : ask "are you sure?"
    public async Task<IActionResult> Delete(long id)
    {
        var book = await _books.GetByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }
    // POST /Books/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        if (await _books.GetByIdAsync(id) == null)
        {
            return NotFound();
        }

        await _books.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

}