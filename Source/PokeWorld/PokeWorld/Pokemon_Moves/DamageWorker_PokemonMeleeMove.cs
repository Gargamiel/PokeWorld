using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    class DamageWorker_PokemonMeleeMove : DamageWorker_AddInjury
    {
        private const float MeleeDamageRandomFactorMin = 0.85f;

        private const float MeleeDamageRandomFactorMax = 1f;
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
                    int attack = casterComp.statTracker.attackStat;
                    int defense;
                    float stab = casterComp.moveTracker.lastUsedMove.IsStab(casterPawn) ? 1.5f : 1f;
                    float typeMultiplier = 1;
                    Pawn targetPawn = thing as Pawn;
                    if (targetPawn != null)
                    {
                        CompPokemon targetComp = targetPawn.TryGetComp<CompPokemon>();
                        if (targetComp != null)
                        {
                            defense = targetComp.statTracker.defenseStat;
                            foreach(TypeDef typeDef in targetComp.types)
                            {
                                typeMultiplier *= typeDef.GetDamageMultiplier(casterComp.moveTracker.lastUsedMove.type);
                            }
                        }
                        else
                        {
                            defense = PokemonDamageUtility.GetNonPokemonPawnDefense(targetPawn);
                        }
                    }
                    else
                    {
                        defense = 50;
                    }
                    float damage = (((2 * level / 5f) + 2) * movePower * (attack / (float)defense) / 10f) * stab * typeMultiplier;
                    damage = Rand.Range(damage * MeleeDamageRandomFactorMin, damage * MeleeDamageRandomFactorMax);
                    dinfo.SetAmount(damage);
                    //Log.Message(casterPawn.ToString() + " - " + dinfo.Amount.ToString());
                }
            }       
            return base.Apply(dinfo, thing);
        }
    }
}
