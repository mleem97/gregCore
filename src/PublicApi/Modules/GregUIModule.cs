using gregCore.UI;
using UnityEngine;

namespace gregCore.PublicApi.Modules
{
    public sealed class GregUIModule
    {
        private readonly GregApiContext _ctx;
        internal GregUIModule(GregApiContext ctx) => _ctx = ctx;

        public GregUIBuilder CreateBuilder(string title) => GregUIBuilder.Create(title);

        public void ShowNotification(string message, float duration = 3f)
        {
            // Integration with notification service
            // GregServiceContainer.Get<Infrastructure.Settings.Services.GregNotificationService>()?.Show(message, duration);
        }
    }
}
