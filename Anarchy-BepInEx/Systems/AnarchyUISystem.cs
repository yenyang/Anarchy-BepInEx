// <copyright file="AnarchyUISystem.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

// #define VERBOSE
namespace Anarchy.Systems
{
    using System;
    using System.IO;
    using Anarchy.Tooltip;
    using Anarchy.Utils;
    using cohtml.Net;
    using Colossal.Logging;
    using Game.SceneFlow;
    using Game.Tools;
    using Game.UI;
    using Unity.Entities;

    /// <summary>
    /// UI system for Object Tool while using tree prefabs.
    /// </summary>
    public partial class AnarchyUISystem : UISystemBase
    {
        private View m_UiView;
        private ToolSystem m_ToolSystem;
        private string m_InjectedJS = string.Empty;
        private string m_AnarchyItemScript = string.Empty;
        private ILog m_Log;
        private AnarchySystem m_AnarchySystem;
        private bool m_AnarchyOptionShown;
        private bool m_DisableAnarchyWhenCompleted;
        private string m_LastTool;

        /// <summary>
        /// So Anarchy System can toggle the button selection with Keybind.
        /// </summary>
        public void ToggleAnarchyButton()
        {
            if (m_UiView == null)
            {
                return;
            }

            if (m_AnarchySystem.AnarchyEnabled)
            {
                // This script finds sets Anarchy button selected if toggled using key board shortcut.
                m_UiView.ExecuteScript($"var buttonYYA = document.getElementById(\"YYA-Anarchy-Button\"); if (buttonYYA != null) buttonYYA.classList.add(\"selected\");");

                // This script finds sets Anarchy button colored if toggled using key board shortcut.
                m_UiView.ExecuteScript($"var imageYYA = document.getElementById(\"YYA-Anarchy-Image\"); if (imageYYA != null) {{ imageYYA.src = \"coui://uil/Colored/Anarchy.svg\"; }} ");
                if (AnarchyMod.Settings.FlamingChirper)
                {
                    // This script sets flaming chirper if toggled using key board shortcut.
                    m_UiView.ExecuteScript($"var tagsYYA = document.getElementsByTagName(\"img\"); for (var iYYA = 0; iYYA < tagsYYA.length; iYYA++) {{ if (tagsYYA[iYYA].src == \"coui://GameUI/Media/Game/Icons/Chirper.svg\" || tagsYYA[iYYA].src == \"Media/Game/Icons/Chirper.svg\") tagsYYA[iYYA].src = \"coui://uil/Colored/AnarchyChirper.svg\"; }}");
                }

                return;
            }

            // This script resets Anarchy button if toggled using key board shortcut.
            m_UiView.ExecuteScript($"var buttonYYA = document.getElementById(\"YYA-Anarchy-Button\"); if (buttonYYA != null) buttonYYA.classList.remove(\"selected\");");

            // This script finds resets Anarchy button if toggled using key board shortcut.
            m_UiView.ExecuteScript($"var imageYYA = document.getElementById(\"YYA-Anarchy-Image\"); if (imageYYA != null) {{ imageYYA.src = \"coui://uil/Standard/Anarchy.svg\"; }} ");

            // This script resets chirper if toggled using key board shortcut.
            m_UiView.ExecuteScript($"var tagsYYA = document.getElementsByTagName(\"img\"); for (var iYYA = 0; iYYA < tagsYYA.length; iYYA++) {{ if (tagsYYA[iYYA].src == \"coui://uil/Colored/AnarchyChirper.svg\") tagsYYA[iYYA].src = \"coui://GameUI/Media/Game/Icons/Chirper.svg\"; }}");
            m_DisableAnarchyWhenCompleted = false;
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            m_Log = AnarchyMod.Instance.Logger;
            m_ToolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ToolSystem>();
            m_UiView = GameManager.instance.userInterface.view.View;
            m_AnarchySystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<AnarchySystem>();

            if (m_UiView != null)
            {
                m_UiView.RegisterForEvent("YYA-AnarchyToggled", (Action<bool>)AnarchyToggled);
                m_UiView.RegisterForEvent("CheckForElement-YYA-anarchy-item", (Action<bool>)ElementCheck);
                m_InjectedJS = UIFileUtils.ReadJS(Path.Combine(UIFileUtils.AssemblyPath, "ui.js"));
                m_UiView.RegisterForEvent("YYA-log", (Action<string>)LogFromJS);
                m_AnarchyItemScript = UIFileUtils.ReadHTML(Path.Combine(UIFileUtils.AssemblyPath, "YYA-anarchy-tool-row.html"), "if (document.getElementById(\"YYA-anarchy-item\") == null) { divYYA.className = \"item_bZY\"; divYYA.id = \"YYA-anarchy-item\"; var entitiesYYA = document.getElementsByClassName(\"tool-options-panel_Se6\"); if (entitiesYYA[0] != null) { entitiesYYA[0].insertAdjacentElement('afterbegin', divYYA); setupAnarchyItemYYA(); } }");
            }
            else
            {
                m_Log.Info($"{nameof(AnarchyUISystem)}.{nameof(OnCreate)} m_UiView == null");
            }

            base.OnCreate();
        }

