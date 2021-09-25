using System;
using System.Collections.Generic;
using System.Text;

namespace SP.Core.Enum
{
    /// <summary>
    /// Тип округления
    /// </summary>
    public enum Rounding
    {
        /// <summary>
        /// Округление вниз
        /// </summary>
        Floor = 1,
        /// <summary>
        /// Округление вверх
        /// </summary>
        Ceiling = 2,
        /// <summary>
        /// Округление до ближайшего целого
        /// </summary>
        Round = 3,
    }
}
