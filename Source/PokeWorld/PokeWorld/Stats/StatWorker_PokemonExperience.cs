using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    public class StatWorker_PokemonExperience : StatWorker
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (!base.ShouldShowFor(req))
            {
                return false;
            }
            if (req.Def is ThingDef thingDef && thingDef.HasComp(typeof(CompPokemon)) && req.HasThing && req.Thing.Faction == Faction.OfPlayer)
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
                return pawn.TryGetComp<CompPokemon>().levelTracker.experience;
            }
            return 0;
        }
        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            return "";
        }
        public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
        {
            if(req.Thing != null)
            {
                Pawn pawn = req.Thing as Pawn;
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine("PW_StatExperienceDesc".Translate(pawn.TryGetComp<CompPokemon>().levelTracker.experience, pawn.TryGetComp<CompPokemon>().levelTracker.totalExpForNextLevel.ToString()));
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("PW_StatExperienceProgress".Translate((pawn.TryGetComp<CompPokemon>().levelTracker.experience / (float)pawn.TryGetComp<CompPokemon>().levelTracker.totalExpForNextLevel).ToStringPercent()));
                return stringBuilder.ToString();
            }
            return "";
        }
    }
}
