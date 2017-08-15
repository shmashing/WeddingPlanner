using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models {
    public class AddWeddingViewModel : BaseEntity {

        [Required]
        public int WedderOneId { get; set; }

        [Required(ErrorMessage="Please enter your wedder")]
        public int WedderTwoId { get; set; }

        [Required(ErrorMessage="Please enter a date")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage="Please enter an address")]
        [MinLength(4)]
        public string Address { get; set; }

    }
}