using Microsoft.AspNetCore.Builder;

namespace AnimalsApp.Models
{
    public class Animal
    { 
            public long id { get; set; }
            public string? Name { get; set; }
            public string? Breed { get; set; }
            public string? BirthDate { get; set; }
            public string?  Sex { get; set; }
            public long? Price { get; set; }
            public bool? IsActive { get; set; }
        
    }
}
