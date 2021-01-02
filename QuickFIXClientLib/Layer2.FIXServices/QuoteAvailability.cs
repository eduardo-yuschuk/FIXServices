using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickFix;

namespace Layer2.FIXServices
{
  public class QuoteAvailability
  {
    public QuoteID QuoteID { get; set; }
    public Symbol Symbol { get; set; }
    public QuoteType QuoteType { get; set; }
    public void Update(QuoteAvailability qfhQuoteInfo)
    {
      this.QuoteID = qfhQuoteInfo.QuoteID;
      this.Symbol = qfhQuoteInfo.Symbol;
      this.QuoteType = qfhQuoteInfo.QuoteType;
    }
  }
}
