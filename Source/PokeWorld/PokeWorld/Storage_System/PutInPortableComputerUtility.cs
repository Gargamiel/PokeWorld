using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace PokeWorld
{
    public static class PutInPortableComputerUtility
    {
        public static void UpdatePutInPortableComputerDesignation(ThingWithComps t)
        {
            CryptosleepBall ball = t as CryptosleepBall;
            Designation designation = t.Map.designationManager.DesignationOn(t, DefDatabase<DesignationDef>.GetNamed("PW_PutInPortableComputer"));
            if(ball == null || !(ball.ContainedThing is Pawn pawn) || pawn.Faction != Faction.OfPlayer)
            {
                Messages.Message("PW_CantStoreBallInPCWarning".Translate(), ball, MessageTypeDefOf.RejectInput);
            }
            else if (ball != null && designation == null)
            {
                ball.wantPutInPortableComputer = true;
                t.Map.designationManager.AddDesignation(new Designation(t, DefDatabase<DesignationDef>.GetNamed("PW_PutInPortableComputer")));
            }
            else if (ball != null)
            {
                ball.wantPutInPortableComputer = false;
                designation?.Delete();
            }
        }
    }
}
