using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using UnityEngine;
using SettingsHelper;

namespace PokeWorld
{
    public class PokeWorldSettings : ModSettings
    {

        // min = no pokémon, max/2 = 50/50, max = only pokémon
        public static float minFrequency = 0f;
        public static float maxFrequency = 4f;
        public static float selectedPokemonFrequency = maxFrequency;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref selectedPokemonFrequency, "PW_selectedPokemonFrequency", maxFrequency);
            base.ExposeData();
        }
        public static bool OkforPokemon()
        {
            return Rand.Range(minFrequency, maxFrequency) <= selectedPokemonFrequency;
        }
        public static bool minSelected()
        {
            return selectedPokemonFrequency == minFrequency;
        }
        public static bool maxSelected()
        {
            return selectedPokemonFrequency == maxFrequency;
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
            Listing_Standard listingStandard = ListingStandardHelper.BeginListingStandard(inRect);  

            ListingStandardHelper.AddLabeledSlider(listingStandard, "PW_SettingsWildPokemonFrequency".Translate(), ref PokeWorldSettings.selectedPokemonFrequency, PokeWorldSettings.minFrequency, PokeWorldSettings.maxFrequency, "PW_SettingsNoPokemon".Translate(), "PW_SettingsOnlyPokemon".Translate());
            ListingStandardHelper.AddLabelLine(listingStandard, "PW_SettingsWildPokemonFrequencyDesc".Translate());
            /*
            ListingStandardHelper.AddLabeledCheckbox(listingStandard, "Replace trader carriers with Pokémon", );
            ListingStandardHelper.AddLabeledCheckbox(listingStandard, "Replace insects in cave and infestation by Pokémon", );
            ListingStandardHelper.AddLabeledCheckbox(listingStandard, "Replace insects in cave and infestation by Pokémon", );
            */
            PokeWorldSettings.selectedPokemonFrequency = (float)Math.Round(PokeWorldSettings.selectedPokemonFrequency);
            listingStandard.End();
            base.DoSettingsWindowContents(inRect);
        }

        public override string SettingsCategory()
        {
            return "PW_PokeWorld".Translate();
        }
    }

}
