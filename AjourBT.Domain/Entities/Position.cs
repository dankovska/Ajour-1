using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AjourBT.Domain.Entities
{
    public class Position
    {
        [Key]
        public int PositionID { get; set; }

        [Required(ErrorMessage = "Будь ласка, введіть посаду")]
        public string TitleUk { get; set; }

        [Required(ErrorMessage = "Please enter Position")]
        public string TitleEn { get; set; }

        public virtual List<Employee> Employees { get; set; }

        [ConcurrencyCheck]
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
