using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aggregator.Core.Domain;

namespace Aggregator.Core.Abstractions;

public interface IAggregationService
{
    Task<IReadOnlyList<AggregatedItem>> AggregateAsync(
        AggregationContext context,
        IEnumerable<IExternalProvider> providers,
        CancellationToken ct);
}