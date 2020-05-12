using Manatee.Trello;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedLeg.Coaching
{
    public class IndexModel : PageModel
    {
        private readonly Task<IMe> me;

        public IDictionary<IBoard, IEnumerable<List>> Data { get; private set; } = new Dictionary<IBoard, IEnumerable<List>>();

        public IndexModel(Task<IMe> me)
        {
            this.me = me;
        }

        public async Task OnGet()
        {
            var handle = await me;

            foreach (var board in handle.Boards)
            {
                if (board.IsClosed == true)
                {
                    Data.Add(board, Enumerable.Empty<List>());
                }
            }
        }
    }
}