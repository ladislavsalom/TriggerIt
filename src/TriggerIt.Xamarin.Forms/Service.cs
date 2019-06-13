using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TriggerIt.Xamarin.Forms
{
    public static class Service
    {
        public static TriggerIt Instance
        {
            get;
            private set;
        }

        public static async Task InitAsync(Persisting.IEventsPersitor eventsPersitor)
        {
            Instance = new TriggerIt(new XamarinTimer(), eventsPersitor);
            await Instance.InitializeAsync();
        }
    }
}
