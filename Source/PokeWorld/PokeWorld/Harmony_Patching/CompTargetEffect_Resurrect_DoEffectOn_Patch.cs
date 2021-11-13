using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using HarmonyLib;

namespace PokeWorld
{
	[HarmonyPatch(typeof(CompTargetEffect_Resurrect))]
	[HarmonyPatch("DoEffectOn")]
	public class CompTargetEffect_Resurrect_DoEffectOn_Patch
	{
		public static bool Prefix(Thing __1)
		{
			if (__1 != null && __1 is Corpse corpse && corpse.InnerPawn != null && corpse.InnerPawn.TryGetComp<CompPokemon>() != null)
			{
				Log.Error("Tried to resurrect a Pokémon, this is currently bugged and therefore disabled: " + corpse.InnerPawn.ToStringSafe());
				return false;
			}
			return true;
		}
	}
}
