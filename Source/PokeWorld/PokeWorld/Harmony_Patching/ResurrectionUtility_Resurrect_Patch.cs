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
    [HarmonyPatch(typeof(ResurrectionUtility))]
    [HarmonyPatch("Resurrect")]
    public class ResurrectionUtility_Resurrect_Patch
    {
		public static bool Prefix(Pawn __0)
		{
			if (__0 != null && __0.TryGetComp<CompPokemon>() != null)
			{
				Log.Warning("Tried to resurrect a Pokémon, this is currently bugged and therefore disabled: " + __0.ToStringSafe());
				return false;
			}
			return true;
		}
	}
}
