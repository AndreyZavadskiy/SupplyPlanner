﻿using System.ComponentModel.DataAnnotations.Schema;

namespace SP.Core.Master
{
    /// <summary>
    /// Населенный пункт
    /// </summary>
    [Table("Settlement", Schema = "dic")]
    public class Settlement : DictionaryItem
    {
    }
}
