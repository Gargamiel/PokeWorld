using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    public class NatureDef : Def
    {
        public StatDef increasedStat;
        public StatDef decreasedStat;
        public float increaseMult = 1.1f;
        public float decreaseMult = 0.9f;
        public float GetMultiplier(string statDefName)
        {
            if(increasedStat != decreasedStat)
            {
                if (statDefName == increasedStat.defName)
                {
                    return increaseMult;
                }
                else if (statDefName == decreasedStat.defName)
                {
                    return decreaseMult;
                }
            }
            return 1;
        }
    }

}
