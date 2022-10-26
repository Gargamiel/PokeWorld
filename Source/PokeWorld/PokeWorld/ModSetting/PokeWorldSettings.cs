﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;

namespace PokeWorld
{
    public class PokeWorldSettings : ModSettings
    {

        // min = no pokémon, max/2 = 50/50, max = only pokémon
        public static float minFrequency = 0f;
        public static float maxFrequency = 8f;
        public static float selectedPokemonFrequency = maxFrequency;
        public static bool allowGen1 = true;
        public static bool allowGen2 = true;
        public static bool allowGen3 = true;
        public static bool allowGen4 = true;
        public static bool allowGen5 = true;
        public static bool allowGen6 = true;
        public static bool allowGen7 = true;
        public static bool allowGen8 = true;
        public static bool allowPokemonInfestation = true;
        public static bool allowNPCPokemonPack = true;
        public static bool enableShinyMote = true;
        //public static bool allowPokemonInRaid = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref selectedPokemonFrequency, "selectedPokemonFrequency", maxFrequency);
            Scribe_Values.Look(ref allowGen1, "allowGen1", true);
            Scribe_Values.Look(ref allowGen2, "allowGen2", true);
            Scribe_Values.Look(ref allowGen3, "allowGen3", true);
            Scribe_Values.Look(ref allowGen4, "allowGen4", true);
            Scribe_Values.Look(ref allowGen5, "allowGen5", true);
            Scribe_Values.Look(ref allowGen6, "allowGen6", true);
            Scribe_Values.Look(ref allowGen7, "allowGen7", true);
            Scribe_Values.Look(ref allowGen8, "allowGen8", true);
            Scribe_Values.Look(ref allowPokemonInfestation, "allowPokemonInfestation", true);
            Scribe_Values.Look(ref allowNPCPokemonPack, "allowNPCPokemonPack", true);
            Scribe_Values.Look(ref enableShinyMote, "enableShinyMote", true);
            //Scribe_Values.Look(ref allowPokemonInRaid, "allowPokemonInRaid", true);
            base.ExposeData();
        }
        public static bool OkforPokemon()
        {
            return Rand.Range(minFrequency, maxFrequency) <= selectedPokemonFrequency;
        }
        public static bool MinSelected()
        {
            return selectedPokemonFrequency == minFrequency;
        }
        public static bool MaxSelected()
        {
            return selectedPokemonFrequency == maxFrequency;
        }
        public static bool GenerationAllowed(int generation)
        {
            switch (generation)
            {
                case 1:
                    return allowGen1;
                case 2:
                    return allowGen2;
                case 3:
                    return allowGen3;
                case 4:
                    return allowGen4;
                case 5:
                    return allowGen5;
                case 6:
                    return allowGen6;
                case 7:
                    return allowGen7;
                case 8:
                    return allowGen8;
                default:
                    return true;
            }
        }
    }

    public class PokeWorldMod : Mod
    {
        PokeWorldSettings settings;

        public PokeWorldMod(ModContentPack content) : base(content)
        {
            this.settings = GetSettings<PokeWorldSettings>();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Listing_Standard listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            PokeWorldSettings.selectedPokemonFrequency = listingStandard.SliderLabeled("PW_SettingsWildPokemonFrequency".Translate(), PokeWorldSettings.selectedPokemonFrequency, PokeWorldSettings.minFrequency, PokeWorldSettings.maxFrequency);
            listingStandard.Label("PW_SettingsWildPokemonFrequencyDesc".Translate());
            listingStandard.GapLine();
            listingStandard.CheckboxLabeled("PW_SettingsAllowPokemonInfestation".Translate(), ref PokeWorldSettings.allowPokemonInfestation);
            listingStandard.CheckboxLabeled("PW_SettingsAllowNPCPokemonPack".Translate(), ref PokeWorldSettings.allowNPCPokemonPack);
            listingStandard.CheckboxLabeled("PW_SettingsEnableShinyMote".Translate(), ref PokeWorldSettings.enableShinyMote);
            //listingStandard.CheckboxLabeled("PW_SettingsAllowPokemonInRaid".Translate(), ref PokeWorldSettings.allowPokemonInRaid);
            listingStandard.GapLine();
            listingStandard.CheckboxLabeled("PW_SettingsAllowGeneration".Translate(1), ref PokeWorldSettings.allowGen1);
            listingStandard.CheckboxLabeled("PW_SettingsAllowGeneration".Translate(2), ref PokeWorldSettings.allowGen2);
            listingStandard.CheckboxLabeled("PW_SettingsAllowGeneration".Translate(3), ref PokeWorldSettings.allowGen3);
            listingStandard.CheckboxLabeled("PW_SettingsAllowGeneration".Translate(4), ref PokeWorldSettings.allowGen4);
            listingStandard.CheckboxLabeled("PW_SettingsAllowGeneration".Translate(5), ref PokeWorldSettings.allowGen5);
            listingStandard.CheckboxLabeled("PW_SettingsAllowGeneration".Translate(6), ref PokeWorldSettings.allowGen6);
            listingStandard.CheckboxLabeled("PW_SettingsAllowGeneration".Translate(7), ref PokeWorldSettings.allowGen7);
            listingStandard.CheckboxLabeled("PW_SettingsAllowGeneration".Translate(8), ref PokeWorldSettings.allowGen8);
            listingStandard.Label("PW_SettingsWarningGenAllowed".Translate());
            PokeWorldSettings.selectedPokemonFrequency = (float)Math.Round(PokeWorldSettings.selectedPokemonFrequency);
            if(!PokeWorldSettings.allowGen1 && !PokeWorldSettings.allowGen2  && !PokeWorldSettings.allowGen3 && !PokeWorldSettings.allowGen4 && !PokeWorldSettings.allowGen5 && !PokeWorldSettings.allowGen6 && !PokeWorldSettings.allowGen7 && !PokeWorldSettings.allowGen8)
            {
                PokeWorldSettings.allowGen1 = true;
            }
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "PW_PokeWorld".Translate();
        }
    }

}
