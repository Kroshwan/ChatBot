using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatBot.UnitTests.DirectLine
{
    [TestClass]
    public static class GeneralTestClass
    {
        private static BotHelper botHelper;
        public static TestContext testContext { get; set; }
        internal static BotHelper BotHelper => botHelper;

        // Will run once before all of the tests in the project. We start assuming the user is already logged in to Azure,
        // which should  be done separately via the AzureBot.ConsoleConversation or some other means. 
        [AssemblyInitialize]
        public static void SetUp(TestContext context)
        {
            testContext = context;
            var directLineToken = context.Properties["DirectLineToken"].ToString();
            var fromUser = context.Properties["FromUser"].ToString();
            var botId = context.Properties["BotId"].ToString();

            botHelper = new BotHelper(directLineToken, fromUser, botId);
        }

        // Will run after all the tests have finished
        [AssemblyCleanup]
        public static void CleanUp()
        {
            if (botHelper != null)
            {
                botHelper.Dispose();
            }
        }
    }
}
