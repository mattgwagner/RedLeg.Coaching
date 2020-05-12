namespace RedLeg.Coaching
{
    public class TrelloConfiguration
    {
        /// <summary>
        /// Get an app key via https://trello.com/app-key
        /// </summary>
        public string AppKey { get; set; }

        /// <summary>
        /// Generate an Api Key via 'server token' since we are too lazy to bother with OAuth flow
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// The unique ID of the board we want to reference for our One-on-One cards
        /// </summary>
        public string BoardId { get; set; }
    }
}