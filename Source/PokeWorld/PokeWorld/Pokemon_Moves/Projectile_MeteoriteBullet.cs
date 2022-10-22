using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    class Projectile_MeteoriteBullet : Bullet
    {
        #region Overrides
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = base.Map;
            IntVec3 position = base.Position;

            base.Impact(hitThing);

            SkyfallerMaker.SpawnSkyfaller(ThingDefOf.MeteoriteIncoming, position, map);

            Messages.Message("Meteorite_hit", MessageTypeDefOf.NeutralEvent);


        }
        #endregion Overrides


    }
}
