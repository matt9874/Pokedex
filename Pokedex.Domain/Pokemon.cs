using System;
using System.Collections.Generic;
using System.Text;

namespace Pokedex.Domain
{
    public class Pokemon
    {
        public string Name { get; }
        public string Description { get; }
        public Habitat Habitat { get; set; }
        public bool IsLegendary { get; set; }
    }
}
