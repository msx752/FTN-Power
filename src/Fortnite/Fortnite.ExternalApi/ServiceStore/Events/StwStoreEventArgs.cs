using System;

namespace Fortnite.External.ServiceStore.Events
{
    public class StwStoreEventArgs : EventArgs
    {
        public string StoreFileName { set; get; }
        public string Title { get; set; }

        public StwStoreEventArgs(string title, string filePath)
        {
            StoreFileName = filePath;
            Title = title;
        }
    }
}