using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.UnitTests.DirectLine
{
    internal static class TestRunner
    {
        internal static async Task RunTestCaseAsync(DialogStep step)
        {
            await RunTestCasesAsync(new List<DialogStep> { step });
        }

        internal static async Task RunTestCasesAsync(IEnumerable<DialogStep> steps)
        {
            await GeneralTestClass.BotHelper.StartConversationAsync();

            // Wait for Hello + Help
            await GeneralTestClass.BotHelper.WaitForLongRunningOperations(s => { }, 2);

            foreach (var step in steps)
            {
                await GeneralTestClass.BotHelper.SendMessageAsync(step.Action);

                Action<IList<string>> action = (replies) =>
                {
                    var isMatched = string.IsNullOrEmpty(step.ExpectedReply) || replies.Any(stringToCheck => stringToCheck != null && stringToCheck.ToLowerInvariant().Contains(step.ExpectedReply.ToLowerInvariant()));
                    Assert.IsTrue(isMatched, step.ErrorMessageHandler(step.Action, step.ExpectedReply, string.Join(", ", replies)));
                    step.Verified?.Invoke(replies.LastOrDefault());
                };

                await GeneralTestClass.BotHelper.WaitForLongRunningOperations(action, step.OperationsToWait);
            }
        }
    }
}
