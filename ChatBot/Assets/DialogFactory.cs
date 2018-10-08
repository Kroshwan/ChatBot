using Autofac;
using Microsoft.Bot.Builder.Internals.Fibers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatBot.Assets
{
    public class DialogFactory : IDialogFactory
    {
        protected readonly IComponentContext Scope;

        public DialogFactory(IComponentContext scope)
        {
            SetField.NotNull(out this.Scope, nameof(scope), scope);
        }

        public T Create<T>()
        {
            return this.Scope.Resolve<T>();
        }

        public T Create<T, U>(params U[] orderedParameters)
        {
            if (orderedParameters == null)
            {
                throw new ArgumentNullException(nameof(orderedParameters));
            }

            return this.Scope.Resolve<T>(orderedParameters.Select(param => TypedParameter.From(param)));
        }

        public T Create<T>(IDictionary<string, object> namedParameters)
        {
            if (namedParameters == null)
            {
                throw new ArgumentNullException(nameof(namedParameters));
            }

            return this.Scope.Resolve<T>(namedParameters.Select(kv => new NamedParameter(kv.Key, kv.Value)));
        }
    }
}