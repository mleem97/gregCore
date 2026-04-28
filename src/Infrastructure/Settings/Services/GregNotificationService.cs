using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using gregCore.Core.Abstractions;

namespace gregCore.Infrastructure.Settings.Services
{
    public class GregNotificationService
    {
        private readonly IGregLogger _logger;
        private readonly List<Notification> _activeNotifications = new();

        public GregNotificationService(IGregLogger logger)
        {
            _logger = logger.ForContext("NotificationService");
        }

        public void Show(string title, string message, float duration = 5f)
        {
            _activeNotifications.Add(new Notification 
            { 
                Title = title, 
                Message = message, 
                Expiration = Time.time + duration 
            });
            _logger.Info($"Notification: {title} - {message}");
            
            BuildNotificationUI(title, message, duration);
        }

        private void BuildNotificationUI(string title, string message, float duration)
        {
            var notification = new VisualElement
            {
                name = $"Notify_{Guid.NewGuid()}",
                style =
                {
                    width = 300,
                    backgroundColor = new Color(0.12f, 0.12f, 0.12f, 0.95f),
                    borderTopColor = GregUITheme.PrimaryAccent,
                    borderBottomColor = GregUITheme.PrimaryAccent,
                    borderLeftColor = GregUITheme.PrimaryAccent,
                    borderRightColor = GregUITheme.PrimaryAccent,
                    borderTopWidth = 2,
                    borderBottomWidth = 2,
                    borderLeftWidth = 2,
                    borderRightWidth = 2,
                    borderRadius = 6,
                    paddingTop = 8,
                    paddingBottom = 8,
                    paddingLeft = 10,
                    paddingRight = 10,
                    marginBottom = 8,
                    position = Position.Absolute,
                    right = 20,
                    bottom = 20
                }
            };

            var titleLabel = new Label(title)
            {
                style =
                {
                    fontSize = 14,
                    unityFontStyleAndWeight = FontStyle.Bold,
                    color = GregUITheme.SecondaryColor,
                    marginBottom = 4
                }
            };
            notification.Add(titleLabel);

            var messageLabel = new Label(message)
            {
                style =
                {
                    fontSize = 12,
                    color = new Color(0.88f, 0.88f, 0.88f)
                }
            };
            notification.Add(messageLabel);

            GregUIManager.RegisterPanel(notification.name, notification);
            
            // Auto-remove after duration
            _logger.Info($"Notification UI built: {notification.name}");
        }

        private class Notification
        {
            public string Title { get; set; } = null!;
            public string Message { get; set; } = null!;
            public float Expiration { get; set; }
        }
    }
}
