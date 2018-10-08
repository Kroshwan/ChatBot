using ChatBot.Assets;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Serilog;
using System;
using System.Threading.Tasks;

namespace ChatBot.Dialogs
{
    /// <summary>
    /// See https://github.com/Microsoft/BotBuilder/blob/master/CSharp/Library/Microsoft.Bot.Builder/Dialogs/LuisDialog.cs for more details
    /// </summary>
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        private readonly IChatBotDialogFactory dialogFactory;

        public RootDialog(IChatBotDialogFactory factory, ILuisService service) : base(service)
        {
            SetField.NotNull(out dialogFactory, nameof(factory), factory);
        }

        protected static void LogIntent(string entry, string returnValue, string user)
        {
            Log.Information("User entry '{entry}' return value '{returnValue}' from {user}", entry, returnValue, user);
        }

        #region Luis Intents
        [LuisIntent(LuisModelContract.Intent_None)]
        [LuisIntent(LuisModelContract.Intent_Empty)]
        public Task None(IDialogContext context, LuisResult result)
        {
            LogIntent(result.Query, nameof(RootDialog.None), context.Activity.From.Name);

            //var dialog = this.dialogFactory.CreateDialog<MySubDialog>(result);
            //context.Call(dialog, this.ResumeAfterForward);

            return Task.CompletedTask;
        }
        #endregion

        private async Task ResumeAfterForward(IDialogContext context, IAwaitable<object> argument)
        {
            await argument;
            context.Wait(MessageReceived);
        }
    }
}