# RedLeg.Coaching

A proof of concept app for using the Trello API and cards as a backend for tracking items for one-on-ones with my mentoring group.

Right now, it is intended to pull OAuth configuration from the appsettings.json for authentication.

Configure Trello access by setting the AppKey, AppId, and BoardId so the system knows where to pull the data from. See `blob/master/TrelloConfiguration.cs` for information on where to get those values from.
