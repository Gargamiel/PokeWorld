﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using UnityEngine;
using Verse;

namespace PokeWorld
{
	public class PawnKindColumnDef : Def
	{
		public Type workerClass = typeof(PawnKindColumnWorker);

		public bool sortable;

		public bool ignoreWhenCalculatingOptimalTableSize;

		[NoTranslate]
		public string headerIcon;

		public Vector2 headerIconSize;

		[MustTranslate]
		public string headerTip;

		public bool headerAlwaysInteractable;

		public bool paintable;

		public TrainableDef trainable;

		public int gap;

		public WorkTypeDef workType;

		public bool moveWorkTypeLabelDown;

		public int widthPriority;

		public int width = -1;

		[Unsaved(false)]
		private PawnKindColumnWorker workerInt;

		[Unsaved(false)]
		private Texture2D headerIconTex;

		private const int IconWidth = 26;

		private static readonly Vector2 IconSize = new Vector2(26f, 26f);

		public PawnKindColumnWorker Worker
		{
			get
			{
				if (workerInt == null)
				{
					workerInt = (PawnKindColumnWorker)Activator.CreateInstance(workerClass);
					workerInt.def = this;
				}
				return workerInt;
			}
		}

		public Texture2D HeaderIcon
		{
			get
			{
				if (headerIconTex == null && !headerIcon.NullOrEmpty())
				{
					headerIconTex = ContentFinder<Texture2D>.Get(headerIcon);
				}
				return headerIconTex;
			}
		}

		public Vector2 HeaderIconSize
		{
			get
			{
				if (headerIconSize != default(Vector2))
				{
					return headerIconSize;
				}
				if (HeaderIcon != null)
				{
					return IconSize;
				}
				return Vector2.zero;
			}
		}

		public bool HeaderInteractable
		{
			get
			{
				if (!sortable && headerTip.NullOrEmpty())
				{
					return headerAlwaysInteractable;
				}
				return true;
			}
		}
	}
}
