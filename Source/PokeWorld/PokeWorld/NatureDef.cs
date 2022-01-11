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
        public float GetMultiplier(StatDef stat)
        {
            if(increasedStat != null && decreasedStat != null && increasedStat != decreasedStat)
            {
                if (stat == increasedStat)
                {
                    return increaseMult;
                }
                else if (stat == decreasedStat)
                {
                    return decreaseMult;
                }
            }
            return 1;
        }
    }

}