        /// <inheritdoc/>
        protected override void OnUpdate()
        {
            if (m_UiView == null)
            {
                return;
            }

            if (m_ToolSystem.activeTool.toolID == null)
            {
                if (m_AnarchyOptionShown == true)
                {
                    // This script destroys the anarchy item if it exists.
                    UIFileUtils.ExecuteScript(m_UiView, DestroyElementByID("YYA-anarchy-item"));

                    // This script resets chirper.
                    m_UiView.ExecuteScript($"var tagsYYA = document.getElementsByTagName(\"img\"); for (var iYYA = 0; iYYA < tagsYYA.length; iYYA++) {{ if (tagsYYA[iYYA].src == \"coui://uil/Colored/AnarchyChirper.svg\") tagsYYA[iYYA].src = \"coui://GameUI/Media/Game/Icons/Chirper.svg\"; }}");
                }

                return;
            }

            if (m_ToolSystem.activeTool.toolID != "Bulldoze Tool" && m_DisableAnarchyWhenCompleted)
            {
                m_AnarchySystem.AnarchyEnabled = false;
                m_DisableAnarchyWhenCompleted = false;
                ToggleAnarchyButton();
            }

            if (!m_AnarchySystem.IsToolAppropriate(m_ToolSystem.activeTool.toolID))
            {
                if (m_AnarchyOptionShown == true)
                {
                    // This script destroys the anarchy item if it exists.
                    UIFileUtils.ExecuteScript(m_UiView, DestroyElementByID("YYA-anarchy-item"));

                    // This script resets chirper.
                    m_UiView.ExecuteScript($"var tagsYYA = document.getElementsByTagName(\"img\"); for (var iYYA = 0; iYYA < tagsYYA.length; iYYA++) {{ if (tagsYYA[iYYA].src == \"coui://uil/Colored/AnarchyChirper.svg\") tagsYYA[iYYA].src = \"coui://GameUI/Media/Game/Icons/Chirper.svg\"; }}");
                }

                return;
            }

            if (m_AnarchyOptionShown == false)
            {
                // Implements Anarchic Bulldozer when bulldoze tool is activated from inappropriate tool.
                if (AnarchyMod.Settings.AnarchicBulldozer && m_AnarchySystem.AnarchyEnabled == false && m_ToolSystem.activeTool.toolID == "Bulldoze Tool")
                {
                    m_AnarchySystem.AnarchyEnabled = true;
                    m_DisableAnarchyWhenCompleted = true;
                    ToggleAnarchyButton();
                }

                if (AnarchyMod.Settings.ToolIcon)
                {
                    // This script passes whether Anarchy is Enabled to JS.
                    UIFileUtils.ExecuteScript(m_UiView, $"var anarchyEnabledYYA = {BoolToString(m_AnarchySystem.AnarchyEnabled)};");

                    // This script passes the option to have flaming Chirper to JS.
                    UIFileUtils.ExecuteScript(m_UiView, $"var flamingChirperYYA = {BoolToString(AnarchyMod.Settings.FlamingChirper)};");

                    // This script defines the JS functions if they are not defined.
                    UIFileUtils.ExecuteScript(m_UiView, m_InjectedJS);

                    // This script creates the anarchy item and sets up the buttons.
                    UIFileUtils.ExecuteScript(m_UiView, m_AnarchyItemScript);
                }
                else if (AnarchyMod.Settings.FlamingChirper)
                {
                    ToggleAnarchyButton();
                }

                m_AnarchyOptionShown = true;
            }
            else
            {
                // This script checks to see if there is a tool options panel. If there isn't one then it removes it.
                UIFileUtils.ExecuteScript(m_UiView, $"var entitiesYYA = document.getElementsByClassName(\"tool-options-panel_Se6\"); if (entitiesYYA[0] == null) {{ var divYYA = document.createElement(\"div\"); divYYA.className = \"tool-options-panel_Se6\"; document.getElementsByClassName(\"tool-side-column_l9i\")[0].appendChild(divYYA); }}");

                // This script checks if multiple tool options panels exist. If anarchy is the only one in that tool panel then it removes whole panel.
                UIFileUtils.ExecuteScript(m_UiView, $"var entitiesYYA = document.getElementsByClassName(\"tool-options-panel_Se6\"); if (entitiesYYA.length > 1) {{  var itemYYA = document.getElementById(\"YYA-anarchy-item\"); if (itemYYA != null) {{ if (itemYYA.parentElement.children.length == 1) {{   itemYYA.parentElement.parentElement.removeChild(itemYYA.parentElement); }} }} }}");

                // This script checks if anarchy item exists. If it doesn't it triggers anarchy item being recreated.
                UIFileUtils.ExecuteScript(m_UiView, $"if (document.getElementById(\"YYA-anarchy-item\") == null) engine.trigger('CheckForElement-YYA-anarchy-item', false);");

                // This script checks if anarchy is first child and if not removes it.
                UIFileUtils.ExecuteScript(m_UiView, $"var itemYYA = document.getElementById(\"YYA-anarchy-item\"); if (itemYYA != null) {{  if (itemYYA.parentElement.firstChild != itemYYA) {{  itemYYA.parentElement.removeChild(itemYYA);   }}  }}");

                // Makes it so Anarchic Bulldozer will work next frame when bulldoze tool is activated from other appropriate tool.
                if (m_ToolSystem.activeTool.toolID == "Bulldoze Tool" && AnarchyMod.Settings.AnarchicBulldozer && m_AnarchySystem.AnarchyEnabled == false && m_LastTool != "Bulldoze Tool")
                {
                    m_AnarchyOptionShown = false;
                    m_Log.Debug("Anarchic bulldozer from other appropriate tool.");
                }

                m_LastTool = m_ToolSystem.activeTool.toolID;
            }

            base.OnUpdate();
        }

