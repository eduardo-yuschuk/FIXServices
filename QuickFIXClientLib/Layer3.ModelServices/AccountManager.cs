using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Layer3.ModelServices
{
  public class AccountManager
  {
    private AccountManager() { }
    private static AccountManager _instance = null;
    public static AccountManager Instance
    {
      get
      {
        if (_instance == null) _instance = new AccountManager();
        return _instance;
      }
    }

    public void DeliverAccountInfo(Layer2.FIXServices.BrokerAdapters.AccountInfoAdapted accountInfoAdapted)
    {
      lock (accountInfoLock)
      {
        if (this._account == null)
        {
          this._account = new Account();
          this._account.InitialEquity = accountInfoAdapted.Equity;
        }
        this._account.AccountName = accountInfoAdapted.AccountName;
        this._account.Currency = accountInfoAdapted.Currency;
        this._account.Leverage = accountInfoAdapted.Leverage;
        this._account.Equity = accountInfoAdapted.Equity;
        this._account.UsableMargin = accountInfoAdapted.UsableMargin;
      }
    }

    // por ahora una sola cuenta

    private Account _account = null;
    private object accountInfoLock = new object();
    public Account GetAccount() { lock (accountInfoLock) { return new Account(_account); } }
    public bool HasAccountInfo { get { lock (this.accountInfoLock) { return _account != null; } } }
  }
}
