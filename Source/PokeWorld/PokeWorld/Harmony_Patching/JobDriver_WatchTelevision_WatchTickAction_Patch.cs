using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using HarmonyLib;
using RimWorld;

namespace PokeWorld
{
    [HarmonyPatch(typeof(JobDriver_WatchTelevision))]
    [HarmonyPatch("WatchTickAction")]
    class JobDriver_WatchTelevision_WatchTickAction_Patch
    {
        public static void Prefix(JobDriver_WatchTelevision __instance)
        {
            Thing thing = __instance.job.targetA.Thing;
            CompPokemonSpawner comp = thing.TryGetComp<CompPokemonSpawner>();
            if (comp != null)
            {
                comp.TickAction(__instance);                
            }
        }
    }
}
