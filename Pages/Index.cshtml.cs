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

        public IDictionary<IBoard, IDictionary<IList, IEnumerable<ICard>>> Data { get; private set; } = new Dictionary<IBoard, IDictionary<IList, IEnumerable<ICard>>>();

        public IndexModel(ITrelloFactory factory, IOptions<TrelloConfiguration> configuration)
        {
            this.factory = factory;
            this.configuration = configuration;
        }

        public async Task OnGet()
        {
            var me = await factory.Me();

            await me.Refresh(force: true);

            //var my_board = new Board(configuration.Value.BoardId);

            //Data.Add(my_board, my_board.Cards.Select(card => card));

            foreach (var board in me.Boards)
            {
                if (board.Id == configuration.Value.BoardId)
                {
                    await board.Refresh();

                    var list_data = new Dictionary<IList, IEnumerable<ICard>>();

                    foreach (var list in board.Lists)
                    {
                        await list.Refresh();

                        list_data.Add(list, list.Cards.Where(card => card.IsArchived == false).Select(card => card));
                    }

                    Data.Add(board, list_data);
                }
            }
        }
    }
}