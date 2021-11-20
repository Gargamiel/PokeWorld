using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;

namespace PokeWorld
{
    [HarmonyPatch(typeof(Pawn_TrainingTracker))]
    [HarmonyPatch("Train")]
    public class Pawn_TrainingTracker_Train_Patch
    {
        public static bool Prefix(TrainableDef __0, Pawn __1, Pawn_TrainingTracker __instance)
        {           
            if(__0 == DefDatabase<TrainableDef>.GetNamed("PW_TrainXp"))
            {
                CompPokemon comp = __instance.pawn.TryGetComp<CompPokemon>();
                if(comp != null && __1 != null && !__1.skills.GetSkill(SkillDefOf.Animals).TotallyDisabled)
                {
                    int skillValue = __1.skills.GetSkill(SkillDefOf.Animals).Level;
                    int expAmount = 10 + (skillValue * 20) + (int)Mathf.Lerp(0, comp.levelTracker.totalExpForNextLevel / 5f, skillValue / 20f);
                    expAmount = Mathf.Clamp(expAmount, 0, comp.levelTracker.totalExpForNextLevel * 4);
                    comp.levelTracker.IncreaseExperience(expAmount);
                    comp.friendshipTracker.ChangeFriendship(2);
                }
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
