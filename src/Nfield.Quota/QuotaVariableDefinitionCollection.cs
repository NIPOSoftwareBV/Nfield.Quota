using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace Nfield.Quota
{
    public class QuotaVariableDefinitionCollection : Collection<QuotaVariableDefinition>
    {
        private readonly QuotaFrame _quotaFrame;

        public QuotaVariableDefinitionCollection()
        {
            
        }

        public QuotaVariableDefinitionCollection(QuotaFrame quotaFrame)
        {
            _quotaFrame = quotaFrame;
        }

        public bool IsVariableDefined(string variableDefinitionId)
        {
            return Items.Any(item => item.Id == variableDefinitionId);
        }

        protected override void InsertItem(int index, QuotaVariableDefinition item)
        {
            if (Items.Any(v => v.Id == item.Id))
                throw new ArgumentException(
                    string.Format(CultureInfo.InvariantCulture, "Quota frame already contains a variable with the same ID: '{0}'.", item.Id)
                    );

            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            // TODO remove all the references
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, QuotaVariableDefinition item)
        {
            // TODO remove all the references to the replaced item
            base.SetItem(index, item);
        }

        protected override void ClearItems()
        {
            // TODO clear out all the references
            base.ClearItems();
        }

    }
}
