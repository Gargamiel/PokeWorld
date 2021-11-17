using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RimWorld;
using Verse;

namespace PokeWorld
{
    public static class PokemonDamageUtility
    {
        public static int GetNonPokemonPawnDefense(Pawn pawn)
        {
			float armorSharp = GetOverallArmor(pawn, StatDefOf.ArmorRating_Sharp);
			float armorBlunt = GetOverallArmor(pawn, StatDefOf.ArmorRating_Blunt);
			float armorHeat = GetOverallArmor(pawn, StatDefOf.ArmorRating_Heat);
			float totalAverage = (armorSharp * 0.4f + armorBlunt * 0.4f + armorHeat * 0.2f);
			return 15 + (int)(totalAverage * 100);
		}
		private static float GetOverallArmor(Pawn pawn, StatDef stat)
		{
			float num = 0f;
			float num2 = Mathf.Clamp01(pawn.GetStatValue(stat) / 2f);
			List<BodyPartRecord> allParts = pawn.RaceProps.body.AllParts;
			List<Apparel> list = ((pawn.apparel != null) ? pawn.apparel.WornApparel : null);
			for (int i = 0; i < allParts.Count; i++)
			{
				float num3 = 1f - num2;
				if (list != null)
				{
					for (int j = 0; j < list.Count; j++)
					{
						if (list[j].def.apparel.CoversBodyPart(allParts[i]))
						{
							float num4 = Mathf.Clamp01(list[j].GetStatValue(stat) / 2f);
							num3 *= 1f - num4;
						}
					}
				}
				num += allParts[i].coverageAbs * (1f - num3);
			}
			return Mathf.Clamp(num * 2f, 0f, 2f);
		}
    }
}
