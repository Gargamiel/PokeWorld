using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.Sound;

namespace PokeWorld
{
	public class Command_PokemonVerbTarget : Command_Target
	{
		public Verb verb;

		public bool drawRadius = true;

		public override void GizmoUpdateOnMouseover()
		{
			if (!drawRadius)
			{
				return;
			}
			verb.verbProps.DrawRadiusRing(verb.caster.Position);
		}
	}
}
