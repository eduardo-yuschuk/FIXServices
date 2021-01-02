using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Layer3.ModelServices
{
  public class Account
  {
    public Account() { }
    public Account(Account account)
    {
      this.AccountName = account.AccountName;
      this.Currency = account.Currency;
      this.Leverage = account.Leverage;
      this.Equity = account.Equity;
      this.UsableMargin = account.UsableMargin;
    }
    public string AccountName { get; set; }
    public string Currency { get; set; }
    public decimal Leverage { get; set; }
    public decimal InitialEquity { get; set; }
    public decimal Equity { get; set; }
    public decimal UsableMargin { get; set; }
  }
}
