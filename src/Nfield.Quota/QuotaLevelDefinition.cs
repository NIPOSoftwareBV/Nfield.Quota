using System;

namespace Nfield.Quota
{
    public class QuotaLevelDefinition : IEquatable<QuotaLevelDefinition>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public static bool operator ==(QuotaLevelDefinition existingDefinition, QuotaLevelDefinition newDefinition)
        {
            return existingDefinition?.Equals(newDefinition) ?? false;
        }

        public static bool operator !=(QuotaLevelDefinition existingDefinition, QuotaLevelDefinition newDefinition)
        {
            return !(existingDefinition == newDefinition);
        }

        public override bool Equals(object obj)
        {
            var other = obj as QuotaLevelDefinition;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Name.GetHashCode();
        }

        public bool Equals(QuotaLevelDefinition other)
        {
            return other != null && (Id == other.Id) && (Name == other.Name);
        }
    }
}