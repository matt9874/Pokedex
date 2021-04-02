using System;

namespace Pokedex.Domain
{
    public class Pokemon: IEquatable<Pokemon>
    {
        public Pokemon(string name, TranslatedText description, string habitat, bool isLegendary)
        {
            Name = name ?? throw new ArgumentNullException("name", "Name of Pokemon cannot be null");
            Description = description;
            Habitat = habitat;
            IsLegendary = isLegendary;
        }

        public string Name { get; }
        public TranslatedText Description { get; }
        public string Habitat { get; }
        public bool IsLegendary { get; }

        public bool Equals(Pokemon other)
        {
            if (other == null)
                return false;
            return Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            Pokemon other = obj as Pokemon;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
