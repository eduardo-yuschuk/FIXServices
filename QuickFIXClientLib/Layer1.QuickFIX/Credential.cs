using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FIXCommon;

namespace Layer1.QuickFIX
{
  public class Credential
  {
    public Counterpart Source { get; private set; }
    public string TradingSenderCompID { get; private set; }
    public string TradingTargetCompID { get; private set; }
    public string FeedSenderCompID { get; private set; }
    public string FeedTargetCompID { get; private set; }

    public Credential(Counterpart source, string tradingSenderCompID, string tradingTargetCompID, string feedSenderCompID, string feedTargetCompID)
    {
      this.Source = source;
      this.TradingSenderCompID = tradingSenderCompID;
      this.TradingTargetCompID = tradingTargetCompID;
      this.FeedSenderCompID = feedSenderCompID;
      this.FeedTargetCompID = feedTargetCompID;
    }
  }
}
