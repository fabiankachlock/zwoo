using System;
using Semver;

namespace Mongo.Migration.Documents
{
    public struct DocumentVersion : IComparable<DocumentVersion>
    {
        public readonly SemVersion Version;

        public DocumentVersion(string version) => Version = SemVersion.Parse(version, SemVersionStyles.Strict);
        
        public static DocumentVersion Default() => Empty();

        public static DocumentVersion Empty()
        {
            return new DocumentVersion("0.0.1");
        }

        public static implicit operator DocumentVersion(string version)
        {
            return new DocumentVersion(version);
        }

        public static implicit operator string(DocumentVersion documentVersion)
        {
            return documentVersion.ToString();
        }

        public override string ToString() => Version?.ToString() ?? "";

        #region compare

        public int CompareTo(DocumentVersion other)
        {
            if (Equals(other))
            {
                return 0;
            }

            return this > other ? 1 : -1;
        }

        public static bool operator ==(DocumentVersion a, DocumentVersion b) => a.Equals(b);

        public static bool operator !=(DocumentVersion a, DocumentVersion b) => !(a == b);

        public static bool operator >(DocumentVersion a, DocumentVersion b) => a.Version.CompareSortOrderTo(b.Version) > 0;

        public static bool operator <(DocumentVersion a, DocumentVersion b) => a != b && !(a > b);

        public static bool operator <=(DocumentVersion a, DocumentVersion b) => a == b || a < b;

        public static bool operator >=(DocumentVersion a, DocumentVersion b) => a == b || a > b;

        public bool Equals(DocumentVersion other) => Version == other.Version;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            if (obj.GetType() != typeof(DocumentVersion))
                return false;

            return Equals((DocumentVersion)obj);
        }

        public override int GetHashCode() => Version.GetHashCode();

        #endregion
    }
}