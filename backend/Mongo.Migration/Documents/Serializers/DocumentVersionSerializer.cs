using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Mongo.Migration.Documents.Serializers
{
    public class DocumentVersionSerializer : SerializerBase<DocumentVersion>
    {
        public override void Serialize(
            BsonSerializationContext context,
            BsonSerializationArgs args,
            DocumentVersion value)
        {
            context.Writer.WriteString(value.ToString());
        }

        public override DocumentVersion Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var versionString = context.Reader.ReadString();
            return new DocumentVersion(versionString);
        }
    }
}