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
    public class FormTracker : IExposable
    {
        public CompPokemon comp;
        public Pawn pokemonHolder;

        public int currentFormIndex = -1;

        public FormTracker(CompPokemon comp)
        {
            this.comp = comp;
            pokemonHolder = comp.Pokemon;
            if (comp.formChangerCondition == FormChangerCondition.Fixed)
            {
                ChangeIntoFixed(comp.forms.RandomElementByWeight((PokemonForm form) => form.weight));
            }
            else if(currentFormIndex == -1)
            {
                IEnumerable<PokemonForm> forms = comp.forms.Where((PokemonForm x) => x.isDefault);
                if (forms.Any())
                {
                    ChangeIntoFixed(forms.First());
                    return;
                }
            }                        
        }
        public bool TryInheritFormFromPreEvo(FormTracker preEvoFormTracker)
        {
            if(comp.formChangerCondition == FormChangerCondition.Fixed)
            {
                string key = preEvoFormTracker.GetCurrentFormKey();
                if(key != "")
                {
                    IEnumerable<PokemonForm> forms = comp.forms.Where((PokemonForm form) => form.texPathKey == key);
                    if (forms.Any())
                    {
                        ChangeIntoFixed(forms.First());
                        return true;
                    }
                }
            }
            return false;
        }
        public bool TryInheritFormFromParent(FormTracker parentFormTracker)
        {
            if (comp.formChangerCondition == FormChangerCondition.Fixed)
            {
                string key = parentFormTracker.GetCurrentFormKey();
                if (key != "")
                {
                    IEnumerable<PokemonForm> forms = comp.forms.Where((PokemonForm form) => form.texPathKey == key);
                    if (forms.Any())
                    {
                        ChangeIntoFixed(forms.First());
                        return true;
                    }
                }
            }
            return false;
        }
        public IEnumerable<Gizmo> GetGizmos()
        {
            if (pokemonHolder.Faction != null && pokemonHolder.Faction.IsPlayer)
            {
                if(comp.formChangerCondition == FormChangerCondition.Selectable)
                {
                    Command_Action command_Action = new Command_Action();
                    command_Action.action = delegate
                    {
                        ProcessInput();
                    };
                    if(currentFormIndex != -1)
                    {
                        command_Action.defaultLabel = "PW_FormName".Translate(comp.forms[currentFormIndex].label);
                    }
                    else
                    {
                        command_Action.defaultLabel = "PW_BaseForm".Translate();
                    }
                    command_Action.defaultDesc = "PW_ChangeForm".Translate();
                    command_Action.hotKey = KeyBindingDefOf.Misc3;
                    command_Action.icon = ContentFinder<Texture2D>.Get(pokemonHolder.Drawer.renderer.graphics.nakedGraphic.path + "_east");
                    yield return command_Action;
                }
            }
        }
        private bool CanUseForm(PokemonForm form)
        {
            if (comp.forms.Contains(form) && CheckTimeOfDay(form) && CheckWeather(form) && CheckBiome(form))
            {
                return true;
            }
            return false;
        }
        private bool CheckWeather(PokemonForm form)
        {
            WeatherDef currentWeather = pokemonHolder.Map.weatherManager.curWeather;
            if((form.includeWeathers == null || form.includeWeathers.Contains(currentWeather)) && (form.excludeWeathers == null || !form.excludeWeathers.Contains(currentWeather)))
            {
                return true;
            }
            return false;
        }
        private bool CheckTimeOfDay(PokemonForm form)
        {
            if (form.timeOfDay == TimeOfDay.Any)
            {
                return true;
            }
            int currentMapTime = GenLocalDate.HourOfDay(pokemonHolder.Map);
            if (form.timeOfDay == TimeOfDay.Day)
            {
                if(7 <= currentMapTime && currentMapTime < 19)
                {
                    return true;
                }           
            }
            if (form.timeOfDay == TimeOfDay.Night)
            {
                if (19 <= currentMapTime || currentMapTime < 7)
                {
                    return true;
                }
            }
            return false;
        }
        private bool CheckBiome(PokemonForm form)
        {         
            BiomeDef currentBiome = pokemonHolder.Map.Biome;
            if ((form.includeBiomes == null || form.includeBiomes.Contains(currentBiome)) && (form.excludeBiomes == null || !form.excludeBiomes.Contains(currentBiome)))
            {
                return true;
            }
            return false;
        }
        public string GetCurrentFormKey()
        {
            if(currentFormIndex == -1)
            {
                return "";
            }
            else
            {
                return comp.forms[currentFormIndex].texPathKey;
            }
        }
        private void ReturnToBaseForm()
        {
            currentFormIndex = -1;
            pokemonHolder.Drawer.renderer.graphics.ResolveAllGraphics();
        }
        private void ChangeInto(PokemonForm form)
        {
            currentFormIndex = comp.forms.IndexOf(form);
            for (int i = 0; i < 6; i++)
            {
                FleckMaker.ThrowDustPuff(pokemonHolder.Position, pokemonHolder.Map, 2f);
            }
            pokemonHolder.Drawer.renderer.graphics.ResolveAllGraphics();
        }
        private void ChangeIntoFixed(PokemonForm form)
        {
            currentFormIndex = comp.forms.IndexOf(form);      
        }
        public void FormTick()
        {
            if (comp.formChangerCondition == FormChangerCondition.Environnement && pokemonHolder.Spawned)
            {
                if (currentFormIndex == -1 || comp.forms[currentFormIndex].isDefault || !CanUseForm(comp.forms[currentFormIndex]))
                {
                    foreach (PokemonForm form in comp.forms)
                    {
                        if (!form.isDefault && CanUseForm(form))
                        {
                            ChangeInto(form);
                            return;
                        }
                    }
                    if (currentFormIndex == -1 || !comp.forms[currentFormIndex].isDefault)
                    {
                        IEnumerable<PokemonForm> forms = comp.forms.Where((PokemonForm x) => x.isDefault);
                        if(forms.Any())
                        {
                            ChangeInto(forms.First());
                            return;
                        }
                        if(currentFormIndex != -1)
                        {
                            ReturnToBaseForm();
                        }                                     
                    }                        
                }             
            }
        }
        private bool CanKeepCurrentForm(PokemonForm form)
        {
            if (CheckTimeOfDay(form) && CheckWeather(form) && CheckBiome(form))
            {
                return true;
            }
            return false;
        }
        private void ProcessInput()
        {
            List<FloatMenuOption> list = new List<FloatMenuOption>();
            foreach (PokemonForm form in comp.forms)
            {
                PokemonForm localForm = form;
                FloatMenuOption floatMenuOption = new FloatMenuOption(comp.forms[currentFormIndex].label.CapitalizeFirst(), delegate
                {
                    ChangeInto(localForm);
                });
                list.Add(floatMenuOption);            
            }
            if (list.Count == 0)
            {
                Messages.Message("PW_NoFormsChangeInto".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                return;
            }
            FloatMenu floatMenu = new FloatMenu(list);
            floatMenu.vanishIfMouseDistant = true;
            Find.WindowStack.Add(floatMenu);
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref currentFormIndex, "PW_currentForm", -1);
        }
    }
}
