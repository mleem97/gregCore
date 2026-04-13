using System;
using System.Collections.Generic;
using MelonLoader;

namespace greg.Sdk.Services;

/// <summary>
/// A task data container transferred across the mod framework.
/// </summary>
public class GregAssignedTask
{
    public string SkillName { get; set; }
    public string Description { get; set; }
    public string Payload { get; set; }
    public int Priority { get; set; }
}

/// <summary>
/// Centralized message-bus for queueing tasks to worker entities (like Gregs).
/// Mods can push tasks here. Subscribing Mods (like GregifyEmployees) will
/// pull tasks from this queue and execute them.
/// </summary>
public static class GregTaskQueueService
{
    private static readonly Queue<GregAssignedTask> _taskQueue = new();

    /// <summary>Queue an external task for any available worker.</summary>
    public static void QueueTask(string skillName, string description, string payload = null, int priority = 25)
    {
        if (string.IsNullOrWhiteSpace(skillName)) return;

        _taskQueue.Enqueue(new GregAssignedTask
        {
            SkillName = skillName,
            Description = description,
            Payload = payload,
            Priority = priority
        });

        MelonLogger.Msg($"[GregTaskQueue] Queued task: {skillName} - {description}");
    }

    /// <summary>
    /// Check if there are any tasks in the queue.
    /// </summary>
    public static bool HasPendingTasks => _taskQueue.Count > 0;

    /// <summary>
    /// Dequeue the next task.
    /// </summary>
    public static GregAssignedTask DequeueTask()
    {
        if (_taskQueue.Count > 0)
            return _taskQueue.Dequeue();
        return null;
    }

    /// <summary>
    /// Peek at the current queue count.
    /// </summary>
    public static int QueueCount => _taskQueue.Count;

    /// <summary>
    /// Re-queue a task if a worker could not fulfill it (e.g., missing skill).
    /// </summary>
    public static void RequeueTask(GregAssignedTask task)
    {
        if (task != null)
            _taskQueue.Enqueue(task);
    }
}

