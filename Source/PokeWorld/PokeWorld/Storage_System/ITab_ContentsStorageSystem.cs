using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.Sound;

namespace PokeWorld
{
    class ITab_ContentsStorageSystem : ITab
	{
		private Vector2 scrollPosition = Vector2.zero;

		private float scrollViewHeight;

		private const float TopPadding = 20f;

		public static readonly Color ThingLabelColor = new Color(0.9f, 0.9f, 0.9f, 1f);

		public static readonly Color HighlightColor = new Color(0.5f, 0.5f, 0.5f, 1f);

		private const float ThingIconSize = 28f;

		private const float ThingRowHeight = 28f;

		private const float ThingLeftX = 36f;

		private const float StandardLineHeight = 22f;

		private const float InitialHeight = 450f;

		private static List<Thing> workingInvList = new List<Thing>();

		private bool flagSortDex = false;
		private bool flagSortName = false;
		private bool flagSortLevel = false;

		public override bool IsVisible
		{
			get
			{
				return true;
			}
		}

		private List<Thing> listInt = new List<Thing>();

		public IList<Thing> container
		{
			get
			{
				Building_PortableComputer portableComputer = base.SelThing as Building_PortableComputer;
				StorageSystem storageSystem = Find.World.GetComponent<StorageSystem>();
				listInt.Clear();
				if (portableComputer != null && storageSystem.ContainedThing != null)
				{
					listInt = storageSystem.ContainedThing;
				}
				if (flagSortDex)
				{
					return listInt.OrderBy((Thing x) => x.TryGetComp<CompPokemon>().pokedexNumber).ToList();
				}
				else if (flagSortName)
                {
					return listInt.OrderBy((Thing x) => x.def.label).ToList();
				}
				else if (flagSortLevel)
				{
					return listInt.OrderByDescending((Thing x) => x.TryGetComp<CompPokemon>().levelTracker.level).ToList();
				}
				else
                {
					return listInt;				
				}				
			}
		}

		protected override void FillTab()
		{
			Text.Font = GameFont.Small;
			Rect rect = new Rect(0f, 20f, size.x, size.y - 20f).ContractedBy(10f);
			Rect position = new Rect(rect.x, rect.y, rect.width, rect.height);
			GUI.BeginGroup(position);
			Text.Font = GameFont.Small;
			GUI.color = Color.white;
			Rect outRect = new Rect(0f, 0f, position.width, position.height);
			Rect viewRect = new Rect(0f, 0f, position.width - 16f, scrollViewHeight);
			Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
			float curY = 0f;
			DrawSortCheckbox(position.width - 220, ref curY);
			StorageSystem storageSystem = Find.World.GetComponent<StorageSystem>();
			string header = "PW_StorageSystemContainedPokemon".Translate(storageSystem.GetDirectlyHeldThings().Count, storageSystem.maxCount);
			Widgets.ListSeparator(ref curY, viewRect.width, header);
			foreach (Thing item2 in container)
			{
				DrawThingRow(ref curY, viewRect.width, item2);
			}			
			if (Event.current.type == EventType.Layout)
			{
				if (curY + 70f > 450f)
				{
					size.y = Mathf.Min(curY + 70f, (float)(UI.screenHeight - 35) - 165f - 30f);
				}
				else
				{
					size.y = 450f;
				}
				scrollViewHeight = curY + 20f;
			}
			Widgets.EndScrollView();
			GUI.EndGroup();
			GUI.color = Color.white;
			Text.Anchor = TextAnchor.UpperLeft;
		}
		public void DrawSortCheckbox(float x, ref float y)
		{
			bool flag1 = flagSortDex;
			bool flag2 = flagSortName;
			bool flag3 = flagSortLevel;
			Rect rect1 = new Rect(x, y, 180, 28);
			Text.Anchor = TextAnchor.MiddleLeft;
			Text.WordWrap = false;
			Text.Font = GameFont.Small;
			Widgets.Label(rect1, "PW_StorageSystemOrderByNumber".Translate());
			Widgets.Checkbox(rect1.xMax, rect1.y, ref flagSortDex, 25, false, paintable: true);	
			Rect rect2 = new Rect(x, y += 28, 180, 28);
			Widgets.Label(rect2, "PW_StorageSystemOrderByName".Translate());
			Widgets.Checkbox(rect2.xMax, rect2.y, ref flagSortName, 25, false, paintable: true);
			Rect rect3 = new Rect(x, y += 28, 180, 28);
			Widgets.Label(rect3, "PW_StorageSystemOrderByLevel".Translate());
			Widgets.Checkbox(rect3.xMax, rect3.y, ref flagSortLevel, 25, false, paintable: true);
			Text.WordWrap = true;
			if (flag1 != flagSortDex && flagSortDex == true)
			{
				flagSortName = false;
				flagSortLevel = false;
			}
			else if (flag2 != flagSortName && flagSortName == true)
			{
				flagSortDex = false;
				flagSortLevel = false;
			}
			else if (flag3 != flagSortLevel && flagSortLevel == true)
			{
				flagSortDex = false;
				flagSortName = false;
			}
			y += 2;
		}

