using System.Collections.Generic;
using Stryker.Core.Mutants;

namespace Stryker.Core.Reporters.Html.Realtime
{
    public interface IRealtimeMutantHandler
    {
        int Port { get; }
        void OpenSseEndpoint();
        void CloseSseEndpoint();
        void SendMutantTestedEvent(IReadOnlyMutant testedMutant);
        IEnumerable<IReadOnlyMutant> GetTestedMutants();
    }
}
