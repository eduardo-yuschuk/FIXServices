using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using QuickFix;
using Layer1.QuickFIX;
using Layer2.FIXServices.BrokerAdapters.Dukascopy;
using FIXCommon;
using Layer2.FIXServices.BrokerAdapters;

namespace Layer2.FIXServices
{
  /// <summary>
  /// Implementacion generica
  /// </summary>
  public class FIXServicesImpl : IFIXServices
  {
    QuickFixHost host;

    private FIXServicesImpl()
    {
      if (this.host == null)
      {
        this.host = new QuickFixHost();
        this.host.Start(this);
      }
    }

    public void Start() { /* :) */ }

    private static FIXServicesImpl _instance;
    public static FIXServicesImpl Instance
    {
      get
      {
        if (_instance == null) _instance = new FIXServicesImpl();
        return _instance;
      }
    }

    #region Feed

    public event OnQuoteDelegate OnQuote;

    ConcurrentDictionary<string, QuoteAvailability> quotesStatus = new ConcurrentDictionary<string, QuoteAvailability>();

    public IEnumerable<string> TradableSymbols
    {
      get
      {
        return this.quotesStatus
          .Values
          .Where(qi => qi.QuoteType.getValue() == QuoteType.TRADEABLE)
          .Select(qi => qi.Symbol.ToString());
      }
    }

    public IEnumerable<string> NonTradableSymbols
    {
      get
      {
        return this.quotesStatus
          .Values
          .Where(qi => qi.QuoteType.getValue() != QuoteType.TRADEABLE)
          .Select(qi => qi.Symbol.ToString());
      }
    }

    public void Subscribe(Counterpart counterpart, string ticker)
    {
      switch (counterpart)
      {
        case Counterpart.Dukascopy:
          SubscriptionRequestType subscriptionRequestType = new SubscriptionRequestType(SubscriptionRequestType.SNAPSHOT_PLUS_UPDATES);
          FIXServicesImpl_Dukascopy.UpdateFeedSubscription(ticker, subscriptionRequestType);
          break;
        default:
          throw new InvalidOperationException();
      }
    }

    public void Unsubscribe(Counterpart counterpart, string ticker)
    {
      switch (counterpart)
      {
        case Counterpart.Dukascopy:
          SubscriptionRequestType subscriptionRequestType = new SubscriptionRequestType(SubscriptionRequestType.DISABLE_PREVIOUS_SNAPSHOT_PLUS_UPDATE_REQUEST);
          FIXServicesImpl_Dukascopy.UpdateFeedSubscription(ticker, subscriptionRequestType);
          break;
        default:
          throw new InvalidOperationException();
      }
    }

    public void NotifyQuoteStatus(QuoteAvailability qfhQuoteInfo)
    {
      quotesStatus.AddOrUpdate(qfhQuoteInfo.Symbol.getValue(), qfhQuoteInfo, (key, value) => qfhQuoteInfo);
    }

    public void NotifyQuote(Counterpart counterpart, string ticker, decimal ask, decimal askSize, decimal bid, decimal bidSize, DateTime datetime)
    {
      if (this.OnQuote != null) this.OnQuote(counterpart, ticker, ask, askSize, bid, bidSize, datetime);
    }

    #endregion

    #region Trading

    public event OnExecutionInfoDelegate OnExecutionInfo;

    public void NotifyExecutionInfo(Counterpart counterpart, ExecutionReportAdapted executionReportAdapted)
    {
      if (this.OnExecutionInfo != null) this.OnExecutionInfo(counterpart, executionReportAdapted);
    }

    private Side ComputeSide(OrderSide orderSide)
    {
      Side side = new Side();
      switch (orderSide)
      {
        case OrderSide.Buy:
          side.setValue(Side.BUY);
          break;
        case OrderSide.Sell:
          side.setValue(Side.SELL);
          break;
        default:
          throw new NotImplementedException("?");
      }
      return side;
    }

    public void SubmitMarketOrder(Counterpart counterpart, string id, string ticker, decimal qty, OrderSide orderSide)
    {
      switch (counterpart)
      {
        case Counterpart.Dukascopy:
          // conversion del side lingua franca al side FIX
          Side side = this.ComputeSide(orderSide);
          // despacho del mensaje
          FIXServicesImpl_Dukascopy.SubmitOrder(
            new ClOrdID(id),
            null,
            new OrdType(OrdType.MARKET),
            new TimeInForce(TimeInForce.GOOD_TILL_CANCEL),
            new Symbol(ticker),
            new OrderQty((double)qty),
            side,
            null,
            null,
            null);
          break;
        default:
          throw new InvalidOperationException();
      }
    }

