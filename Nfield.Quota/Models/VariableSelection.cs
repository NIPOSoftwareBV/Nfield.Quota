namespace Nfield.Quota.Models
{
    public enum VariableSelection
    {
        /// <summary>
        /// Selection is not possible
        /// </summary>
        NotApplicable = 0,

        /// <summary>
        /// Selection can be skipped
        /// </summary>
        Optional = 1,

        /// <summary>
        /// Selection is mandatory
        /// </summary>
        Mandatory = 2,


    }
}