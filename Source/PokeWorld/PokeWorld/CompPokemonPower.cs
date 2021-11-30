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
    public class CompPokemonPower : CompPowerTrader
    {
        private float maxDistance = 7f;
        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            PowerOn = true;
        }
        public override void CompTick()
        {
            base.CompTick();                        
            if(parent is Pawn pawn && pawn.Spawned && !pawn.Dead && pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                PowerOutput = (pawn.Starving() ? 1f : -1f) * (Props.basePowerConsumption * Mathf.Sqrt(pawn.TryGetComp<CompPokemon>().levelTracker.level) / 2);
                if (connectParent == null)
                {
                    PowerConnectionMaker.TryConnectToAnyPowerNet(this);
                }
                else if(connectParent != null && IntVec3Utility.DistanceTo(parent.Position, connectParent.parent.Position) > maxDistance)
                {
                    PowerConnectionMaker.DisconnectFromPowerNet(this);
                }
            }            
        }
        public override string CompInspectStringExtra()
        {
            if(parent is Pawn pawn && pawn.Spawned && !pawn.Dead && pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                string text = (PowerOutput < 0) ? ((string)("PowerNeeded".Translate() + ": " + (0f - PowerOutput).ToString("#####0") + " W")) : ((string)("PowerOutput".Translate() + ": " + PowerOutput.ToString("#####0") + " W"));
                if (connectParent == null)
                {
                    text += " (" + "PowerNotConnected".Translate().Replace(".", "") + ")";
                }
                return text;
            }
            return null;
        }
    }
}
