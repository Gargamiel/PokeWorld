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
    public class ShinyTracker : IExposable
    {
        public CompPokemon comp;
        public Pawn pokemonHolder;

        public bool femaleGraphicDifference;
        public GraphicData shinyGraphicData;
        public GraphicData femaleShinyGraphicData;
        public float shinyChance;
        public bool isShiny;
        public bool flagLetter = true;

        public ShinyTracker(CompPokemon comp)
        {
            this.comp = comp;
            pokemonHolder = comp.Pokemon;
            shinyChance = comp.shinyChance;
            TryMakeShiny();
        }
        public void TryMakeShiny()
        {
            float num = Rand.Range(0f, 1f);
            if (num < shinyChance)
            {
                MakeShiny();
            }
            else
            {
                isShiny = false;
            }
        }
        public void MakeShiny()
        {
            isShiny = true;           
        }
        public void ShinyTickRare()
        {
            if (isShiny)
            {
                if (flagLetter && pokemonHolder.Spawned && (pokemonHolder.Faction == null || pokemonHolder.Faction == Find.FactionManager.AllFactions.Where((Faction f) => f.def.defName == "PW_HostilePokemon").First()))
                {                  
                    Letter letter = LetterMaker.MakeLetter("PW_ShinyPokemonLetter".Translate(), "PW_ShinyPokemonLetterDesc".Translate(), LetterDefOf.PositiveEvent, pokemonHolder);
                    Find.LetterStack.ReceiveLetter(letter);
                    flagLetter = false;
                }
                TryMakeShinyMote();
            }
        }
        public void TryMakeShinyMote()
        {
            if (pokemonHolder.Spawned && !pokemonHolder.Position.Fogged(pokemonHolder.Map))
            {
                int num = Rand.RangeInclusive(2, 5);
                for(int i = 0; i < num; i++)
                {
                    ThrowShinyIcon(pokemonHolder.Position, pokemonHolder.Map, DefDatabase<ThingDef>.GetNamed("Mote_ShinyStar"));
                }             
            }
        }
        public void ExposeData()
        {
            Scribe_Values.Look(ref isShiny, "PW_isShiny");
            Scribe_Values.Look(ref flagLetter, "PW_flagLetter");
        }

        public static Mote ThrowShinyIcon(IntVec3 cell, Map map, ThingDef moteDef)
        {
            if (!cell.ShouldSpawnMotesAt(map) || map.moteCounter.Saturated)
            {
                return null;
            }
            MoteThrown obj = (MoteThrown)ThingMaker.MakeThing(moteDef);
            obj.Scale = 0.7f;
            obj.rotationRate = Rand.Range(-3f, 3f);
            obj.exactPosition = cell.ToVector3Shifted();
            obj.exactPosition += new Vector3(0.35f, 0f, 0.35f);
            obj.exactPosition += new Vector3(Rand.Value, 0f, Rand.Value) * 0.1f;
            obj.SetVelocity(Rand.Range(0, 360), 0.42f);
            GenSpawn.Spawn(obj, cell, map);
            return obj;
        }
    }
}
