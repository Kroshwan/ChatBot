using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChatBot.UnitTests.DirectLine;
using System.Threading.Tasks;

namespace ChatBot.UnitTests
{
    [TestClass]
    public class TestBook
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        [TestCategory("Parallel")]
        public async Task Welcome()
        {
            var testCase = new DialogStep()
            {
                Action = "message to the bot",
                ExpectedReply = "message from the bot",
            };

            await TestRunner.RunTestCaseAsync(testCase);
        }
    }
}
