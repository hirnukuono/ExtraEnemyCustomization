using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace EECustom.Configs
{
    public abstract class Config
    {
        [JsonIgnore]
        public abstract string FileName { get; }

        public virtual Config CreateBlankConfig()
        {
            var config = Activator.CreateInstance(GetType()) as Config;
            SetDefaultArrayPropertyValue(config, GetType());

            return config;
        }

        private void SetDefaultArrayPropertyValue(object obj, Type type)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (!property.CanWrite)
                    continue;

                var propertyType = property.PropertyType;
                if (!propertyType.IsArray)
                {
                    if (!propertyType.IsValueType)
                        SetDefaultArrayPropertyValue(property.GetValue(obj), propertyType);
                    continue;
                }

                if (!propertyType.HasElementType)
                    continue;

                var elementType = propertyType.GetElementType();
                try
                {
                    var array = Array.CreateInstance(elementType, 1);
                    var instance = Activator.CreateInstance(elementType);
                    array.SetValue(instance, 0);
                    property.SetValue(obj, array);

                    SetDefaultArrayPropertyValue(instance, elementType);
                }
                catch (Exception)
                {
                    //Logger.Error(e.ToString());
                }
                finally
                {
                }
            }
        }

        public virtual void Loaded()
        {

        }

        public virtual void Unloaded()
        {

        }
    }
}
