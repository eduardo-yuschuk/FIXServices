using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Layer1.QuickFIX;
using FIXCommon;
using Layer2.FIXServices.BrokerAdapters;

namespace Layer2.FIXServices
{
  /// <summary>
  /// OrderSide lingua franca
  /// </summary>
  public enum OrderSide
  {
    Buy,
    Sell,
  }
  /// <summary>
  /// OrderStatus lingua franca
  /// </summary>
  public enum OrderStatus
  {
    PendingNew,
    New,
    PartiallyFilled,
    Filled,
    PendingCancel,
    Cancelled,
    PendingReplace,
    Replaced,
    Rejected,
  }
  /// <summary>
  /// OrderType lingua franca
  /// </summary>
  public enum OrderType
  {
    Market,
    //Limit,
    Stop,
    StopLimit, // la que conocemos como limit
  }
  /// <summary>
  /// PositionSide lingua franca
  /// </summary>
  public enum PositionSide
  {
    Long,
    Short,
  }
  /*
  public enum TimeInForce
  {
    Day, //Day 
    GTC, //Good Till Cancel 
    OPG, //At the Opening 
    IOC, //Immediate or Cancel 
    FOK, //Fill or Kill 
    GTX, //Good Till Crossing 
    GTD, //Good Till Date 
    ATC, //At the Close 
  }
  */
  public class Instrument
  {
    public string Ticker { get; private set; }
    public Instrument(string ticker)
    {
      this.Ticker = ticker;
    }
    public override string ToString()
    {
      return this.Ticker;
    }
  }

  public delegate void OnQuoteDelegate(Counterpart counterpart, string ticker, decimal ask, decimal askSize, decimal bid, decimal bidSize, DateTime datetime);
  public delegate void OnExecutionInfoDelegate(Counterpart counterpart, ExecutionReportAdapted executionReportAdapted);
  public delegate void OnAccountInfoDelegate(Counterpart counterpart, AccountInfoAdapted accountInfoAdapted);

  /// <summary>
  /// Contrato que se compromete a cumplir cualquier FIXServicesImplementation
  /// </summary>
  public interface IFIXServices
  {
    #region Feed
    
    event OnQuoteDelegate OnQuote;
    void Subscribe(Counterpart counterpart, string ticker);
    void Unsubscribe(Counterpart counterpart, string ticker);
    void NotifyQuoteStatus(QuoteAvailability qfhQuoteInfo);
    void NotifyQuote(Counterpart counterpart, string ticker, decimal ask, decimal askSize, decimal bid, decimal bidSize, DateTime datetime);
    
    #endregion
    
    #region Trading
    
    event OnExecutionInfoDelegate OnExecutionInfo;
    //void NotifyExecutionInfo(Counterpart counterpart, string orderID, string clOrdID, string ordStatus, string execType);
    void NotifyExecutionInfo(Counterpart counterpart, ExecutionReportAdapted executionReportAdapted);
    void SubmitMarketOrder(Counterpart counterpart, string id, string ticker, decimal qty, OrderSide orderSide);
    void SubmitLimitOrder(Counterpart counterpart, string id, string ticker, decimal qty, OrderSide orderSide, decimal price);
    void SubmitStopOrder(Counterpart counterpart, string id, string ticker, decimal qty, OrderSide orderSide, decimal price);
    void SubmitStopLimitOrder(Counterpart counterpart, string id, string ticker, decimal qty, OrderSide orderSide, decimal price);
    void CancelOrder(Counterpart counterpart, string clOrdID, string orderID, string ticker, OrderSide orderSide);
    
    #endregion

    #region Account

    void RequestAccountInfo();
    void NotifyAccountInfo(Counterpart counterpart, AccountInfoAdapted accountInfoAdapted);

    #endregion
  }
}
