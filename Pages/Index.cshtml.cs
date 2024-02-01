using IdentityRazor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityRazor.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly MyBlogContext _myContext;

    public IndexModel(ILogger<IndexModel> logger, MyBlogContext myContext)
    {
        _logger = logger;
        _myContext = myContext;
    }

    public void OnGet()
    {

    }
}
