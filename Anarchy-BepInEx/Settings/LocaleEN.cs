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
                { m_Setting.GetOptionLabelLocaleID(nameof(AnarchyModSettings.PreventAccidentalPropCulling)), "Prevent Accidental Prop Culling" },
                { m_Setting.GetOptionDescLocaleID(nameof(AnarchyModSettings.PreventAccidentalPropCulling)), "This will routinely trigger a graphical refresh to props placed with Anarchy that have been culled to prevent accidental culling of props. This affects performance." },
                { m_Setting.GetOptionLabelLocaleID(nameof(AnarchyModSettings.PropRefreshFrequency)), "Prop Refresh Frequency" },
                { m_Setting.GetOptionDescLocaleID(nameof(AnarchyModSettings.PropRefreshFrequency)), "This is number of frames between graphical refreshes to props placed with Anarchy to prevent accidental culling. Higher numbers will have better performance, but longer possible time that props may be missing." },
                { m_Setting.GetOptionLabelLocaleID(nameof(AnarchyModSettings.RefreshProps)), "Refresh Props" },
                { m_Setting.GetOptionDescLocaleID(nameof(AnarchyModSettings.RefreshProps)), "If props placed with Anarchy have been accidently culled, you can press this button to bring them back now. This doesn't negatively effect performance." },
                { "YY_ANARCHY.AnarchyButton", "Anarchy" },
                { "YY_ANARCHY_DESCRIPTION.AnarchyButton", "Disables error checks for tools and does not display errors. When applicable, you can place vegetation and props (with DevUI 'Add Object' menu) overlapping or inside the boundaries of other objects and close together." },
                { "YY_ANARCHY.RaycastSurfacesButton", "Target Surfaces" },
                { "YY_ANARCHY_DESCRIPTION.RaycastSurfacesButton", "Makes the bulldozer EXCLUSIVELY target surfaces so you can remove them in one click. With Anarchy on you can bulldoze surfaces within buidings or areas. You must turn this off to bulldoze anything else." },
                { "YY_ANARCHY.ShowMarkersButton", "Show Markers" },
                { "YY_ANARCHY_DESCRIPTION.ShowMarkersButton", "Shows markers and invisible roads. With this and Anarchy enabled you can demolish invisible roads, but SAVE FIRST!" },
                { "YY_ANARCHY.GameplayManipulationButton", "Gameplay Manipulation" },
                { "YY_ANARCHY_DESCRIPTION.GameplayManipulationButton", "Allows you to use the bulldozer on moving objects such as vehicles or cims." },
                { "YY_ANARCHY.BypassConfirmationButton", "Bypass Confirmation" },
                { "YY_ANARCHY_DESCRIPTION.BypassConfirmationButton", "Disables the prompt for whether you are sure you want to demolish a building." },
            };

        }

        /// <inheritdoc/>
        public void Unload()
        {
        }
    }
}