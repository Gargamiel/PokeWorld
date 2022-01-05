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
    public class Evolution
    {
        public PawnKindDef pawnKind;
        public EvolutionRequirement requirement;
        public OtherEvolutionRequirement otherRequirement = OtherEvolutionRequirement.none;

        public ThingDef item;            

        public int level = 1;                                
        public int friendship = 0;                           
        public TimeOfDay timeOfDay = TimeOfDay.Any;  
        public Gender gender = Gender.None;    

        public Evolution()
        {
        }        
    }
    public enum EvolutionRequirement
    {
        level = 0,
        item = 1
    }
    public enum OtherEvolutionRequirement
    {
        none = 0,
        attack = 1,
        defense = 2,
        balanced = 3
    }
}
