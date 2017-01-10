﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Splat;


namespace Acr.UserDialogs
{
    public abstract class AbstractUserDialogs : IUserDialogs
    {
        const string NO_ONACTION = "OnAction should not be set as async will not use it";

        public abstract IAlertDialog CreateDialog();
        protected abstract IProgressDialog CreateDialogInstance(ProgressDialogConfig config);
        public abstract IDisposable Toast(ToastConfig config);
        public abstract IDisposable DatePrompt(DatePromptConfig config);
        public abstract IDisposable TimePrompt(TimePromptConfig config);
        public abstract void ShowImage(IBitmap image, string message, int timeoutMillis);


        public virtual IDisposable Toast(string message, TimeSpan? dismissTimer)
        {
            return this.Toast(new ToastConfig(message)
            {
                Duration = dismissTimer ?? ToastConfig.DefaultDuration
            });
        }


        static void Cancel<TResult>(IDisposable disp, TaskCompletionSource<TResult> tcs)
        {
            disp.Dispose();
            tcs.TrySetCanceled();
        }


        #region Progress Dialogs

        IProgressDialog loading;
        public virtual void ShowLoading(string title, MaskType? maskType)
        {
            if (this.loading != null)
                this.HideLoading();

            this.loading = this.Loading(title, null, null, true, maskType);
        }


        public virtual void HideLoading()
        {
            this.loading?.Dispose();
            this.loading = null;
        }


        public virtual IProgressDialog Loading(string title, Action onCancel, string cancelText, bool show, MaskType? maskType)
        {
            return this.Progress(new ProgressDialogConfig
            {
                Title = title ?? ProgressDialogConfig.DefaultTitle,
                AutoShow = show,
                CancelText = cancelText ?? ProgressDialogConfig.DefaultCancelText,
                MaskType = maskType ?? ProgressDialogConfig.DefaultMaskType,
                IsDeterministic = false,
                OnCancel = onCancel
            });
        }


        public virtual IProgressDialog Progress(string title, Action onCancel, string cancelText, bool show, MaskType? maskType)
        {
            return this.Progress(new ProgressDialogConfig
            {
                Title = title ?? ProgressDialogConfig.DefaultTitle,
                AutoShow = show,
                CancelText = cancelText ?? ProgressDialogConfig.DefaultCancelText,
                MaskType = maskType ?? ProgressDialogConfig.DefaultMaskType,
                IsDeterministic = true,
                OnCancel = onCancel
            });
        }


        public virtual IProgressDialog Progress(ProgressDialogConfig config)
        {
            var dlg = this.CreateDialogInstance(config);
            dlg.Title = config.Title;

            if (config.AutoShow)
                dlg.Show();

            return dlg;
        }

        #endregion

        #region ActionSheets

        public virtual IAlertDialog ActionSheet(ActionSheetConfig config)
        {
            var dlg = this.CreateDialog();
            //dlg.Positive = config.Positive;

            return null;
        }

        public virtual async Task<string> ActionSheetAsync(string title, string cancel, string destructive, CancellationToken? cancelToken = null, params string[] buttons)
        {
            var tcs = new TaskCompletionSource<string>();
            var cfg = new ActionSheetConfig();
            if (title != null)
                cfg.Title = title;

            // you must have a cancel option for actionsheetasync
            //cfg.SetCancel(cancel, () => tcs.TrySetResult(cancel));

            //if (destructive != null)
            //    cfg.SetDestructive(destructive, () => tcs.TrySetResult(destructive));

            //foreach (var btn in buttons)
            //    cfg.Add(btn, () => tcs.TrySetResult(btn));

            var disp = this.ActionSheet(cfg);
            using (cancelToken?.Register(() => Cancel(disp, tcs)))
            {
                return await tcs.Task;
            }
        }

        #endregion

        #region Alerts

        public virtual IAlertDialog Alert(AlertConfig config)
        {
            var dlg = this.CreateDialog();

            dlg.Show();

            return null;
        }


