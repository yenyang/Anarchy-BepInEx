// <copyright file="AnarchyMod.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

#define DEBUG
namespace Anarchy
{
    using System;
    using System.IO;
    using System.Linq;
    using Anarchy.Settings;
    using Anarchy.Systems;
    using Anarchy.Tooltip;
    using Colossal.IO.AssetDatabase;
    using Colossal.Localization;
    using Colossal.Logging;
    using Game;
    using Game.Modding;
    using Game.SceneFlow;

    /// <summary>
    /// Mod entry point.
    /// </summary>
    public class AnarchyMod : IMod
    {
        /// <summary>
        /// Gets the install folder for the mod.
        /// </summary>
        private static string m_modInstallFolder;

        /// <summary>
        ///  Gets or sets the static version of the Anarchy Mod Settings.
        /// </summary>
        public static AnarchyModSettings Settings { get; set; }

        /// <summary>
        /// Gets the static reference to the mod instance.
        /// </summary>
        public static AnarchyMod Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the Install Folder for the mod as a string.
        /// </summary>
        public static string ModInstallFolder
        {
            get
            {
                if (m_modInstallFolder is null)
                {
                    m_modInstallFolder = Path.GetDirectoryName(typeof(AnarchyPlugin).Assembly.Location);
                }

                return m_modInstallFolder;
            }
        }

        /// <summary>
        /// Gets ILog for mod.
        /// </summary>
        internal ILog Logger { get; private set; }

        /// <inheritdoc/>
        public void OnLoad()
        {
            Instance = this;
            Logger = LogManager.GetLogger("Mods_Yenyang_Anarchy", false);
            Logger.Info("Loading. . .");
        }

        /// <inheritdoc/>
        public void OnCreateWorld(UpdateSystem updateSystem)
        {
            Logger.Info("Initializing Settings.");
            Settings = new (this);
            Settings.RegisterInOptionsUI();
            AssetDatabase.global.LoadSettings(nameof(AnarchyMod), Settings, new AnarchyModSettings(this));
            Settings.Contra = false;
            Logger.Info("Handling create world");
            Logger.Info("ModInstallFolder = " + ModInstallFolder);
            LoadLocales();
            updateSystem.UpdateAfter<AnarchyTooltipSystem>(SystemUpdatePhase.UITooltip);
            updateSystem.UpdateAt<AnarchySystem>(SystemUpdatePhase.ToolUpdate);
            updateSystem.UpdateBefore<DisableToolErrorsSystem>(SystemUpdatePhase.ModificationEnd);
            updateSystem.UpdateAfter<EnableToolErrorsSystem>(SystemUpdatePhase.ModificationEnd);
            updateSystem.UpdateAt<AnarchyUISystem>(SystemUpdatePhase.UIUpdate);
        }

        /// <inheritdoc/>
        public void OnDispose()
        {
            Logger.Info("Disposing..");
        }

        private void LoadLocales()
        {
            var file = Path.Combine(ModInstallFolder, $"i18n.csv");
            if (File.Exists(file))
            {
                var fileLines = File.ReadAllLines(file).Select(x => x.Split('\t'));
                var enColumn = Array.IndexOf(fileLines.First(), "en-US");
                var enMemoryFile = new MemorySource(fileLines.Skip(1).ToDictionary(x => x[0], x => x.ElementAtOrDefault(enColumn)));
                foreach (var lang in GameManager.instance.localizationManager.GetSupportedLocales())
                {
                    GameManager.instance.localizationManager.AddSource(lang, enMemoryFile);
                    if (lang != "en-US")
                    {
                        var valueColumn = Array.IndexOf(fileLines.First(), lang);
                        if (valueColumn > 0)
                        {
                            var i18nFile = new MemorySource(fileLines.Skip(1).ToDictionary(x => x[0], x => x.ElementAtOrDefault(valueColumn)));
                            GameManager.instance.localizationManager.AddSource(lang, i18nFile);
                        }
                    }
                }
            }
            else
            {
                Logger.Info($"[{nameof(AnarchyMod)}] {nameof(LoadLocales)} Couldn't find localization files. Used Mod Generated defaults.");
                foreach (var lang in GameManager.instance.localizationManager.GetSupportedLocales())
                {
                    GameManager.instance.localizationManager.AddSource(lang, new LocaleEN(Settings));
                }
            }
        }
    }
}
