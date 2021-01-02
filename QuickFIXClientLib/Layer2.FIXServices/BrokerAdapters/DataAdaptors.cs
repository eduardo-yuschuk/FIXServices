using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickFix;
using Layer2.FIXServices.BrokerAdapters.Dukascopy;
using Layer2.FIXServices;

namespace Layer2.FIXServices.BrokerAdapters
{
  public class ExecutionReportToAdapt
  {
  }
  public class DukascopyExecutionReportToAdapt : ExecutionReportToAdapt
  {
    public DukascopyExecutionReportToAdapt(QuickFix44.ExecutionReport message)
    {
      this.OrderID = message.isSetOrderID() ? message.getOrderID() : null;
      this.ClOrdID = message.isSetClOrdID() ? message.getClOrdID() : null;
      this.ExecID = message.isSetExecID() ? message.getExecID() : null;
      this.OrdStatus = message.isSetOrdStatus() ? message.getOrdStatus() : null;
      this.ExecType = message.isSetExecType() ? message.getExecType() : null;
      this.Symbol = message.isSetSymbol() ? message.getSymbol() : null;
      this.TimeInForce = message.isSetTimeInForce() ? message.getTimeInForce() : null;
      this.CumQty = message.isSetCumQty() ? message.getCumQty() : null;
      this.LeavesQty = message.isSetLeavesQty() ? message.getLeavesQty() : null;
      this.OrderQty = message.isSetOrderQty() ? message.getOrderQty() : null;
      this.Side = message.isSetSide() ? message.getSide() : null;
      this.OrdType = message.isSetOrdType() ? message.getOrdType() : null;
      this.AvgPx = message.isSetAvgPx() ? message.getAvgPx() : null;
      this.ExpireTime = message.isSetExpireTime() ? message.getExpireTime() : null;
      this.TransactTime = message.isSetTransactTime() ? message.getTransactTime() : null;
      this.LastRptRequested = message.isSetLastRptRequested() ? message.getLastRptRequested() : null;
      this.Account = message.isSetAccount() ? message.getAccount() : null;
      this.Slippage = message.isSetField(Slippage.FIELD) ? new Slippage(message.getDouble(Slippage.FIELD)) : null;
      this.OrdRejReason = message.isSetOrdRejReason() ? message.getOrdRejReason() : null;
      this.CashMargin = message.isSetCashMargin() ? message.getCashMargin() : null;
    }
    public OrderID OrderID;
    public ClOrdID ClOrdID;
    public ExecID ExecID;
    public OrdStatus OrdStatus;
    public ExecType ExecType;
    public Symbol Symbol;
    public TimeInForce TimeInForce;
    public CumQty CumQty;
    public LeavesQty LeavesQty;
    public OrderQty OrderQty;
    public Side Side;
    public OrdType OrdType;
    public AvgPx AvgPx;
    public ExpireTime ExpireTime;
    public TransactTime TransactTime;
    public LastRptRequested LastRptRequested;
    public Account Account;
    public Slippage Slippage;
    public OrdRejReason OrdRejReason;
    public CashMargin CashMargin;
  }
  public class ExecutionReportAdapted
  {
    public string OrderID;
    public string ClOrdID;
    public string ExecID;
    public OrderStatus OrdStatus;
    public string ExecType;
    public string Symbol;
    public string TimeInForce;
    public decimal CumQty;
    public decimal LeavesQty;
    public decimal OrderQty;
    public OrderSide Side;
    public string OrdType;
    public decimal AvgPx;
    public string ExpireTime;
    public string TransactTime;
    public string LastRptRequested;
    public string Account;
    public decimal Slippage;
    public string OrdRejReason;
    public string CashMargin;
  }

  public class DukascopyInstrumentPositionInfoToAdapt
  {
    public DukascopyInstrumentPositionInfoToAdapt(Layer2.FIXServices.BrokerAdapters.Dukascopy.InstrumentPositionInfo message)
    {
      this.Account = message.isSetAccount() ? message.getAccount() : null;
      this.AccountName = message.isSetAccountName() ? message.getAccountName() : null;
      this.Amount = message.isSetAmount() ? message.getAmount() : null;
      this.Symbol = message.isSetSymbol() ? message.getSymbol() : null;
    }

    public Account Account { get; private set; }
    public AccountName AccountName { get; private set; }
    public Amount Amount { get; private set; }
    public Symbol Symbol { get; private set; }
  }

  public class InstrumentPositionInfoAdapted
  {
    public string Account { get; set; }
    public string AccountName { get; set; }
    public decimal Amount { get; set; }
    public string Symbol { get; set; }
  }

