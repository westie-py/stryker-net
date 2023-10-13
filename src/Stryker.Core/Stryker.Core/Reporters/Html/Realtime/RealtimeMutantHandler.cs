using System.Collections.Generic;
using Stryker.Core.Mutants;

namespace Stryker.Core.Reporters.Html.Realtime
{
    public class RealtimeMutantHandler : IRealtimeMutantHandler
    {
        private readonly ISseServer _server;
        private readonly List<IReadOnlyMutant> _testedMutants = new List<IReadOnlyMutant>();

        public int Port => _server.Port;

        public RealtimeMutantHandler(ISseServer server = null)
            => _server = server ?? new SseServer();

        public void OpenSseEndpoint() => _server.OpenSseEndpoint();

        public void CloseSseEndpoint()
        {
            _server.SendEvent(new SseEvent<string> { Event = SseEventType.Finished, Data = "" });
            _server.CloseSseEndpoint();
        }

        public void SendMutantTestedEvent(IReadOnlyMutant testedMutant)
        {
            var jsonMutant = new JsonMutant(testedMutant);
            _server.SendEvent(new SseEvent<JsonMutant> { Event = SseEventType.MutantTested, Data = jsonMutant });
            _testedMutants.Add(testedMutant);
        }

        public IEnumerable<IReadOnlyMutant> GetTestedMutants()
        {
            return _testedMutants;
        }
    }
    [TestClass]
public class RealtimeMutantHandlerTests
{
    [TestMethod]
    public void SendMutantTestedEvent_ShouldAddMutantToTestedMutantsList()
    {
        // Arrange
        var handler = new RealtimeMutantHandler();
        var mutant = new Mock<IReadOnlyMutant>().Object;

        // Act
        handler.SendMutantTestedEvent(mutant);

        // Assert
        var testedMutants = handler.GetTestedMutants();
        Assert.IsTrue(testedMutants.Contains(mutant));
    }
}
}
