using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;

namespace PokeWorld
{
	[HarmonyPatch(typeof(ThingOwnerUtility))]
	[HarmonyPatch("ContentsSuspended")]
	class ThingOwnerUtility_ContentsSuspended_Patch
	{
		public static bool Prefix(IThingHolder __0, ref bool __result)
		{
			while (__0 != null)
			{
				if (__0 is Building_CryptosleepCasket || __0 is ImportantPawnComp || __0 is CryptosleepBall || __0 is StorageSystem)
				{
					__result = true;
					return false;
				}
				__0 = __0.ParentHolder;
			}
			__result = false;
			return false;
		}
	}
}
