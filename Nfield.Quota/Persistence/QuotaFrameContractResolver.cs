using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nfield.Quota.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Nfield.Quota.Persistence
{
    /// <summary>
    /// Special JsonConvert resolver that allows you to ignore properties.  See http://stackoverflow.com/a/13588192/1037948
    /// </summary>
    public class QuotaFrameContractResolver
        : CamelCasePropertyNamesContractResolver
    {
        private readonly Dictionary<Type, HashSet<string>> _ignores;

        public QuotaFrameContractResolver()
        {
            _ignores = new Dictionary<Type, HashSet<string>>();
        }

        /// <summary>
        /// Explicitly ignore the given property(s) for the given type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propertyName">one or more properties to ignore.  Leave empty to ignore the type entirely.</param>
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "We want propertyName in lowercase and we don't care about locale")]
        public void Ignore(Type type, params string[] propertyName)
        {
            if(propertyName is null)
            {
                return;
            }

            // start bucket if DNE
            if (!_ignores.ContainsKey(type))
            {
                _ignores[type] = new HashSet<string>();
            }

            foreach (var prop in propertyName)
            {
                _ignores[type].Add(prop.ToLowerInvariant());
            }
        }

        /// <summary>
        /// Is the given property for the given type ignored?
        /// </summary>
        public bool IsIgnored(Type type, string propertyName)
        {
            if (!_ignores.ContainsKey(type))
            {
                return false;
            }

            // if no properties provided, ignore the type entirely
            if (_ignores[type].Count == 0)
            {
                return true;
            }

            return _ignores[type].Contains(propertyName);
        }


        /// <summary>
        /// The decision logic goes here
        /// </summary>
        [SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "We want propertyName in lowercase and we don't care about locale")]
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var propertyName = property.PropertyName.ToLowerInvariant();

            if (IsIgnored(property.DeclaringType, propertyName)
                // need to check basetype as well for EF -- @per comment by user576838
                || IsIgnored(property.DeclaringType.BaseType, propertyName))
            {
                property.ShouldSerialize = instance => false;
            }

            return property;
        }
    }
}
