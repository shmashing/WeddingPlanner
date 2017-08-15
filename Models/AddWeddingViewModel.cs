using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models {
    public class AddWeddingViewModel : BaseEntity {

        [Required]
        public int WedderOneId { get; set; }
        [Required]
        public int WedderTwoId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
        [Required]
        [MinLength(4)]
        public string Address { get; set; }

    }
}