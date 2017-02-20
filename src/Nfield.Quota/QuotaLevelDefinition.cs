using System;

namespace Nfield.Quota
{
    public class QuotaLevelDefinition : IEquatable<QuotaLevelDefinition>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public static bool operator ==(QuotaLevelDefinition left, QuotaLevelDefinition right)
        {
            return left?.Equals(right) ?? false;
        }

        public static bool operator !=(QuotaLevelDefinition left, QuotaLevelDefinition right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            var other = obj as QuotaLevelDefinition;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        { 
            // we can't do better than this
            return base.GetHashCode();
        }

        public bool Equals(QuotaLevelDefinition other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(other, null)) return false;

            return (Id == other.Id) && (Name == other.Name);
        }
    }
}