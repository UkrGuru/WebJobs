using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using UkrGuru.SqlJson;
using UkrGuru.WebJobs.Data;

namespace WebJobsDemo.Pages.Actions;

public class CreateModel : PageModel
{
    public IActionResult OnGet()
    {
        return Page();
    }

    [BindProperty]
    public ActionInput Action { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        await DbHelper.ExecAsync("WJbActions_Ins_Demo", Action);

        return RedirectToPage("./Index");
    }
}
