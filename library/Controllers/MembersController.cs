using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace library.Controllers;

public class MembersController : Controller
{
    private readonly MemberRepository _members;

    public MembersController(MemberRepository members)
    {
        _members = members;
    }

    public async Task<IActionResult> Index()
    {
        var members = await _members.GetAllAsync();
        return View(members);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new Member());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Member member)
    {
        if (!ModelState.IsValid)
        {
            return View(member);
        }

        await _members.AddAsync(member);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(long id)
    {
        var member = await _members.GetByIdAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(long id, Member member)
    {
        if (id != member.MemberId)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(member);
        }

        if (await _members.GetByIdAsync(id) == null)
        {
            return NotFound();
        }

        await _members.UpdateAsync(member);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(long id)
    {
        var member = await _members.GetByIdAsync(id);

        if (member == null)
        {
            return NotFound();
        }

        return View(member);
    }


    // POST /Members/Delete/5 : the delete was confirmed.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        try
        {
            // A. Delete the member.
            await _members.DeleteAsync(id);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            // B. Load the member again, to show them on the page.
            var member = await _members.GetByIdAsync(id);

            // E. Check if there is no such member.
            if (member == null)
            {
                // C. Answer "not found".
                return NotFound();
            }

            // H. Set the message the page will show.
            ViewBag.Error = "This member has loans on record, so the database will not delete them.";

            // F. Show the Delete page again, with the member.
            return View("Delete", member);
        }

        // D. The member was deleted: go back to the list.
        return RedirectToAction(nameof(Index));
    }


}