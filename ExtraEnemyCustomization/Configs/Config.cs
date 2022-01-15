using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace EECustom.Configs
{
    public abstract class Config
    {
        [JsonIgnore]
        public abstract string FileName { get; }

        public virtual void Loaded()
        {

        }

        public virtual void Unloaded()
        {

        }
    }
}
