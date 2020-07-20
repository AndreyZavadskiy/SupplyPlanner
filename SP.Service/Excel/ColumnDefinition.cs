using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Service.Excel
{
    /// <summary>
    /// Определение колонок для парсинга
    /// </summary>
    public class ColumnDefinition
    {
        // Название колонки (внутреннее имя)
        public string Name { get; set; }
        /// <summary>
        /// Заголовок или ключевая фраза, которые должны быть в таблице
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Режим сравнения заголовке
        /// </summary>
        public ComparisonMode TitleComparisonMode { get; set; }
        /// <summary>
        /// Предопределенный индекс колонки (начиная с 1)
        /// </summary>
        public int DefaultIndex { get; set; }
        /// <summary>
        /// Индекс колонки в таблице
        /// null, если колонка не найдена
        /// </summary>
        public int? ColumnIndex { get; set; }
        /// <summary>
        /// Является необязательной
        /// </summary>
        public bool IsNullable { get; set; }
    }
}
