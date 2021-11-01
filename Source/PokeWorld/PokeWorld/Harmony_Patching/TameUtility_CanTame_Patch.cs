using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using HarmonyLib;
using Verse;

namespace PokeWorld
{
    [HarmonyPatch(typeof(TameUtility))]
    [HarmonyPatch("CanTame")]
    class TameUtility_CanTame_Patch
    {
        public static void Postfix(Pawn __0, ref bool __result)
        {
            if (__0.TryGetComp<CompPokemon>() != null )
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(PawnColumnWorker_Tame))]
    [HarmonyPatch("HasCheckbox")]
    class PawnColumnWorker_Tame_HasCheckbox_Patch
    {
        public static void Postfix(Pawn __0, ref bool __result)
        {
            if (__0.TryGetComp<CompPokemon>() != null)
            {
                __result = false;
            }
        }
    }
}
