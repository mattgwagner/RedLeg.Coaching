using Manatee.Trello;
using Microsoft.AspNetCore.Mvc;
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

        public IDictionary<IBoard, IDictionary<IList, IEnumerable<ICard>>> Data { get; private set; } = new Dictionary<IBoard, IDictionary<IList, IEnumerable<ICard>>>();

        public IndexModel(ITrelloFactory factory, IOptions<TrelloConfiguration> configuration)
        {
            this.factory = factory;
            this.configuration = configuration;
        }

        public async Task OnGet()
        {
            Data = await GetData();
        }

        public async Task<IActionResult> OnPostAddItem(string board, string list, string card, string checklist, string text)
        {
            // Oh god this is ugly

            var data = await GetData(board, list, card);

            var checklist_reference =
                data
                .Where(b => b.Key.Id == board)
                .SelectMany(b => b.Value)
                .Where(l => l.Key.Id == list)
                .SelectMany(l => l.Value)
                .Where(c => c.Id == card)
                .SelectMany(c => c.CheckLists)
                .Where(cl => cl.Id == checklist)
                .SingleOrDefault();

            if (checklist_reference != null)
            {
                await checklist_reference.CheckItems.Add(text);
            }

            return Redirect("/");
        }

        public async Task<IActionResult> OnPostToggleCheck(string board, string list, string card, string checklist, string checklistitem, Boolean isChecked)
        {
            // Oh god this is ugly

            var data = await GetData(board, list, card);

            var checklistitem_reference =
                data
                .Where(b => b.Key.Id == board)
                .SelectMany(b => b.Value)
                .Where(l => l.Key.Id == list)
                .SelectMany(l => l.Value)
                .Where(c => c.Id == card)
                .SelectMany(c => c.CheckLists)
                .Where(cl => cl.Id == checklist)
                .SelectMany(cli => cli.CheckItems)
                .Where(cli => cli.Id == checklistitem)
                .SingleOrDefault();

            if (checklistitem_reference != null)
            {
                checklistitem_reference.State = isChecked ? CheckItemState.Complete : CheckItemState.Incomplete;
            }

            return Redirect("/");
        }

        protected async Task<IDictionary<IBoard, IDictionary<IList, IEnumerable<ICard>>>> GetData(string boardId = null, string listId = null, string cardId = null)
        {
            var results = new Dictionary<IBoard, IDictionary<IList, IEnumerable<ICard>>>();

            var me = await factory.Me();

            await me.Refresh(force: true);

            // Is the user I'm logged in as match the admin email of the api keys?

            IsAdmin = string.Equals(me.Email, Email, StringComparison.InvariantCultureIgnoreCase);

            foreach (var board in me.Boards.Where(b => string.IsNullOrWhiteSpace(boardId) || b.Id == boardId))
            {
                // Go through each board, but we're only display cards on the configured board of interest

                if (board.Id == configuration.Value.BoardId)
                {
                    await board.Refresh();

                    var list_data = new Dictionary<IList, IEnumerable<ICard>>();

                    // Go through all of the lists and collect up non-archived cards with the `One-on-One` tag

                    foreach (var list in board.Lists.Where(l => string.IsNullOrWhiteSpace(listId) || l.Id == listId))
                    {
                        await list.Refresh();

                        var cards = new List<ICard>();

                        foreach (var card in list.Cards.Where(c => string.IsNullOrWhiteSpace(cardId) || c.Id == cardId))
                        {
                            if (await Should_Display(card))
                            {
                                cards.Add(card);
                            }
                        }

                        if (cards.Any())
                        {
                            list_data.Add(list, cards);
                        }
                    }

                    results.Add(board, list_data);
                }
            }

            return results;
        }

        protected async Task<Boolean> Should_Display(ICard card)
        {
            if (card.IsArchived == true) return false;

            if (!card.Labels.Any(label => label.Name == "One-on-One")) return false;

            await card.Refresh();

            if (IsAdmin) return true;

            var custom_field = card.CustomFields.FirstOrDefault() as CustomField<string>;

            if (custom_field != null)
            {
                return string.Equals(Email, custom_field.Value, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }
    }
}