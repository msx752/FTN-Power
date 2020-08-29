using System;

namespace Fortnite.External.ServiceStore.Events
{
    public class BrDailyStoreEventArgs : EventArgs
    {
        public string StoreFileName { set; get; }
        public string Title { get; set; }

        public BrDailyStoreEventArgs(string title, string filePath)
        {
            StoreFileName = filePath;
            Title = title;
        }
    }
}