namespace DictionaryInWinform
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class WordPairs
    {
        public int Id { get; set; }

        [StringLength(255)]
        public string Eng { get; set; }

        [StringLength(255)]
        public string Hun { get; set; }
    }
}
