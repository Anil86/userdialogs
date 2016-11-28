﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Splat;
using Xamarin.Forms;


namespace Samples.ViewModels
{
    public class StandardViewModel : AbstractViewModel
    {
        public IList<CommandViewModel> Commands { get; }


        public StandardViewModel(IUserDialogs dialogs) : base(dialogs)
        {
            this.Commands = new List<CommandViewModel>
            {
                new CommandViewModel
                {
                    Text = "Alert",
                    Command = this.Create(async token => await this.Dialogs.AlertAsync("Test alert", "Alert Title", null, token))
                },
                new CommandViewModel
                {
                    Text = "Alert Long Text",
                    Command = this.Create(async token =>
                        await this.Dialogs.AlertAsync(
                            "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc consequat diam nec eros ornare, vitae cursus nunc molestie. Praesent eget lacus non neque cursus posuere. Nunc venenatis quam sed justo bibendum, ut convallis arcu lobortis. Vestibulum in diam nisl. Nulla pulvinar lacus vel laoreet auctor. Morbi mi urna, viverra et accumsan in, pretium vel lorem. Proin elementum viverra commodo. Sed nunc justo, sollicitudin eu fermentum vitae, faucibus a est. Nulla ante turpis, iaculis et magna sed, facilisis blandit dolor. Morbi est felis, semper non turpis non, tincidunt consectetur enim.",
                            cancelToken: token
                        )
                    )
                },
                new CommandViewModel
                {
                    Text = "Alert - 3 Choice",
                    Command = this.Create(async token =>
                    {
                        var result = await this.Dialogs.AlertAsync(new AlertConfig()
                            .SetTitle("Three Choice Alert")
                            .SetMessage("Choose your destiny")
                            .SetText(DialogChoice.Positive, "Good")
                            .SetText(DialogChoice.Neutral, "Bad")
                            .SetText(DialogChoice.Negative, "Ugly"),
                            token
                        );
                        this.Result("Alert Choice: " + result);
                    })
                },
                new CommandViewModel
                {
                    Text = "Action Sheet",
                    Command = this.CreateActionSheetCommand(false, true, 6)
                },
                new CommandViewModel
                {
                    Text = "Action Sheet /w Message",
                    Command = this.CreateActionSheetCommand(false, false, 6, "This is an example of using a message in Acr.UserDialogs actionsheets.  I needed a long message here!")
                },
                new CommandViewModel
                {
                    Text = "Action Sheet (No Cancel)",
                    Command = this.CreateActionSheetCommand(false, false, 3)
                },
                new CommandViewModel
                {
                    Text = "Action Sheet (async)",
                    Command = this.Create(async token =>
                    {
                        var result = await this.Dialogs.ActionSheetAsync("Test Title", "Cancel", "Destroy", token, "Button1", "Button2", "Button3");
                        this.Result(result);
                    })
                },
                new CommandViewModel
                {
                    Text = "Bottom Sheet (Android Only)",
                    Command = this.CreateActionSheetCommand(true, true, 6)
                },
                new CommandViewModel
                {
                    Text = "Confirm",
                    Command = this.Create(async token =>
                    {
                        var r = await this.Dialogs.ConfirmAsync("Pick a choice", "Pick Title", cancelToken: token);
                        var text = r ? "Yes" : "No";
                        this.Result($"Confirmation Choice: {text}");
                    })
                },
                new CommandViewModel
                {
                    Text = "Login",
                    Command = this.Create(async token =>
                    {
                        var r = await this.Dialogs.LoginAsync(new LoginConfig
                        {
                            //LoginValue = "LastUserName",
                            Message = "DANGER",
                            //PositiveText = "DO IT",
                            //NeutralText = "STOP",
                            //NegativeText = "OH HELL NO",
                            LoginPlaceholder = "Username Placeholder",
                            PasswordPlaceholder = "Password Placeholder"
                        }, token);
                        this.Result($"Login: {r.Choice} - User Name: {r.Value.UserName} - Password: {r.Value.Password}");
                    })
                },
                new CommandViewModel
                {
                    Text = "Prompt",
                    Command = new Command(() => this.Dialogs.ActionSheet(new ActionSheetConfig()
                        .SetTitle("Choose Type")
                        .Add("Default", () => this.PromptCommand(InputType.Default))
                        .Add("E-Mail", () => this.PromptCommand(InputType.Email))
                        .Add("Name", () => this.PromptCommand(InputType.Name))
                        .Add("Number", () => this.PromptCommand(InputType.Number))
                        .Add("Number with Decimal", () => this.PromptCommand(InputType.DecimalNumber))
                        .Add("Password", () => this.PromptCommand(InputType.Password))
                        .Add("Numeric Password (PIN)", () => this.PromptCommand(InputType.NumericPassword))
                        .Add("Phone", () => this.PromptCommand(InputType.Phone))
                        .Add("Url", () => this.PromptCommand(InputType.Url))
                        .SetCancel()
                    ))
                },
                new CommandViewModel
                {
                    Text = "Prompt Max Length",
                    Command = this.Create(async token =>
                    {
                        var result = await this.Dialogs.PromptAsync(new PromptConfig()

                            .SetTitle("Max Length Prompt")
                            .SetPlaceholder("Maximum Text Length (10)")
                            .SetMaxLength(10), token);

                        this.Result($"Result - {result.Choice} - {result.Value}");
                    })
                },
                new CommandViewModel
                {
                    Text = "Prompt (No Text or Cancel)",
                    Command = this.Create(async token =>
                    {
                        var result = await this.Dialogs.PromptAsync(new PromptConfig
                        {
                            Title = "PromptWithTextAndNoCancel",
                            Text = "Existing Text"
                        }, token);
                        this.Result($"Result - {result.Choice} - {result.Value}");
                    })
                },
                new CommandViewModel
                {
                    Text = "Prompt Text Validate",
                    Command = new Command(async () =>
                    {
                        var result = await this.Dialogs.PromptAsync(new PromptConfig
                        {
                            Title = "Prompt Text Validate",
                            Message = "You must type the word \"yes\" to enable OK button",
                            OnTextChanged = args => args.IsValid = args.Value.Equals("yes", StringComparison.CurrentCultureIgnoreCase)
                        });
                        this.Result($"Result - {result.Choice} - {result.Value}");
                    })
                },
                new CommandViewModel
                {
                    Text = "Prompt Text Format",
                    Command = new Command(async () =>
                    {
                        var result = await this.Dialogs.PromptAsync(new PromptConfig
                        {
                            Title = "Prompt Text Format",
                            Message = "Type in lower case and it will convert to upper case",
                            OnTextChanged = args => args.Value = args.Value.ToUpper()
                        });
                        this.Result($"Result - {result.Choice} - {result.Value}");
                    })
                },
                new CommandViewModel
                {
                    Text = "Date",
                    Command = this.Create(async token =>
                    {
                        var result = await this.Dialogs.DatePromptAsync(new DatePromptConfig
                        {
                            MinimumDate = DateTime.Now.AddDays(-3),
                            MaximumDate = DateTime.Now.AddDays(1)
                        }, token);
                        this.Result($"Date Prompt: {result.Choice} - Value: {result.Value}");
                    })
                },
                new CommandViewModel
                {
                    Text = "Time",
                    Command = this.Create(async token =>
                    {
                        var result = await this.Dialogs.TimePromptAsync(new TimePromptConfig(), token);
                        this.Result($"Time Prompt: {result.Choice} - Value: {result.Value}");
                    })
                },
                new CommandViewModel
                {
                    Text = "Time (24 hour clock)",
                    Command = this.Create (async token => {
                        var result = await this.Dialogs.TimePromptAsync(new TimePromptConfig {
                            Use24HourClock = true
                        }, token);
                        this.Result ($"Time Prompt: {result.Choice} - Value: {result.Value}");
                    })
                }
            };
        }


