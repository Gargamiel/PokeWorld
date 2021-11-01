using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    public class StatWorker_PokemonLevel : StatWorker
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (!base.ShouldShowFor(req))
            {
                return false;
            }
            if (req.Def is ThingDef thingDef && thingDef.HasComp(typeof(CompPokemon)) && req.HasThing)
            {
                return true;
            }
            return false;
        }
        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            return ValueFromReq(req);
        }
        private float ValueFromReq(StatRequest req)
        {
            if (req.Thing != null && req.Thing is Pawn pawn && pawn.TryGetComp<CompPokemon>() != null)
            {
                return pawn.TryGetComp<CompPokemon>().levelTracker.level;
            }
            return 0;
        }
        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            return "";
        }
        public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
        {
            return "";
        }
    }
}
