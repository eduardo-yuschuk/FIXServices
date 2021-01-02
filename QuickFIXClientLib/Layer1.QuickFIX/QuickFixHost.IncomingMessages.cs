using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickFix;
using System.Threading;
using System.Collections.Concurrent;
using Layer2.FIXServices.BrokerAdapters.Dukascopy;
using Layer2.FIXServices;
using FIXCommon;
using Layer2.FIXServices.BrokerAdapters;
using Layer3.ModelServices;

namespace Layer1.QuickFIX
{
  public partial class QuickFixHost
  {
    #region Connection Interface

    public override void onMessage(QuickFix44.Logon message, SessionID session)
    {
      // getting attributes

      HeartBtInt heartBtInt = message.getHeartBtInt();

      ResetSeqNumFlag resetSeqNumFlag = message.getResetSeqNumFlag();

      // no me lo mandan
      //Username username = message.getUsername();

      // no me lo mandan
      //Password password = message.getPassword();

      // firing event

      Console.WriteLine("QuickFix44.Logon: {0}, {1}", heartBtInt, resetSeqNumFlag);
    }

    #endregion

    #region Data Feed Interface

    public override void onMessage(QuickFix44.MarketDataSnapshotFullRefresh message, SessionID session)
    {
      // getting attributes

      Symbol symbol = message.getSymbol();
      SendingTime sendingTime = new SendingTime(message.getHeader().getUtcTimeStamp(SendingTime.FIELD));
      NoMDEntries noMDEntries = message.getNoMDEntries();

      decimal ask = 0m;
      decimal bid = 0m;
      decimal askSize = 0m;
      decimal bidSize = 0m;

      for (int i = 0; i < noMDEntries.getValue(); i++)
      {
        QuickFix44.MarketDataSnapshotFullRefresh.NoMDEntries group = new QuickFix44.MarketDataSnapshotFullRefresh.NoMDEntries();
        message.getGroup((uint)i + 1, group);
        MDEntryType mdEntryType = group.getMDEntryType();
        MDEntryPx mdEntryPx = group.getMDEntryPx();
        MDEntrySize mdEntrySize = group.getMDEntrySize();

        if (mdEntryType.getValue() == MDEntryType.BID)
        {
          bid = (decimal)mdEntryPx.getValue();
          bidSize = (decimal)mdEntrySize.getValue();
        }
        else if (mdEntryType.getValue() == MDEntryType.OFFER)
        {
          ask = (decimal)mdEntryPx.getValue();
          askSize = (decimal)mdEntrySize.getValue();
        }
      }

      // firing event

      Console.WriteLine("QuickFix44.MarketDataSnapshotFullRefresh: {0}, {1}/{2}, {3}/{4}, {5}", symbol, ask, bid, askSize, bidSize, sendingTime);

      this.fixServices.NotifyQuote(Counterpart.Dukascopy, symbol.getValue(), ask, askSize, bid, bidSize, sendingTime.getValue());
    }

    public override void onMessage(QuickFix44.QuoteStatusReport message, SessionID session)
    {
      // getting attributes

      QuoteID quoteID = message.getQuoteID();
      Symbol symbol = message.getSymbol();
      QuoteType quoteType = message.getQuoteType();

      QuoteAvailability qfhQuoteInfo = new QuoteAvailability()
      {
        QuoteID = quoteID,
        Symbol = symbol,
        QuoteType = quoteType,
      };

      // firing event

      //Console.WriteLine("QuickFix44.QuoteStatusReport: {0}, {1}, {2}", quoteID, symbol, quoteType);

      this.fixServices.NotifyQuoteStatus(qfhQuoteInfo);
    }

    public override void onMessage(QuickFix44.MarketDataRequestReject message, SessionID session)
    {
      // getting attributes

      MDReqID mdReqID = message.getMDReqID();
      MDReqRejReason mdReqRejReason = message.getMDReqRejReason();

      // firing event

      Console.WriteLine("QuickFix44.MarketDataRequestReject: {0}, {1}", mdReqID, mdReqRejReason);
    }

    #endregion

    #region Trading Interface

    public override void onMessage(QuickFix44.ExecutionReport message, SessionID session)
    {
      OrderID orderID = message.getOrderID();
      ClOrdID clOrdID = message.getClOrdID();
      OrdStatus ordStatus = message.getOrdStatus();
      Symbol symbol = message.getSymbol();

      // firing event

      Console.WriteLine("QuickFix44.ExecutionReport: {0}, {1}, {2}, {3}", orderID, clOrdID, ordStatus, symbol);

      this.fixServices.NotifyExecutionInfo(Counterpart.Dukascopy, DataAdaptors.AdaptExecutionReport(new DukascopyExecutionReportToAdapt(message)));
    }

