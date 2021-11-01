using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PokeWorld
{
    class CompPokeball : ThingComp
    {
        public CompProperties_Pokeball Props => (CompProperties_Pokeball)this.props;
        public ThingDef ballDef => Props.ballDef;
    }
    public class CompProperties_Pokeball : CompProperties
    {
        public ThingDef ballDef;

        public CompProperties_Pokeball()
        {
            this.compClass = typeof(CompPokeball);
        }

        public CompProperties_Pokeball(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }
    }
}
