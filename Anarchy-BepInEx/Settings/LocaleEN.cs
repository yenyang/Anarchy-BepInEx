// <copyright file="LocaleEN.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

namespace Anarchy.Settings
{
    using System.Collections.Generic;
    using Colossal;

    /// <summary>
    /// Localization for Anarchy Mod in English.
    /// </summary>
    public class LocaleEN : IDictionarySource
    {
        private readonly AnarchyModSettings m_Setting;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocaleEN"/> class.
        /// </summary>
        /// <param name="setting">Settings class.</param>
        public LocaleEN(AnarchyModSettings setting)
        {
            m_Setting = setting;
        }

        /// <inheritdoc/>
        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors, Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Anarchy" },
                { m_Setting.GetOptionLabelLocaleID(nameof(AnarchyModSettings.AnarchicBulldozer)), "Always enable Anarchy with Bulldoze Tool" },
                { m_Setting.GetOptionDescLocaleID(nameof(AnarchyModSettings.AnarchicBulldozer)), "With this option enabled the Bulldoze Tool will always have anarchy enabled." },
                { m_Setting.GetOptionLabelLocaleID(nameof(AnarchyModSettings.FlamingChirper)), "Flaming Chirper" },
                { m_Setting.GetOptionDescLocaleID(nameof(AnarchyModSettings.FlamingChirper)), "With this option enabled the Chirper will be on fire when Anarchy is active for appropriate tools. Image Credit: Bad Peanut." },
                { m_Setting.GetOptionLabelLocaleID(nameof(AnarchyModSettings.ShowTooltip)), "Show Tooltip" },
                { m_Setting.GetOptionDescLocaleID(nameof(AnarchyModSettings.ShowTooltip)), "With this option enabled a tooltip with Ⓐ will be shown when Anarchy is active for appropriate tools." },
                { m_Setting.GetOptionLabelLocaleID(nameof(AnarchyModSettings.ToolIcon)), "Tool Icon" },
                { m_Setting.GetOptionDescLocaleID(nameof(AnarchyModSettings.ToolIcon)), "With this option enabled a icon row with a single button for Anarchy will show up when using appropriate tools." },
                { m_Setting.GetOptionLabelLocaleID(nameof(AnarchyModSettings.ResetModSettings)), "Reset Anarchy Settings" },
                { m_Setting.GetOptionDescLocaleID(nameof(AnarchyModSettings.ResetModSettings)), "Upon confirmation this will reset the settings for Anarchy mod." },
                { m_Setting.GetOptionWarningLocaleID(nameof(AnarchyModSettings.ResetModSettings)), "Reset Anarchy Settings?" },
                { m_Setting.GetOptionLabelLocaleID(nameof(AnarchyModSettings.PermanetlyPreventOverride)), "Permanetly Prevent Override" },
                { m_Setting.GetOptionDescLocaleID(nameof(AnarchyModSettings.PermanetlyPreventOverride)), "Newly affected trees and props will not get overriden later even if you have anarchy disabled" },
                { m_Setting.GetOptionLabelLocaleID(nameof(AnarchyModSettings.PreventAccidentalPropCulling)), "Prevent Accidental Prop Culling" },
                { m_Setting.GetOptionDescLocaleID(nameof(AnarchyModSettings.PreventAccidentalPropCulling)), "This will routinely trigger a graphical update to props placed with Anarchy that have been culled to prevent accidental culling of props. This affects performance." },
                { m_Setting.GetOptionLabelLocaleID(nameof(AnarchyModSettings.PropUpdateFrequency)), "Prop Update Frequency" },
                { m_Setting.GetOptionDescLocaleID(nameof(AnarchyModSettings.PropUpdateFrequency)), "This is number of frames between graphical updates to props placed with Anarchy to prevent accidental culling. Higher numbers will have better performance, but longer possible time that props may be missing." },
                { m_Setting.GetOptionLabelLocaleID(nameof(AnarchyModSettings.RefreshProps)), "Refresh Props" },
                { m_Setting.GetOptionDescLocaleID(nameof(AnarchyModSettings.RefreshProps)), "If props placed with Anarchy have been accidently culled, you can press this button to bring them back now. This doesn't negatively effect performance." },
            };

        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}
