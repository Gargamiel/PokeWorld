using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    class DamageWorker_PokemonRangedMove : DamageWorker_AddInjury
    {
        private const float RangedDamageRandomFactorMin = 0.85f;

        private const float RangedDamageRandomFactorMax = 1f;
        public override DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            Pawn casterPawn = dinfo.Instigator as Pawn;
            if (casterPawn != null)
            {
                CompPokemon casterComp = casterPawn.TryGetComp<CompPokemon>();
                if (casterComp != null)
                {
                    int level = casterComp.levelTracker.level;
                    float movePower = dinfo.Amount;
                    int spAttack = casterComp.statTracker.attackSpStat;
                    int spDefense;
                    float stab = casterComp.moveTracker.lastUsedMove.IsStab(casterPawn) ? 1.5f : 1f;
                    float typeMultiplier = 1;
                    Pawn targetPawn = thing as Pawn;
                    if (targetPawn != null)
                    {
                        CompPokemon targetComp = targetPawn.TryGetComp<CompPokemon>();
                        if (targetComp != null)
                        {
                            spDefense = targetComp.statTracker.defenseSpStat;
                            foreach (TypeDef typeDef in targetComp.types)
                            {
                                typeMultiplier *= typeDef.GetDamageMultiplier(casterComp.moveTracker.lastUsedMove.type);
                            }
                        }
                        else
                        {
                            spDefense = 50;//(int)(targetPawn.GetStatValue(StatDefOf.ArmorRating_Blunt) * 40 + targetPawn.GetStatValue(StatDefOf.ArmorRating_Sharp) * 40 + targetPawn.GetStatValue(StatDefOf.ArmorRating_Heat) * 20);
                        }
                    }
                    else
                    {
                        spDefense = 30;//(int)(thing.GetStatValue(StatDefOf.ArmorRating_Blunt) * 40 + thing.GetStatValue(StatDefOf.ArmorRating_Sharp) * 40 + thing.GetStatValue(StatDefOf.ArmorRating_Heat) * 20);
                    }
                    float damage = (((2 * level / 5f) + 2) * movePower * (spAttack / (float)spDefense) / 10f) * stab * typeMultiplier;
                    damage = Rand.Range(damage * RangedDamageRandomFactorMin, damage * RangedDamageRandomFactorMax);
                    dinfo.SetAmount(damage);
                    //Log.Message(casterPawn.ToString() + " - " + dinfo.Amount.ToString());
                }
            }
            return base.Apply(dinfo, thing);
        }
    }
}
