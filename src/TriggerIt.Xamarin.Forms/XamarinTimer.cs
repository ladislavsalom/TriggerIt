using System;
using Xamarin.Forms;

namespace TriggerIt.Xamarin.Forms
{
    public class XamarinTimer : ITimer
    {
        private bool run = true;

        public void Destroy()
        {
            this.run = false;
        }

        public void Start(TimeSpan interval, Action tickAction)
        {
            Device.StartTimer(interval, () =>
            {
                if (this.run == false) return false; // prevent next running after Destroy call

                tickAction();

                return this.run;
            });
        }
    }
}
