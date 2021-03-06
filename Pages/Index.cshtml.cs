using Manatee.Trello;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RedLeg.Coaching
{
    public class IndexModel : PageModel
    {
        private readonly ITrelloFactory factory;

        private readonly IOptions<TrelloConfiguration> configuration;

        public string Email => User.FindFirstValue(ClaimTypes.Email);

        public Boolean IsAdmin { get; private set; }

        public ICollection<ICard> Cards { get; private set; }

        public IndexModel(ITrelloFactory factory, IOptions<TrelloConfiguration> configuration)
        {
            this.factory = factory;
            this.configuration = configuration;
        }

        public async Task OnGet()
        {
            var results = new List<ICard>();

            var me = await factory.Me();

            await me.Refresh(force: true);

            // Is the user I'm logged in as match the admin email of the api keys?

            IsAdmin = string.Equals(me.Email, Email, StringComparison.InvariantCultureIgnoreCase);

            foreach (var board in me.Boards)
            {
                // Go through each board, but we're only display cards on the configured board of interest

                if (board.Id == configuration.Value.BoardId)
                {
                    await board.Refresh();

                    var list_data = new Dictionary<IList, IEnumerable<ICard>>();

                    // Go through all of the lists and collect up non-archived cards with the `One-on-One` tag

                    await Task.WhenAll(board.Lists.Select(list => list.Refresh()));

                    foreach (var list in board.Lists)
                    {
                        foreach (var card in list.Cards)
                        {
                            if (await Should_Display(card))
                            {
                                results.Add(card);
                            }
                        }
                    }
                }
            }

            Cards = results;
        }

        protected async Task<Boolean> Should_Display(ICard card)
        {
            if (card.IsArchived == true) return false;

            if (!card.Labels.Any(label => label.Name == "One-on-One")) return false;

            await card.Refresh();

            if (IsAdmin) return true;

            if (card.CustomFields.FirstOrDefault() is CustomField<string> custom_field)
            {
                return string.Equals(Email, custom_field.Value, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }
    }
}