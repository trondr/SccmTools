using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SccmTools.Library.Module.Commands.CreateApplication2
{
    public class DetectionMethodFile
    {
        public string FileName { get; set; }

        public Version FileVersion { get; set; }

        public DateTime Modified { get; set; }

        public DateTime Created { get; set; }

        public long SizeInBytes { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public FileRuleType RuleType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public RuleOperator RuleOperator { get; set; }
    }
}