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
        
        public static bool operator ==(QuotaVariableDefinition existingDefinition, QuotaVariableDefinition newDefinition)
        {
            return existingDefinition?.Equals(newDefinition) ?? false;
        }

        public static bool operator !=(QuotaVariableDefinition existingDefinition, QuotaVariableDefinition newDefinition)
        {
            return !(existingDefinition == newDefinition);
        }

        public override bool Equals(object obj)
        {
            var other = obj as QuotaVariableDefinition;
            return other != null && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Name.GetHashCode() ^ OdinVariableName.GetHashCode() ^ Levels.GetHashCode();
        }

        public bool Equals(QuotaVariableDefinition other)
        {
            return other != null 
                && (Id == other.Id) 
                && (Name == other.Name) 
                && (OdinVariableName == other.OdinVariableName) 
                && ScrambledLevelsEquals(Levels,other.Levels);
        }
        private static bool ScrambledLevelsEquals<T>(IEnumerable<T> existingDefinition, IEnumerable<T> newDefinition)
        {
            var levels = new Dictionary<T, int>();
            foreach (var ed in existingDefinition)
            {
                if (levels.ContainsKey(ed))
                {
                    levels[ed]++;
                }
                else
                {
                    levels.Add(ed, 1);
                }
            }
            foreach (var nd in newDefinition)
            {
                if (levels.ContainsKey(nd))
                {
                    levels[nd]--;
                }
                else
                {
                    return false;
                }
            }
            return levels.Values.All(qld=> qld == 0);
        }
       
    }
}