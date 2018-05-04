using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Collections.Generic;
using BOT.Trump.WhatSay.Helper;
using BOT.Trump.WhatSay.Model;

namespace BOT.Trump.WhatSay.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private string Name;
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"We need to gather some information to set you up!!");
            context.Wait(this.MessageReceivedAsync);

        }

        /// <summary>
        /// Message from the user for the root dialog
        /// </summary>
        /// <param name="context">Dialog Context</param>
        /// <param name="result">Message from the user</param>
        /// <returns>user context</returns>
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;
            SendWelcomeMessageAsync(context);
        }

        /// <summary>
        /// welcome message on load
        /// </summary>
        /// <param name="context">User contect</param>
        /// <returns>User message</returns>
        private async Task SendWelcomeMessageAsync(IDialogContext context)
        {
            context.Call(new NameDialog(), this.ResumeAfterNameDialog);
        }


        /// <summary>
        /// Get the name and start the conversation
        /// </summary>
        /// <param name="context">User context</param>
        /// <param name="result">User result</param>
        /// <returns>End of Conversation</returns>
        private async Task ResumeAfterNameDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var activity = await result;
                this.Name = Convert.ToString(activity);
                await context.PostAsync($"Thanks " + activity + " Let's get started. Ask me something!!!");
                context.Wait(StartConversation);
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
                context.Done(EndOfConversationCodes.ChannelFailed);
            }
        }

        /// <summary>
        /// Get conversation started
        /// </summary>
        /// <param name="context">User context</param>
        /// <param name="result">User result</param>
        /// <returns></returns>
        private async Task StartConversation(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            try
            {
                var activity = await result;
                List<Tweet> getTweets = BOTHelper.ReturnTrumpTweet(activity.Text);
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

                await context.PostAsync($"*********--you can keep asking more by just typing--");
                context.Wait(StartConversation);
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
                context.Done(EndOfConversationCodes.ChannelFailed);
            }
        }
    }
}