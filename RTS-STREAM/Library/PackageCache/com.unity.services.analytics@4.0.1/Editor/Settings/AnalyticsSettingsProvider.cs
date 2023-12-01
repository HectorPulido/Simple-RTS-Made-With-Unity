using Unity.Services.Core.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.Services.Analytics.Editor.Settings
{
    class AnalyticsSettingsProvider : EditorGameServiceSettingsProvider
    {
        const string k_Title = "Analytics - Gaming Services";
        const string k_GoToDashboardContainer = "dashboard-button-container";
        const string k_GoToDashboardBtn = "dashboard-link-button";

        protected override IEditorGameService EditorGameService => k_GameService;
        protected override string Title => k_Title;
        protected override string Description => "Analytics enables you to easily understand game performance and player behaviors so you can make strategic decisions.";

        static readonly AnalyticsEditorGameService k_GameService = new AnalyticsEditorGameService();

        AnalyticsSettingsProvider(SettingsScope scopes)
            : base(GenerateProjectSettingsPath(k_Title), scopes) { }

        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            return new AnalyticsSettingsProvider(SettingsScope.Project);
        }

        // This method must be implemented as part of EditorGameServiceSettingsProvider.
        // It is used to create UI elements in the window, but there's nothing to add atm so
        // it is essentially empty.
        protected override VisualElement GenerateServiceDetailUI()
        {
            var containerVisualElement = new VisualElement();

            return containerVisualElement;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);
            SetDashboardButton(rootElement);
        }

        static void SetDashboardButton(VisualElement rootElement)
        {
            rootElement.Q(k_GoToDashboardContainer).style.display = DisplayStyle.Flex;
            var goToDashboard = rootElement.Q(k_GoToDashboardBtn);

            if (goToDashboard != null)
            {
                var clickable = new Clickable(() =>
                {
                    Application.OpenURL(k_GameService.GetFormattedDashboardUrl());
                });
                goToDashboard.AddManipulator(clickable);
            }
        }
    }
}
