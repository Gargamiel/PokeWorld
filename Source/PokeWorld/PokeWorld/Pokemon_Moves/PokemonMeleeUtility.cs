using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;
using RimWorld;

namespace PokeWorld
{
    public static class PokemonMeleeUtility
    {
        public static Action GetPokemonMeleeAttackAction(Pawn pawn, LocalTargetInfo target, out string failStr)
        {
            failStr = "";
            if (target.IsValid && !pawn.CanReach(target, PathEndMode.Touch, Danger.Deadly))
            {
                failStr = "NoPath".Translate();
            }
            else if (pawn.meleeVerbs.TryGetMeleeVerb(target.Thing) == null)
            {
                failStr = "Incapable".Translate();
            }
            else if (pawn == target.Thing)
            {
                failStr = "CannotAttackSelf".Translate();
            }
            else if (IntVec3Utility.DistanceTo(pawn.playerSettings.Master.Position,target.Cell)>20)
            {
                failStr = "PW_WarningTargetTooFarFromMaster".Translate();
            }
            else
            {
                Pawn target2;
                if ((target2 = target.Thing as Pawn) == null || (!pawn.InSameExtraFaction(target2, ExtraFactionType.HomeFaction) && !pawn.InSameExtraFaction(target2, ExtraFactionType.MiniFaction)))
                {
                    return delegate
                    {
                        Job job = JobMaker.MakeJob(JobDefOf.AttackMelee, target);
                        Pawn pawn2 = target.Thing as Pawn;
                        if (pawn2 != null)
                        {
                            job.killIncappedTarget = pawn2.Downed;
                        }
                        pawn.jobs.TryTakeOrderedJob(job);
                    };
                }
                failStr = "CannotAttackSameFactionMember".Translate();
            }
            failStr = failStr.CapitalizeFirst();
            return null;
        }
    }
}
