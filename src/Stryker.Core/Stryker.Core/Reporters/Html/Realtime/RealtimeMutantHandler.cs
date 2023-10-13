using Stryker.Core.Mutants;
using System.Collections.Generic;

namespace Stryker.Core.Reporters.Html.Realtime
{
    public class RealtimeMutantHandler : IRealtimeMutantHandler
    {
        public int Port => _server.Port;
        private readonly ISseServer _server;
        private readonly List<SseEvent<JsonMutant>> _bufferedEvents = new List<SseEvent<JsonMutant>>();
        private bool _clientConnected = false;

        public RealtimeMutantHandler(StrykerOptions options, ISseServer server = null)
            => _server = server ?? new SseServer();

        public void OpenSseEndpoint()
        {
            _clientConnected = true; // Detecteer dat de client is verbonden
            _server.OpenSseEndpoint();

            // Verzend gebufferde gebeurtenissen naar de client
            foreach (var bufferedEvent in _bufferedEvents)
            {
                _server.SendEvent(bufferedEvent);
            }
            _bufferedEvents.Clear();
        }

        public void CloseSseEndpoint()
        {
            _server.SendEvent(new SseEvent<string> { Event = SseEventType.Finished, Data = "" });
            _server.CloseSseEndpoint();
        }

        public void SendMutantTestedEvent(IReadOnlyMutant testedMutant)
        {
            var jsonMutant = new JsonMutant(testedMutant);
            var mutantEvent = new SseEvent<JsonMutant> { Event = SseEventType.MutantTested, Data = jsonMutant };

            if (_clientConnected)
            {
                _server.SendEvent(mutantEvent);
            }
            else
            {
                _bufferedEvents.Add(mutantEvent);
            }
        }
    }
}
