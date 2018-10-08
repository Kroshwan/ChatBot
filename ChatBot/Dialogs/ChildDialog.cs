using ChatBot.Assets;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;

namespace ChatBot.Dialogs
{
    /// <summary>
    /// A base class for sub-dialogs.
    /// </summary>
    public abstract class ChildDialog : LuisDialog<object>
    {
        private readonly LuisResult originalLuisResult;
        private readonly IChatBotDialogFactory dialogFactory;

        protected ChildDialog(ILuisService service, IChatBotDialogFactory dialogFactory, LuisResult result) : base(service)
        {
            SetField.NotNull(out this.dialogFactory, nameof(dialogFactory), dialogFactory);
            SetField.NotNull(out this.originalLuisResult, nameof(result), result);
        }

        protected IChatBotDialogFactory DialogFactory => dialogFactory;

        public override sealed Task StartAsync(IDialogContext context)
        {
            return ProcessIntent(context, originalLuisResult);
        }

        /// <summary>
        /// Entry point of the Dialog.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The Luis Result coming from Luis or the Caller Dialog.</param>
        /// <returns>An asynchronous operation.</returns>
        public abstract Task ProcessIntent(IDialogContext context, LuisResult result);

        /// <summary>
        /// Returns to the Caller Dialog with the given Luis Result.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The Luis Result the Caller shall process.</param>
        /// <returns>An asynchronous Task.</returns>
        [LuisIntent(LuisModelContract.Intent_Empty)]
        public Task EndDialog(IDialogContext context, LuisResult result)
        {
            context.Done(result);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns to the Caller Dialog with the given Luis Result.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="argument">The Luis Result to wait before passing it to the Caller for processing.</param>
        /// <returns>An asynchronous Task.</returns>
        protected async Task EndDialog(IDialogContext context, IAwaitable<LuisResult> argument)
        {
            var result = await argument;
            context.Done(result);
        }
    }
}