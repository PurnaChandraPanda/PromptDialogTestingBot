using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using static Microsoft.Bot.Builder.Dialogs.PromptDialog;

namespace PromptDialogTestingBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        protected int count = 1;

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text.Equals("reset", StringComparison.InvariantCultureIgnoreCase))
            {
                // Show "reset" question with yes/ no option, and read yes/ no only
                //ResetOptionSimple(context);

                // Show "reset" question with yes/ no option, and allow pattern to read
                ResetOptionWithYesNoPattern(context);

                // Show "reset" question without yes/ no option, and allow pattern to read
                //ResetOptionWithoutYesNoPattern(context);

                // Show "reset" option with choice
                //ResetOptionWithChoice(context);
            }
            else
            {
                // Calculate something for us to return
                int length = (activity.Text ?? string.Empty).Length;

                // Return our reply to the user
                await context.PostAsync($"{count++}. You sent {activity.Text} which was {length} characters");

                context.Wait(MessageReceivedAsync);
            }                        
        }

        private async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> result)
        {
            var confirm = await result;
            if (confirm)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

        private void ResetOptionSimple(IDialogContext context)
        {
            PromptDialog.Confirm(context, AfterResetAsync, "Are you sure you want to reset?", promptStyle: PromptStyle.Auto);
        }

        private void ResetOptionWithYesNoPattern(IDialogContext context)
        {
            var pattern = new string[][] {
                    new string[]{ "yes", "yeah", "yep", "yeppy" },
                    new string[]{ "no", "nope", "nono", "nop" }
                };

            var promptOption = new PromptOptions<string>("Are you sure you want to reset?", null, null,
                                                                options: PromptConfirm.Options, promptStyler: new PromptStyler(promptStyle: PromptStyle.Auto));
            PromptDialog.Confirm(context, AfterResetAsync, promptOption, pattern);
        }

        private void ResetOptionWithoutYesNoPattern(IDialogContext context)
        {
            var pattern = new string[][] {
                    new string[]{ "yes", "yeah", "yep", "yeppy" },
                    new string[]{ "no", "nope", "nono", "nop" }
                };

            var promptOption = new PromptOptions<string>("Are you sure you want to reset?");
            PromptDialog.Confirm(context, AfterResetAsync, promptOption, pattern);
        }

        private void ResetOptionWithChoice(IDialogContext context)
        {
            PromptDialog.Choice<ResetOptions>(
                context: context,                
                resume: AfterChoiceSelected,
                options: (IEnumerable<ResetOptions>)Enum.GetValues(typeof(ResetOptions)),
                prompt: "Are you sure you want to reset?",
                retry: "Please try again.",
                promptStyle: PromptStyle.Auto
                );
        }

        private async Task AfterChoiceSelected(IDialogContext context, IAwaitable<ResetOptions> result)
        {
            var confirm = await result;
            if (confirm == ResetOptions.Yes)
            {
                this.count = 1;
                await context.PostAsync("Reset count.");
            }
            else
            {
                await context.PostAsync("Did not reset count.");
            }
            context.Wait(MessageReceivedAsync);
        }

        private enum ResetOptions
        {
            Yes,
            No,
            Thankyou
        }
    }
}