        public virtual IAlertDialog Alert(string message, string title, string okText, Action<DialogChoice> action)
        {
            //return this.Alert(new AlertConfig()
            //    .SetMessage(message)
            //    .SetTitle(title)
            //    .SetText(DialogChoice.Positive, okText ?? AlertConfig.DefaultPositive.Text)
            //);
            return null;
        }

        public virtual async Task<DialogChoice> AlertAsync(AlertConfig config, CancellationToken? cancelToken = null)
        {
            if (config.OnAction != null)
                throw new ArgumentException(NO_ONACTION);

            var tcs = new TaskCompletionSource<DialogChoice>();
            config.OnAction = x => tcs.TrySetResult(x);

            var disp = this.Alert(config);
            using (cancelToken?.Register(() => Cancel(disp, tcs)))
            {
                return await tcs.Task;
            }
        }


        public virtual Task<DialogChoice> AlertAsync(string message, string title, string okText, CancellationToken? cancelToken = null)
        {
            //return this.AlertAsync(new AlertConfig()
            //    .SetMessage(message)
            //    .SetTitle(title)
            //    .SetText(DialogChoice.Positive, okText ?? AlertConfig.DefaultPositive.Text),
            //    cancelToken
            //);
            return null;
        }

        #endregion

        #region Confirm

        public virtual IAlertDialog Confirm(ConfirmConfig config)
        {
            var dlg = this.CreateDialog();
            dlg.Title = config.Title;
            dlg.Message = config.Message;
            dlg.IsCancellable = config.IsCancellable;

            dlg.Add(new DialogAction
            {
                Label = config.Positive.Label, // ?? ConfirmConfig.DefaultPositive.Label
                Choice = DialogChoice.Positive,
                Tap = x => config.OnAction(true)
            });
            if (config.Neutral != null)
            {
                dlg.Add(new DialogAction
                {
                    Label = config.Neutral.Label, // ?? ConfirmConfig.DefaultNeutral.Label
                    Choice = DialogChoice.Neutral,
                    Tap = x => config.OnAction(false)
                });
            }
            dlg.Show();

            return dlg;
        }


        public virtual IAlertDialog Confirm(string message, Action<bool> onAction, string title, string okText, string cancelText)
        {
            return this.Confirm(new ConfirmConfig
            {
                Title = title,
                Message = message,
                Positive = new DialogAction
                {
                    Label = okText,
                    Tap = x => onAction(true)
                },
                Neutral = new DialogAction
                {
                    Label = cancelText,
                    Tap = x => onAction(true)
                }
            });
        }


        public virtual Task<bool> ConfirmAsync(string message, string title, string okText, string cancelText, CancellationToken? cancelToken = null)
        {
            // TODO: assert onaction not set
            var tcs = new TaskCompletionSource<bool>();
            var dlg = this.Confirm(message, x => tcs.TrySetResult(x), title, okText, cancelText);
            using (cancelToken?.Register(dlg.Dismiss))
            {
                return tcs.Task;
            }
        }


        public virtual Task<bool> ConfirmAsync(ConfirmConfig config, CancellationToken? cancelToken)
        {
            // TODO: assert onaction not set
            var tcs = new TaskCompletionSource<bool>();
            config.OnAction = x => tcs.TrySetResult(x);
            var dlg = this.Confirm(config);

            using (cancelToken?.Register(() => Cancel(dlg, tcs)))
            {
                return tcs.Task;
            }
        }

        #endregion

        #region Date/Time

        public virtual async Task<DialogResult<DateTime>> DatePromptAsync(DatePromptConfig config, CancellationToken? cancelToken = null)
        {
            if (config.OnAction != null)
                throw new ArgumentException(NO_ONACTION);

            var tcs = new TaskCompletionSource<DialogResult<DateTime>>();
            config.OnAction = x => tcs.TrySetResult(x);

            var disp = this.DatePrompt(config);
            using (cancelToken?.Register(() => Cancel(disp, tcs)))
            {
                return await tcs.Task;
            }
        }


