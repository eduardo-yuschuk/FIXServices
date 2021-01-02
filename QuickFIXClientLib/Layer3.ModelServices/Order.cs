using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Layer2.FIXServices;
using FIXCommon;
using System.ComponentModel;
using Utils;
using QuickFix;
using Layer2.FIXServices.BrokerAdapters.Dukascopy;
using Layer2.FIXServices.BrokerAdapters;
using System.Threading.Tasks;
using System.Threading;

namespace Layer3.ModelServices
{
  public class Order : INotifyPropertyChanged
  {
    #region PropertyChanges
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name)
    {
      PropertyChangedEventHandler handler = PropertyChanged;
      if (handler != null) handler(this, new PropertyChangedEventArgs(name));
    }
    private List<string> _attributes = null;
    private void NotifyChanges()
    {
      if (this._attributes == null) this._attributes = ReflectionHelper.GetFieldsAsStrings(this).Keys.ToList();
      this._attributes.ForEach(x => OnPropertyChanged(x));
    }
    #endregion
    public DateTime CreationTime { get; private set; }
    public DateTime FillTime { get; private set; }
    public string OrderID { get; private set; }
    public string ClOrdID { get; private set; }
    public List<string> ExecIDList = new List<string>();
    public static string GetNewClOrdID()
    {
      return "O_" + Guid.NewGuid();
    }
    public uint InternalUInt { get; private set; }
    public Counterpart Counterpart { get; private set; }

    public string Account { get; private set; }
    public decimal AvgPrice { get; private set; }
    public decimal CumQty { get; private set; }
    //public DateTime DateTime { get; }
    //public DateTime ExpireTime { get; set; }
    public Instrument Instrument { get; private set; }
    //public bool IsCancelled { get; }
    //public bool IsDone { get; }
    //public bool IsFilled { get; }
    //public bool IsNew { get; }
    //public bool IsPartiallyFilled { get; }
    //public bool IsPendingCancel { get; }
    //public bool IsPendingNew { get; }
    //public bool IsPendingReplace { get; }
    //public bool IsRejected { get; }
    //public decimal LastPrice { get; }
    //public decimal LastQty { get; }
    public decimal LeavesQty { get; private set; }
    //public string OCAGroup { get; set; }
    public decimal Price { get; set; }
    public decimal Qty { get; set; }
    public OrderSide Side { get; private set; }
    public OrderStatus Status { get; private set; }
    //public decimal StopPrice { get; set; }
    //public bool StrategyFill { get; set; }
    //public decimal StrategyPrice { get; set; }
    //public string Text { get; set; }
    //public TimeInForce TimeInForce { get; set; }
    //public decimal TrailingAmt { get; set; }
    public OrderType Type { get; private set; }
    public bool Sent { get; private set; }

    public Order(uint internalUInt, Counterpart counterpart, string clOrdID, string account, Instrument instrument, decimal price, decimal qty, OrderSide orderSide, /*decimal stopPrice, */OrderType orderType)
    {
      this.CreationTime = DateTime.UtcNow;
      this.InternalUInt = internalUInt;
      this.Counterpart = counterpart;
      this.ClOrdID = clOrdID;
      this.Account = account;
      this.Instrument = instrument;
      this.Price = price;
      this.Qty = qty;
      this.Side = orderSide;
      //this.StopPrice = stopPrice;
      this.Type = orderType;
    }

    public void Send()
    {
      if (this.Sent) throw new Exception("envio 2 veces la misma orden?");

      switch (this.Type)
      {
        case OrderType.StopLimit:
          FIXServicesImpl.Instance.SubmitStopLimitOrder(this.Counterpart, this.ClOrdID, this.Instrument.Ticker, this.Qty, this.Side, this.Price);
          break;
        case OrderType.Market:
          FIXServicesImpl.Instance.SubmitMarketOrder(this.Counterpart, this.ClOrdID, this.Instrument.Ticker, this.Qty, this.Side);
          break;
        case OrderType.Stop:
          FIXServicesImpl.Instance.SubmitStopOrder(this.Counterpart, this.ClOrdID, this.Instrument.Ticker, this.Qty, this.Side, this.Price);
          break;
        default:
          throw new Exception("tipo de orden no soportado");
      }

      this.Sent = true;
    }

    public void Cancel()
    {
      Task.Factory.StartNew(() =>
        {
          while (this.OrderID == null) Thread.Sleep(100);
          FIXServicesImpl.Instance.CancelOrder(this.Counterpart, this.ClOrdID, this.OrderID, this.Instrument.Ticker, this.Side);
        });
    }

    public void Replace()
    {
      Task.Factory.StartNew(() =>
        {
          while (this.OrderID == null) Thread.Sleep(100);

          switch (this.Type)
          {
            case OrderType.StopLimit:
              FIXServicesImpl.Instance.ReplaceStopLimitOrder(Counterpart.Dukascopy, this.ClOrdID, this.OrderID, this.Instrument.Ticker, this.Qty, this.Side, this.Price);
              break;
            case OrderType.Stop:
              FIXServicesImpl.Instance.ReplaceStopOrder(Counterpart.Dukascopy, this.ClOrdID, this.OrderID, this.Instrument.Ticker, this.Qty, this.Side, this.Price);
              break;
            default:
              throw new Exception("?");
          }
        });
    }

    public int ExecutionReportsDelivered { get; private set; }

    public void DeliverExecutionReport(ExecutionReportAdapted executionReportAdapted)
    {
      //executionReportAdapted.Account
      this.AvgPrice = executionReportAdapted.AvgPx;
      //executionReportAdapted.CashMargin
      //executionReportAdapted.ClOrdID
      this.CumQty = executionReportAdapted.CumQty;
      this.ExecIDList.Add(executionReportAdapted.ExecID);
      //executionReportAdapted.ExecType
      //executionReportAdapted.ExpireTime
      //executionReportAdapted.LastRptRequested
      this.LeavesQty = executionReportAdapted.LeavesQty;
      this.OrderID = executionReportAdapted.OrderID;
      //executionReportAdapted.OrderQty
      //executionReportAdapted.OrdRejReason
      this.Status = executionReportAdapted.OrdStatus;
      if (this.Status == OrderStatus.Filled) this.FillTime = DateTime.UtcNow;
      //executionReportAdapted.OrdType
      //executionReportAdapted.Side
      //executionReportAdapted.Slippage
      //executionReportAdapted.Symbol
      //executionReportAdapted.TimeInForce
      //executionReportAdapted.TransactTime

      this.NotifyChanges();
      this.ExecutionReportsDelivered++;
    }

    //public OrderStatus State { get; set; }
  }
}
