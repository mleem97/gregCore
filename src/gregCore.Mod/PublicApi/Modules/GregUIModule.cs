using System;
using gregCore.UI;
using UnityEngine;

namespace gregCore.PublicApi.Modules
{
    public sealed class GregUIModule
    {
        private readonly GregApiContext _ctx;
        internal GregUIModule(GregApiContext ctx) => _ctx = ctx;

        public GregUIBuilder CreateBuilder(string title) => GregUIBuilder.Create(title);

        public void ShowNotification(string message)
        {
            ShowNotification(message, 3f);
        }

        public void ShowNotification(string message, float duration)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            GregNotificationManager.Show(message, Math.Max(0.25f, duration));
        }
    }
}
