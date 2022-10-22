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
        private float maxDistance = 7.5f;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            PowerOn = true;
            base.PostSpawnSetup(respawningAfterLoad);
        }

        public override void CompTick()
        {
            base.CompTick();                        
            if(parent is Pawn pawn && pawn.Spawned && !pawn.Dead && pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                if (!PowerOn)
                {
                    PowerOn = true;
                }
                PowerOutput = (pawn.Starving() ? 1f : -1f) * (Props.PowerConsumption * Mathf.Sqrt(pawn.TryGetComp<CompPokemon>().levelTracker.level) / 2);
                if (PowerNet == null && connectParent == null)
                {
                    parent.Map.powerNetManager.Notify_ConnectorWantsConnect(this);
                }
                else if(PowerNet != null && connectParent != null)
                {
                    if (IntVec3Utility.DistanceTo(parent.Position, connectParent.parent.Position) > maxDistance + connectParent.parent.def.Size.Magnitude)
                    {
                        parent.Map.powerNetManager.Notify_ConnectorDespawned(this);
                    }
                }
                else if(PowerNet != null && connectParent == null)
                {
                    PowerNet.DeregisterConnector(this);
                }
                else if (PowerNet == null && connectParent != null)
                {
                    if (connectParent.connectChildren != null)
                    {
                        connectParent.connectChildren.Remove(this);
                        if (connectParent.connectChildren.Count == 0)
                        {
                            connectParent.connectChildren = null;
                        }
                    }
                    connectParent = null;
                }
            }            
        }
        public override void ResetPowerVars()
        {
            base.ResetPowerVars();
            if(parent is Pawn pawn && pawn.Spawned && !pawn.Dead && pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                PowerOutput = (pawn.Starving() ? 1f : -1f) * (Props.PowerConsumption * Mathf.Sqrt(pawn.TryGetComp<CompPokemon>().levelTracker.level) / 2);         
            }       
        }
        public override void SetUpPowerVars()
        {
            base.SetUpPowerVars();
            if (parent is Pawn pawn && pawn.Spawned && !pawn.Dead && pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                PowerOutput = (pawn.Starving() ? 1f : -1f) * (Props.PowerConsumption * Mathf.Sqrt(pawn.TryGetComp<CompPokemon>().levelTracker.level) / 2);
            }
        }
        public override string CompInspectStringExtra()
        {
            if(parent is Pawn pawn && pawn.Spawned && !pawn.Dead && pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                string text = (PowerOutput < 0) ? ((string)("PowerNeeded".Translate() + ": " + (0f - PowerOutput).ToString("#####0") + " W")) : ((string)("PowerOutput".Translate() + ": " + PowerOutput.ToString("#####0") + " W"));
                if (PowerNet == null)
                {
                    text += " (" + "PowerNotConnected".Translate().Replace(".", "") + ")";
                }
                return text;
            }
            return null;
        }       
    }
}