    public void SubmitLimitOrder(Counterpart counterpart, string id, string ticker, decimal qty, OrderSide orderSide, decimal price)
    {
      throw new NotImplementedException("no existe en dukascopy");
    }

    public void SubmitStopOrder(Counterpart counterpart, string id, string ticker, decimal qty, OrderSide orderSide, decimal price)
    {
      switch (counterpart)
      {
        case Counterpart.Dukascopy:
          // conversion del side lingua franca al side FIX
          Side side = this.ComputeSide(orderSide);
          // despacho del mensaje
          FIXServicesImpl_Dukascopy.SubmitOrder(
            new ClOrdID(id),
            price,
            new OrdType(OrdType.STOP),
            new TimeInForce(TimeInForce.GOOD_TILL_CANCEL),
            new Symbol(ticker),
            new OrderQty((double)qty),
            side,
            null,
            null,
            null);
          break;
        default:
          throw new InvalidOperationException();
      }
    }

    public void SubmitStopLimitOrder(Counterpart counterpart, string id, string ticker, decimal qty, OrderSide orderSide, decimal price)
    {
      switch (counterpart)
      {
        case Counterpart.Dukascopy:
          // conversion del side lingua franca al side FIX
          Side side = this.ComputeSide(orderSide);
          // despacho del mensaje
          FIXServicesImpl_Dukascopy.SubmitOrder(
            new ClOrdID(id),
            price,
            new OrdType(OrdType.STOP_LIMIT),
            new TimeInForce(TimeInForce.GOOD_TILL_CANCEL),
            new Symbol(ticker),
            new OrderQty((double)qty),
            side,
            null,
            null,
            null);
          break;
        default:
          throw new InvalidOperationException();
      }
    }

    public void CancelOrder(Counterpart counterpart, string clOrdID, string orderID, string ticker, OrderSide orderSide)
    {
      switch (counterpart)
      {
        case Counterpart.Dukascopy:
          // conversion del side lingua franca al side FIX
          Side side = this.ComputeSide(orderSide);
          // despacho del mensaje
          FIXServicesImpl_Dukascopy.CancelOrder(clOrdID, orderID, ticker, side);
          break;
        default:
          throw new InvalidOperationException();
      }
    }

    // esto esta hecho para endurecer determinadas acciones. Solo pueden reemplazarse ordenes que no sean market. Y por el momento solo manejamos
    // ordenes stop limit (a las que llamamos limit) y ordenes stop

    public void ReplaceStopLimitOrder(Counterpart counterpart, string clOrdID, string orderID, string ticker, decimal qty, OrderSide orderSide, decimal price)
    {
      this.ReplaceOrder(counterpart, clOrdID, orderID, ticker, qty, orderSide, price, new OrdType(OrdType.STOP_LIMIT));
    }

    public void ReplaceStopOrder(Counterpart counterpart, string clOrdID, string orderID, string ticker, decimal qty, OrderSide orderSide, decimal price)
    {
      this.ReplaceOrder(counterpart, clOrdID, orderID, ticker, qty, orderSide, price, new OrdType(OrdType.STOP));
    }

    private void ReplaceOrder(Counterpart counterpart, string clOrdID, string orderID, string ticker, decimal qty, OrderSide orderSide, decimal price, OrdType ordType)
    {
      switch (counterpart)
      {
        case Counterpart.Dukascopy:
          // conversion del side lingua franca al side FIX
          Side side = this.ComputeSide(orderSide);
          // despacho del mensaje
          FIXServicesImpl_Dukascopy.ReplaceOrder(
            new OrderID(orderID),
            new ClOrdID(clOrdID),
            price,
            ordType,
            new TimeInForce(TimeInForce.GOOD_TILL_CANCEL),
            new Symbol(ticker),
            new OrderQty((double)qty),
            side,
            null,
            null,
            null);
          break;
        default:
          throw new InvalidOperationException();
      }
    }

    #endregion

    #region Account

    public event OnAccountInfoDelegate OnAccountInfo;

    public void RequestAccountInfo()
    {
      FIXServicesImpl_Dukascopy.SendAccountInfoRequest(null);
    }

    public void NotifyAccountInfo(Counterpart counterpart, AccountInfoAdapted accountInfoAdapted)
    {
      if (this.OnAccountInfo != null) this.OnAccountInfo(counterpart, accountInfoAdapted);
    }

    #endregion
  }
}
