using System;
using System.Diagnostics.CodeAnalysis;

namespace SP.Service.DTO
{
    /// <summary>
    /// Ключ уникальности для парсинга ТМЦ
    /// </summary>
    public class InventoryParsingKey : IComparable<InventoryParsingKey>
    {
        /// <summary>
        /// Код SAP АЗС
        /// </summary>
        public string StationCodeSAP { get; set; }
        /// <summary>
        /// Код ТМЦ
        /// </summary>
        public string InventoryCode { get; set; }

        public int CompareTo([AllowNull] InventoryParsingKey other)
        {
            if (other == null)
                throw new ArgumentException("Incorrect value to compare");

            if (StationCodeSAP == other.StationCodeSAP)
                return InventoryCode.CompareTo(other.InventoryCode);
            else
            {
                return StationCodeSAP.CompareTo(other.StationCodeSAP);
            }
        }
    }
}
