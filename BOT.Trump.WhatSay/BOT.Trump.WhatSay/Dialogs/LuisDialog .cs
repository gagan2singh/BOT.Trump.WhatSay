using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Dialogs;
using System.Configuration;
using System.Threading.Tasks;
using BOT.Trump.WhatSay.Model;
using BOT.Trump.WhatSay.Helper;
using Microsoft.Bot.Connector;

namespace BOT.Trump.WhatSay.Dialogs
{
    [Serializable]
    public class LuisDialog: LuisDialog<object>
    {
        /// <summary>
        /// LUIS dialog with App ID, API Key and Host name
        /// </summary>
        public LuisDialog() : base(new LuisService(new LuisModelAttribute(
            ConfigurationManager.AppSettings["LuisAppId"], 
            ConfigurationManager.AppSettings["LuisAPIKey"], 
            domain: ConfigurationManager.AppSettings["LuisAPIHostName"])))
        {
        }

        /// <summary>
        /// LUIS nonw intent
        /// </summary>
        /// <param name="context">User context</param>
        /// <param name="result">User message</param>
        /// <returns>No intent identified</returns>
        [LuisIntent("None")]
        public async Task NoneIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        /// <summary>
        /// LUIS intent 'Word'
        /// </summary>
        /// <param name="context">User context</param>
        /// <param name="result">User message</param>
        /// <returns>Word intent</returns>
        // Go to https://luis.ai and create a new intent, then train/publish your luis app.
        [LuisIntent("Word")]
        public async Task WordIntent(IDialogContext context, LuisResult result)
        {
            await this.ShowLuisResult(context, result);
        }

        /// <summary>
        /// Show the results form the tweets
        /// </summary>
        /// <param name="context">User context</param>
        /// <param name="result">User message</param>
        /// <returns>Tweets from trumop archive</returns>
        private async Task ShowLuisResult(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"You have reached {result.Intents[0].Intent}. You said: {result.Query}");
            try
            {
                string getSearchWordFromSentence = string.Empty;

                if (result.Entities.Count == 0)
                {
                    getSearchWordFromSentence = result.Query;
                }
                else
                {
                    string wordsToFetch = string.Empty;
                    foreach(EntityRecommendation entity in result.Entities)
                    {
                        wordsToFetch += entity.Entity + " ";
                    }

                    // remove any extra spaces
                    wordsToFetch.Trim();

                    getSearchWordFromSentence = wordsToFetch;
                }
                List<Tweet> getTweets = BOTHelper.ReturnTrumpTweet(getSearchWordFromSentence);
                await context.PostAsync($"Well you know what Trump tweeted about the text you searched");

                int count = 0;
                int counter = 1;
                foreach (Tweet trumpTweet in getTweets)
                {
                    if (count == 1)
                    {
                        await context.PostAsync($"hang On -- There's more");
                    }
                    if (count == 2)
                    {
                        await context.PostAsync($"And more");
                    }
                    if (count == 4)
                    {
                        await context.PostAsync($"This Guy can't keep quite");
                    }

                    // return our reply to the user
                    await context.PostAsync($"" + counter + ". (" + trumpTweet.text + ")");
                    count++;
                    counter++;
                }

                await context.PostAsync($"*--*****you can keep asking more by just typing*****--*");
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
                context.Done(EndOfConversationCodes.ChannelFailed);
            }
            context.Wait(MessageReceived);
        }
    }
}