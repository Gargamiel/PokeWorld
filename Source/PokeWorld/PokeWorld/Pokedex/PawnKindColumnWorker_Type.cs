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
	[StaticConstructorOnStartup]
	public class PawnKindColumnWorker_Type : PawnKindColumnWorker_Icon
	{
		protected override int Width => 32;
		protected override int Padding => 0;
		protected override Texture2D GetIconFor(PawnKindDef pawnKind)
		{
			if (Find.World.GetComponent<PokedexManager>().IsPokemonCaught(pawnKind))
			{
				Texture2D Icon = null;
				List<TypeDef> types = pawnKind.race.GetCompProperties<CompProperties_Pokemon>().types;
				if (def == DefDatabase<PawnKindColumnDef>.GetNamed("Type1"))
                {
					Icon = types[0].uiIcon;
				}
				else if (def == DefDatabase<PawnKindColumnDef>.GetNamed("Type2") && types.Count > 1)
                {
					Icon = pawnKind.race.GetCompProperties<CompProperties_Pokemon>().types[1].uiIcon;
				}				
				return Icon;
			}
			//Texture2D emptyIcon = Texture2D.blackTexture;
			return null;
		}


		protected override string GetIconTip(PawnKindDef pawnKind)
		{
			if (Find.World.GetComponent<PokedexManager>().IsPokemonCaught(pawnKind))
			{
				if (def == DefDatabase<PawnKindColumnDef>.GetNamed("Type1"))
				{
					return pawnKind.race.GetCompProperties<CompProperties_Pokemon>().types[0].label;
				}
				else if (def == DefDatabase<PawnKindColumnDef>.GetNamed("Type2"))
				{
					return pawnKind.race.GetCompProperties<CompProperties_Pokemon>().types[1]?.label;
				}			
			}
			return null;
		}
		protected override Vector2 GetIconSize(PawnKindDef pawnKind)
		{
			if (GetIconFor(pawnKind) == null)
			{
				return Vector2.zero;
			}
			return new Vector2(Width, 14);
		}
	}
}
