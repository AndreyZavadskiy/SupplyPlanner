using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SP.Core.View;
using SP.Data;

namespace SP.Service.Services
{
    public interface IReportService
    {
        Task<Dictionary<string, string>> GetGlobalStatistics();
    }

    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, string>> GetGlobalStatistics()
        {
            string sqlText = @"
                SELECT 1 AS id,
                    'Работающие АЗС' AS ""IndicatorName"", 
                    CAST(COUNT(*) AS varchar(20)) AS ""IndicatorValue""
                FROM public.""GasStation"" gs 
                WHERE gs.""StationStatusId"" = 1
                UNION
                SELECT 2 AS id,
                    'Номенклатура' as ""IndicatorName"",
                    CAST(COUNT(*) AS varchar(20)) AS ""IndicatorValue""
                FROM public.""Nomenclature"" n
                WHERE n.""IsActive"" = true 
                UNION
                SELECT 3 AS id,
                    'Последняя загрузка остатков' as ""IndicatorName"",
                    to_char(max(i.""LastUpdate""), 'DD.MM.YYYY HH24:MI') AS ""IndicatorValue""
                FROM ""Inventory"" i 
                UNION
                SELECT 4 AS id,
                    'Последняя плановая поставка' as ""IndicatorName"",
                    to_char(max(o.""OrderDate""), 'DD.MM.YYYY HH24:MI') AS ""IndicatorValue""
                FROM ""Order"" o
                WHERE o.""OrderType"" = 2
                UNION
                SELECT 5 AS id,
                    'Последний заказ ТМЦ' as ""IndicatorName"",
                    to_char(max(o.""OrderDate""), 'DD.MM.YYYY HH24:MI') AS ""IndicatorValue""
                FROM ""Order"" o
                WHERE o.""OrderType"" = 1
                UNION
                SELECT 6 AS id,
                    'Всего записей в БД' as ""IndicatorName"",
                    to_char(SUM(n_live_tup), '999 999 999 999') AS ""IndicatorValue""
                FROM pg_stat_user_tables 
                WHERE schemaname != 'log'
                ORDER BY id;";

            var indicators = await _context.Set<IndicatorView>()
                .FromSqlRaw(sqlText)
                .ToArrayAsync();

            return indicators.ToDictionary(x => x.IndicatorName, x => x.IndicatorValue);
        }
    }
}
