using System;

namespace Nfield.Quota
{
    internal interface IQuotaCell
    {
        string Name { get; }
        int? Target { get; }
        int? MaxTarget { get; }
        Guid Id { get; }
    }
}