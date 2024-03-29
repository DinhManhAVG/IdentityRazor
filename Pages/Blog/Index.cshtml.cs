using IdentityRazor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace IdentityRazor.Pages.Blog
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly MyBlogContext _context;

        public IndexModel(MyBlogContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get;set; }

        public const int ITEMS_PER_PAGE = 15;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }

        public int countPages { get; set; }



        public async Task OnGetAsync(string SearchString)
        {
            // Article = await _context.articles.ToListAsync();

            int totalArticle = await _context.articles.CountAsync();
            countPages = (int)Math.Ceiling((double)totalArticle / ITEMS_PER_PAGE);

            if (currentPage < 1)
                currentPage = 1;
            if (currentPage > countPages)
                currentPage = countPages;

            var qr = _context.articles.OrderBy(a => a.Created).Select(a => a)
                                      .Skip((currentPage - 1) * ITEMS_PER_PAGE)
                                      .Take(ITEMS_PER_PAGE);


            if (!string.IsNullOrEmpty(SearchString))       
            {
                Article = qr.Where(a => a.Title.Contains(SearchString)).ToList();
            }  
            else
            {
                Article = await qr.ToListAsync(); 
            }

                   
        }
    }
}