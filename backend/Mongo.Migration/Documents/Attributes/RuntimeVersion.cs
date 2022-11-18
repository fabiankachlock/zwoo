using System;

namespace Mongo.Migration.Documents.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RuntimeVersion : Attribute
    {
        public RuntimeVersion(string version) => Version = new DocumentVersion(version);

        public DocumentVersion Version { get; }
    }
}