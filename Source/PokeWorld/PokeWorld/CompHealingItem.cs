using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PokeWorld
{
    class CompHealingItem : ThingComp
    {

        public CompProperties_HealingItem Props => (CompProperties_HealingItem)this.props;
        public float healingAmount => Props.healingAmount;
    }

    class CompProperties_HealingItem : CompProperties
    {
        public float healingAmount = 20;
        public CompProperties_HealingItem()
        {
            this.compClass = typeof(CompHealingItem);
        }

        public CompProperties_HealingItem(Type compClass) : base(compClass)
        {
            this.compClass = compClass;
        }
    }
}
