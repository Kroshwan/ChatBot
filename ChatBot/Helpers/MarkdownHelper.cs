using System.Text;

namespace ChatBot.Helpers
{
    public static class MarkdownHelper
    {
        public const string CariageReturn = "\n\n";

        public static string AsHyperlink(string text, string link)
        {
            return $"[{link}]({text})";
        }

        public static string AsImage(string altText, string source)
        {
            return $"![{altText}]({source})";
        }
    }
}