    public override void onMessage(QuickFix44.UserResponse message, SessionID session)
    {
      // getting attributes

      UserRequestID userRequestID = message.getUserRequestID();
      Username username = message.getUsername();

      // firing event

      Console.WriteLine("QuickFix44.ExecutionReport: {0}, {1}", userRequestID, username);
    }

    public override void onMessage(QuickFix44.TradingSessionStatus message, SessionID session)
    {
      // getting attributes

      TradSesStatus tradSesStatus = message.getTradSesStatus();

      // firing event

      Console.WriteLine("QuickFix44.TradingSessionStatus: {0}", tradSesStatus);
    }

    #endregion

    #region Error Handling

    public override void onMessage(QuickFix44.Reject message, SessionID session)
    {
      // getting attributes

      RefMsgType refMsgType = message.getRefMsgType();
      RefSeqNum refSeqNum = message.getRefSeqNum();
      RefTagID refTagID = message.getRefTagID();
      SessionRejectReason sessionRejectReason = message.getSessionRejectReason();
      Text text = message.getText();

      // firing event

      Console.WriteLine("QuickFix44.Reject: {0}, {1}, {2}, {3}, {4}", refMsgType, refSeqNum, refTagID, sessionRejectReason, text);
    }

    public override void onMessage(QuickFix44.BusinessMessageReject message, SessionID session)
    {
      RefSeqNum refSeqNum = message.isSetRefSeqNum() ? message.getRefSeqNum() : new RefSeqNum(-1);
      Text text = message.getText();
      RefMsgType refMsgType = message.getRefMsgType();
      BusinessRejectRefID businessRejectRefID = message.isSetBusinessRejectRefID() ? message.getBusinessRejectRefID() : new BusinessRejectRefID("not set");
      BusinessRejectReason businessRejectReason = message.getBusinessRejectReason();

      Console.WriteLine("QuickFix44.BusinessMessageReject: {0}, {1}, {2}, {3}, {4}", refSeqNum, text, refMsgType, businessRejectRefID, businessRejectReason);
    }

    #endregion

    #region User Messages Interface

    public void onMessage(Layer2.FIXServices.BrokerAdapters.Dukascopy.Notification message, SessionID session)
    {
      // getting attributes

      QuickFix.Account account = message.isSetAccount() ? message.getAccount() : new QuickFix.Account("not set");
      AccountName accountName = message.getAccountName();
      NotifPriority notifPriority = message.getNotifPriority();
      Text text = message.getText();

      // firing event

      Console.WriteLine("Dukascopy.Notification: {0}, {1}, {2}, {3}", account, accountName, notifPriority, text);
    }

    public void onMessage(Layer2.FIXServices.BrokerAdapters.Dukascopy.AccountInfo message, SessionID session)
    {
      // getting attributes

      Leverage leverage = message.getLeverage();
      UsableMargin usableMargin = message.getUsableMargin();
      Equity equity = message.getEquity();
      Currency currency = message.getCurrency();
      AccountName accountName = message.getAccountName();

      // firing event

      Console.WriteLine("Dukascopy.AccountInfo: {0}, {1}, {2}, {3}, {4}", leverage, usableMargin, equity, currency, accountName);

      this.fixServices.NotifyAccountInfo(Counterpart.Dukascopy, DataAdaptors.AdaptAccountInfo(new DukascopyAccountInfoToAdapt(message)));
    }

    public void onMessage(Layer2.FIXServices.BrokerAdapters.Dukascopy.InstrumentPositionInfo message, SessionID session)
    {
      // getting attributes

      QuickFix.Account account = message.isSetAccount() ? message.getAccount() : new QuickFix.Account("not set");
      AccountName accountName = message.getAccountName();
      Amount amount = message.getAmount();
      Symbol symbol = message.getSymbol();

      // firing event

      Console.WriteLine("Dukascopy.InstrumentPositionInfo: {0}, {1}, {2}, {3}", account, accountName, amount, symbol);

      PositionsManager.Instance.DeliverInstrumentPositionInfo(DataAdaptors.AdaptInstrumentPositionInfo(new DukascopyInstrumentPositionInfoToAdapt(message)));
    }

    public void onMessage(Layer2.FIXServices.BrokerAdapters.Dukascopy.ActivationResponse message, SessionID session)
    {
      // getting attributes

      Username username = message.getUsername();
      QuickFix.Account account = message.isSetAccount() ? message.getAccount() : new QuickFix.Account("not set");

      // firing event

      Console.WriteLine("Dukascopy.ActivationResponse: {0}, {1}", username, account);
    }

    #endregion
  }
}
