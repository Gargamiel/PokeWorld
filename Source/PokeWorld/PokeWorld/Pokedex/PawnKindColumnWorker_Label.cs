using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace PokeWorld
{
	public class PawnKindColumnWorker_Label : PawnKindColumnWorker
	{
		private const int LeftMargin = 3;

		private static Dictionary<string, string> labelCache = new Dictionary<string, string>();

		private static float labelCacheForWidth = -1f;

		public override void DoCell(Rect rect, PawnKindDef pawnKind, PawnKindTable table)
		{
			Rect rect2 = new Rect(rect.x, rect.y, rect.width, Mathf.Min(rect.height, 30f));
			if (Mouse.IsOver(rect2))
			{
				GUI.DrawTexture(rect2, TexUI.HighlightTex);
			}
			string str;
			if (Find.World.GetComponent<PokedexManager>().IsPokemonSeen(pawnKind))
			{
				str = pawnKind.label;
			}
			else
			{
				str = "PW_PokedexPokemonUnseenLabel".Translate();
			}
			Rect rect4 = rect2;
			rect4.xMin += 3f;
			if (rect4.width != labelCacheForWidth)
			{
				labelCacheForWidth = rect4.width;
				labelCache.Clear();
			}
			Text.Font = GameFont.Small;
			Text.Anchor = TextAnchor.MiddleLeft;
			Text.WordWrap = false;
			Widgets.Label(rect4, str.Truncate(rect4.width, labelCache));
			Text.WordWrap = true;
			Text.Anchor = TextAnchor.UpperLeft;
		}

		public override int GetMinWidth(PawnKindTable table)
		{
			return Mathf.Max(base.GetMinWidth(table), 80);
		}

		public override int GetOptimalWidth(PawnKindTable table)
		{
			return Mathf.Clamp(100, GetMinWidth(table), GetMaxWidth(table));
		}
	}

}
