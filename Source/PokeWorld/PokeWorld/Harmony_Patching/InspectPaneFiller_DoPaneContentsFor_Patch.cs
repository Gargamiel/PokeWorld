using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using RimWorld;
using UnityEngine;

namespace PokeWorld
{
	[StaticConstructorOnStartup]
	[HarmonyPatch(typeof(InspectPaneFiller))]
    [HarmonyPatch("DoPaneContentsFor")]
    class InspectPaneFiller_DoPaneContentsFor_Patch
    {
		private static readonly Texture2D BarBGTex = SolidColorMaterials.NewSolidColorTexture(new ColorInt(10, 10, 10).ToColor);

		private static readonly Texture2D ExperienceTex = SolidColorMaterials.NewSolidColorTexture(new ColorInt(35, 35, 35).ToColor);
		public static void Postfix(ISelectable __0, Rect __1)
		{
			Pawn pawn = __0 as Pawn;
			if (pawn != null)
			{
				CompPokemon comp = pawn.TryGetComp<CompPokemon>();
				if (comp != null)
				{
					try
					{
						GUI.BeginGroup(__1);
						float num;
						bool flagFaction;
						if (pawn.Faction == Faction.OfPlayer)
                        {
							num = 198f;
							flagFaction = true;
						}
                        else
                        {
							num = 99f;
							flagFaction = false;
                        }
						WidgetRow row = new WidgetRow(num, 3f);
						DrawExperience(row, comp, flagFaction);
						if (Find.World.GetComponent<PokedexManager>().IsPokemonCaught(pawn.kindDef))
						{
							float num2;
							if (flagFaction)
							{
								num2 = 297f;
							}
							else
							{
								num2 = 198f;
							}
							DrawType(num2, comp);
						}						
					}
					catch (Exception ex)
					{
						Log.ErrorOnce(string.Concat("Error in DoPaneContentsFor ", Find.Selector.FirstSelectedObject, ": ", ex.ToString()), 754672);
					}
					finally
					{
						GUI.EndGroup();
					}
				}
			}
		}
		
		public static void DrawType(float num, CompPokemon comp)
		{
			int x = 0;
			foreach(TypeDef typeDef in comp.types)
            {
				Texture2D texture = typeDef.uiIcon;
				Widgets.DrawTextureFitted(new Rect(num + 37f * x, 4f, 32f, 14f), texture, 1);
				x++;
			}
		}
		public static void DrawExperience(WidgetRow row, CompPokemon comp, bool flag)
		{
			float fillPct;
			string label;
			GUI.color = Color.yellow;
			if(comp.levelTracker.level < 100 && flag)
            {
				fillPct = comp.levelTracker.experience / (float)comp.levelTracker.totalExpForNextLevel;
				label = "Lv." + comp.levelTracker.level + "  " + comp.levelTracker.experience + "/" + comp.levelTracker.totalExpForNextLevel;
			}
            else
            {
				fillPct = flag ? 1 : 0;
				label = "Lv." + comp.levelTracker.level;
			}
			row.FillableBar(93f, 16f, fillPct, label, ExperienceTex, BarBGTex);
			GUI.color = Color.white;
		}
	}
}
