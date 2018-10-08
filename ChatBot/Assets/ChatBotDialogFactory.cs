using Autofac;
using ChatBot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatBot.Assets
{
    internal class ChatBotDialogFactory : DialogFactory, IChatBotDialogFactory
    {
        public ChatBotDialogFactory(IComponentContext scope)
            : base(scope)
        {
        }

        public ChildDialog CreateDialog<T>(LuisResult result) where T : ChildDialog
        {
            var resultParam = TypedParameter.From<LuisResult>(result);

            return this.Scope.Resolve<T>(resultParam);
        }

        public IDialog<object> CreateDialog<T>() where T : IDialog<object>
        {
            return this.Create<T>();
        }
    }
}