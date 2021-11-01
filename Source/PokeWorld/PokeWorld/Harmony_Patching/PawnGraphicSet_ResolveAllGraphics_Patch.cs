using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;


namespace PokeWorld
{
    [HarmonyPatch(typeof(PawnGraphicSet))]
    [HarmonyPatch("ResolveAllGraphics")]
    class PawnGraphicSet_ResolveAllGraphics_Patch
    {
        static void Postfix( ref PawnGraphicSet __instance)
        {
            CompPokemon compPokemon = __instance.pawn.TryGetComp<CompPokemon>();
            if(compPokemon != null)
            {
                bool flag = false;
                string texPath = "";
                if (compPokemon.formTracker != null)
                {
                    texPath += compPokemon.formTracker.GetCurrentFormKey();
                    flag = true;
                }
                if (compPokemon.shinyTracker != null && compPokemon.shinyTracker.isShiny)
                {
                    texPath += "Shiny";
                    flag = true;
                }
                if (flag)
                {
                    GraphicData graphicData = new GraphicData();
                    graphicData.CopyFrom(__instance.nakedGraphic.data);
                    graphicData.texPath += texPath;
                    __instance.nakedGraphic = graphicData.Graphic;
                }
            }         
        }
    }
}
