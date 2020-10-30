using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Nfield.Quota.Helpers;

namespace Nfield.Quota
{
    public sealed class QuotaVariableDefinitionCollection : List<QuotaVariableDefinition>
    {
        public QuotaVariableDefinitionCollection(IEnumerable<QuotaVariableDefinition> collection) : base(collection)
        {
        }

        public QuotaVariableDefinitionCollection()
        {
        }

        [SuppressMessage("Blocker Code Smell", "S3875:\"operator==\" should not be overloaded on reference types", Justification = "The operator compares references with null")]
        public static bool operator ==(QuotaVariableDefinitionCollection left, QuotaVariableDefinitionCollection right)
        {
            return left?.Equals(right) ?? ReferenceEquals(right, null);
        }

        public static bool operator !=(QuotaVariableDefinitionCollection left, QuotaVariableDefinitionCollection right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            var result = 0;
            foreach (var variableDefinition in this)
            {
                result ^= variableDefinition.GetHashCode();
            }
            return result;
        }

        public override bool Equals(object obj)
        {
            var other = obj as QuotaVariableDefinitionCollection;
            return Equals(other);
        }

        public bool Equals(ICollection<QuotaVariableDefinition> other)
        {
            if (ReferenceEquals(this, other)) return true;
            return !ReferenceEquals(other, null) && this.ScrambledDefinitionsEquals(other);
        }
    }
}