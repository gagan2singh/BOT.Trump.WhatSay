﻿using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BOT.Trump.WhatSay.Dialogs
{
    [Serializable]
    public class NameDialog : IDialog<object>
    {
        private int attempts = 3;

        /// <summary>
        /// Start Async method
        /// </summary>
        /// <param name="context">Dialog Context</param>
        /// <returns>user context</returns>
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Please ener your Name (e.g. 'Bill', 'Melinda')?");
            context.Wait(this.MessageReceivedAsync);
        }

        /// <summary>
        /// Message received event fro the user
        /// </summary>
        /// <param name="context">User context</param>
        /// <param name="result">User message</param>
        /// <returns>context with Done for Name dialog</returns>
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            /* If the message returned is a valid name, return it to the calling dialog. */
            if ((message.Text != null) && (message.Text.Trim().Length > 0))
            {
                /* Completes the dialog, removes it from the dialog stack, and returns the result to the parent/calling
                    dialog. */
                context.Done(message.Text);
            }
            /* Else, try again by re-prompting the user. */
            else
            {
                --attempts;
                if (attempts > 0)
                {
                    await context.PostAsync("I'm sorry, I don't understand your reply. What is your name (e.g. 'Bill', 'Melinda')?");
                    context.Wait(this.MessageReceivedAsync);
                }
                else
                {
                    /* Fails the current dialog, removes it from the dialog stack, and returns the exception to the 
                        parent/calling dialog. */
                    context.Fail(new TooManyAttemptsException("Message was not a string or was an empty string."));
                }
            }
        }
    }
}