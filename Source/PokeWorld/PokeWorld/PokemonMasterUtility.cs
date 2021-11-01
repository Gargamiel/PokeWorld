using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace PokeWorld
{
    public static class PokemonMasterUtility
    {
        public static float GetMasterObedienceRadius(Pawn pokemon)
        {
            Pawn master = pokemon.playerSettings.Master;
            if (master.skills != null && !master.skills.GetSkill(SkillDefOf.Animals).TotallyDisabled)
            {
                return 5.9f + master.skills.GetSkill(SkillDefOf.Animals).Level;
            }
            return 0;
        }
        public static void DrawObedienceRadiusRingAroundMaster(Pawn pokemon)
        {
            Pawn master = pokemon.playerSettings.Master;
            if (Find.CurrentMap == null || master == null)
            {
                return;
            }
            float num = GetMasterObedienceRadius(pokemon);
            if (num > 0f && num < GenRadial.MaxRadialPatternRadius)
            {
                GenDraw.DrawRadiusRing(master.Position, num, Color.blue);
            }
        }
        public static bool IsPokemonMasterDrafted(Pawn pokemon)
        {
            if(pokemon.playerSettings != null)
            {
                Pawn master = pokemon.playerSettings.Master;
                if (pokemon.Faction != null && pokemon.Faction.IsPlayer && master != null && master.Drafted && master.Map == pokemon.Map)
                {
                    return true;
                }
            }        
            return false;
        }
        public static bool IsPokemonInMasterRange(Pawn pokemon)
        {
            if (IsPokemonMasterDrafted(pokemon))
            {
                if(IntVec3Utility.DistanceTo(pokemon.Position, pokemon.playerSettings.Master.Position) <= GetMasterObedienceRadius(pokemon))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
