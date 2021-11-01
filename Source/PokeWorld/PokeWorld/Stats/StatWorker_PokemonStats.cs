using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    class StatWorker_PokemonStats : StatWorker
    {
        public override bool ShouldShowFor(StatRequest req)
        {
            if (!base.ShouldShowFor(req))
            {
                return false;
            }
            if (req.HasThing && req.Thing.TryGetComp<CompPokemon>() != null)
            {
                return true;
            }
            return false;
        }
        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (req.HasThing && req.Thing.TryGetComp<CompPokemon>() != null)
            {
                float value;
                switch (stat.defName)
                {
                    case "PW_HP":
                        value = req.Thing.TryGetComp<CompPokemon>().baseHP;
                        break;
                    case "PW_Attack":
                        value = req.Thing.TryGetComp<CompPokemon>().baseAttack;
                        break;
                    case "PW_Defense":
                        value = req.Thing.TryGetComp<CompPokemon>().baseDefense;
                        break;
                    case "PW_SpecialAttack":
                        value = req.Thing.TryGetComp<CompPokemon>().baseSpAttack;
                        break;
                    case "PW_SpecialDefense":
                        value = req.Thing.TryGetComp<CompPokemon>().baseSpDefense;
                        break;
                    case "PW_Speed":
                        value = req.Thing.TryGetComp<CompPokemon>().baseSpeed;
                        break;
                    default:
                        value = 0;
                        break;

                }
                return value;
            }
            return 0;
        }
        public override void FinalizeValue(StatRequest req, ref float val, bool applyPostProcess)
        {
            if (req.HasThing)
            {
                CompPokemon comp = req.Thing.TryGetComp<CompPokemon>();             
                if (comp != null)
                {
                    int IV = comp.statTracker.GetIV(stat);
                    int EV = comp.statTracker.GetEV(stat);
                    int level = comp.levelTracker.level;
                    if (stat.defName == "PW_HP")
                    {
                        val = (int)((2 * val + IV + (EV / 4)) * level / 100) + level + 10;
                    }
                    else
                    {
                        float natureMultiplier = GetNatureMultiplier(comp, stat.defName);
                        val = (int)((((2 * val + IV + (EV / 4)) * level / 100) + 5) * natureMultiplier);
                    }
                }
            }
        }
        
        public float GetNatureMultiplier(CompPokemon comp, string statDefName)
        {
            return comp.statTracker.nature.GetMultiplier(statDefName);
        }
        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            if (req.HasThing)
            {
                CompPokemon comp = req.Thing.TryGetComp<CompPokemon>();
                if(comp != null && comp.statTracker != null)
                {
                    return "PW_StatBaseValue".Translate(GetValueUnfinalized(req).ToString());
                }
            }
            return "";
        }
        public override string GetExplanationFinalizePart(StatRequest req, ToStringNumberSense numberSense, float finalVal)
        {
            if (req.HasThing)
            {
                CompPokemon comp = req.Thing.TryGetComp<CompPokemon>();
                if (comp != null && comp.statTracker != null)
                {
                    float value = GetValueUnfinalized(req);
                    FinalizeValue(req, ref value, true);
                    return "PW_StatFinalValue".Translate(value.ToString());
                }
            }
            return "";
        }
    }
}
