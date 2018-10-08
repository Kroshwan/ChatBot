using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.UnitTests.DirectLine
{
    internal class DialogStep
    {
        public DialogStep()
        {
            this.ErrorMessageHandler = DefaultErrorMessageHandler;
            this.OperationsToWait = 1;
        }

        public string Action { get; internal set; }

        public string ExpectedReply { get; internal set; }

        public int OperationsToWait { get; internal set; }

        public Action<string> Verified { get; internal set; }

        public Func<string, string, string, string> ErrorMessageHandler { get; internal set; }

        private static string DefaultErrorMessageHandler(string action, string expectedReply, string receivedReply)
        {
            return $"'{action}' received reply '{receivedReply}' that doesn't contain the expected message: '{expectedReply}'";
        }
    }
}
