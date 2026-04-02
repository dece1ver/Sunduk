using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure
{
    public static class JsInterop
    {
        public static async ValueTask TriggerAutoGrow(this IJSRuntime js, int delay = 300)
        {
            if (delay > 0) await Task.Delay(delay);
            await js.InvokeVoidAsync("triggerAutoGrow");
        }
    }
}
