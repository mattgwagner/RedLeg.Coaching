namespace RedLeg.Coaching
{
    public class TrelloConfiguration
    {
        // Get an app key via https://trello.com/app-key
        // Then get a 'server token' since we are too lazy to bother with OAuth flow

        public string AppKey { get; set; }

        public string ApiKey { get; set; }

        public string BoardId { get; set; }
    }
}