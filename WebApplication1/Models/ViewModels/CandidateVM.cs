using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models.ViewModels
{
    public class CandidateVM
    {
        public int CandidateId { get; set; }
        [Required, StringLength(50), Display(Name = "Candidate Name")]
        public string CandidateName { get; set; } = default!;
        [Required, Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }
        public string Phone { get; set; } = default!;
        public string? Image { get; set; }
        public IFormFile? ImageFile { get; set; }
        public bool Fresher { get; set; }
        public List<int> candidateskill { get; set; } = new List<int>();
    }
}
