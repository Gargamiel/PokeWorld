using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld.Planet;
using RimWorld;
using RimWorld.QuestGen;
using Verse;

namespace PokeWorld
{
    public class LegendaryQuestsTracker : WorldComponent
    {
        private DefMap<QuestConditionDef, bool> questConditionDefMap = new DefMap<QuestConditionDef, bool>();
        public LegendaryQuestsTracker(World world) : base(world)
        {

        }
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look(ref questConditionDefMap, "PW_questConditionDefMap");
        }
        public override void WorldComponentTick()
        {
            base.WorldComponentTick();
            if (Find.TickManager.TicksGame % 60000 == 0)
            {
                foreach (QuestConditionDef condition in DefDatabase<QuestConditionDef>.AllDefs)
                {                
                    if (!questConditionDefMap[condition])
                    {
                        if (condition.CheckCompletion())
                        {
                            Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(condition.questScriptDef, new Slate());
                            if (!quest.hidden && condition.questScriptDef.sendAvailableLetter)
                            {
                                QuestUtility.SendLetterQuestAvailable(quest);
                            }
                            questConditionDefMap[condition] = true;
                        }
                    }                   
                }             
            }
        }
    }
}
