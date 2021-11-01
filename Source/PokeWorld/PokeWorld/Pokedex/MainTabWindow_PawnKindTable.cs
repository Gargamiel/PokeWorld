using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace PokeWorld
{
	public abstract class MainTabWindow_PawnKindTable : MainTabWindow
	{
		private PawnKindTable table;

		protected virtual float ExtraBottomSpace => 53f;

		protected virtual float ExtraTopSpace => 0f;

		protected abstract PawnKindTableDef PawnKindTableDef
		{
			get;
		}

		protected override float Margin => 6f;

		public override Vector2 RequestedTabSize
		{
			get
			{
				if (table == null)
				{
					return Vector2.zero;
				}
				return new Vector2(table.Size.x + Margin * 2f, table.Size.y + ExtraBottomSpace + ExtraTopSpace + Margin * 2f);
			}
		}

		protected virtual IEnumerable<PawnKindDef> PawnKindDefs => DefDatabase<PawnKindDef>.AllDefs.Where((PawnKindDef x) => x.race.HasComp(typeof(CompPokemon)));

		public override void PostOpen()
		{
			if (table == null)
			{
				table = CreateTable();
			}
			SetDirty();
		}

		public override void DoWindowContents(Rect rect)
		{
			table.PawnKindTableOnGUI(new Vector2(rect.x, rect.y + ExtraTopSpace));
		}

		public void Notify_PawnsChanged()
		{
			SetDirty();
		}

		public override void Notify_ResolutionChanged()
		{
			table = CreateTable();
			base.Notify_ResolutionChanged();
		}

		private PawnKindTable CreateTable()
		{
			return (PawnKindTable)Activator.CreateInstance(PawnKindTableDef.workerClass, PawnKindTableDef, (Func<IEnumerable<PawnKindDef>>)(() => PawnKindDefs), UI.screenWidth - (int)(Margin * 2f), (int)((float)(UI.screenHeight - 35) - ExtraBottomSpace - ExtraTopSpace - Margin * 2f));
		}

		protected void SetDirty()
		{
			table.SetDirty();
			SetInitialSizeAndPosition();
		}
	}
}
