using System;
using System.Collections.Generic;
using Nfield.Quota.Helpers;

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

        public bool? IsSelectionOptional { get; set; }

        /// <summary>
        /// Indication more then one selected level allowed
        /// </summary>
        public bool IsMulti { get; set; }

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
            return Equals(other);
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

            return Id == other.Id
                   && Name == other.Name
                   && OdinVariableName == other.OdinVariableName
                   && IsSelectionOptional == other.IsSelectionOptional
                   && Levels.ScrambledDefinitionsEquals(other.Levels);
        }
    }
}