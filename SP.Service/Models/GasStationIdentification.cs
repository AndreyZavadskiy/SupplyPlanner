using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Service.Models
{
    /// <summary>
    /// Идентификационные данные АЗС
    /// </summary>
    public class GasStationIdentification
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Код SAP
        /// </summary>
        public string CodeSAP { get; set; }
    }
}
