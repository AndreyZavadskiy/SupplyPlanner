using System;

namespace SP.Service.Background
{
    /// <summary>
    /// Информация о выполнении фоновой службы
    /// </summary>
    public class BackgroundServiceData
    {
        /// <summary>
        /// Гуид записи о выполнении фоновой службы
        /// </summary>
        public Guid Key { get; set; }
        /// <summary>
        /// Статус
        /// </summary>
        public BackgroundServiceStatus Status { get; set; }
        /// <summary>
        /// Шаг выполнения
        /// </summary>
        public string Step { get; set; }
        /// <summary>
        /// Индикатор выполнения
        /// </summary>
        public decimal Progress { get; set; }
        /// <summary>
        /// Лог действий
        /// </summary>
        public string Log { get; set; }
    }
}
