using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    public class StatTracker : IExposable
    {
        public CompPokemon comp;
        public Pawn pokemonHolder;

        public int hpStat;
        public int attackStat;
        public int defenseStat;
        public int attackSpStat;
        public int defenseSpStat;
        public int speedStat;

        public float healthScaleMult = 1;

        public NatureDef nature;

        private int hpIV;
        private int attackIV;
        private int defenseIV;
        private int attackSpIV;
        private int defenseSpIV;
        private int speedIV;

        private int TotalEV
        {
            get
            {
                return hpEV + attackEV + defenseEV + attackSpEV + defenseSpEV + speedEV;
            }
        }

        private int hpEV;
        private int attackEV;
        private int defenseEV;
        private int attackSpEV;
        private int defenseSpEV;
        private int speedEV;

        public StatTracker(CompPokemon comp)
        {
            this.comp = comp;
            pokemonHolder = comp.Pokemon;
            nature = DefDatabase<NatureDef>.GetRandom();
            RandomizeIV();
            SetAllEVZero();
        }
        public void RandomizeIV()
        {
            hpIV = Rand.RangeInclusive(0, 31);
            attackIV = Rand.RangeInclusive(0, 31);
            defenseIV = Rand.RangeInclusive(0, 31);
            attackSpIV = Rand.RangeInclusive(0, 31);
            defenseSpIV = Rand.RangeInclusive(0, 31);
            speedIV = Rand.RangeInclusive(0, 31);
        }
        public void SetAllEVZero()
        {
            hpEV = 0;
            attackEV = 0;
            defenseEV = 0;
            attackSpEV = 0;
            defenseSpEV = 0;
            speedEV = 0;
        }
        private void Increase(ref int ev, int amount)
        {
            if (ev + amount > 252)
            {
                ev = 252;
            }
            else
            {
                ev += amount;
            }
        }
        public void IncreaseEV(StatDef stat, int amount)
        {
            if(TotalEV >= 510)
            {
                return;
            }
            else if(TotalEV + amount > 510)
            {
                amount = 510 - TotalEV;
            }
            switch (stat.defName)
            {
                case "PW_HP":
                    Increase(ref hpEV, amount);
                    break;
                case "PW_Attack":
                    Increase(ref attackEV, amount);
                    break;
                case "PW_Defense":
                    Increase(ref defenseEV, amount);
                    break;
                case "PW_SpecialAttack":
                    Increase(ref attackSpEV, amount);
                    break;
                case "PW_SpecialDefense":
                    Increase(ref defenseSpEV, amount);
                    break;
                case "PW_Speed":
                    Increase(ref speedEV, amount);
                    break;
                default:
                    break;
            }
        }
        public int GetIV(StatDef stat)
        {
            int value;
            switch (stat.defName)
            {
                case "PW_HP":
                    value = hpIV;
                    break;
                case "PW_Attack":
                    value = attackIV;
                    break;
                case "PW_Defense":
                    value = defenseIV;
                    break;
                case "PW_SpecialAttack":
                    value = attackSpIV;
                    break;
                case "PW_SpecialDefense":
                    value = defenseSpIV;
                    break;
                case "PW_Speed":
                    value = speedIV;
                    break;
                default:
                    value = 0;
                    break;
            }
            return value;
        }
        public int GetEV(StatDef stat)
        {
            int value;
            switch (stat.defName)
            {
                case "PW_HP":
                    value = hpEV;
                    break;
                case "PW_Attack":
                    value = attackEV;
                    break;
                case "PW_Defense":
                    value = defenseEV;
                    break;
                case "PW_SpecialAttack":
                    value = attackSpEV;
                    break;
                case "PW_SpecialDefense":
                    value = defenseSpEV;
                    break;
                case "PW_Speed":
                    value = speedEV;
                    break;
                default:
                    value = 0;
                    break;
            }
            return value;
        }
        public void UpdateStats()
        {
            hpStat = (int)StatExtension.GetStatValue(pokemonHolder, DefDatabase<StatDef>.GetNamed("PW_HP"));
            attackStat = (int)StatExtension.GetStatValue(pokemonHolder, DefDatabase<StatDef>.GetNamed("PW_Attack"));
            defenseStat = (int)StatExtension.GetStatValue(pokemonHolder, DefDatabase<StatDef>.GetNamed("PW_Defense"));
            attackSpStat = (int)StatExtension.GetStatValue(pokemonHolder, DefDatabase<StatDef>.GetNamed("PW_SpecialAttack"));
            defenseSpStat = (int)StatExtension.GetStatValue(pokemonHolder, DefDatabase<StatDef>.GetNamed("PW_SpecialDefense"));
            speedStat = (int)StatExtension.GetStatValue(pokemonHolder, DefDatabase<StatDef>.GetNamed("PW_Speed"));
            healthScaleMult = hpStat / 60f;
            /*
            foreach(Tool tool in pokemonHolder.Tools)
            {
                tool.power = attackStat / 5f;
                tool.cooldownTime = (float)Math.Pow(attackStat + speedStat + 10, -0.3f) * 7;
            }   
            */
        }
        public void ExposeData()
        {
            Scribe_Values.Look(ref hpStat, "PW_hpStat", 1);
            Scribe_Values.Look(ref attackStat, "PW_attackStat", 1);
            Scribe_Values.Look(ref defenseStat, "PW_defenseStat", 1);
            Scribe_Values.Look(ref attackSpStat, "PW_attackSpStat", 1);
            Scribe_Values.Look(ref defenseSpStat, "PW_defenseSpStat", 1);
            Scribe_Values.Look(ref speedStat, "PW_speedStat", 1);

            Scribe_Values.Look(ref hpIV, "PW_hpIV", 0);
            Scribe_Values.Look(ref attackIV, "PW_attackIV", 0);
            Scribe_Values.Look(ref defenseIV, "PW_defenseIV", 0);
            Scribe_Values.Look(ref attackSpIV, "PW_attackSpIV", 0);
            Scribe_Values.Look(ref defenseSpIV, "PW_defenseSpIV", 0);
            Scribe_Values.Look(ref speedIV, "PW_speedIV", 0);

            Scribe_Values.Look(ref hpEV, "PW_hpEV", 0);
            Scribe_Values.Look(ref attackEV, "PW_attackEV", 0);
            Scribe_Values.Look(ref defenseEV, "PW_defenseEV", 0);
            Scribe_Values.Look(ref attackSpEV, "PW_attackSpEV", 0);
            Scribe_Values.Look(ref defenseSpEV, "PW_defenseSpEV", 0);
            Scribe_Values.Look(ref speedEV, "PW_speedEV", 0);

            Scribe_Values.Look(ref healthScaleMult, "PW_healthScaleMult", 1);
            Scribe_Defs.Look(ref nature, "PW_nature");
        }
    }
}
