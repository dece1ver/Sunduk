using BlazorCommon.RazorLib.BackgroundTaskCase;
using BlazorCommon.RazorLib.ComponentRenderers;
using BlazorCommon.RazorLib.Notification;
using BlazorCommon.RazorLib.WatchWindow;
using BlazorCommon.RazorLib.WatchWindow.TreeViewDisplays;
using BlazorDownloadFile;
using Blazored.LocalStorage;
using BlazorTextEditor.RazorLib;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Sunduk.PWA
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddMudServices(config =>
            {
                config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomCenter;

                config.SnackbarConfiguration.PreventDuplicates = false;
                config.SnackbarConfiguration.NewestOnTop = false;
                config.SnackbarConfiguration.ShowCloseIcon = true;
                config.SnackbarConfiguration.VisibleStateDuration = 3000;
                config.SnackbarConfiguration.HideTransitionDuration = 200;
                config.SnackbarConfiguration.ShowTransitionDuration = 200;
                config.SnackbarConfiguration.SnackbarVariant = Variant.Text;
            });
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddBlazorDownloadFile();

            var watchWindowTreeViewRenderers = new WatchWindowTreeViewRenderers(
                typeof(TreeViewTextDisplay),
                typeof(TreeViewReflectionDisplay),
                typeof(TreeViewPropertiesDisplay),
                typeof(TreeViewInterfaceImplementationDisplay),
                typeof(TreeViewFieldsDisplay),
                typeof(TreeViewExceptionDisplay),
                typeof(TreeViewEnumerableDisplay));

            var commonRendererTypes = new BlazorCommonComponentRenderers(
                typeof(BackgroundTaskDisplay),
                typeof(CommonErrorNotificationDisplay),
                typeof(CommonInformativeNotificationDisplay),
                typeof(TreeViewExceptionDisplay),
                typeof(TreeViewMissingRendererFallbackDisplay),
                watchWindowTreeViewRenderers);

            builder.Services.AddSingleton<IBlazorCommonComponentRenderers>(_ => commonRendererTypes);
            builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            builder.Services.AddSingleton<IBackgroundTaskMonitor, BackgroundTaskMonitor>();
            builder.Services.AddHostedService<QueuedHostedService>();
            builder.Services.AddBlazorTextEditor();

            await builder.Build().RunAsync();
        }
    }
}
