using Microsoft.Extensions.Configuration;
using SlackAPI;
using System;
using System.Threading.Tasks;

namespace NotificationProvider.Services
{
    public class Slack
    {
        #region Properties

        private readonly SlackTaskClient client;

        private readonly SlackClient cClient;

        #endregion

        #region Constructor

        public Slack(IConfiguration config)
        {
            var oAuthToken = config["Slack:OAuthToken"];

            if (oAuthToken.Length > 0)
            {
                client = new SlackTaskClient(oAuthToken);
                cClient = new SlackClient(oAuthToken);
            }
            else
            {
                throw new ArgumentNullException();
            }
        }

        #endregion

        #region Public Methods

        public void PostToUser(string username, string message)
        {
          ////var test = FetchUsers();

          //  this.client.GetUserListAsync();

          //  var user = client.Users.Find(x => x.name.Equals("slackbot")); // you can replace slackbot with everyone else here

          // // var user = GetUser(username);
          //  var dmchannel = GetDMForUser(user);

          //  client.PostMessageAsync(dmchannel.id, message);

            cClient.GetUserList((ulr) => { Console.WriteLine("got users"); });
            var user = client.Users.Find(x => x.name.Equals("slackbot")); // you can replace slackbot with everyone else here
            var dmchannel = client.DirectMessages.Find(x => x.user.Equals(user.id));
            cClient.PostMessage((mr) => Console.WriteLine("sent! to " + dmchannel.id), dmchannel.id, "I don't know you yet!");
        }

        public void PostToChannel(string channel, string message)
        {
            client.PostMessageAsync(channel, message);
        }

        public void FetchChannelList()
        {
            client.GetChannelListAsync(ExcludeArchived: true);
        }

        #endregion

        #region Private Helper Methods

        private User GetUser(string username)
        {
            return this.client.Users.Find(x => x.name.Equals(username));
        }

        private DirectMessageConversation GetDMForUser(User user)
        {
            return client.DirectMessages.Find(x => x.user.Equals(user.id));
        }

        private async Task<UserListResponse> FetchUsers()
        {
            return await this.client.GetUserListAsync();
        }

        #endregion
    }
}