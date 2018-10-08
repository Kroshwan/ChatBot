using Autofac;
using Autofac.Integration.WebApi;
using ChatBot.Assets;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
using Serilog;
using System.Web.Http;

namespace ChatBot
{
    public class Global : System.Web.HttpApplication
    {
        // More examples can be found at https://github.com/Microsoft/BotBuilder-Samples/tree/master/CSharp
        protected void Application_Start()
        {
            RegisterBotDependencies();

            GlobalConfiguration.Configure(WebApiConfig.Register);

            Log.Logger = Conversation.Container.Resolve<ILogger>();

            Log.Warning("Bot Started");
        }

        private void RegisterBotDependencies()
        {
            Conversation.UpdateContainer(builder =>
            {
                var assembly = typeof(Global).Assembly;

                builder.RegisterApiControllers(assembly);

                builder.RegisterAssemblyModules(assembly);
                builder.RegisterModule<ReflectionSurrogateModule>();
            });

            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(Conversation.Container);
        }

        /*public static ILifetimeScope FindContainer()
        {
            var config = GlobalConfiguration.Configuration;
            var resolver = (AutofacWebApiDependencyResolver)config.DependencyResolver;

            return resolver.Container;
        }*/
    }
}
