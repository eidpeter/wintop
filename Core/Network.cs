using System.Collections.Generic;
using System.Linq;

namespace wintop.Core
{
    public class Network
    {
        public List<NetworkDetails> Interfaces { get; set; } = new List<NetworkDetails>();

        public long BytesSent => Interfaces.Sum(i => i.BytesSent);

        public long BytesReceived => Interfaces.Sum(i => i.BytesReceived);
    }
}