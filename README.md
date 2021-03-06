
# Bot: PromptDialog options in the Dialog flow
Recently, I have come across an interesting support case scenario, where customer wanted to know whether bot users can be allowed to provide some other options (more humane) than just - yes/ no - in case of prompt dialog options. It was an interesting research work, where bulk of time spent in reviewing the [PromptDialog](https://github.com/Microsoft/BotBuilder/blob/master/CSharp/Library/Microsoft.Bot.Builder/Dialogs/PromptDialog.cs) class source code. Hence, some of the thought out scenarios are tried out.

### Scenario - 1
Show reset question with yes/ no option, and accept yes/ no only.

````C#
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text.Equals("reset", StringComparison.InvariantCultureIgnoreCase))
            {
                // Show "reset" question with yes/ no option, and read yes/ no only
                ResetOptionSimple(context);
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
 ````
 
### Output

![reset with yes no](Images/resetyesnoimage.png) 
 
 
### Scenario - 2
Show reset question with yes/ no option, and allow a custom pattern to be read as well on behalf of yes/ no.

````C#
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text.Equals("reset", StringComparison.InvariantCultureIgnoreCase))
            {
                // Show "reset" question with yes/ no option, and allow pattern to read
                ResetOptionWithYesNoPattern(context);
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
````

### Output

![reset with yes no and patterns](Images/resetyesnopattern.png) 


### Scenario - 3
Show reset question without yes/ no option, and allow a custom pattern to be read as well on behalf of yes/ no.

````C#
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text.Equals("reset", StringComparison.InvariantCultureIgnoreCase))
            {
                // Show "reset" question with yes/ no option, and allow pattern to read
                ResetOptionWithoutYesNoPattern(context);
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

        private void ResetOptionWithoutYesNoPattern(IDialogContext context)
        {
            var pattern = new string[][] {
                    new string[]{ "yes", "yeah", "yep", "yeppy" },
                    new string[]{ "no", "nope", "nono", "nop" }
                };

            var promptOption = new PromptOptions<string>("Are you sure you want to reset?");
            PromptDialog.Confirm(context, AfterResetAsync, promptOption, pattern);
        }
````

### Output

![reset without yes no and patterns](Images/resetwithoutyesnopattern.png) 


### Scenario - 4
Show reset question with options as a "choice".

````C#
        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text.Equals("reset", StringComparison.InvariantCultureIgnoreCase))
            {                
                // Show "reset" question option with choice
                ResetOptionWithChoice(context);
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
````

### Output

![reset with choice](Images/resetwithchoice.png) 


--


Please clone the code and have fun!

 
 
 
 

