using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace PokeWorld
{
	[StaticConstructorOnStartup]
	public static class MoveCardUtility
	{
		public const float RowHeight = 28f;

		private const float InfoHeaderHeight = 50f;

		[TweakValue("Interface", -100f, 300f)]
		private static float PokemonMoveLeft = 220f;

		[TweakValue("Interface", -100f, 300f)]
		private static float PokemonMoveTop = 0f;

		private static readonly Texture2D LearnedTrainingTex = ContentFinder<Texture2D>.Get("UI/Icons/FixedCheck");

		private static readonly Texture2D LearnedNotTrainingTex = ContentFinder<Texture2D>.Get("UI/Icons/FixedCheckOff");

		public static void DrawMoveCard(Rect rect, Pawn pawn)
		{
			Text.Font = GameFont.Small;
			Rect rect2 = new Rect(PokemonMoveLeft, PokemonMoveTop, 30f, 30f);
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.Begin(rect);
			listing_Standard.Label("PW_TabMovesPokemonLevel".Translate(pawn.Label, pawn.TryGetComp<CompPokemon>().levelTracker.level), 22f);
			listing_Standard.Label("PW_TabMovesPokemonExperience".Translate(pawn.Label, pawn.TryGetComp<CompPokemon>().levelTracker.experience, pawn.TryGetComp<CompPokemon>().levelTracker.totalExpForNextLevel), 22f);
			/*
			if (pawn.training.HasLearned(TrainableDefOf.Obedience))
			{
				Rect rect3 = listing_Standard.GetRect(25f);
				Widgets.Label(rect3, "Master".Translate() + ": ");
				rect3.xMin = rect3.center.x;
				TrainableUtility.MasterSelectButton(rect3, pawn, paintable: false);
				listing_Standard.Gap();
				Rect rect4 = listing_Standard.GetRect(25f);
				bool checkOn = pawn.playerSettings.followDrafted;
				Widgets.CheckboxLabeled(rect4, "CreatureFollowDrafted".Translate(), ref checkOn);
				if (checkOn != pawn.playerSettings.followDrafted)
				{
					pawn.playerSettings.followDrafted = checkOn;
				}
				Rect rect5 = listing_Standard.GetRect(25f);
				bool checkOn2 = pawn.playerSettings.followFieldwork;
				Widgets.CheckboxLabeled(rect5, "CreatureFollowFieldwork".Translate(), ref checkOn2);
				if (checkOn2 != pawn.playerSettings.followFieldwork)
				{
					pawn.playerSettings.followFieldwork = checkOn2;
				}
			}
			*/
			listing_Standard.Gap();
			//Vector2 scrollPosition = Vector2.zero;
			//Rect position = new Rect(rect.x, rect.y, rect.width, rect.height);
			//Rect viewRect = new Rect(0f, 0f, position.width - 16f, 0);
			//listing_Standard.BeginScrollView(rect, ref scrollPosition, ref viewRect);

			DrawHeader(listing_Standard.GetRect(28f));
			listing_Standard.GapLine(10);		
			foreach (KeyValuePair<MoveDef, int> kvp in pawn.TryGetComp<CompPokemon>().moveTracker.unlockableMoves)
			{
				if (kvp.Key == DefDatabase<MoveDef>.GetNamed("Struggle"))
				{
					continue;
				}
				DrawMoveRow(listing_Standard.GetRect(28f), pawn, kvp);
			}
			//listing_Standard.EndScrollView(ref viewRect);
			listing_Standard.End();
		}

		public static float TotalHeightForPawn(Pawn p)
		{
			if (p == null || p.TryGetComp<CompPokemon>() == null)
			{
				return 0f;
			}
			int num = 1;
			foreach (KeyValuePair<MoveDef, int> kvp in p.TryGetComp<CompPokemon>().moveTracker.unlockableMoves)
			{		
				if(kvp.Key == DefDatabase<MoveDef>.GetNamed("Struggle"))
                {
					continue;
                }
				num++;		
			}
			float num2 = 122f + 28f * (float)num;
			return num2;
		}

		private static void DrawMoveRow(Rect rect, Pawn pawn, KeyValuePair<MoveDef, int> kvp)
		{
			bool flag = pawn.TryGetComp<CompPokemon>().moveTracker.HasUnlocked(kvp.Key);
			//AcceptanceReport canTrain = pawn.training.CanAssignToTrain(md, out visible);
			Widgets.DrawHighlightIfMouseover(rect);
			Text.Anchor = TextAnchor.MiddleLeft;
			Rect rect2 = rect;
			rect2.xMax = rect2.xMin + 100; 
			Widgets.Label(rect2, kvp.Key.LabelCap);
			Rect rect3 = new Rect(rect2.xMax + 17f, rect2.yMin + ((rect2.yMax - rect2.yMin - 14) / 2), 32, 14);
			Widgets.DrawTextureFitted(rect3, kvp.Key.type.uiIcon, 1);
			Rect rect4 = rect;
			rect4.xMin = rect3.xMax + 17f;
			rect4.xMax = rect4.xMin + 40f;			
			Rect rect5 = rect;
			rect5.xMin = rect4.xMax + 17f;
			rect5.xMax = rect5.xMin + 50f;
			if (kvp.Key.tool != null)
            {
				Widgets.Label(rect4, kvp.Key.tool.power.ToString());
				Widgets.Label(rect5, "PW_Melee".Translate());
			}
            else
            {
				Widgets.Label(rect4, kvp.Key.verb.defaultProjectile.projectile.GetDamageAmount(1).ToString());
				Widgets.Label(rect5, kvp.Key.verb.range.ToString());
			}
			Rect rect6 = rect;
			rect6.xMin = rect5.xMax + 17f;
			rect6.xMax = rect6.xMin + 50f;
			if (flag)
            {
				DoPokemonMoveCheckbox(rect6, pawn, kvp);
			}
			else
            {		
				Widgets.Label(rect6, "PW_LevelShort".Translate(kvp.Value));
			}
			Text.Anchor = TextAnchor.UpperLeft;
			
			if (flag)
			{
				GUI.color = Color.green;
			}			
			DoPokemonMoveTooltip(rect, pawn, kvp);
			GUI.color = Color.white;
		}
		private static void DrawHeader(Rect rect)
		{
			Text.Anchor = TextAnchor.MiddleLeft;
			Rect rect2 = rect;
			rect2.xMax = rect2.xMin + 100;
			Widgets.Label(rect2, "PW_Moves".Translate());
			Rect rect3 = rect;
			rect3.xMin = rect2.xMax + 17f;
			rect3.xMax = rect3.xMin + 32f;
			Widgets.Label(rect3, "PW_Type".Translate());
			Rect rect4 = rect;
			rect4.xMin = rect3.xMax + 17f;
			rect4.xMax = rect4.xMin + 40f;
			Widgets.Label(rect4, "PW_Power".Translate());
			Rect rect5 = rect;
			rect5.xMin = rect4.xMax + 17f;
			rect5.xMax = rect5.xMin + 50f;
			Widgets.Label(rect5, "PW_Range".Translate());
			Rect rect6 = rect;
			rect6.xMin = rect5.xMax + 17f;
			rect6.xMax = rect6.xMin + 65f;
			Widgets.Label(rect6, "PW_Used".Translate());
			Text.Anchor = TextAnchor.UpperLeft;
		}

		public static void DoPokemonMoveCheckbox(Rect rect, Pawn pawn, KeyValuePair<MoveDef, int> kvp)
		{
			bool num = pawn.TryGetComp<CompPokemon>().moveTracker.HasUnlocked(kvp.Key);
			bool checkOn = pawn.TryGetComp<CompPokemon>().moveTracker.GetWanted(kvp.Key);
			bool flag = checkOn;
			Texture2D texChecked = (num ? LearnedTrainingTex : null);
			Texture2D texUnchecked = (num ? LearnedNotTrainingTex : null);
			Widgets.Checkbox(rect.position, ref checkOn, rect.width/2, false, paintable: true);	
			if (checkOn != flag)
			{			
				pawn.TryGetComp<CompPokemon>().moveTracker.SetWanted(kvp.Key, checkOn);
			}
		}

		private static void DoPokemonMoveTooltip(Rect rect, Pawn pawn, KeyValuePair<MoveDef, int> kvp)
		{
			if (!Mouse.IsOver(rect))
			{
				return;
			}
			MoveDef md = kvp.Key;
			TooltipHandler.TipRegion(rect, delegate
			{
				string text = md.LabelCap + "\n\n" + md.type.LabelCap + "\n\n" + md.description;
				if(md.tool != null)
                {
					text += "\n\n" + "PW_PowerMeleeMove".Translate(md.tool.power);
				}
                else if (md.verb != null)
                {
					text += "\n\n" + "PW_PowerRangedMove".Translate(md.verb.defaultProjectile.projectile.GetDamageAmount(1), md.verb.range);
				}
                if (!pawn.TryGetComp<CompPokemon>().moveTracker.HasUnlocked(md))
                {
					text += "\n\n" + "PW_MoveUnlockAt".Translate(kvp.Value);
				}
				return text;
			}, (int)(rect.y * 612f + rect.x));
		}
	}
}
