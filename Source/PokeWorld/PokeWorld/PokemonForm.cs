using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    public enum FormChangerCondition
    {
        Fixed,
        Selectable,
        Environment
    }
    
    public class PokemonForm
    {
        public List<WeatherDef> includeWeathers;
        public List<WeatherDef> excludeWeathers;
        public TimeOfDay timeOfDay = TimeOfDay.Any;
        public List<BiomeDef> includeBiomes;
        public List<BiomeDef> excludeBiomes;
        public float weight = 1;
        public bool isDefault = false;
        [NoTranslate]
        public string texPathKey;
        public string label;
        public TypeDef type1;
        public TypeDef type2;
        public int baseHP;
        public int baseAttack;
        public int baseDefense;
        public int baseSpAttack;
        public int baseSpDefense;
        public int baseSpeed;
    }
}
