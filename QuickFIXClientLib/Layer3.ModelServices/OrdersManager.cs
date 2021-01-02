using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using Layer2.FIXServices;
using FIXCommon;
using System.Diagnostics;
using Layer2.FIXServices.BrokerAdapters;

namespace Layer3.ModelServices
{
  public class OrdersManager
  {
    private OrdersManager() { }
    private static OrdersManager _instance = null;
    public static OrdersManager Instance
    {
      get
      {
        if (_instance == null) _instance = new OrdersManager();
        return _instance;
      }
    }
    private static object uintlock = new object();
    private static uint internalUInt = 0;
    private static uint GetInternalUInt()
    {
      lock (uintlock)
      {
        return internalUInt++;
      }
    }

    ConcurrentDictionary<uint, Order> OrdersBook = new ConcurrentDictionary<uint, Order>();
    private bool subscribedToOrdersNotification = false;
    private Order AddToOrdersBook(Order order)
    {
      if (!subscribedToOrdersNotification)
      {
        FIXServicesImpl.Instance.OnExecutionInfo += new OnExecutionInfoDelegate(Instance_OnExecutionInfo);
        subscribedToOrdersNotification = true;
      }
      this.OrdersBook[order.InternalUInt] = order;
      return order;
    }

    void Instance_OnExecutionInfo(Counterpart counterpart, ExecutionReportAdapted executionReportAdapted)
    {
      switch (counterpart)
      {
        case Counterpart.Dukascopy:
          Order order = this.OrdersBook.Values.ToList().FirstOrDefault(ord => ord.ClOrdID == executionReportAdapted.ClOrdID);
          if (order != null)
          {
            order.DeliverExecutionReport(executionReportAdapted);
          }
          else
          {
            throw new Exception("no se encontro la orden destino del mensaje");
          }
          break;
        default:
          break;
      }
    }

    public Order BuyLimitOrder(Counterpart counterpart, string clOrdID, string account, Instrument instrument, decimal qty, decimal limitPrice)
    {
      return this.AddToOrdersBook(new Order(GetInternalUInt(), counterpart, clOrdID, account, instrument, limitPrice, qty, OrderSide.Buy, OrderType.StopLimit));
    }

    public Order BuyOrder(Counterpart counterpart, string clOrdID, string account, Instrument instrument, decimal qty)
    {
      return this.AddToOrdersBook(new Order(GetInternalUInt(), counterpart, clOrdID, account, instrument, 0m, qty, OrderSide.Buy, OrderType.Market));
    }

    public Order BuyStopOrder(Counterpart counterpart, string clOrdID, string account, Instrument instrument, decimal qty, decimal stopPrice)
    {
      return this.AddToOrdersBook(new Order(GetInternalUInt(), counterpart, clOrdID, account, instrument, stopPrice, qty, OrderSide.Buy, OrderType.Stop));
    }

    public Order SellLimitOrder(Counterpart counterpart, string clOrdID, string account, Instrument instrument, decimal qty, decimal limitPrice)
    {
      return this.AddToOrdersBook(new Order(GetInternalUInt(), counterpart, clOrdID, account, instrument, limitPrice, qty, OrderSide.Sell, OrderType.StopLimit));
    }

    public Order SellOrder(Counterpart counterpart, string clOrdID, string account, Instrument instrument, decimal qty)
    {
      return this.AddToOrdersBook(new Order(GetInternalUInt(), counterpart, clOrdID, account, instrument, 0m, qty, OrderSide.Sell, OrderType.Market));
    }

    private Order SellStopOrder(Counterpart counterpart, string clOrdID, string account, Instrument instrument, decimal qty, decimal stopPrice)
    {
      return this.AddToOrdersBook(new Order(GetInternalUInt(), counterpart, clOrdID, account, instrument, stopPrice, qty, OrderSide.Sell, OrderType.Stop));
    }

    // hacemos un interface publico con Send por que me olvido de enviar las ordenes

    public Order SendBuyLimitOrder(Counterpart counterpart, string clOrdID, string account, Instrument instrument, decimal qty, decimal limitPrice)
    {
      Order order = this.BuyLimitOrder(counterpart, clOrdID, account, instrument, qty, limitPrice);
      order.Send();
      return order;
    }

    public Order SendBuyOrder(Counterpart counterpart, string clOrdID, string account, Instrument instrument, decimal qty)
    {
      Order order = this.BuyOrder(counterpart, clOrdID, account, instrument, qty);
      order.Send();
      return order;
    }

    public Order SendBuyStopOrder(Counterpart counterpart, string clOrdID, string account, Instrument instrument, decimal qty, decimal stopPrice)
    {
      Order order = this.BuyStopOrder(counterpart, clOrdID, account, instrument, qty, stopPrice);
      order.Send();
      return order;
    }

    public Order SendSellLimitOrder(Counterpart counterpart, string clOrdID, string account, Instrument instrument, decimal qty, decimal limitPrice)
    {
      Order order = this.SellLimitOrder(counterpart, clOrdID, account, instrument, qty, limitPrice);
      order.Send();
      return order;
    }

    public Order SendSellOrder(Counterpart counterpart, string clOrdID, string account, Instrument instrument, decimal qty)
    {
      Order order = this.SellOrder(counterpart, clOrdID, account, instrument, qty);
      order.Send();
      return order;
    }

    public Order SendSellStopOrder(Counterpart counterpart, string clOrdID, string account, Instrument instrument, decimal qty, decimal stopPrice)
    {
      Order order = this.SellStopOrder(counterpart, clOrdID, account, instrument, qty, stopPrice);
      order.Send();
      return order;
    }

    public void CancelAll()
    {
      this.OrdersBook.Values.ToList().ForEach(ord =>
        {
          ord.Cancel();
        });
    }

    public Order GetOrder(string clOrdID)
    {
      return this.OrdersBook.Values.FirstOrDefault(ord => ord.ClOrdID == clOrdID);
    }
  }
}
