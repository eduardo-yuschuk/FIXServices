using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickFix;
using Layer1.QuickFIX;
using FIXCommon;

namespace Layer2.FIXServices.BrokerAdapters.Dukascopy
{
  public class FIXServicesImpl_Dukascopy
  {
    /// <summary>
    /// genera un mensaje especifico para Dukascopy
    /// </summary>
    /// <param name="clOrdID"></param>
    /// <param name="price"></param>
    /// <param name="ordType"></param>
    /// <param name="timeInForce"></param>
    /// <param name="symbol"></param>
    /// <param name="orderQty"></param>
    /// <param name="side"></param>
    /// <param name="expireTime"></param>
    /// <param name="account"></param>
    /// <param name="slippage"></param>
    public static void SubmitOrder(ClOrdID clOrdID, decimal? price, OrdType ordType, TimeInForce timeInForce, Symbol symbol, OrderQty orderQty, Side side, ExpireTime expireTime, Account account, decimal? slippage)
    {
      QuickFix44.NewOrderSingle message = new QuickFix44.NewOrderSingle(
        clOrdID,
        side,
        new TransactTime(DateTime.UtcNow),
        ordType);

      if (price.HasValue) message.set(new Price((double)price.Value));
      message.set(timeInForce);
      message.set(symbol);
      message.set(orderQty);
      if (expireTime != null) message.set(expireTime);
      if (account != null) message.set(account);
      if (slippage.HasValue) message.setDouble(7011, (double)slippage.Value);

      Credential dukascopyCredential = CredentialFactory.GetCredential(Counterpart.Dukascopy);
      Session.sendToTarget(message, dukascopyCredential.TradingSenderCompID, dukascopyCredential.TradingTargetCompID);
    }

    /// <summary>
    /// genera un mensaje especifico para Dukascopy
    /// </summary>
    /// <param name="id"></param>
    /// <param name="ticker"></param>
    /// <param name="side"></param>
    public static void CancelOrder(string clOrdID, string orderID, string ticker, Side side)
    {
      QuickFix44.OrderCancelRequest message = new QuickFix44.OrderCancelRequest(
        new OrigClOrdID(clOrdID),
        new ClOrdID(clOrdID),
        side,
        new TransactTime(DateTime.UtcNow));

      message.set(new OrderID(orderID));
      message.set(new Symbol(ticker));

      Credential dukascopyCredential = CredentialFactory.GetCredential(Counterpart.Dukascopy);
      Session.sendToTarget(message, dukascopyCredential.TradingSenderCompID, dukascopyCredential.TradingTargetCompID);
    }

    public static void ReplaceOrder(OrderID orderID, ClOrdID clOrdID, decimal price, OrdType ordType, TimeInForce timeInForce, Symbol symbol,
      OrderQty orderQty, Side side, ExpireTime expireTime, Account account, decimal? slippage)
    {
      QuickFix44.OrderCancelReplaceRequest message = new QuickFix44.OrderCancelReplaceRequest(
        new OrigClOrdID(clOrdID.getValue()),
        clOrdID,
        side,
        new TransactTime(DateTime.UtcNow),
        ordType);

      message.set(orderID);
      message.set(new Price((double)price));
      message.set(timeInForce);
      message.set(symbol);
      message.set(orderQty);

      if (expireTime != null) message.set(expireTime);
      if (account != null) message.set(account);
      if (slippage.HasValue) message.setDouble(7011, (double)slippage.Value);

      Credential dukascopyCredential = CredentialFactory.GetCredential(Counterpart.Dukascopy);
      Session.sendToTarget(message, dukascopyCredential.TradingSenderCompID, dukascopyCredential.TradingTargetCompID);
    }

    /// <summary>
    /// genera un mensaje especifico para Dukascopy
    /// </summary>
    /// <param name="ticker"></param>
    /// <param name="subscriptionRequestType"></param>
    public static void UpdateFeedSubscription(string ticker, SubscriptionRequestType subscriptionRequestType)
    {
      int topOfBook = 1;

      QuickFix44.MarketDataRequest message =
        new QuickFix44.MarketDataRequest(
          new MDReqID("1"),
          subscriptionRequestType,
          new MarketDepth(topOfBook));

      QuickFix44.MarketDataRequest.NoRelatedSym tickersGroup = new QuickFix44.MarketDataRequest.NoRelatedSym();
      tickersGroup.set(new QuickFix.Symbol(ticker));
      message.addGroup(tickersGroup);

      QuickFix44.MarketDataRequest.NoMDEntryTypes sidesGroup = new QuickFix44.MarketDataRequest.NoMDEntryTypes();
      sidesGroup.set(new QuickFix.MDEntryType(MDEntryType.BID));
      message.addGroup(sidesGroup);
      sidesGroup.set(new QuickFix.MDEntryType(MDEntryType.OFFER));
      message.addGroup(sidesGroup);

      message.set(new MDUpdateType(MDUpdateType.FULL_REFRESH));
      message.set(new NoRelatedSym(1));

      Credential dukascopyCredential = CredentialFactory.GetCredential(Counterpart.Dukascopy);
      Session.sendToTarget(message, dukascopyCredential.FeedSenderCompID, dukascopyCredential.FeedTargetCompID);
    }

    public static void SendAccountInfoRequest(Account account)
    {
      AccountInfoRequest message = new AccountInfoRequest();
      if (account != null) message.set(account);

      //Message message = new Message(new BeginString("FIX.4.4"), new MsgType("U7"));

      Credential dukascopyCredential = CredentialFactory.GetCredential(Counterpart.Dukascopy);
      Session.sendToTarget(message, dukascopyCredential.TradingSenderCompID, dukascopyCredential.TradingTargetCompID);
    }
  }
}
