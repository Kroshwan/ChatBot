using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBot.UnitTests.DirectLine
{
    public class BotHelper : IDisposable
    {
        private string watermark;
        private readonly string fromUser;
        private readonly string botId;
        private readonly DirectLineClient directLineClient;
        private Conversation conversation;

        private bool disposed = false;

        public BotHelper(string directLineToken, string fromUser, string BotId)
        {
            this.fromUser = fromUser;
            botId = BotId;
            directLineClient = new DirectLineClient(directLineToken);
        }

        public async Task StartConversationAsync()
        {
            watermark = string.Empty;
            conversation = await directLineClient.Conversations.StartConversationAsync();
        }

        public async Task<string> SendAndReceiveMessageAsync(string msg)
        {
            await SendMessageAsync(msg);
            return await LastMessageFromBotAsync();
        }

        public async Task SendMessageAsync(string msg)
        {
            await directLineClient.Conversations.PostActivityAsync(conversation.ConversationId, MakeActivity(msg), CancellationToken.None);
        }

        private Activity MakeActivity(string msg)
        {
            return new Activity()
            {
                Type = ActivityTypes.Message,
                From = new ChannelAccount { Id = fromUser },
                Text = msg
            };
        }

        public async Task<string> LastMessageFromBotAsync()
        {
            var botMessages = await AllBotMessagesSinceWatermark();
            return botMessages.Last();
        }

        public async Task WaitForLongRunningOperations(Action<IList<string>> resultHandler, int operationsToWait)
        {
            var currentWatermark = watermark;
            var iterations = 0;
            var delayBetweenPoolingInSeconds = 2;
            var maxIterations = 5;

            var messages = await AllBotMessagesSinceWatermark(currentWatermark).ConfigureAwait(false);

            while (iterations < maxIterations && messages.Count < operationsToWait)
            {
                await Task.Delay(TimeSpan.FromSeconds(delayBetweenPoolingInSeconds)).ConfigureAwait(false);
                messages = await AllBotMessagesSinceWatermark(currentWatermark);
                iterations++;
            }

            resultHandler(messages);
        }

        private async Task<IList<string>> AllBotMessagesSinceWatermark(string specificWatermark = null)
        {
            var messages = await GetAllMessagesSinceWatermarkAsync(specificWatermark);
            var messagesText = from x in messages
                               where x.From.Id == botId
                               select string.IsNullOrEmpty(x.Text) ? x.Attachments.FirstOrDefault()?.Content.ToString() : x.Text.Trim();

            return messagesText.ToList();
        }

        private async Task<IList<Activity>> GetAllMessagesSinceWatermarkAsync(string specificWatermark = null)
        {
            specificWatermark = string.IsNullOrEmpty(specificWatermark) ? watermark : specificWatermark;
            var messageSet = await directLineClient.Conversations.GetActivitiesAsync(conversation.ConversationId, specificWatermark);
            watermark = messageSet.Watermark;

            return messageSet.Activities;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                directLineClient.Dispose();
            }

            disposed = true;
        }
    }
}
