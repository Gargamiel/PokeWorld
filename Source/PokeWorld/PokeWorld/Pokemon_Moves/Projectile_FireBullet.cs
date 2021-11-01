using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    class Projectile_FireBullet : Bullet
    {
        #region Overrides
        protected override void Impact(Thing hitThing)
        {
            Map map = base.Map;
            IntVec3 position = base.Position;

            base.Impact(hitThing);

            GenExplosion.DoExplosion(position, map, 1.9f, DamageDefOf.Flame, null);

            Messages.Message("Lightning_hit", MessageTypeDefOf.NeutralEvent);


        }
        #endregion Overrides
    }
}
