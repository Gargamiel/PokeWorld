using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    class CompXpEvGiver : ThingComp
    {
        private int lastHitTime = -1;
        private int maxCount = 8;
        private int expToGive = 0;
        private List<Pawn> giveTo;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            giveTo = new List<Pawn>();
        }
        public override void CompTickRare()
        {
            if (giveTo.Count > 0 && GenTicks.TicksAbs - lastHitTime > 60000)
            {
                giveTo.Clear();
            }
        }       
        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(dinfo, out absorbed);
            if(lastHitTime == -1)
            {
                expToGive = GetExperienceYield();
            }       
        }        
        public override void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.PostPostApplyDamage(dinfo, totalDamageDealt);
            lastHitTime = GenTicks.TicksAbs;
            Pawn pawn = parent as Pawn;
            if (dinfo.Instigator is Pawn instigator)
            {
                CompPokemon instigatorComp = instigator.TryGetComp<CompPokemon>();
                if (instigatorComp != null && instigator.Faction == Faction.OfPlayer && parent.Faction != Faction.OfPlayer)
                {
                    if (pawn == null || !pawn.Downed || pawn.Dead || parent.Destroyed)
                    {
                        if (!giveTo.Contains(instigator) && giveTo.Count < maxCount && instigator != parent)
                        {
                            giveTo.Add(instigator);
                        }                        
                    }
                }
            }
            if ((pawn != null && pawn.Dead) || parent.Destroyed)
            {                
                DistributeXPandEV();
            }
        }       
        private int GetExperienceYield()
        {
            return (int)parent.GetStatValue(DefDatabase<StatDef>.GetNamed("PW_BaseXPYield"));
        }
        private void FilterDeadAndDownedFromGiveTo()
        {
            giveTo = giveTo.Where((Pawn pawn) => pawn != null && !pawn.Dead && !pawn.Downed).ToList();
        }
        private void DistributeXPandEV()
        {
            FilterDeadAndDownedFromGiveTo();
            CompPokemon ownComp = parent.TryGetComp<CompPokemon>();
            foreach (Pawn pawn in giveTo)
            {                            
                CompPokemon ennemyComp = pawn.TryGetComp<CompPokemon>();
                if (ennemyComp != null)
                {
                    ennemyComp.levelTracker.IncreaseExperience(expToGive / giveTo.Count);
                    if (ownComp != null)
                    {
                        foreach (EVYield EV in ownComp.EVYields)
                        {
                            ennemyComp.statTracker.IncreaseEV(EV.stat, EV.value);
                        }
                    }
                }         
            }
        }
        public void DistributeAfterCatch()
        {
            DistributeXPandEV();
        }
        public override void PostExposeData()
        {
            Scribe_Values.Look(ref lastHitTime, "lastHitTime", -1);
            Scribe_Values.Look(ref expToGive, "expToGive", 0);
            Scribe_Collections.Look(ref giveTo, "giveTo", LookMode.Reference);
        }
    }
}
