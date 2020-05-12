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

        public IDictionary<IBoard, IEnumerable<IList>> Data { get; private set; } = new Dictionary<IBoard, IEnumerable<IList>>();

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
                    await board.Refresh();

                    Data.Add(board, board.Lists.Select(list => list));
                }
            }
        }
    }
}