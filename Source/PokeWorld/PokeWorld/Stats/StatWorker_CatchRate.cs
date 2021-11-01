using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse; 

namespace PokeWorld
{
    class StatWorker_CatchRate : StatWorker
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (!base.ShouldShowFor(req))
            {
                return false;
            }
            if (req.Def is ThingDef thingDef && thingDef.HasComp(typeof(CompPokemon)))
            {
                return true;
            }
            return false;
        }
        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (req.Def is ThingDef thingDef && thingDef.HasComp(typeof(CompPokemon)))
            {
                return thingDef.GetCompProperties<CompProperties_Pokemon>().catchRate;
            }
            return 0;
        }
        public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
        {
            float currentHealthPercent = 1;
            float catchRate = GetValueUnfinalized(req);
            float bonusBall = 1;
            float aValue = (1 - ((2 / 3f) * currentHealthPercent)) * catchRate * bonusBall;
            float bValue = aValue / 255;         
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("PW_StatCatchRateDesc".Translate(bValue.ToStringPercent()));
            return stringBuilder.ToString();
        }
    }
}
