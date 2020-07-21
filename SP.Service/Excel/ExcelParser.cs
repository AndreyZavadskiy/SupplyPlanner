using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;

namespace SP.Service.Excel
{
    public interface IExcelParser
    {
        /// <summary>
        /// Проверить заголовки колонок
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="colDefs"></param>
        /// <param name="headerRow"></param>
        /// <returns></returns>
        bool ProbeColumnHeaders(ExcelWorksheet ws, IEnumerable<ColumnDefinition> colDefs, out int headerRow);

        Dictionary<string, string> ParseDataRow(ExcelWorksheet ws, IEnumerable<ColumnDefinition> colDefs, int rowIndex);
    }

    public class ExcelParser : IExcelParser
    {
        /// <summary>
        /// Проверить заголовки колонок
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="colDefs"></param>
        /// <param name="headerRow"></param>
        /// <returns></returns>
        public bool ProbeColumnHeaders(ExcelWorksheet ws, IEnumerable<ColumnDefinition> colDefs, out int headerRow)
        {
            int lastRow = ws.Dimension.End.Row;
            headerRow = -1;
            if (lastRow <= 1)
            {
                return false;
            }

            if (!CheckColumnDefinitions(colDefs))
            {
                throw new InvalidOperationException("Неправильно определены колонки для распознавания.");
            }

            for (int r = 1; r <= lastRow; r++)
            {
                if (!ProbeHeaderRow(ws, colDefs, r))
                {
                    continue;
                }

                headerRow = r;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Проверить правильность указания определений колонок
        /// </summary>
        /// <param name="colDefs"></param>
        /// <returns></returns>
        private bool CheckColumnDefinitions(IEnumerable<ColumnDefinition> colDefs)
        {
            if (colDefs == null || !colDefs.Any())
            {
                return false;
            }

            // TODO: написать проверку уникальности определений
            // внутреннее имя, заголовок, индекс по умолчанию

            return true;
        }

        /// <summary>
        /// Проверить строку заголовка таблицы
        /// </summary>
        /// <param name="ws"></param>
        /// <param name="colDefs"></param>
        /// <param name="rowIndex"></param>
        /// <returns></returns>
        private bool ProbeHeaderRow(ExcelWorksheet ws, IEnumerable<ColumnDefinition> colDefs, int rowIndex)
        {
            foreach (var def in colDefs)
            {
                if (!CompareCellText(def.Title, ws.Cells[rowIndex, def.DefaultIndex].Text, def.TitleComparisonMode))
                {
                    return false;
                }

                def.ColumnIndex = def.DefaultIndex;
            }

            bool hasMissingColumn = colDefs.Any(x => x.ColumnIndex == null);
            
            // TODO: написать поиск в других колонках по названию

            return !hasMissingColumn;
        }

        /// <summary>
        /// Сравнить текст ячейки
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="comparisonMode"></param>
        /// <returns></returns>
        private bool CompareCellText(string expected, string actual, ComparisonMode comparisonMode)
        {
            if (string.IsNullOrWhiteSpace(actual))
            {
                return false;
            }

            // TODO: удалить все двойные пробелы
            actual = actual.Trim();

            switch (comparisonMode)
            {
                case ComparisonMode.Equals:
                    return actual.Equals(expected, StringComparison.InvariantCultureIgnoreCase);
                case ComparisonMode.Contains:
                    return actual.Contains(expected, StringComparison.InvariantCultureIgnoreCase);
                case ComparisonMode.StartsWith:
                    return actual.StartsWith(expected, StringComparison.InvariantCultureIgnoreCase);
                case ComparisonMode.EndsWith:
                    return actual.EndsWith(expected, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        public Dictionary<string, string> ParseDataRow(ExcelWorksheet ws, IEnumerable<ColumnDefinition> colDefs, int rowIndex)
        {
            var result = new Dictionary<string, string>();
            
            foreach (var def in colDefs)
            {
                string cellText = ws.Cells[rowIndex, def.ColumnIndex.Value].Text;
                if (string.IsNullOrWhiteSpace(cellText) && !def.IsNullable)
                {
                    return null;
                }

                result.Add(def.Name, cellText);
            }

            return result;
        }
    }
}
