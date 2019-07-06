namespace Go.Editor.Intellisense.QuickInfo
{
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;

    [DataContract]
    public sealed class GoGetDocJsonDataContract
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "import")]
        public string Import { get; set; }

        [DataMember(Name = "pkg")]
        public string Package { get; set; }

        [DataMember(Name = "decl")]
        public string Declaration { get; set; }

        [DataMember(Name = "doc")]
        public string Documentation { get; set; }

        [DataMember(Name = "pos")]
        public string Position { get; set; }

        public static GoGetDocJsonDataContract ReadToObject(string json)
        {
            var deserializedDocumentation = new GoGetDocJsonDataContract();
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var ser = new DataContractJsonSerializer(deserializedDocumentation.GetType());
            deserializedDocumentation = ser.ReadObject(ms) as GoGetDocJsonDataContract;
            ms.Close();
            return deserializedDocumentation;
        }
    }
}
