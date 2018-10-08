using Autofac;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using Serilog;
using Serilog.Context;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace ChatBot.Controllers
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            // The whole conversation logs may be filtered by the Conversation Id
            using (LogContext.PushProperty("ConversationId", activity.Conversation.Id))
            {
                switch (activity.GetActivityType())
                {
                    case ActivityTypes.Message:
                        await ProcessMessageActivity(activity);
                        break;
                    case ActivityTypes.ConversationUpdate:
                        await ProcessConversationUpdateActivity(activity);
                        break;
                    case ActivityTypes.ContactRelationUpdate:
                    case ActivityTypes.Typing:
                    case ActivityTypes.DeleteUserData:
                    default:
                        Log.Warning("Activity type {Activity} was ignored.", activity.GetActivityType());
                        break;
                }
            }

            return new HttpResponseMessage(HttpStatusCode.Accepted);
        }

        private static async Task ProcessConversationUpdateActivity(Activity activity)
        {
            if (activity.MembersAdded != null && activity.MembersAdded.Any())
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                var user = activity.MembersAdded.First();

                if (user.Id.Contains(ConfigurationManager.AppSettings["BotId"]))
                {
                    using (var connector = new ConnectorClient(new Uri(activity.ServiceUrl)))
                    {
                        var replyMessage = activity.CreateReply(Properties.Resources.Root_WelcomeMessage);
                        replyMessage.Type = ActivityTypes.Message;
                        await connector.Conversations.ReplyToActivityAsync(replyMessage);

                        var typingMessage = activity.CreateReply();
                        typingMessage.Type = ActivityTypes.Typing;
                        await connector.Conversations.ReplyToActivityAsync(typingMessage);
                    }
                }
            }
        }

        private async Task ProcessMessageActivity(Activity activity)
        {
            // Send a typing message
            using (var connector = new ConnectorClient(new Uri(activity.ServiceUrl)))
            {
                var typingReply = activity.CreateReply();
                typingReply.Type = ActivityTypes.Typing;

                await connector.Conversations.ReplyToActivityAsync(typingReply);
            }

            // Create the conversation
            using (var lifetimeScope = DialogModule.BeginLifetimeScope(Conversation.Container, activity))
            {
                var dialog = lifetimeScope.Resolve<IDialog<object>>();
                await Conversation.SendAsync(activity, () => dialog);
            }
        }
    }
}