using System.Collections.Generic;
using Nfield.Quota.Helpers;

namespace Nfield.Quota
{
    public class QuotaVariableDefinitionCollection : List<QuotaVariableDefinition>
    {
        public QuotaVariableDefinitionCollection(IEnumerable<QuotaVariableDefinition> collection) : base(collection)
        {
        }

        public QuotaVariableDefinitionCollection()
        {
        }

        public static bool operator ==(QuotaVariableDefinitionCollection left, QuotaVariableDefinitionCollection right)
        {
            return left?.Equals(right) ?? ReferenceEquals(right, null);
        }

        public static bool operator !=(QuotaVariableDefinitionCollection left, QuotaVariableDefinitionCollection right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            var other = obj as QuotaVariableDefinitionCollection;
            return Equals(other);
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

        public bool Equals(ICollection<QuotaVariableDefinition> other)
        {
            if (ReferenceEquals(this, other)) return true;
            return !ReferenceEquals(other, null) && this.ScrambledDefinitionsEquals(other);
        }
    }
}