  public class DukascopyAccountInfoToAdapt
  {
    public DukascopyAccountInfoToAdapt(Layer2.FIXServices.BrokerAdapters.Dukascopy.AccountInfo message)
    {
      this.Leverage = message.isSetLeverage() ? message.getLeverage() : null;
      this.UsableMargin = message.isSetUsableMargin() ? message.getUsableMargin() : null;
      this.Equity = message.isSetEquity() ? message.getEquity() : null;
      this.Currency = message.isSetCurrency() ? message.getCurrency() : null;
      this.AccountName = message.isSetAccountName() ? message.getAccountName() : null;
    }

    public Leverage Leverage { get; private set; }
    public UsableMargin UsableMargin { get; private set; }
    public Equity Equity { get; private set; }
    public Currency Currency { get; private set; }
    public AccountName AccountName { get; private set; }
  }

  public class AccountInfoAdapted
  {
    public decimal Leverage { get; set; }
    public decimal UsableMargin { get; set; }
    public decimal Equity { get; set; }
    public string Currency { get; set; }
    public string AccountName { get; set; }
  }

  public class DataAdaptors
  {
    private static string SafeToString(object field)
    {
      if (field != null) return field.ToString();
      return "";
    }

    private static string SafeStringValue(StringField field)
    {
      if (field != null) return field.getValue();
      return "";
    }

    private static decimal SafeDecimalValue(DoubleField field)
    {
      if (field != null) return (decimal)field.getValue();
      return 0m;
    }

    public static ExecutionReportAdapted AdaptExecutionReport(DukascopyExecutionReportToAdapt input)
    {
      ExecutionReportAdapted output = new ExecutionReportAdapted();
      output.OrderID = SafeStringValue(input.OrderID);
      output.ClOrdID = SafeStringValue(input.ClOrdID);
      output.ExecID = SafeStringValue(input.ExecID);

      output.CumQty = SafeDecimalValue(input.CumQty);
      output.LeavesQty = SafeDecimalValue(input.LeavesQty);

      //output.OrderQty = SafeDecimalValue(input.OrderQty);

      switch (input.OrdStatus.getValue())
      {
        case OrdStatus.REJECTED:
          output.OrdStatus = OrderStatus.Rejected;
          break;
        case OrdStatus.PENDING_NEW:
          output.OrdStatus = OrderStatus.PendingNew;
          break;
        case OrdStatus.CALCULATED:
          output.OrdStatus = OrderStatus.PendingNew;
          break;
        case OrdStatus.CANCELED:
          output.OrdStatus = OrderStatus.Cancelled;
          break;
        case OrdStatus.FILLED:
          if (output.LeavesQty > 0) output.OrdStatus = OrderStatus.PartiallyFilled;
          else output.OrdStatus = OrderStatus.Filled;
          break;
      }

      output.ExecType = SafeToString(input.ExecType);
      //output.Symbol = SafeStringValue(input.Symbol);

      //switch (input.Side.getValue())
      //{
      //  case Side.SELL:
      //    output.Side = OrderSide.Sell;
      //    break;
      //  case Side.BUY:
      //    output.Side = OrderSide.Buy;
      //    break;
      //  case Side.UNDISC:
      //  default:
      //    throw new Exception("?");
      //}

      output.AvgPx = SafeDecimalValue(input.AvgPx);
      output.Account = SafeStringValue(input.Account);
      output.Slippage = SafeDecimalValue(input.Slippage);
      output.OrdRejReason = SafeToString(input.OrdRejReason);

      return output;
    }

    public static InstrumentPositionInfoAdapted AdaptInstrumentPositionInfo(DukascopyInstrumentPositionInfoToAdapt input)
    {
      InstrumentPositionInfoAdapted output = new InstrumentPositionInfoAdapted();
      output.Account = SafeStringValue(input.Account);
      output.AccountName = SafeStringValue(input.AccountName);
      output.Amount = SafeDecimalValue(input.Amount);
      output.Symbol = SafeStringValue(input.Symbol);

      return output;
    }

    public static AccountInfoAdapted AdaptAccountInfo(DukascopyAccountInfoToAdapt input)
    {
      AccountInfoAdapted output = new AccountInfoAdapted();
      output.Leverage = SafeDecimalValue(input.Leverage);
      output.UsableMargin = SafeDecimalValue(input.UsableMargin);
      output.Equity = SafeDecimalValue(input.Equity);
      output.Currency = SafeStringValue(input.Currency);
      output.AccountName = SafeStringValue(input.AccountName);

      return output;
    }
  }
}
