// <copyright file="AnarchyModSettings.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

namespace Anarchy.Settings
{
    using Colossal.IO.AssetDatabase;
    using Game.Modding;
    using Game.Settings;

    /// <summary>
    /// The mod settings for the Anarchy Mod.
    /// </summary>
    [FileLocation("Mods_Yenyang_Anarchy")]
    public class AnarchyModSettings : ModSetting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnarchyModSettings"/> class.
        /// </summary>
        /// <param name="mod">AnarchyMod.</param>
        public AnarchyModSettings(IMod mod)
            : base(mod)
        {
            SetDefaults();
        }

        /// <summary>
        /// Gets or sets a value indicating whether Anarchy should always be enabled when using Bulldozer tool.
        /// </summary>
        public bool AnarchicBulldozer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show the tooltip.
        /// </summary>
        public bool ShowTooltip { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to have chirper be on fire.
        /// </summary>
        public bool FlamingChirper { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether to have chirper be on fire.
        /// </summary>
        public bool ToolIcon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether: Used to force saving of Modsettings if settings would result in empty Json.
        /// </summary>
        [SettingsUIHidden]
        public bool Contra { get; set; }

        /// <summary>
        /// Sets a value indicating whether: a button for Resetting the settings for the Mod.
        /// </summary>
        [SettingsUIButton]
        [SettingsUIConfirmation]
        public bool ResetModSettings
        {
            set
            {
                SetDefaults();
                Contra = false;
                ApplyAndSave();
            }
        }


        /// <inheritdoc/>
        public override void SetDefaults()
        {
            AnarchicBulldozer = true;
            ShowTooltip = false;
            FlamingChirper = true;
            Contra = true;
            ToolIcon = true;
        }
    }
}
