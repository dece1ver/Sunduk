using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sunduk.PWA.Infrastructure
{
    public class ChangelogInfo
    {
        public Version Version { get; set; }
        public DateTime? Date { get; set; }
        public string[] Description { get; set; }
        
        /// <summary>
        /// Изменение
        /// </summary>
        /// <param name="version">Версия</param>
        /// <param name="date">Дата</param>
        /// <param name="description">Массив строк, где каждая строка - один параграф</param>
        public ChangelogInfo(Version version, DateTime? date, string[] description)
        {
            Version = version;
            Date = date;
            Description = description;
        }
    }
}
