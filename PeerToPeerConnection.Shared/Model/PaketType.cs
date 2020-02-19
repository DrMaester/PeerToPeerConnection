using System;
using System.Collections.Generic;
using System.Text;

namespace PeerToPeerConnection.Shared.Model
{
    public enum PaketType
    {
        Connect,
        RequestExternalAddress,
        AnswerExternalAddress,
        Greeting
    }
}
