using Unity.Services.Core.Editor;
using Unity.Services.Core.Editor.OrganizationHandler;
using UnityEditor;

namespace Unity.Services.Analytics.Editor.Settings
{
    struct AnalyticsIdentifier : IEditorGameServiceIdentifier
    {
        public string GetKey() => "Analytics - Gaming Services";
    }

    class AnalyticsEditorGameService : IEditorGameService
    {
        public string Name => "Analytics - Gaming Services";
        public IEditorGameServiceIdentifier Identifier => k_Identifier;
        public bool RequiresCoppaCompliance => false;
        public bool HasDashboard => true;
        public IEditorGameServiceEnabler Enabler => null;

        static readonly AnalyticsIdentifier k_Identifier = new AnalyticsIdentifier();

        public string GetFormattedDashboardUrl()
        {
            return $"https://dashboard.unity3d.com/organizations/{OrganizationProvider.Organization.Key}/projects/{CloudProjectSettings.projectId}/analytics/v2/events";
        }
    }
}
