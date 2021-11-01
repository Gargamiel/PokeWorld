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
    public class MoveTracker : IExposable
    {
        public CompPokemon comp;
        public Pawn pokemonHolder;
        public Dictionary<MoveDef, int> unlockableMoves;
        public Dictionary<MoveDef, bool> wantedMoves;
        public MoveDef lastUsedMove;
        private List<Verb> initializedVerbs;

        public MoveTracker(CompPokemon comp)
        {
            this.comp = comp;
            pokemonHolder = (Pawn)comp.parent;
            unlockableMoves = new Dictionary<MoveDef, int>();
            wantedMoves = new Dictionary<MoveDef, bool>();
            initializedVerbs = new List<Verb>();
            foreach (Move move in comp.moves)
            {
                unlockableMoves.Add(move.moveDef, move.unlockLevel);
                wantedMoves.Add(move.moveDef, true);
            }
            OrderMoves();
        }
               
        public IEnumerable<Gizmo> GetGizmos()
        {
            if (PokemonMasterUtility.IsPokemonMasterDrafted(pokemonHolder))
            {
                foreach (Gizmo attackGizmo in PokemonAttackGizmoUtility.GetAttackGizmos(pokemonHolder))
                {
                    yield return attackGizmo;
                }          
            }
        }
        public bool HasUnlocked(MoveDef moveDef)
        {
            if (unlockableMoves.TryGetValue(moveDef, out int unlockLevel))
            {
                if (unlockLevel <= comp.levelTracker.level)
                {
                    return true;
                }             
            }
            return false;
        }
        public bool GetWanted(MoveDef moveDef)
        {
            if (wantedMoves.ContainsKey(moveDef))
            {
                return wantedMoves[moveDef];
            }
            return false;
        }
        public void SetWanted(MoveDef moveDef, bool wanted)
        {
            if (wantedMoves.ContainsKey(moveDef))
            {
                wantedMoves[moveDef] = wanted;
            }
        }
        
        public void ExposeData()
        {
            Scribe_Collections.Look(ref unlockableMoves, "unlockableMoves", LookMode.Def, LookMode.Value);
            Scribe_Collections.Look(ref wantedMoves, "wantedMoves", LookMode.Def, LookMode.Value);
            Scribe_Collections.Look(ref initializedVerbs, "initializedVerbs", LookMode.Deep);
            Scribe_Defs.Look(ref lastUsedMove, "lastUsedMove");
        }
        public void GetUnlockedMovesFromPreEvolution(CompPokemon preEvoComp)
        {
            foreach(KeyValuePair<MoveDef, int> kvp in preEvoComp.moveTracker.unlockableMoves)
            {
                if (kvp.Key == DefDatabase<MoveDef>.GetNamed("Struggle"))
                {
                    continue;
                }
                if (preEvoComp.moveTracker.HasUnlocked(kvp.Key))
                {
                    if (unlockableMoves.Keys.Contains(kvp.Key))
                    {
                        unlockableMoves.Remove(kvp.Key);
                        wantedMoves.Remove(kvp.Key);
                    }
                    unlockableMoves.Add(kvp.Key, kvp.Value);
                    wantedMoves.Add(kvp.Key, preEvoComp.moveTracker.wantedMoves[kvp.Key]);
                }           
            }
            OrderMoves();
        }
        private void OrderMoves()
        {
            List<KeyValuePair<MoveDef, int>> myList = unlockableMoves.ToList();
            myList.Sort(
                delegate (KeyValuePair<MoveDef, int> pair1,
                KeyValuePair<MoveDef, int> pair2)
                {
                    return pair1.Value.CompareTo(pair2.Value);
                }
            );
            unlockableMoves = myList.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
