using System.Collections.Generic;

namespace ChatBot.Assets
{
    public interface IDialogFactory
    {
        T Create<T>();
        T Create<T, U>(params U[] orderedParameters);
        T Create<T>(IDictionary<string, object> namedParameters);
    }
}