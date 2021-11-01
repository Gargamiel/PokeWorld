using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;

namespace PokeWorld
{
    [HarmonyPatch(typeof(Pawn), "GetGizmos")]
    class Pawn_GetGizmos_Patch
    {
        static void Postfix(ref IEnumerable<Gizmo> __result, ref Pawn __instance)
        {
            CompPokemon comp = __instance.TryGetComp<CompPokemon>();
            if (comp != null)
            {
                __result = __result.Concat(comp.CompGetGizmosExtra());
            }
        }
    }
}
