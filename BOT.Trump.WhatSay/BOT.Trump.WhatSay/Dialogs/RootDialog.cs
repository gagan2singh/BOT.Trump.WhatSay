using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace BOT.Trump.WhatSay.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            SendWelcomeMessage(context);
        }

        /// <summary>
        /// welcome message on load
        /// </summary>
        /// <param name="context">User contect</param>
        /// <returns>User message</returns>
        private void SendWelcomeMessage(IDialogContext context)
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
                var activity = await result as Activity;

                // Get the tweets
                /* Add function here 
                 * */

                // return our reply to the user
                await context.PostAsync($"");

                context.Wait(ResumeAfterNameDialog);
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
                context.Done(EndOfConversationCodes.ChannelFailed);
            }
        }
    }
}