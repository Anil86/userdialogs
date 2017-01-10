﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace Acr.UserDialogs
{
    public abstract class AbstractAlertDialog : IAlertDialog

    {
        protected AbstractAlertDialog()
        {
            this.TextEntries = new ReadOnlyCollection<TextEntry>(this.InternalTextEntries);
            this.Actions = new ReadOnlyCollection<DialogAction>(this.InternalActions);
        }


        ~AbstractAlertDialog()
        {
            Dispose(false);
        }


        public abstract void Show();
        public abstract void Dismiss();

        protected List<TextEntry> InternalTextEntries { get; } = new List<TextEntry>();
        protected List<DialogAction> InternalActions { get; } = new List<DialogAction>();


        public string Message { get; set; }
        public string Title { get; set; }
        public bool IsCancellable { get; set; }
        public DialogAction Positive { get; set; }
        public DialogAction Neutral { get; set; }
        public DialogAction Negative { get; set; }
        public Action Dismissed { get; set; }
        public IReadOnlyList<DialogAction> Actions { get; }
        public IReadOnlyList<TextEntry> TextEntries { get; }


        public IAlertDialog Add(TextEntry entry)
        {
            this.InternalTextEntries.Add(entry);
            return this;
        }


        public IAlertDialog Add(DialogAction action)
        {
            this.InternalActions.Add(action);
            return this;
        }


        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
        }
    }
}