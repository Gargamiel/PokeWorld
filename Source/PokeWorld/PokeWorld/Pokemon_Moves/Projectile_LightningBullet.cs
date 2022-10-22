using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    class Projectile_LightningBullet : Bullet
    {
        #region Overrides
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = base.Map;
            IntVec3 position = base.Position;

            base.Impact(hitThing);

            map.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(map, position));

        Messages.Message("Lightning_hit", MessageTypeDefOf.NeutralEvent);
            

        }
        #endregion Overrides


    }
}
