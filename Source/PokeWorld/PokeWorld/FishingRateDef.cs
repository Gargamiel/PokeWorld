using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using System.Xml;

namespace PokeWorld
{
    public class FishingRateDef : Def
    {

        private List<FishingRateBiomeRecord> biomes = new List<FishingRateBiomeRecord>();
        private Dictionary<PawnKindDef, float> rateTable;
        public PawnKindDef getRandomFish(BiomeDef biome, TerrainDef terrain, ThingDef rod)
        {
            List<FishingRatePokemonRecord> rates = biomes.Where((FishingRateBiomeRecord b) => b.biome == biome).First().terrains.Where((FishingRateTerrainRecord t) => t.terrain == terrain).First().rods.Where((FishingRateRodRecord r) => r.rod == rod).First().pokemons;

            rateTable = new Dictionary<PawnKindDef, float>();
            for (int i = 0; i < rates.Count; i++)
            {
                if (rates[i].pokemon != null)
                {
                    rateTable.Add(rates[i].pokemon, rates[i].rate);
                }
            }

            return rateTable.Keys.RandomElementByWeight((PawnKindDef def) => rateTable[def]);
        }
        public void LoadDataFromXmlCustom(XmlNode xmlRoot)
        {
            biomes = DirectXmlToObject.ObjectFromXml<List<FishingRateBiomeRecord>>(xmlRoot.SelectSingleNode("biomes"), false);
        }

        public class FishingRateBiomeRecord
        {
            public BiomeDef biome;
            public List<FishingRateTerrainRecord> terrains = new List<FishingRateTerrainRecord>();
            public void LoadDataFromXmlCustom(XmlNode xmlRoot)
            {
                DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "biome", xmlRoot.Name);
                terrains = DirectXmlToObject.ObjectFromXml<List<FishingRateTerrainRecord>>(xmlRoot, false);
            }
        }

        public class FishingRateTerrainRecord
        {
            public TerrainDef terrain;
            public List<FishingRateRodRecord> rods = new List<FishingRateRodRecord>();
            public void LoadDataFromXmlCustom(XmlNode xmlRoot)
            {
                DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "terrain", xmlRoot.Name);
                rods = DirectXmlToObject.ObjectFromXml<List<FishingRateRodRecord>>(xmlRoot, false);
            }
        }

        public class FishingRateRodRecord
        {
            public ThingDef rod;
            public List<FishingRatePokemonRecord> pokemons = new List<FishingRatePokemonRecord>();

            public void LoadDataFromXmlCustom(XmlNode xmlRoot)
            {
                DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "rod", xmlRoot.Name);
                pokemons = DirectXmlToObject.ObjectFromXml<List<FishingRatePokemonRecord>>(xmlRoot, false);
            }
        }

        public class FishingRatePokemonRecord
        {
            public PawnKindDef pokemon;
            public float rate;

            public void LoadDataFromXmlCustom(XmlNode xmlRoot)
            {
                DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(this, "pokemon", xmlRoot);
                rate = ParseHelper.FromString<float>(xmlRoot.FirstChild.Value);
            }
        }
    }
}
