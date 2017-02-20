using System;
using System.Collections.Generic;
using System.Linq;

namespace Nfield.Quota
{
    public class QuotaVariableDefinition : IEquatable<QuotaVariableDefinition>
    {
        public QuotaVariableDefinition()
        {
            Levels = new List<QuotaLevelDefinition>();
        }

        public QuotaVariableDefinition(IEnumerable<QuotaLevelDefinition> levels)
        {
            Levels = new List<QuotaLevelDefinition>(levels);
        }

        public Guid Id { get; set; }
        public string Name { get; set; }

        public string OdinVariableName { get; set; }

        public ICollection<QuotaLevelDefinition> Levels { get; }
        
        public static bool operator ==(QuotaVariableDefinition left, QuotaVariableDefinition right)
        {
            return left?.Equals(right) ?? false;
        }

        public static bool operator !=(QuotaVariableDefinition left, QuotaVariableDefinition right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            var other = obj as QuotaVariableDefinition;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            // we can't do better than this
            return base.GetHashCode();
        }

        public bool Equals(QuotaVariableDefinition other)
        {
            if (ReferenceEquals(this, other)) return true;
            if (ReferenceEquals(other, null)) return false;

            return (Id == other.Id)
                   && (Name == other.Name)
                   && (OdinVariableName == other.OdinVariableName)
                   && ScrambledLevelsEquals(Levels, other.Levels);
        }

        private static bool ScrambledLevelsEquals<T>(ICollection<T> left, ICollection<T> right)
        {
            return left.Count == right.Count && left.All(right.Contains);
        }
    }
}