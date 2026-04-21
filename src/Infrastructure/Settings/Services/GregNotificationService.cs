using System;
using System.Collections.Generic;
using UnityEngine;
using gregCore.Core.Abstractions;

namespace gregCore.Infrastructure.Settings.Services;

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
        _activeNotifications.Add(new Notification { Title = title, Message = message, Expiration = Time.time + duration });
        _logger.Info($"Notification: {title} - {message}");
    }

    public void OnGUI()
    {
        int y = Screen.height - 100;
        foreach (var notification in _activeNotifications.ToArray())
        {
            if (Time.time > notification.Expiration)
            {
                _activeNotifications.Remove(notification);
                continue;
            }

            GUI.Box(new Rect(Screen.width - 320, y, 300, 60), $"{notification.Title}\n{notification.Message}");
            y -= 70;
        }
    }

    private class Notification
    {
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public float Expiration { get; set; }
    }
}
