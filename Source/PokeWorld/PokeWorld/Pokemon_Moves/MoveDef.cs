using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    public class MoveDef : Def
    {
        public TypeDef type;
        public float accuracy;
        public VerbProperties verb;
        public Tool tool;
        public bool IsStab(Pawn pawn)
        {
            CompPokemon comp = pawn.TryGetComp<CompPokemon>();
            if (comp != null)
            {
                if (comp.types.Contains(type))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
