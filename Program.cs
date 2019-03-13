using System;
using System.Threading.Tasks;
using DSharpPlus;
using HtmlAgilityPack;


namespace FB_Video_Handler
{
    class Program
    {
        static DiscordClient discord;

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        static async Task MainAsync(string[] args)
        {
            discord = new DiscordClient(new DiscordConfiguration
            {
                // Your Discord Bot token is inserted here as a string
                Token = "YOUR_TOKEN_HERE",
                TokenType = TokenType.Bot
            });

            // Following is executed when a message is sent by another user in a Discord Text Channel
            discord.MessageCreated += async e =>
            {
                // If message starts with https://www.facebook.com/ the Bot starts handling the message
                if (e.Message.Content.ToLower().StartsWith("https://www.facebook.com/"))
                {
                    // Gets the content of the msg, which should be a link
                    string weblink = e.Message.Content;

                    // Gets the webpage's content
                    var pageContent = (new HtmlWeb()).Load(@weblink);
                    string pageContentStr = HtmlEntity.DeEntitize(pageContent.Text.ToString());

                    // Finds the start of the video URL in the webpage's content
                    int linkStartPos = pageContentStr.IndexOf("hd_src");

                    // Removes the part of the page preceding the video URL
                    string pageContentFromLinkStart = pageContentStr.Substring(linkStartPos + 8);

                    // Finds the end of the video URL in the webpage's (trimmed) content
                    int linkEndPos = pageContentFromLinkStart.IndexOf("sd_src");

                    // Removes the part of the page succeeding the video URL
                    // Now we just have the video URL left
                    string videoURL = pageContentFromLinkStart.Remove((linkEndPos - 2));

                    // Sends the video URL
                    await e.Message.RespondAsync(videoURL);
                }
            };

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}
