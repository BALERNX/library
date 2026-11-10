
using library.Data;
using library.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace library.Controllers;

public class MembersController : Controller
{
    private readonly MemberRepository _members;
    private readonly LoanRepository _loans;

    // ASP.NET Core hands in the repositories.
    public MembersController(MemberRepository members, LoanRepository loans)
    {
        _members = members;
        _loans = loans;
    }

    // GET /Members : display all members.
    public async Task<IActionResult> Index()
    {
        var members = await _members.GetAllAsync();
        return View(members);
    }

    // GET /Members/Create : display the empty form.
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Member());
    }

    // POST /Members/Create : the form was submitted.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Member member)
    {
        if (!ModelState.IsValid)
        {
            return View(member);
        }

        try
        {
            await _members.AddAsync(member);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ModelState.AddModelError("Email", "Another member already uses this email.");
            return View(member);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET /Members/Edit/5 : display the edit form.
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

    // POST /Members/Edit/5 : the changed form was submitted.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Member member)
    {
        if (!ModelState.IsValid)
        {
            return View(member);
        }

        try
        {
            await _members.UpdateAsync(member);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ModelState.AddModelError("Email", "Another member already uses this email.");
            return View(member);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET /Members/Delete/5 : display the delete confirmation.
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
            await _members.DeleteAsync(id);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
        {
            var member = await _members.GetByIdAsync(id);

            if (member == null)
            {
                return NotFound();
            }

            ViewBag.Error = "This member has loans on record, so the database will not delete them.";

            return View("Delete", member);
        }

        return RedirectToAction(nameof(Index));
    }

    // GET /Members/Details/5 : one member and their loans.
    public async Task<IActionResult> Details(long id)
    {
        // 1. Load the member.
        var member = await _members.GetByIdAsync(id);

        // 2. Check if the member does not exist.
        if (member == null)
        {
            // 3. Answer "not found".
            return NotFound();
        }

        // 4. Put the member and their loans together.
        var details = new MemberDetails
        {
            Member = member,
            Loans = await _loans.GetByMemberAsync(id)
        };

        // 5. Show the page.
        return View(details);
    }
}
