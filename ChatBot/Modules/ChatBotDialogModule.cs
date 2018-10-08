using Autofac;
using ChatBot.Assets;
using ChatBot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using System.Linq;

namespace ChatBot.Modules
{
    public class ChatBotDialogModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<ChatBotDialogFactory>()
               .Keyed<IChatBotDialogFactory>(FiberModule.Key_DoNotSerialize)
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();

            // Register the Root Dialog
            builder
                .RegisterType<RootDialog>()
                .As<IDialog<object>>()
                .InstancePerDependency();

            // Register other Dialogs
            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .Where(t => t.BaseType == typeof(ChildDialog))
                .InstancePerDependency();
        }
    }
}