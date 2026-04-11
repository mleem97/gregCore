using System.Reflection;
using System.Text;

namespace gregSdk;

/// <summary>
/// Helpers for anonymous payload objects passed through <see cref="gregEventDispatcher"/>.
/// </summary>
public static class gregPayload
{
    public static T Get<T>(object payload, string fieldName, T fallback = default)
    {
        if (payload == null || string.IsNullOrEmpty(fieldName))
            return fallback;

        try
        {
            var prop = payload.GetType().GetProperty(
                fieldName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop == null)
                return fallback;

            var val = prop.GetValue(payload);
            if (val is T typed)
                return typed;

            if (val == null)
                return fallback;

            try
            {
                return (T)System.Convert.ChangeType(val, typeof(T));
            }
            catch
            {
                return fallback;
            }
        }
        catch
        {
            return fallback;
        }
    }

    public static string Dump(object payload)
    {
        if (payload == null)
            return "<null>";

        var sb = new StringBuilder("{");
        foreach (var prop in payload.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            try
            {
                sb.Append(' ').Append(prop.Name).Append('=').Append(prop.GetValue(payload));
            }
            catch
            {
                sb.Append(' ').Append(prop.Name).Append("=<error>");
            }
        }

        sb.Append(" }");
        return sb.ToString();
    }
}


