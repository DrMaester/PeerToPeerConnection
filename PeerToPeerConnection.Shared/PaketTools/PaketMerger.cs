using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PeerToPeerConnection.Shared.Model;

namespace PeerToPeerConnection.Shared.PaketTools
{
    public static class PaketMerger
    {
        public static byte[] MergePakets(Paket[] pakets)
        {
            return pakets.SelectMany(p => p.Data).ToArray();
        }
    }
}
