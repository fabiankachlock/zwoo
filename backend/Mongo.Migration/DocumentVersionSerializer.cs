using Mongo.Migration.Documents;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Mongo.Migration
{
    public class DocumentVersionSerializer : SerializerBase<DocumentVersion>
    {
        public override DocumentVersion Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args) => new DocumentVersion(context.Reader.ReadString());

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, DocumentVersion value) => context.Writer.WriteString(value.ToString());
    }
}