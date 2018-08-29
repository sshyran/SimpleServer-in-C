#region

using System.Collections.Generic;

#endregion

namespace Ultz.SimpleServer.Common
{
    // 1. Read the Type attribute
    // 2. Try to resolve a Type corresponding to the namespace in the type attribute
    // 3. Find out which IValves the Type can be casted to
    // 4. Execute the appropriate method
    public class Valve
    {
        public string Key { get; set; }
        public List<Setting> Settings { get; set; }
    }

    public interface IValve
    {
        string Id { get; }
    }

    public interface IValve<in T> : IValve where T : IConfigurable
    {
        void Execute(T obj, Dictionary<string, string> settings);
    }
}