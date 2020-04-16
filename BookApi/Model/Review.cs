using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BookApi.Model
{
    public class Review
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(200, MinimumLength = 10, ErrorMessage = "It must be betwwen 20 to 200 characters")]
        public string HeadLine { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 50, ErrorMessage = "It must be between 50 to 2000 characters")]
        public string ReviewText { get; set; }

        [Required]
        [Range(1,5, ErrorMessage = "It must be between 1 to 5 stars")]
        public int Rating { get; set; }

        public virtual Reviewer Reviewer { get; set; }

        public virtual Book Book { get; set; }
    }
}
