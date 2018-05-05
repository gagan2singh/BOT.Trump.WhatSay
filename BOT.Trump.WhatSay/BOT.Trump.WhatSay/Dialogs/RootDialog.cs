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
                context.Call(new LuisDialog(), null);
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
                context.Done(EndOfConversationCodes.ChannelFailed);
            }
        }        
    }
}