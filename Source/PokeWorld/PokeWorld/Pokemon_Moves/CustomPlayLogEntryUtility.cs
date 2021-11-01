using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.Grammar;

namespace PokeWorld
{
	public static class CustomPlayLogEntryUtility
	{
		public static IEnumerable<Rule> RulesForOptionalMove(string prefix, MoveDef moveDef, ThingDef projectileDef)
		{
			if (moveDef == null)
			{
				yield break;
			}
			foreach (Rule item in GrammarUtility.RulesForDef(prefix, moveDef))
			{
				yield return item;
			}
			ThingDef thingDef = projectileDef;
			if (thingDef == null && moveDef.verb != null)
			{
				thingDef = moveDef.verb.defaultProjectile;
			}
			if (thingDef == null)
			{
				yield break;
			}
			foreach (Rule item2 in GrammarUtility.RulesForDef(prefix + "_projectile", thingDef))
			{
				yield return item2;
			}
		}
	}
}
