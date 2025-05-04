using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Models
{
    public class DefaultUser: IdentityUser
    {
        [PersonalData]
        public string FirstName {  get; set; }

        [PersonalData]
        public string LastName { get; set; }

        [PersonalData]
        public string Address { get; set; }

        [PersonalData]
        public string ZipCode { get; set; }

        [PersonalData]
        public string City { get; set; }

        [PersonalData]
        [DataType(DataType.Date)]
        public DateTime UserCreationDate { get; set; } = DateTime.Now;

        public bool IsBlocked { get; set; } 

    }
}
