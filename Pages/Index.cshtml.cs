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

        public ICollection<ICard> Cards { get; private set; }

        public IndexModel(ITrelloFactory factory, IOptions<TrelloConfiguration> configuration)
        {
            this.factory = factory;
            this.configuration = configuration;
        }

        public async Task OnGet()
        {
            Cards = await GetData();
        }

        public async Task<IActionResult> OnPostAddItem(string checklist, string text)
        {
            var data = await GetData();

            var checklist_reference =
                data
                .SelectMany(c => c.CheckLists)
                .Where(cl => cl.Id == checklist)
                .SingleOrDefault();

            if (checklist_reference != null)
            {
                await checklist_reference.CheckItems.Add(text);
            }

            return Redirect("/");
        }

        public async Task<IActionResult> OnPostToggleCheck(string checklistitem, Boolean isChecked)
        {
            var data = await GetData();

            var checklistitem_reference =
                data
                .SelectMany(c => c.CheckLists)
                .SelectMany(cli => cli.CheckItems)
                .Where(cli => cli.Id == checklistitem)
                .SingleOrDefault();

            if (checklistitem_reference != null)
            {
                checklistitem_reference.State = isChecked ? CheckItemState.Complete : CheckItemState.Incomplete;
            }

            return Redirect("/");
        }

        protected async Task<IList<ICard>> GetData()
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