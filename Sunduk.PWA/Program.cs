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
using MudExtensions.Services;
using Sunduk.PWA.Infrastructure.State;

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

            builder.Services.AddMudExtensions();

            builder.Services.AddSingleton<SunducamState>();
            builder.Services.AddSingleton<AngleConverterState>();
            builder.Services.AddSingleton<PointCoordinatesState>();
            builder.Services.AddSingleton<SawingTimeState>();
            builder.Services.AddSingleton<TurningTimeState>();
            builder.Services.AddSingleton<DrillAngleState>();
            builder.Services.AddSingleton<HoleCapacityState>();
            builder.Services.AddSingleton<RadiusState>();
            builder.Services.AddSingleton<NippleState>();
            builder.Services.AddSingleton<Arc2State>();
            builder.Services.AddSingleton<ArcGeneralState>();
            builder.Services.AddSingleton<ChamferState>();
            builder.Services.AddSingleton<ThreadTurningState>();
            builder.Services.AddSingleton<ToleranceState>();
            builder.Services.AddSingleton<CalculatorPageState>();
            // Scoped, не Singleton: ILocalStorageService у Blazored.LocalStorage зарегистрирован
            // как Scoped, а singleton не может зависеть от scoped-сервиса. В WASM-приложении
            // весь рантайм — один scope на всё время жизни вкладки, так что для наших целей
            // Scoped здесь эквивалентен Singleton (тот же экземпляр везде).
            builder.Services.AddScoped<MachineRegistry>();

            await builder.Build().RunAsync();
        }
    }
}
