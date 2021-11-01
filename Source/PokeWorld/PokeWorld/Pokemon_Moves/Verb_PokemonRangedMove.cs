using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
	public class Verb_PokemonRangedMove : Verb_LaunchProjectile
	{
		protected override int ShotsPerBurst => verbProps.burstShotCount;
		public override bool Available()
		{
			CompPokemon comp = ((Pawn)caster).TryGetComp<CompPokemon>();
			if (comp != null)
			{
				MoveDef moveDef = comp.moveTracker.unlockableMoves.Keys.Where((MoveDef x) => x.verb == verbProps).First();
				return PokemonAttackGizmoUtility.ShouldUseMove((Pawn)caster, moveDef);
			}
			return false;
		}
		public override ThingDef Projectile
		{
			get
			{
				return verbProps.defaultProjectile;
			}
		}
		public override void WarmupComplete()
		{
			CompPokemon comp = caster.TryGetComp<CompPokemon>();
			if (comp != null)
			{
				comp.moveTracker.lastUsedMove = comp.moveTracker.unlockableMoves.Keys.Where((MoveDef x) => x.verb == verbProps).First();
			}
			base.WarmupComplete();
			Find.BattleLog.Add(new BattleLogEntry_RangedFire(caster, currentTarget.HasThing ? currentTarget.Thing : null, (base.EquipmentSource != null) ? base.EquipmentSource.def : null, Projectile, ShotsPerBurst > 1));
		}
	}
}
