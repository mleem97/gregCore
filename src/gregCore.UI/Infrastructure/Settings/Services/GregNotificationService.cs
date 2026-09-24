using System;
using UnityEngine;
using gregCore.Core.Abstractions;
using gregCore.UI;

namespace gregCore.Infrastructure.Settings.Services
{
    public class GregNotificationService
    {
        private readonly IGregLogger _logger;
        public GregNotificationService(IGregLogger logger)
        {
            _logger = logger.ForContext("NotificationService");
        }

        public void Show(string title, string message, float duration = 5f)
        {
            _logger.Info($"Notification: {title} - {message}");
            GregNotificationManager.Show(string.IsNullOrWhiteSpace(title) ? message : $"{title}: {message}", duration);
        }
    }
}
