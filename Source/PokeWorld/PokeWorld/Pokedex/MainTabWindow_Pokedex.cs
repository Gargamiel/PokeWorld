using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using RimWorld.Planet;
using Verse;
using UnityEngine;

namespace PokeWorld
{
	public class MainTabWindow_Pokedex : MainTabWindow_PawnKindTable
	{
		protected override PawnKindTableDef PawnKindTableDef => DefDatabase<PawnKindTableDef>.GetNamed("Pokedex");
		protected override IEnumerable<PawnKindDef> PawnKindDefs => DefDatabase<PawnKindDef>.AllDefs.Where((PawnKindDef x) => x.race.HasComp(typeof(CompPokemon)));
		protected override float ExtraTopSpace => 100f;

		public override void PostOpen()
		{
			base.PostOpen();
			Find.World.renderer.wantedMode = WorldRenderMode.None;
		}
		public override void DoWindowContents(Rect rect)
		{
			base.DoWindowContents(rect);
			if (Event.current.type != EventType.Layout)
			{
				GUI.color = Color.white;				
				Text.Font = GameFont.Medium;
				Text.Anchor = TextAnchor.UpperCenter;
				Widgets.Label(new Rect(rect.x/2, rect.y + 5f, rect.width, 30f), Faction.OfPlayer.HasName ? "PW_PokedexNamed".Translate(Faction.OfPlayer.Name) : "PW_Pokedex".Translate());
				Text.Font = GameFont.Small;
				Text.Anchor = TextAnchor.UpperLeft;
				Widgets.Label(new Rect(5f, rect.y + 40f, 160f, 30f), "PW_PokedexPokemonSeenCount".Translate(Find.World.GetComponent<PokedexManager>().TotalSeen().ToString()));
				Widgets.Label(new Rect(5f, rect.y + 60f, 160f, 30f), "PW_PokedexPokemonCaughtCount".Translate(Find.World.GetComponent<PokedexManager>().TotalCaught().ToString()));								
			}
		}
	}
}
