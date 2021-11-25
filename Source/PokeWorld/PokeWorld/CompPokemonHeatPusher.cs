using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;

namespace PokeWorld
{
	class CompPokemonHeatPusher : CompHeatPusher
	{
		public override void CompTickRare()
		{
			if (parent != null && parent is Pawn && parent.Spawned && ShouldPushHeatNow)
			{
				CompPokemon comp = parent.TryGetComp<CompPokemon>();
				if (comp != null)
				{
					GenTemperature.PushHeat(parent.PositionHeld, parent.MapHeld, Props.heatPerSecond * 4.16666651f * Mathf.Sqrt(comp.levelTracker.level) / 2);
				}
			}
		}
	}
}