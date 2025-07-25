using System;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure.Extensions
{
    public static class TaskExtensions
    {
        /// <summary>
         /// Запускает задачу без ожидания и подавляет предупреждение компилятора.
         /// Можно передать необязательный обработчик ошибок.
         /// </summary>
        public static void AndForget(this Task task, Action<Exception>? onError = null)
        {
            _ = ForgetInternal(task, onError);
        }

        private static async Task ForgetInternal(Task task, Action<Exception>? onError)
        {
            try
            {
                await task.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex);
            }
        }
    }
}
