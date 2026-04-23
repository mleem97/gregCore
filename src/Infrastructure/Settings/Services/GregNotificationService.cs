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
        
        // Build UGUI Notification
        var builder = gregCore.UI.GregUIBuilder.Create($"Notify_{Guid.NewGuid()}")
            .SetSize(300, 60);
        builder.AddLabel($"{title}\n{message}");
        var obj = builder.Build();
        
        // Position at bottom-right
        var rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(1, 0);
        rt.anchorMax = new Vector2(1, 0);
        rt.pivot = new Vector2(1, 0);
        rt.anchoredPosition = new Vector2(-20, 20 + (_activeNotifications.Count * 70));
        
        // Auto-destroy after duration
        UnityEngine.Object.Destroy(obj, duration);
    }

    public void OnGUI()
    {
        // IMGUI disabled
    }

    private class Notification
    {
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public float Expiration { get; set; }
    }
}
