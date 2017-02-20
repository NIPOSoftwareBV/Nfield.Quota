using System.Collections.Generic;
using Nfield.Quota.Helpers;

namespace Nfield.Quota
{
    public class QuotaVariableDefinitionCollection : List<QuotaVariableDefinition>
    {
        public ICollection<QuotaVariableDefinition> Variables { get; }
        public QuotaVariableDefinitionCollection()
        {
            Variables = new List<QuotaVariableDefinition>();
        }

        public QuotaVariableDefinitionCollection(IEnumerable<QuotaVariableDefinition> variables)
        {
            Variables = new List<QuotaVariableDefinition>(variables);
        }
        
        public static bool operator ==(QuotaVariableDefinitionCollection left, QuotaVariableDefinitionCollection right)
        {
            return left?.Equals(right) ?? false;
        }

        public static bool operator !=(QuotaVariableDefinitionCollection left, QuotaVariableDefinitionCollection right)
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

        public bool Equals(ICollection<QuotaVariableDefinition> other)
        {
            if (ReferenceEquals(this, other)) return true;
            return !ReferenceEquals(other, null) && Variables.ScrambledDefinitionsEquals(other);
        }
    }
}