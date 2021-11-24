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
		private const int HeatPushInterval = 60;

		public override void CompTick()
		{
			base.CompTick();
			if (parent.IsHashIntervalTick(60) && ShouldPushHeatNow)
			{
				CompPokemon comp = parent.TryGetComp<CompPokemon>();
				if (comp != null)
				{
					GenTemperature.PushHeat(parent.PositionHeld, parent.MapHeld, Props.heatPerSecond * Mathf.Sqrt(comp.levelTracker.level) / 2);
				}
			}
		}

		public override void CompTickRare()
		{
			base.CompTickRare();
			if (ShouldPushHeatNow)
			{
				CompPokemon comp = parent.TryGetComp<CompPokemon>();
				if (comp != null)
				{
					GenTemperature.PushHeat(parent.PositionHeld, parent.MapHeld, Props.heatPerSecond * Mathf.Sqrt(comp.levelTracker.level) / 2);
				}
			}
		}
	}
}