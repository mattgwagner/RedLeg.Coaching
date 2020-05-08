using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Manatee.Trello;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace One_on_Ones
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration configuration;

        public IBoardCollection Boards { get; private set; }

        public IndexModel(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task OnGet()
        {
            // Get an app key via https://trello.com/app-key
            // Then get a 'server token' since we are too lazy to bother with OAuth flow

            var trello = await new TrelloFactory().Me(new TrelloAuthorization
            {
                AppKey = "Wags-Test",
                UserToken = configuration.GetValue<string>("Trello-Api-Key")
            });

            Boards = trello.Boards;

        }
    }
}
