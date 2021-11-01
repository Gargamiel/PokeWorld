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
    public class TypeDef : Def
    {
        public List<TypeDef> resistances;
        public List<TypeDef> weaknesses;
        public List<TypeDef> immunities;
        public string gizmoIconPath;
        public float superEffectiveFactor = 2;
        public float notVeryEffeciveFactor = 0.5f;
        public float immuneFactor = 0;
        [NoTranslate]
        public string uiIconPath;
        [Unsaved(false)]
        public Texture2D uiIcon = BaseContent.BadTex;
        [Unsaved(false)]
        public Texture2D gizmoIcon = BaseContent.BadTex;
        public override void PostLoad()
        {
            base.PostLoad();
            LongEventHandler.ExecuteWhenFinished(delegate
            {
                uiIcon = ContentFinder<Texture2D>.Get(uiIconPath);                              
            });
        }

        public float GetDamageMultiplier(TypeDef attackType)
        {
            if(resistances != null)
            {
                foreach (TypeDef def in resistances)
                {
                    if (attackType == def)
                    {
                        return notVeryEffeciveFactor;
                    }
                }
            }
            if (weaknesses != null)
            {
                foreach (TypeDef def in weaknesses)
                {
                    if (attackType == def)
                    {
                        return superEffectiveFactor;
                    }
                }
            }
            if (immunities != null)
            {
                foreach (TypeDef def in immunities)
                {
                    if (attackType == def)
                    {
                        return immuneFactor;
                    }
                }
            }
            return 1;
        }
    }
}
