using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    class StatWorker_FishingSpeed : StatWorker
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (!base.ShouldShowFor(req))
            {
                return false;
            }
            if (req.Def is ThingDef thingDef && thingDef.category == ThingCategory.Pawn)
            {
                return true;
            }
            return false;
        }
    }
}
