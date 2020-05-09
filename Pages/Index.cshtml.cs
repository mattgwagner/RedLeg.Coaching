using Manatee.Trello;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace RedLeg.Coaching
{
    public class IndexModel : PageModel
    {
        private readonly Task<IMe> me;

        public IBoardCollection Boards { get; private set; }

        public IndexModel(Task<IMe> me)
        {
            this.me = me;
        }

        public async Task OnGet()
        {
            Boards = (await me).Boards;
        }
    }
}