        CancellationTokenSource cancelSrc;

        bool autoCancel;
        public bool AutoCancel
        {
            get { return this.autoCancel; }
            set
            {
                if (this.autoCancel == value)
                    return;

                this.autoCancel = value;
                this.OnPropertyChanged();
                if (value)
                    this.cancelSrc = new CancellationTokenSource();
                else
                    this.cancelSrc = null;
            }
        }


        ICommand CreateActionSheetCommand(bool useBottomSheet, bool cancel, int items, string message = null)
        {
            return new Command(() =>
            {
                var cfg = new ActionSheetConfig()
                    .SetTitle("Test Title")
                    .SetMessage(message)
                    .SetUseBottomSheet(useBottomSheet);

                IBitmap testImage = null;
                try
                {
                    testImage = BitmapLoader.Current.LoadFromResource("icon.png", null, null).Result;
                }
                catch
                {
                    Debug.WriteLine("Could not load image");
                }

                for (var i = 0; i < items; i++)
                {
                    var display = i + 1;
                    cfg.Add(
                        "Option " + display,
                        () => this.Result($"Option {display} Selected"),
                        testImage
                    );
                }
                cfg.SetDestructive(null, () => this.Result("Destructive BOOM Selected"), testImage);
                if (cancel)
                    cfg.SetCancel(null, () => this.Result("Cancel Selected"), testImage);

                var disp = this.Dialogs.ActionSheet(cfg);
                if (this.AutoCancel)
                {
                    Task.Delay(TimeSpan.FromSeconds(3))
                        .ContinueWith(x => disp.Dispose());
                }
            });
        }


        ICommand Create(Func<CancellationToken?, Task> action)
        {
            return new Command(async () =>
            {
                try
                {
                    this.cancelSrc?.CancelAfter(TimeSpan.FromSeconds(3));
                    await action(this.cancelSrc?.Token);
                }
                catch (OperationCanceledException)
                {
                    if (this.AutoCancel)
                        this.cancelSrc = new CancellationTokenSource();

                    this.Dialogs.Alert("Task cancelled successfully");
                }
            });
        }


        async Task PromptCommand(InputType inputType)
        {
            var msg = $"Enter a {inputType.ToString().ToUpper()} value";
            this.cancelSrc?.CancelAfter(TimeSpan.FromSeconds(3));
            var r = await this.Dialogs.PromptAsync(msg, inputType: inputType, cancelToken: this.cancelSrc?.Token);
            await Task.Delay(500);
            this.Result($"Prompt: {r.Choice} - Value: {r.Value}");
        }
    }
}