		private void DrawThingRow(ref float y, float width, Thing thing, bool inventory = false)
		{
			Rect rect = new Rect(0f, y, width, 28f);
			Widgets.InfoCardButton(rect.width - 24f, y, thing);
			rect.width -= 24f;
			Rect rect2 = new Rect(rect.width - 24f, y, 24f, 24f);
			bool flagPower = SelThing.TryGetComp<CompPowerTrader>().PowerOn;
			if (Mouse.IsOver(rect2))
			{
				if (flagPower)
				{
					TooltipHandler.TipRegion(rect2, "DropThing".Translate());					
				}
				else
				{
					TooltipHandler.TipRegion(rect2, "PW_StorageSystemPCNotPowered".Translate());
				}
			}
			Color color = (flagPower ? Color.white : Color.grey);
			Color mouseoverColor = (flagPower ? GenUI.MouseoverColor : color);
			if (Widgets.ButtonImage(rect2, ContentFinder<Texture2D>.Get("UI/Buttons/Drop"), color, mouseoverColor, flagPower) && flagPower)
			{
				SoundDefOf.Tick_High.PlayOneShotOnCamera();
				InterfaceDrop(thing);
			}
			rect.width -= 24f;
			
			if (Mouse.IsOver(rect))
			{
				GUI.color = HighlightColor;
				GUI.DrawTexture(rect, TexUI.HighlightTex);
			}
			if (thing.def.DrawMatSingle != null && thing.def.DrawMatSingle.mainTexture != null)
			{
				Widgets.ThingIcon(new Rect(4f, y, 28f, 28f), thing);
			}
			Text.Anchor = TextAnchor.MiddleLeft;
			GUI.color = ThingLabelColor;
			Rect rect5 = new Rect(36f, y, rect.width - 36f, rect.height);
			string text = thing.LabelCap;
			Text.WordWrap = false;
			Widgets.Label(rect5, text.Truncate(rect5.width));
			Text.WordWrap = true;
			CompPokemon comp = thing.TryGetComp<CompPokemon>();
			if (comp != null)
			{
				Rect rect6 = new Rect(170f, y, rect.width - 170f, rect.height);
				string str2 = "Lv." + comp.levelTracker.level.ToString();
				Text.WordWrap = false;
				Widgets.Label(rect6, str2.Truncate(rect6.width));
				Text.WordWrap = true;
				int x = 0;
				foreach(TypeDef typeDef in comp.types)
                {
					Rect rect7 = new Rect(240f + 40f * x, y + 7, 32, 14);
					Widgets.DrawTextureFitted(rect7, typeDef.uiIcon, 1);
					x++;
				}
			}
			if (Mouse.IsOver(rect))
			{
				string text2 = thing.DescriptionDetailed;
				if (thing.def.useHitPoints)
				{
					text2 = text2 + "\n" + thing.HitPoints + " / " + thing.MaxHitPoints;
				}
				TooltipHandler.TipRegion(rect, text2);
			}
			y += 28f;
		}

		private void InterfaceDrop(Thing t)
		{
			ThingWithComps thingWithComps = t as ThingWithComps;
			Pawn pawn = t as Pawn;
			if (pawn != null)
			{
				if(GenDrop.TryDropSpawn(pawn, base.SelThing.Position, base.SelThing.Map, ThingPlaceMode.Near, out var _))
                {
					PutInBallUtility.PutInBall(pawn);
				}			
			}
		}

		public ITab_ContentsStorageSystem()
		{
			size = new Vector2(460f, 450f);
			labelKey = "PW_TabPCContents";
		}
	}
}