        public virtual Task<DialogResult<DateTime>> DatePromptAsync(string title, DateTime? selectedDate, CancellationToken? cancelToken = null)
        {
            return this.DatePromptAsync(
                new DatePromptConfig
                {
                    Title = title,
                    SelectedDate = selectedDate
                },
                cancelToken
            );
        }


        public virtual async Task<DialogResult<TimeSpan>> TimePromptAsync(TimePromptConfig config, CancellationToken? cancelToken = null)
        {
            if (config.OnAction != null)
                throw new ArgumentException(NO_ONACTION);

            var tcs = new TaskCompletionSource<DialogResult<TimeSpan>>();
            config.OnAction = x => tcs.TrySetResult(x);

            var disp = this.TimePrompt(config);
            using (cancelToken?.Register(() => Cancel(disp, tcs)))
            {
                return await tcs.Task;
            }
        }


        public virtual Task<DialogResult<TimeSpan>> TimePromptAsync(string title, TimeSpan? selectedTime, CancellationToken? cancelToken = null)
        {
            return this.TimePromptAsync(
                new TimePromptConfig
                {
                    Title = title,
                    SelectedTime = selectedTime
                },
                cancelToken
            );
        }

        #endregion

        #region Login

        public virtual IAlertDialog Login(LoginConfig config)
        {
            return null;
        }


        public virtual async Task<DialogResult<Credentials>> LoginAsync(LoginConfig config, CancellationToken? cancelToken = null)
        {
            if (config.OnAction != null)
                throw new ArgumentException(NO_ONACTION);

            var tcs = new TaskCompletionSource<DialogResult<Credentials>>();
            config.OnAction = x => tcs.TrySetResult(x);

            var disp = this.Login(config);
            using (cancelToken?.Register(() => Cancel(disp, tcs)))
            {
                return await tcs.Task;
            }
        }


        public virtual Task<DialogResult<Credentials>> LoginAsync(string title, string message, CancellationToken? cancelToken = null)
        {
            return this.LoginAsync(new LoginConfig
            {
                //Title = title ?? LoginConfig.DefaultTitle,
                Message = message
            }, cancelToken);
        }

        #endregion

        #region Prompt

        public virtual IAlertDialog Prompt(PromptConfig config)
        {
            var dlg = this.CreateDialog();
            dlg.Title = config.Title;
            dlg.Message = config.Message;
            dlg.IsCancellable = config.IsCancellable;

            dlg.Dismissed = () => { };

            if (config.Positive != null)
            {

            }
            if (config.Neutral != null)
            {

            }
            if (config.Negative != null)
            {

            }
            // TODO: 3 buttons
            return dlg;
        }


        public virtual Task<DialogResult<string>> PromptAsync(PromptConfig config, CancellationToken? cancelToken = null)
        {
            if (config.OnAction != null)
                throw new ArgumentException(NO_ONACTION);

            var tcs = new TaskCompletionSource<DialogResult<string>>();
            config.OnAction = x => tcs.TrySetResult(x);
            var dlg = this.Prompt(config);

            using (cancelToken?.Register(() => Cancel(dlg, tcs)))
            {
                return tcs.Task;
            }
        }


        public virtual Task<DialogResult<string>> PromptAsync(string message, string title, string okText, string cancelText, string placeholder, KeyboardType inputType, CancellationToken? cancelToken = null)
        {
            //return this.PromptAsync(new PromptConfig()
            //    .SetMessage(message)
            //    .SetTitle(title)
            //    .SetText(DialogChoice.Neutral, cancelText ?? PromptConfig.DefaultNeutral.Text)
            //    .SetText(DialogChoice.Positive, okText ?? PromptConfig.DefaultPositive.Text)
            //    .SetPlaceholder(placeholder)
            //    .SetInputType(inputType),
            //    cancelToken
            //);
            return null;
        }

        #endregion
    }
}
