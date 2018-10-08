using Autofac;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Configuration;

namespace ChatBot.Modules
{
    public class LuisModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder
                .Register(c => new LuisModelAttribute(
                ConfigurationManager.AppSettings["LuisAppId"],
                ConfigurationManager.AppSettings["LuisApiKey"],
                domain:ConfigurationManager.AppSettings["LuisDomain"])
                {
                    Staging = ConfigurationManager.AppSettings["UseStatingLuisApi"] == "1"
                })
                .AsSelf()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .RegisterType<LuisService>()
                .Keyed<ILuisService>(FiberModule.Key_DoNotSerialize)
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .RegisterType<LuisResult>()
                .Keyed<LuisResult>(FiberModule.Key_DoNotSerialize)
                .InstancePerDependency();
        }
    }
}