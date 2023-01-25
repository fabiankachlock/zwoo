using Mongo.Migration.Documents;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Mongo.Migration
{
    public class DocumentVersionSerializer : SerializerBase<DocumentVersion>
    {
        public static string DefaultVersion { get; set; }

        public override DocumentVersion Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            string version = context.Reader.ReadString();
            return new DocumentVersion(string.IsNullOrEmpty(version) ? DefaultVersion : version);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DocumentVersion value)
        {
            string version = value.ToString();
            context.Writer.WriteString(string.IsNullOrEmpty(version) ? DefaultVersion : version);
        }
    }
}