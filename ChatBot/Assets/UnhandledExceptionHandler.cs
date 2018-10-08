using ChatBot.Properties;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using Serilog;
using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;

namespace ChatBot.Assets
{
    /// <summary>
    /// Override the default "Sorry, my bot code is having an issue" message
    /// </summary>
    public sealed class UnhandledExceptionHandler : IPostToBot
    {
        private readonly IPostToBot inner;
        private readonly IBotToUser botToUser;
        private readonly TraceListener trace;

        public UnhandledExceptionHandler(IPostToBot inner, IBotToUser botToUser, TraceListener trace)
        {
            SetField.NotNull(out this.inner, nameof(inner), inner);
            SetField.NotNull(out this.botToUser, nameof(botToUser), botToUser);
            SetField.NotNull(out this.trace, nameof(trace), trace);
        }

        async Task IPostToBot.PostAsync(IActivity activity, CancellationToken token)
        {
            try
            {
                await this.inner.PostAsync(activity, token);
            }
            catch (Exception error)
            {
                this.trace.WriteLine(error);

                Log.Fatal(error, "Impossible to reply to the user because of an {Type} exception: '{Message}'", error.GetType(), error.Message);

                try
                {
                    var message = this.botToUser.MakeMessage();
                    message.Text = Resources.UnhandledException_Message;

                    AddExceptionToMessageAtDebug(message, error);

                    await botToUser.PostAsync(message, cancellationToken: token);
                }
                catch (Exception inner)
                {
                    this.trace.WriteLine(inner);

                    Log.Fatal(inner, "Impossible to notify the user because of an {Type} exception: '{Message}'", inner.GetType(), inner.Message);
                }

                throw;
            }
        }

        [Conditional("DEBUG")]
        private void AddExceptionToMessageAtDebug(IMessageActivity message, Exception ex)
        {
            message.Text = $"Unhandled Exception: '{ex.Message}'\n\n*{ex.StackTrace}";
            message.Attachments = new[]
            {
                new Attachment(contentType: MediaTypeNames.Text.Plain, content: ex.StackTrace)
            };
        }
    }
}