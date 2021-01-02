using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using FIXCommon;

namespace Layer1.QuickFIX
{
  public class CredentialFactory
  {
    private static List<Credential> Credentials = ReceiveCredentials();
    public static Credential GetCredential(Counterpart source)
    {
      switch (source)
      {
        case Counterpart.DBFX:
          throw new NotImplementedException(";)");
        case Counterpart.Dukascopy:
          return Credentials.FirstOrDefault(x => x.Source == source);
        case Counterpart.HotSpotFX:
          throw new NotImplementedException(";)");
        default:
          throw new NotSupportedException("?");
      }
    }

    public static List<Credential> ReceiveCredentials()
    {
      List<Credential> credentials = new List<Credential>();
      credentials.Add(new Credential(Counterpart.Dukascopy, "YQwAm_DEMOFIX", "DUKASCOPYFIX", "FEED_YQwAm_DEMOFIX", "DUKASCOPYFIX"));
#if !CREDENTIALS
#else
      TcpListener listener = new TcpListener(IPAddress.Any, 11111);
      listener.Start();
      using (TcpClient client = listener.AcceptTcpClient())
      {
        NetworkStream stream = client.GetStream();
        XElement xmlCredentials = XElement.Load(stream);
        xmlCredentials
          .Elements("Credentials")
          .Elements("Credential")
          .ToList()
          .ForEach(x =>
          {
            Credential credential = new Credential(
              (Counterpart)Enum.Parse(typeof(Counterpart), x.Element("Source").Value),
              x.Element("TradingSenderCompID").Value,
              x.Element("TradingTargetCompID").Value,
              x.Element("FeedSenderCompID").Value,
              x.Element("FeedTargetCompID").Value);
            credentials.Add(credential);
          });
      }
#endif
      return credentials;
    }
  }
}
