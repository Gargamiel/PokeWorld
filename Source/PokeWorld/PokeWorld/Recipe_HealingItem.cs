using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    public class Recipe_HealingItem : Recipe_Surgery
	{
		//Surgery recipe, not crafting recipe
		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			if (billDoer != null)
			{
				float hpLeftToHeal = ingredients[0].TryGetComp<CompHealingItem>().healingAmount;
				for (int i = 0; i < pawn.health.hediffSet.hediffs.Count; i++)
				{
					Hediff hediff = FindMostBleedingHediff(pawn);
					if (hediff != null)
					{
						float heddifSev = hediff.Severity;
						if (hediff.Severity <= hpLeftToHeal)
						{
							hediff.Heal(heddifSev);
							hpLeftToHeal -= heddifSev;
							continue;
						}
						else
						{
							hediff.Heal(hpLeftToHeal);
							break;
						}						
					}
                    else
                    {
						Hediff_Injury hediff_Injury = FindInjury(pawn);
						if (hediff_Injury != null)
						{
							float heddifSev = hediff_Injury.Severity;
							if (hediff_Injury.Severity <= hpLeftToHeal)
							{
								hediff_Injury.Heal(heddifSev);
								hpLeftToHeal -= heddifSev;
								continue;
							}
							else
							{
								hediff_Injury.Heal(hpLeftToHeal);
								break;
							}							
						}
                        else
                        {
							break;
						}						
					}				
				}				
			}
		}

		private Hediff FindMostBleedingHediff(Pawn pawn)
		{
			float num = 0f;
			Hediff hediff = null;
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				if (hediffs[i].Visible && hediffs[i].BleedRate > 0f)
				{
					float bleedRate = hediffs[i].BleedRate;
					if (bleedRate > num || hediff == null)
					{
						num = bleedRate;
						hediff = hediffs[i];
					}
				}
			}
			return hediff;
		}

		private Hediff_Injury FindInjury(Pawn pawn, IEnumerable<BodyPartRecord> allowedBodyParts = null)
		{
			Hediff_Injury hediff_Injury = null;
			List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
			for (int i = 0; i < hediffs.Count; i++)
			{
				Hediff_Injury hediff_Injury2 = hediffs[i] as Hediff_Injury;
				if (hediff_Injury2 != null && hediff_Injury2.Visible && hediff_Injury2.def.tendable && !hediff_Injury2.IsPermanent() && (allowedBodyParts == null || allowedBodyParts.Contains(hediff_Injury2.Part)) && (hediff_Injury == null || hediff_Injury2.Severity > hediff_Injury.Severity))
				{
					hediff_Injury = hediff_Injury2;
				}
			}
			return hediff_Injury;
		}
	}
}
