using ChatBot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;

namespace ChatBot.Assets
{
    public interface IChatBotDialogFactory
    {
        ChildDialog CreateDialog<T>(LuisResult result) where T : ChildDialog;

        IDialog<object> CreateDialog<T>() where T : IDialog<object>;
    }
}