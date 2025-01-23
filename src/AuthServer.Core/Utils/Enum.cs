using System.ComponentModel;
using System.Reflection;

public static class EnumMethods
{
    public static T ParseFromDescription<T>(string description) where T : Enum
    {
        foreach (var field in typeof(T).GetFields())
        {
            var descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>();
            if (descriptionAttribute?.Description.Equals(description, StringComparison.OrdinalIgnoreCase) == true)
            {
                return (T)field.GetValue(null);
            }
        }
        
        throw new ArgumentException($"Invalid value '{description}' for enum of type {typeof(T).Name}", nameof(description));
    }
}