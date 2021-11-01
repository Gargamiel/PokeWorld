using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace PokeWorld
{
    public class CompPokFossilsEvoStoneDropper : ThingComp
    {
        public CompProperties_PokFossilsEvoStoneDropper Props => (CompProperties_PokFossilsEvoStoneDropper)this.props;
        public float stoneDropRate => Props.stoneDropRate;
        public float fossilDropRate => Props.fossilDropRate;
    }

    public class CompProperties_PokFossilsEvoStoneDropper : CompProperties
    {
        public float stoneDropRate;
        public float fossilDropRate;
        public CompProperties_PokFossilsEvoStoneDropper()
        {
            this.compClass = typeof(CompPokFossilsEvoStoneDropper);
        }
    }
}