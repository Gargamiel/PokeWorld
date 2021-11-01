using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using HarmonyLib;
using System.Reflection;

namespace PokeWorld
{
    [HarmonyPatch(typeof(VerbTracker))]
    [HarmonyPatch("InitVerbs")]
    class VerbTracker_InitVerbs_Patch
    {
		
        public static void Postfix(Func<Type, string, Verb> __0, VerbTracker __instance)
        {
            if (!(__instance.directOwner is Pawn pawn))
            {
                return;
            }
            CompPokemon comp = pawn.TryGetComp<CompPokemon>();
			if(comp == null)
            {
				return;
            }
			foreach(KeyValuePair<MoveDef, int> kvp in comp.moveTracker.unlockableMoves)
			{
				VerbProperties verbProperties = kvp.Key.verb;
				if (verbProperties != null)
				{
					try
					{
						string text = Verb.CalculateUniqueLoadID(pawn, comp.moveTracker.unlockableMoves.Keys.ToList().IndexOf(kvp.Key));

						MethodInfo methodInitVerb = __instance.GetType().GetMethod("InitVerb", BindingFlags.NonPublic | BindingFlags.Instance);
						methodInitVerb.Invoke(__instance, new object[] { __0(verbProperties.verbClass, text), verbProperties, null, null, text });
					}
					catch (Exception ex)
					{
						Log.Error("Could not instantiate Verb (directOwner=" + pawn.ToStringSafe() + "): " + ex);
					}
				}
				Tool tool = kvp.Key.tool;
				if (tool != null)
				{
					ManeuverDef maneuver = tool.Maneuvers.First();
					try
					{
						VerbProperties verb = maneuver.verb;
						string text2 = Verb.CalculateUniqueLoadID(pawn, comp.moveTracker.unlockableMoves.Keys.ToList().IndexOf(kvp.Key));

						MethodInfo methodInitVerb = __instance.GetType().GetMethod("InitVerb", BindingFlags.NonPublic | BindingFlags.Instance);
						methodInitVerb.Invoke(__instance, new object[] { __0(verb.verbClass, text2), verb, tool, maneuver, text2 });
					}
					catch (Exception ex2)
					{
						Log.Error("Could not instantiate Verb (directOwner=" + pawn.ToStringSafe() + "): " + ex2);
					}
					
				}				
			}
		}
    }
}