        /// <summary>
        /// Get a script for Destroing element by id if that element exists.
        /// </summary>
        /// <param name="id">The id from HTML or JS.</param>
        /// <returns>a script for Destroing element by id if that element exists.</returns>
        private string DestroyElementByID(string id)
        {
            return $"var itemYYA = document.getElementById(\"{id}\"); if (itemYYA) itemYYA.parentElement.removeChild(itemYYA);";
        }

        /// <summary>
        /// Logs a string from JS.
        /// </summary>
        /// <param name="log">A string from JS to log.</param>
        private void LogFromJS(string log)
        {
            m_Log.Debug($"{nameof(AnarchyUISystem)}.{nameof(LogFromJS)} {log}");
        }

        /// <summary>
        /// Converts a C# bool to JS string.
        /// </summary>
        /// <param name="flag">a bool.</param>
        /// <returns>"true" or "false"</returns>
        private string BoolToString(bool flag)
        {
            if (flag)
            {
                return "true";
            }

            return "false";
        }

        /// <summary>
        /// An event to Toggle Anarchy.
        /// </summary>
        /// <param name="flag">A bool for whether it's enabled or not.</param>
        private void AnarchyToggled (bool flag)
        {
            if (flag)
            {
                m_AnarchySystem.AnarchyEnabled = true;
            }
            else
            {
                m_AnarchySystem.AnarchyEnabled = false;
                m_DisableAnarchyWhenCompleted = false;
            }
        }

        /// <summary>
        /// C# event handler for event callback from UI JavaScript. If element YYTC-tree-age-item is found then set value to true.
        /// </summary>
        /// <param name="flag">A bool for whether to element was found.</param>
        private void ElementCheck(bool flag)
        {
            if (flag)
            {
                m_AnarchyOptionShown = true;
            }
            else
            {
                m_AnarchyOptionShown = false;
            }
        }
    }
}
