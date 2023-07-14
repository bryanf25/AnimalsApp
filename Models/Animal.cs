using Microsoft.AspNetCore.Builder;

namespace AnimalsApp.Models
{
    public class Animal
    { 
            public long AnimalId { get; set; }
            public string Name { get; set; }
            public string Breed { get; set; }
            public DateTimeOffset BirthDate { get; set; }
            public Sex Sex { get; set; }
            public long Price { get; set; }
            public bool IsActive { get; set; }
        
    }
    public enum Sex { Female, Male };
}
