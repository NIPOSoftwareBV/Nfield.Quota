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
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }

            return left.Equals(right);
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
            return Id.GetHashCode() ^ Name.GetHashCode() ^ OdinVariableName.GetHashCode();
        }

        public bool Equals(QuotaVariableDefinition other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            
            return Id == other.Id
                   && Name == other.Name
                   && OdinVariableName == other.OdinVariableName
                   && IsSelectionOptional == other.IsSelectionOptional
                   && IsMulti == other.IsMulti
                   && Levels.ScrambledDefinitionsEquals(other.Levels);
        }
    }
}