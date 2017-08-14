using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models {
    public class Wedding : BaseEntity {
        public Wedding(){
            guests = new List<User>();
        }
        [Key]
        public int Id { get; set; }
        public int WedderOneId { get; set; }
        public int WedderTwoId { get; set; }
        public User WedderOne { get; set; }
        public User WedderTwo { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<User> guests { get; set; }

    }
}