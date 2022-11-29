using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Cachoutput
{
    public sealed class Person
    {
        public Person(int id, string name, List<string> skills)
        {
            Id = id;
            Name = name;
            Skills = skills;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Skills { get; set; }
    }
}