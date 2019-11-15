using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


/// <summary>
/// AlexaCommand wurd zum Serialisieren und Deserialisieren von Aktionen verwendet.
/// Benutzung erfolgt im Skill und in der durch Kestrel gehosteten Anwendung (InitController).
/// </summary>


namespace SqlPassInstallerShared
{
    public partial class AlexaCommand
    {
        [JsonProperty("Type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        [JsonProperty("Payload", NullValueHandling = NullValueHandling.Ignore)]
        public Payload Payload { get; set; }
    }

    public partial class Payload
    {
        [JsonProperty("SqlServerName", NullValueHandling = NullValueHandling.Ignore)]
        public string SqlServerName { get; set; }

        [JsonProperty("IPAddress", NullValueHandling = NullValueHandling.Ignore)]
        public string IPAddress { get; set; }

        [JsonProperty("InstanceName", NullValueHandling = NullValueHandling.Ignore)]
        public string InstanceName { get; set; }

        [JsonProperty("ResultText", NullValueHandling = NullValueHandling.Ignore)]
        public string ResultText { get; set; }
    }

    public partial class AlexaCommand
    {
        public static AlexaCommand FromJson(string json) => JsonConvert.DeserializeObject<AlexaCommand>(json, SqlPassInstallerShared.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this AlexaCommand self) => JsonConvert.SerializeObject(self, SqlPassInstallerShared.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}
