using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;

namespace PokeWorld
{
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("get_LabelNoCount")]
    public class Pawn_get_LabelNoCount_Patch
    {
        public static void Postfix(Pawn __instance, ref string __result)
        {
            __result = PokemonNamePatchUtility.TryPatchName(__instance, __result);
        }
    }
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("get_LabelShort")]
    public class Pawn_get_LabelShort_Patch
    {
        public static void Postfix(Pawn __instance, ref string __result)
        {
            if(__instance.Name != null)
            {
                __result = PokemonNamePatchUtility.TryPatchName(__instance, __result);
            }         
        }
    }

    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("get_LabelNoCountColored")]    
    public class Pawn_get_LabelNoCountColored_Patch
    {
        public static void Postfix(Pawn __instance, ref TaggedString __result)
        {
            __result = PokemonNamePatchUtility.TryPatchName(__instance, __result);
        }
    }
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("get_NameShortColored")]
    public class Pawn_get_NameShortColored_Patch
    {
        public static void Postfix(Pawn __instance, ref TaggedString __result)
        {
            __result = PokemonNamePatchUtility.TryPatchName(__instance, __result);
        }
    }
    [HarmonyPatch(typeof(Pawn))]
    [HarmonyPatch("get_NameFullColored")]
    public class Pawn_get_NameFullColored_Patch
    {
        public static void Postfix(Pawn __instance, ref TaggedString __result)
        {
            __result = PokemonNamePatchUtility.TryPatchName(__instance, __result);
        }
    }

    public static class PokemonNamePatchUtility
    {
        public static string TryPatchName(Pawn pawn, string name)
        {
            CompPokemon comp = pawn.TryGetComp<CompPokemon>();
            if (comp != null)
            {
                if (comp.shinyTracker != null && comp.shinyTracker.isShiny)
                {
                    name = GetShinyStar() + name;
                }
                if(comp.formTracker != null && comp.showFormLabel && (pawn.Faction == null || pawn.Faction != null && (!pawn.Faction.IsPlayer || pawn.Name.Numerical)))
                {
                    name += " " + GetFormLabel(comp);
                }          
            }
            return name;
        }
        private static string GetShinyStar()
        {
            return "★";
        }
        private static string GetFormLabel(CompPokemon comp)
        {
            if(comp.formTracker.currentFormIndex != -1)
            {
                return comp.forms[comp.formTracker.currentFormIndex].label;
            }
            return "";
        }
    }
}
