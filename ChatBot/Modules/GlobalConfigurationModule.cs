using Autofac;
using ChatBot.Assets;
using Microsoft.Bot.Builder.Autofac.Base;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Serilog;

namespace ChatBot.Modules
{
    public class GlobalConfigurationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // Register the IPostToBot implementations
            builder
                .RegisterType<UnhandledExceptionHandler>()
                .Keyed<IPostToBot>(typeof(UnhandledExceptionHandler))
                .InstancePerLifetimeScope();

            builder
                .RegisterAdapterChain<IPostToBot>
                (
                    typeof(EventLoopDialogTask),
                    typeof(SetAmbientThreadCulture),
                    typeof(PersistentDialogTask),
                    typeof(ExceptionTranslationDialogTask),
                    typeof(SerializeByConversation),
                    typeof(UnhandledExceptionHandler),
                    typeof(LogPostToBot)
                )
                .InstancePerLifetimeScope();

            builder
                .Register(c => new LoggerConfiguration()
                            .ReadFrom.AppSettings()
                            .Enrich.FromLogContext()
                            .CreateLogger())
                .As<ILogger>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}