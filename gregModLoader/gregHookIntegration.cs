using System;
using System.Text;
using gregSdk;

namespace gregModLoader;

/// <summary>
/// Emits canonical greg.* events for the native FFI pipeline. Hook names come from
/// <see cref="gregNativeEventHooks"/> (aligned with greg_hooks.json where listed).
/// </summary>
internal static class GregHookIntegration
{
    internal static void EmitForSimple(uint eventId)
    {
        if (!gregNativeEventHooks.TryGetHookForEvent(eventId, out var greg))
            return;

        gregEventDispatcher.Emit(greg, new GregSimplePayload(eventId));
    }

    internal static void EmitForStruct<T>(uint eventId, T data) where T : struct
    {
        if (!gregNativeEventHooks.TryGetHookForEvent(eventId, out var greg))
            return;

        object payload = BuildPayload(eventId, data);
        gregEventDispatcher.Emit(greg, payload);
    }

    private static object BuildPayload<T>(uint eventId, T data) where T : struct
    {
        switch (eventId)
        {
            case EventIds.MoneyChanged:
                if (data is ValueChangedData mv)
                    return new
                    {
                        coinChangeAmount = (float)mv.Delta,
                        newBalance = (float)mv.NewValue,
                        withoutSound = false,
                        accepted = true
                    };
                break;
            case EventIds.XPChanged:
                if (data is ValueChangedData xv)
                    return new { amount = (float)xv.Delta, xp = (float)xv.NewValue, accepted = true };
                break;
            case EventIds.ReputationChanged:
                if (data is ValueChangedData rv)
                    return new { amount = (float)rv.Delta, reputation = (float)rv.NewValue };
                break;
            case EventIds.ServerPowered:
                if (data is ServerPoweredData s)
                    return new { IsOn = s.PoweredOn != 0 };
                break;
            case EventIds.DayEnded:
                if (data is DayEndedData d)
                    return new { Day = d.Day };
                break;
            case EventIds.CustomerAccepted:
                if (data is CustomerAcceptedData c)
                    return new { CustomerId = c.CustomerId };
                break;
            case EventIds.CustomerSatisfied:
            case EventIds.CustomerUnsatisfied:
                if (data is CustomerSatisfiedData cs)
                    return new { CustomerBaseId = cs.CustomerBaseId };
                break;
            case EventIds.ServerCustomerChanged:
                if (data is ServerCustomerChangedData sc)
                    return new { NewCustomerId = sc.NewCustomerId };
                break;
            case EventIds.ServerAppChanged:
                if (data is ServerAppChangedData sa)
                    return new { NewAppId = sa.NewAppId };
                break;
            case EventIds.MonthEnded:
                if (data is MonthEndedData m)
                    return new { Month = m.Month };
                break;
            case EventIds.ShopItemAdded:
                if (data is ShopItemAddedData sh)
                    return new { ItemId = sh.ItemId, Price = sh.Price, ItemType = sh.ItemType };
                break;
            case EventIds.ShopItemRemoved:
                if (data is ShopItemRemovedData sr)
                    return new { Uid = sr.Uid };
                break;
            case EventIds.NetWatchDispatched:
                if (data is NetWatchDispatchedData nw)
                    return new { DeviceType = nw.DeviceType, Reason = nw.Reason };
                break;
            case EventIds.CustomEmployeeHired:
            case EventIds.CustomEmployeeFired:
                if (data is CustomEmployeeEventData ce)
                    return new { EmployeeId = DecodeEmployeeId(ce.EmployeeId) };
                break;
        }

        return new { EventId = eventId, Data = data.ToString() };
    }

    private static string DecodeEmployeeId(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
            return string.Empty;

        var len = Array.IndexOf(bytes, (byte)0);
        if (len < 0)
            len = bytes.Length;
        return Encoding.ASCII.GetString(bytes, 0, Math.Min(len, bytes.Length));
    }

    private sealed class GregSimplePayload
    {
        public GregSimplePayload(uint eventId)
        {
            EventId = eventId;
        }

        public uint EventId { get; }
    }
}



