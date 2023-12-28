// <copyright file="AnarchyUISystem.cs" company="Yenyang's Mods. MIT License">
// Copyright (c) Yenyang's Mods. MIT License. All rights reserved.
// </copyright>

// #define VERBOSE
namespace Anarchy.Systems
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Anarchy.Tooltip;
    using Anarchy.Utils;
    using cohtml.Net;
    using Colossal.Logging;
    using Game.Net;
    using Game.Prefabs;
    using Game.Rendering;
    using Game.SceneFlow;
    using Game.Tools;
    using Game.UI;
    using Unity.Entities;
    using static Colossal.AssetPipeline.Diagnostic.Report;

    /// <summary>
    /// UI system for Object Tool while using tree prefabs.
    /// </summary>
    public partial class AnarchyUISystem : UISystemBase
    {
        private View m_UiView;
        private ToolSystem m_ToolSystem;
        private string m_InjectedJS = string.Empty;
        private string m_AnarchyItemScript = string.Empty;
        private string m_AnarchyItemLineToolScript = string.Empty;
        private ILog m_Log;
        private AnarchySystem m_AnarchySystem;
        private RenderingSystem m_RenderingSystem;
        private PrefabSystem m_PrefabSystem;
        private bool m_AnarchyOptionShown;
        private bool m_DisableAnarchyWhenCompleted;
        private string m_LastTool;
        private List<BoundEventHandle> m_BoundEventHandles;
        private string m_BulldozeToolItemWithAnarchyScript;
        private string m_BulldozeToolItemWithoutAnarchyScript;
        private BulldozeToolSystem m_BulldozeToolSystem;
        private bool m_LastGamePlayManipulation;
        private bool m_LastBypassConfrimation;
        private bool m_LastShowMarkers;
        private bool m_PrefabIsMarker = false;
        private RaycastTarget m_RaycastTarget;

        /// <summary>
        /// An enum to handle different raycast target options.
        /// </summary>
        public enum RaycastTarget
        {
            /// <summary>
            /// Do not change the raycast targets.
            /// </summary>
            Vanilla,

            /// <summary>
            /// Exclusively target surfaces.
            /// </summary>
            Surfaces,

            /// <summary>
            /// Exclusively target markers.
            /// </summary>
            Markers,
        }

        /// <summary>
        /// Gets a value indicating whether whether Anarchy is only on because of Anarchic Bulldozer setting.
        /// </summary>
        public bool DisableAnarchyWhenCompleted
        {
            get { return m_DisableAnarchyWhenCompleted; }
        }

        /// <summary>
        /// Gets a value indicating whether to raycast surfaces.
        /// </summary>
        public RaycastTarget SelectedRaycastTarget { get => m_RaycastTarget; }

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
                m_UiView.ExecuteScript($"yyAnarchy.buttonElement = document.getElementById(\"YYA-Anarchy-Button\"); if (yyAnarchy.buttonElement != null) yyAnarchy.buttonElement.classList.add(\"selected\");");

                // This script finds sets Anarchy button colored if toggled using key board shortcut.
                m_UiView.ExecuteScript($"yyAnarchy.imageElement = document.getElementById(\"YYA-Anarchy-Image\"); if (yyAnarchy.imageElement != null) {{ yyAnarchy.imageElement.src = \"coui://uil/Colored/Anarchy.svg\"; }} ");
                if (AnarchyMod.Settings.FlamingChirper)
                {
                    // This script sets flaming chirper if toggled using key board shortcut.
                    m_UiView.ExecuteScript($"yyAnarchy.tagElements = document.getElementsByTagName(\"img\"); for (yyAnarchy.i = 0; yyAnarchy.i < yyAnarchy.tagElements.length; yyAnarchy.i++) {{ if (yyAnarchy.tagElements[yyAnarchy.i].src == \"coui://GameUI/Media/Game/Icons/Chirper.svg\" || yyAnarchy.tagElements[yyAnarchy.i].src == \"Media/Game/Icons/Chirper.svg\") yyAnarchy.tagElements[yyAnarchy.i].src = \"coui://uil/Colored/AnarchyChirper.svg\"; }}");
                }

                return;
            }

            // This script resets Anarchy button if toggled using key board shortcut.
            m_UiView.ExecuteScript($"yyAnarchy.buttonElement = document.getElementById(\"YYA-Anarchy-Button\"); if (yyAnarchy.buttonElement != null) yyAnarchy.buttonElement.classList.remove(\"selected\");");

            // This script  resets Anarchy button image if toggled using key board shortcut.
            m_UiView.ExecuteScript($"yyAnarchy.imageElement = document.getElementById(\"YYA-Anarchy-Image\"); if (yyAnarchy.imageElement != null) {{ yyAnarchy.imageElement.src = \"coui://uil/Standard/Anarchy.svg\"; }} ");

            // This script resets chirper if toggled using key board shortcut.
            m_UiView.ExecuteScript($"yyAnarchy.tagElements = document.getElementsByTagName(\"img\"); for (yyAnarchy.i = 0; yyAnarchy.i < yyAnarchy.tagElements.length; yyAnarchy.i++) {{ if (yyAnarchy.tagElements[yyAnarchy.i].src == \"coui://uil/Colored/AnarchyChirper.svg\") yyAnarchy.tagElements[yyAnarchy.i].src = \"coui://GameUI/Media/Game/Icons/Chirper.svg\"; }}");

            m_DisableAnarchyWhenCompleted = false;
        }

        /// <inheritdoc/>
        protected override void OnCreate()
        {
            m_Log = AnarchyMod.Instance.Logger;
            m_ToolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<ToolSystem>();
            m_UiView = GameManager.instance.userInterface.view.View;
            m_AnarchySystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<AnarchySystem>();
            m_BulldozeToolSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<BulldozeToolSystem>();
            m_RenderingSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<RenderingSystem>();
            m_PrefabSystem = World.DefaultGameObjectInjectionWorld?.GetOrCreateSystemManaged<PrefabSystem>();
            ToolSystem toolSystem = m_ToolSystem; // I don't know why vanilla game did this.
            m_ToolSystem.EventToolChanged = (Action<ToolBaseSystem>)Delegate.Combine(toolSystem.EventToolChanged, new Action<ToolBaseSystem>(OnToolChanged));
            ToolSystem toolSystem2 = m_ToolSystem;
            toolSystem2.EventPrefabChanged = (Action<PrefabBase>)Delegate.Combine(toolSystem2.EventPrefabChanged, new Action<PrefabBase>(OnPrefabChanged));
            m_BoundEventHandles = new();

            if (m_UiView != null)
            {
                m_InjectedJS = UIFileUtils.ReadJS(Path.Combine(UIFileUtils.AssemblyPath, "ui.js"));
                m_AnarchyItemScript = UIFileUtils.ReadHTML(Path.Combine(UIFileUtils.AssemblyPath, "YYA-anarchy-tool-row.html"), "if (document.getElementById(\"YYA-anarchy-item\") == null) { yyAnarchy.div.className = \"item_bZY\"; yyAnarchy.div.id = \"YYA-anarchy-item\"; yyAnarchy.entities = document.getElementsByClassName(\"tool-options-panel_Se6\"); if (yyAnarchy.entities[0] != null) { yyAnarchy.entities[0].insertAdjacentElement('afterbegin', yyAnarchy.div); yyAnarchy.setupAnarchyItem(); } }");
                m_AnarchyItemLineToolScript = UIFileUtils.ReadHTML(Path.Combine(UIFileUtils.AssemblyPath, "YYA-anarchy-tool-row.html"), "if (document.getElementById(\"YYA-anarchy-item\") == null) { yyAnarchy.div.className = \"item_bZY\"; yyAnarchy.div.id = \"YYA-anarchy-item\"; yyAnarchy.LineToolTitle = document.getElementById(\"line-tool-title\"); if (yyAnarchy.LineToolTitle != null) { yyAnarchy.LineToolTitle.parentElement.insertAdjacentElement('afterend', yyAnarchy.div); yyAnarchy.setupAnarchyItem(); } }");
                m_BulldozeToolItemWithAnarchyScript = UIFileUtils.ReadHTML(Path.Combine(UIFileUtils.AssemblyPath, "YYA-bulldoze-tool-mode-row.html"), "if (document.getElementById(\"YYA-bulldoze-tool-mode-item\") == null) { yyAnarchy.div.className = \"item_bZY\"; yyAnarchy.div.id = \"YYA-bulldoze-tool-mode-item\"; yyAnarchy.anarchyItem = document.getElementById(\"YYA-anarchy-item\"); if (yyAnarchy.anarchyItem != null) { yyAnarchy.anarchyItem.insertAdjacentElement('afterend', yyAnarchy.div); } }");
                m_BulldozeToolItemWithoutAnarchyScript = UIFileUtils.ReadHTML(Path.Combine(UIFileUtils.AssemblyPath, "YYA-bulldoze-tool-mode-row.html"), "if (document.getElementById(\"YYA-bulldoze-tool-mode-item\") == null) { yyAnarchy.div.className = \"item_bZY\"; yyAnarchy.div.id = \"YYA-bulldoze-tool-mode-item\"; yyAnarchy.entities = document.getElementsByClassName(\"tool-options-panel_Se6\"); if (yyAnarchy.entities[0] != null) { yyAnarchy.entities[0].insertAdjacentElement('afterbegin', yyAnarchy.div); } }");
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
                    UnshowAnarchyOption();
                }

                return;
            }

            if (!m_AnarchySystem.IsToolAppropriate(m_ToolSystem.activeTool.toolID))
            {
                return;
            }

            // This script creates the Anarchy object if it doesn't exist.
            UIFileUtils.ExecuteScript(m_UiView, "if (yyAnarchy == null) var yyAnarchy = {};");

            if (m_AnarchyOptionShown == false)
            {
                if (AnarchyMod.Settings.ToolIcon || m_ToolSystem.activeTool == m_BulldozeToolSystem)
                {
                    SendVariablesToJS();

                    // This script defines the JS functions if they are not defined.
                    UIFileUtils.ExecuteScript(m_UiView, m_InjectedJS);

                    m_BoundEventHandles.Add(m_UiView.RegisterForEvent("YYA-log", (Action<string>)LogFromJS));
                }

                if (AnarchyMod.Settings.ToolIcon)
                {
                    if (m_ToolSystem.activeTool.toolID != "Line Tool")
                    {
                        // This script creates the anarchy item and sets up the buttons.
                        UIFileUtils.ExecuteScript(m_UiView, m_AnarchyItemScript);
                    }
                    else
                    {
                        // This script creates the anarchy item and sets up the buttons specifically for line tool.
                        UIFileUtils.ExecuteScript(m_UiView, m_AnarchyItemLineToolScript);
                    }

                    m_BoundEventHandles.Add(m_UiView.RegisterForEvent("YYA-AnarchyToggled", (Action<bool>)AnarchyToggled));
                    m_BoundEventHandles.Add(m_UiView.RegisterForEvent("CheckForElement-YYA-anarchy-item", (Action<bool>)ElementCheck));
                }
                else if (AnarchyMod.Settings.FlamingChirper)
                {
                    ToggleAnarchyButton();
                }

                if (m_ToolSystem.activeTool == m_BulldozeToolSystem)
                {
                    if (AnarchyMod.Settings.ToolIcon)
                    {
                        // This script creates the bulldozer tool mode row and sets up the buttons.
                        UIFileUtils.ExecuteScript(m_UiView, m_BulldozeToolItemWithAnarchyScript);
                    }
                    else
                    {
                        // This script creates the bulldozer tool mode row and sets up the buttons.
                        UIFileUtils.ExecuteScript(m_UiView, m_BulldozeToolItemWithoutAnarchyScript);
                    }

                    UIFileUtils.ExecuteScript(m_UiView, $"yyAnarchy.setupButton(\"YYA-Bypass-Confirmation-Button\", {BoolToString(m_BulldozeToolSystem.debugBypassBulldozeConfirmation)}, \"BypassConfirmationButton\")");
                    UIFileUtils.ExecuteScript(m_UiView, $"yyAnarchy.setupButton(\"YYA-Gameplay-Manipulation-Button\", {BoolToString(m_BulldozeToolSystem.allowManipulation)}, \"GameplayManipulationButton\")");
                    UIFileUtils.ExecuteScript(m_UiView, $"yyAnarchy.setupButton(\"YYA-Raycast-Markers-Button\", {IsRaycastTargetSelected(RaycastTarget.Markers)}, \"RaycastMarkersButton\")");
                    UIFileUtils.ExecuteScript(m_UiView, $"yyAnarchy.setupButton(\"YYA-Raycast-Surfaces-Button\", {IsRaycastTargetSelected(RaycastTarget.Surfaces)}, \"RaycastSurfacesButton\")");
                    m_BoundEventHandles.Add(m_UiView.RegisterForEvent("YYA-Bypass-Confirmation-Button", (Action<bool>)BypassConfirmationToggled));
                    m_BoundEventHandles.Add(m_UiView.RegisterForEvent("YYA-Gameplay-Manipulation-Button", (Action<bool>)GameplayManipulationToggled));
                    m_BoundEventHandles.Add(m_UiView.RegisterForEvent("YYA-Raycast-Markers-Button", (Action<bool>)RaycastMarkersButtonToggled));
                    m_BoundEventHandles.Add(m_UiView.RegisterForEvent("YYA-Raycast-Surfaces-Button", (Action<bool>)RaycastSurfacesButtonToggled));
                    m_BoundEventHandles.Add(m_UiView.RegisterForEvent("CheckForElement-YYA-bulldoze-tool-mode-item", (Action<bool>)ElementCheck));
                }

                m_AnarchyOptionShown = true;
            }
            else
            {
                // This script checks to see if there is a tool options panel. If there isn't one then it adds one.
                UIFileUtils.ExecuteScript(m_UiView, $"yyAnarchy.entities = document.getElementsByClassName(\"tool-options-panel_Se6\"); if (yyAnarchy.entities[0] == null) {{ yyAnarchy.div = document.createElement(\"div\"); yyAnarchy.div.className = \"tool-options-panel_Se6\"; yyAnarchy.toolColumns = document.getElementsByClassName(\"tool-side-column_l9i\"); if (yyAnarchy.toolColumns[0] != null) yyAnarchy.toolColumns[0].appendChild(yyAnarchy.div); }}");

                // This script checks if multiple tool options panels exist. If anarchy is the only one in that tool panel then it removes whole panel.
                UIFileUtils.ExecuteScript(m_UiView, $"yyAnarchy.entities = document.getElementsByClassName(\"tool-options-panel_Se6\"); if (yyAnarchy.entities.length > 1) {{  yyAnarchy.itemElement = document.getElementById(\"YYA-anarchy-item\"); if (yyAnarchy.itemElement != null) {{ if (yyAnarchy.itemElement.parentElement.children.length == 1) {{   yyAnarchy.itemElement.parentElement.parentElement.removeChild(yyAnarchy.itemElement.parentElement); }} }} }}");

                // This script checks if anarchy item exists. If it doesn't it triggers anarchy item being recreated.
                UIFileUtils.ExecuteScript(m_UiView, $"if (document.getElementById(\"YYA-anarchy-item\") == null) engine.trigger('CheckForElement-YYA-anarchy-item', false);");

                if (m_ToolSystem.activeTool.toolID != "Line Tool")
                {
                    // This script checks if anarchy is first child and if not removes it.
                    UIFileUtils.ExecuteScript(m_UiView, $"yyAnarchy.itemElement = document.getElementById(\"YYA-anarchy-item\"); if (yyAnarchy.itemElement != null) {{  if (yyAnarchy.itemElement.parentElement.firstChild != yyAnarchy.itemElement) {{  yyAnarchy.itemElement.parentElement.removeChild(yyAnarchy.itemElement);   }}  }}");
                }

                if (m_ToolSystem.activeTool != m_BulldozeToolSystem)
                {
                    return;
                }

                // This script checks if bulldoze tool mode item exists. If it doesn't it triggers bulldoze tool mode being recreated.
                UIFileUtils.ExecuteScript(m_UiView, $"if (document.getElementById(\"YYA-bulldoze-tool-mode-item\") == null) engine.trigger('CheckForElement-YYA-bulldoze-tool-mode-item', false);");

                if (m_LastBypassConfrimation != m_BulldozeToolSystem.debugBypassBulldozeConfirmation)
                {
                    if (m_BulldozeToolSystem.debugBypassBulldozeConfirmation)
                    {
                        // This script finds sets Bypass-Confirmation-Button button selected if toggled using DevUI.
                        m_UiView.ExecuteScript($"yyAnarchy.buttonElement = document.getElementById(\"YYA-Bypass-Confirmation-Button\"); if (yyAnarchy.buttonElement != null) yyAnarchy.buttonElement.classList.add(\"selected\");");
                    }
                    else
                    {
                        // This script finds sets Bypass-Confirmation-Button button unselected if toggled using DevUI
                        m_UiView.ExecuteScript($"yyAnarchy.buttonElement = document.getElementById(\"YYA-Bypass-Confirmation-Button\"); if (yyAnarchy.buttonElement != null) yyAnarchy.buttonElement.classList.remove(\"selected\");");
                    }

                    m_LastBypassConfrimation = m_BulldozeToolSystem.debugBypassBulldozeConfirmation;
                }

                if (m_LastGamePlayManipulation != m_BulldozeToolSystem.allowManipulation)
                {
                    if (m_BulldozeToolSystem.allowManipulation)
                    {
                        // This script finds sets Gameplay-Manipulation-Button button selected if toggled using DevUI.
                        m_UiView.ExecuteScript($"yyAnarchy.buttonElement = document.getElementById(\"YYA-Gameplay-Manipulation-Button\"); if (yyAnarchy.buttonElement != null) yyAnarchy.buttonElement.classList.add(\"selected\");");
                    }
                    else
                    {
                        // This script finds sets Gameplay-Manipulation-Button button unselected if toggled using DevUI
                        m_UiView.ExecuteScript($"yyAnarchy.buttonElement = document.getElementById(\"YYA-Gameplay-Manipulation-Button\"); if (yyAnarchy.buttonElement != null) yyAnarchy.buttonElement.classList.remove(\"selected\");");
                    }

                    m_LastGamePlayManipulation = m_BulldozeToolSystem.allowManipulation;
                }
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
            return $"yyAnarchy.itemElement = document.getElementById(\"{id}\"); if (yyAnarchy.itemElement) yyAnarchy.itemElement.parentElement.removeChild(yyAnarchy.itemElement);";
        }

        /// <summary>
        /// Logs a string from JS.
        /// </summary>
        /// <param name="log">A string from JS to log.</param>
        private void LogFromJS(string log) => m_Log.Debug($"{nameof(AnarchyUISystem)}.{nameof(LogFromJS)} {log}");

        /// <summary>
        /// Converts a C# bool to JS string.
        /// </summary>
        /// <param name="flag">a bool.</param>
        /// <returns>"true" or "false".</returns>
        private string BoolToString(bool flag)
        {
            if (flag)
            {
                return "true";
            }

            return "false";
        }

        /// <summary>
        /// Returns a JS string for whether the raycast target is selected or not.
        /// </summary>
        /// <param name="target">A Raycast target</param>
        /// <returns>true or false as a string.</returns>
        private string IsRaycastTargetSelected(RaycastTarget target)
        {
            if (m_RaycastTarget == target)
            {
                return "true";
            }

            return "false";
        }

        /// <summary>
        /// An event to Toggle Anarchy.
        /// </summary>
        /// <param name="flag">A bool for whether it's enabled or not.</param>
        private void AnarchyToggled(bool flag)
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
        /// C# event handler for event callback from UI JavaScript. Toggles the bypassConfirmation field of the bulldozer system.
        /// </summary>
        /// <param name="flag">A bool for what to set the field to.</param>
        private void BypassConfirmationToggled(bool flag)
        {
            m_BulldozeToolSystem.debugBypassBulldozeConfirmation = flag;
            m_LastBypassConfrimation = flag;
        }

        /// <summary>
        /// C# event handler for event callback from UI JavaScript. Toggles the game playmanipulation field of the bulldozer system.
        /// </summary>
        /// <param name="flag">A bool for what to set the field to.</param>
        private void GameplayManipulationToggled(bool flag)
        {
            m_BulldozeToolSystem.allowManipulation = flag;
            m_LastGamePlayManipulation = flag;
        }

        /// <summary>
        /// C# event handler for event callback from UI JavaScript. Toggles the m_RenderingSystem.MarkersVisible.
        /// </summary>
        /// <param name="flag">A bool for what to set the field to.</param>
        private void RaycastMarkersButtonToggled(bool flag)
        {
            m_RenderingSystem.markersVisible = flag;
            if (flag)
            {
                m_RaycastTarget = RaycastTarget.Markers;
                m_UiView.ExecuteScript($"yyAnarchy.buttonElement = document.getElementById(\"YYA-Raycast-Surfaces-Button\"); if (yyAnarchy.buttonElement != null) yyAnarchy.buttonElement.classList.remove(\"selected\");");
            }
            else
            {
                m_RaycastTarget = RaycastTarget.Vanilla;
            }
        }

        /// <summary>
        /// C# event handler for event callback from UI JavaScript. Toggles the m_RaycastSurfaces.
        /// </summary>
        /// <param name="flag">A bool for what to set the field to.</param>
        private void RaycastSurfacesButtonToggled(bool flag)
        {
            if (flag)
            {
                m_RaycastTarget = RaycastTarget.Surfaces;
                m_UiView.ExecuteScript($"yyAnarchy.buttonElement = document.getElementById(\"YYA-Raycast-Markers-Button\"); if (yyAnarchy.buttonElement != null) yyAnarchy.buttonElement.classList.remove(\"selected\");");
                m_RenderingSystem.markersVisible = m_LastShowMarkers;
            }
            else
            {
                m_RaycastTarget = RaycastTarget.Vanilla;
            }
        }

        /// <summary>
        /// C# event handler for event callback from UI JavaScript. If element YYA-anarchy-item is found then set value to true.
        /// </summary>
        /// <param name="flag">A bool for whether to element was found.</param>
        private void ElementCheck(bool flag) => m_AnarchyOptionShown = flag;

        /// <summary>
        /// C# event handler for event callback from UI JavaScript. If element YYTC-tree-age-item is found then set value to true.
        /// </summary>
        private void SendVariablesToJS()
        {
            // This script passes whether Anarchy is Enabled to JS.
            UIFileUtils.ExecuteScript(m_UiView, $"yyAnarchy.enabled = {BoolToString(m_AnarchySystem.AnarchyEnabled)};");

            // This script passes the option to have flaming Chirper to JS.
            UIFileUtils.ExecuteScript(m_UiView, $"yyAnarchy.flamingChirper = {BoolToString(AnarchyMod.Settings.FlamingChirper)};");
        }

        /// <summary>
        /// Handles cleaning up after the icons are no longer needed.
        /// </summary>
        private void UnshowAnarchyOption()
        {
            if (m_UiView == null)
            {
                return;
            }

            // This script checks if tool options panel(s) exist. If anarchy is the only one in that tool panel then it removes whole panel.
            UIFileUtils.ExecuteScript(m_UiView, $"yyAnarchy.entities = document.getElementsByClassName(\"tool-options-panel_Se6\"); if (yyAnarchy.entities.length >= 1) {{  yyAnarchy.itemElement = document.getElementById(\"YYA-anarchy-item\"); if (yyAnarchy.itemElement != null) {{ if (yyAnarchy.itemElement.parentElement.children.length == 1) {{   yyAnarchy.itemElement.parentElement.parentElement.removeChild(yyAnarchy.itemElement.parentElement); }} }} }}");

            // This script destroys the anarchy item if it exists.
            UIFileUtils.ExecuteScript(m_UiView, DestroyElementByID("YYA-anarchy-item"));

            // This script destroys the bulldoze tool mode row if it exists.
            UIFileUtils.ExecuteScript(m_UiView, DestroyElementByID("YYA-bulldoze-tool-mode-item"));

            // This script resets chirper.
            m_UiView.ExecuteScript($"yyAnarchy.tagElements = document.getElementsByTagName(\"img\"); for (yyAnarchy.i = 0; yyAnarchy.i < yyAnarchy.tagElements.length; yyAnarchy.i++) {{ if (yyAnarchy.tagElements[yyAnarchy.i].src == \"coui://uil/Colored/AnarchyChirper.svg\") yyAnarchy.tagElements[yyAnarchy.i].src = \"coui://GameUI/Media/Game/Icons/Chirper.svg\"; }}");

            // This unregisters the events.
            foreach (BoundEventHandle eventHandle in m_BoundEventHandles)
            {
                m_UiView.UnregisterFromEvent(eventHandle);
            }

            m_BoundEventHandles.Clear();

            // This records that everything is cleaned up.
            m_AnarchyOptionShown = false;
        }

        private void OnToolChanged(ToolBaseSystem tool)
        {
            // This script creates the Anarchy object if it doesn't exist.
            UIFileUtils.ExecuteScript(m_UiView, "if (yyAnarchy == null) var yyAnarchy = {};");

            if (!m_AnarchySystem.IsToolAppropriate(tool.toolID))
            {
                UnshowAnarchyOption();
                this.Enabled = false;
            }
            else
            {
                this.Enabled = true;
            }

            // Makes it so Anarchic Bulldozer will work next frame when bulldoze tool is activated from other appropriate tool.
            if (tool.toolID == "Bulldoze Tool" && m_LastTool != "Bulldoze Tool")
            {
                m_AnarchyOptionShown = false;
                m_LastShowMarkers = m_RenderingSystem.markersVisible;
                if (m_RaycastTarget == RaycastTarget.Markers)
                {
                    m_RenderingSystem.markersVisible = true;
                }
            }

            // Removes Anarchy item if activating line tool so that it can be recreated in the new location. 
            if (tool.toolID == "Line Tool")
            {
                UIFileUtils.ExecuteScript(m_UiView, DestroyElementByID("YYA-anarchy-item"));
                m_AnarchyOptionShown = false;
            }

            if (tool.toolID != "Bulldoze Tool" && m_LastTool == "Bulldoze Tool")
            {
                // This script destroys the bulldoze tool mode row if it exists.
                UIFileUtils.ExecuteScript(m_UiView, DestroyElementByID("YYA-bulldoze-tool-mode-item"));

                if (!m_LastShowMarkers || m_RaycastTarget != RaycastTarget.Markers)
                {
                    m_RenderingSystem.markersVisible = false;
                }
            }

            if (tool.toolID != "Bulldoze Tool" && m_DisableAnarchyWhenCompleted)
            {
                m_AnarchySystem.AnarchyEnabled = false;
                m_DisableAnarchyWhenCompleted = false;
                ToggleAnarchyButton();
            }

            // Implements Anarchic Bulldozer when bulldoze tool is activated from inappropriate tool.
            if (AnarchyMod.Settings.AnarchicBulldozer && m_AnarchySystem.AnarchyEnabled == false && tool.toolID == "Bulldoze Tool")
            {
                m_AnarchySystem.AnarchyEnabled = true;
                m_DisableAnarchyWhenCompleted = true;
                ToggleAnarchyButton();
            }

            // Shows markers if appropriate prefab is selected.
            if (tool.GetPrefab() != null)
            {
                Entity prefabEntity = m_PrefabSystem.GetEntity(tool.GetPrefab());
                if (EntityManager.HasComponent<MarkerNetData>(prefabEntity))
                {
                    if (!m_PrefabIsMarker)
                    {
                        m_LastShowMarkers = m_RenderingSystem.markersVisible;
                    }

                    m_RenderingSystem.markersVisible = true;
                    m_PrefabIsMarker = true;
                }
                else if (m_PrefabIsMarker)
                {
                    m_PrefabIsMarker = false;
                    m_RenderingSystem.markersVisible = m_LastShowMarkers;
                }
            }
            else if (m_PrefabIsMarker)
            {
                m_PrefabIsMarker = false;
                m_RenderingSystem.markersVisible = m_LastShowMarkers;
            }

            m_LastTool = tool.toolID;
        }

        // Shows markers if appropriate prefab is selected.
        private void OnPrefabChanged(PrefabBase prefab)
        {
            m_Log.Debug($"{nameof(AnarchyUISystem)}.{nameof(OnPrefabChanged)} {prefab.name}");

            Entity prefabEntity = m_PrefabSystem.GetEntity(prefab);
            if (EntityManager.HasComponent<MarkerNetData>(prefabEntity))
            {
                if (!m_PrefabIsMarker)
                {
                    m_LastShowMarkers = m_RenderingSystem.markersVisible;
                }

                m_RenderingSystem.markersVisible = true;
                m_PrefabIsMarker = true;
            }
            else if (m_PrefabIsMarker)
            {
                m_PrefabIsMarker = false;
                m_RenderingSystem.markersVisible = m_LastShowMarkers;
            }
        }
    }
}
