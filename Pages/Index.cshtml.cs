using Manatee.Trello;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedLeg.Coaching
{
    public class IndexModel : PageModel
    {
        private readonly ITrelloFactory factory;

        private readonly IOptions<TrelloConfiguration> configuration;

        public IDictionary<IBoard, IEnumerable<IList>> Data { get; private set; } = new Dictionary<IBoard, IEnumerable<IList>>();

        public IndexModel(ITrelloFactory factory, IOptions<TrelloConfiguration> configuration)
        {
            this.factory = factory;
            this.configuration = configuration;
        }

        public async Task OnGet()
        {
            var me = await factory.Me();

            await me.Refresh(force: true);

            foreach (var board in me.Boards)
            {
                if (board.IsClosed == false)
                {
                    await board.Refresh();

                    Data.Add(board, board.Lists.Select(list => list));
                }
            }
        }
    }
}