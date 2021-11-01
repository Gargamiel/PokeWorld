using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    public static class PutInBallUtility
    {
        public static void UpdatePutInBallDesignation(ThingWithComps t)
        {
            CompPokemon comp = t.TryGetComp<CompPokemon>();
            Designation designation = t.Map.designationManager.DesignationOn(t, DefDatabase<DesignationDef>.GetNamed("PW_PutInBall"));
            if (comp != null && designation == null)
            {
                comp.wantPutInBall = true;
                t.Map.designationManager.AddDesignation(new Designation(t, DefDatabase<DesignationDef>.GetNamed("PW_PutInBall")));
            }
            else if(comp != null)
            {
                comp.wantPutInBall = false;
                designation?.Delete();
            }
        }

        public static void PutInBall(Pawn pokemon)
        {
            CompPokemon comp = pokemon.TryGetComp<CompPokemon>();
            if (comp != null)
            {
                comp.wantPutInBall = false;
                if (comp.levelTracker.flagIsEvolving)
                {
                    comp.levelTracker.CancelEvolution();
                }
                if (pokemon.carryTracker != null && pokemon.carryTracker.CarriedThing != null)
                {
                    pokemon.carryTracker.TryDropCarriedThing(pokemon.Position, ThingPlaceMode.Near, out Thing droppedThing);
                }
                if (pokemon.inventory != null)
                {
                    pokemon.inventory.DropAllNearPawn(pokemon.Position);
                }
                IntVec3 pos = pokemon.Position;
                Map map = pokemon.Map;
                pokemon.DeSpawn();
                comp.inBall = true;
                Thing thing = ThingMaker.MakeThing(comp.ballDef);
                CryptosleepBall ball = thing as CryptosleepBall;
                ball.stackCount = 1;
                ball.TryAcceptThing(pokemon);
                ball.SetFaction(pokemon.Faction);
                GenPlace.TryPlaceThing(ball, pos, map, ThingPlaceMode.Near);                            
            }          
        }
    }
}
