using System.Text.Json.Serialization;

namespace _Updater
{
    [JsonSerializable(typeof(ProcessArgs))]
    internal partial class ProcessArgsJsonContext : JsonSerializerContext
    {
    }
}