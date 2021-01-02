using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Layer1.QuickFIX;
using Layer2.FIXServices;
using FIXCommon;
using Layer3.ModelServices;
using Utils;
using System.Threading.Tasks;
using System.Threading;

namespace QuickFIXManualTestPanel
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();
    }

    private void Start_Click(object sender, RoutedEventArgs e)
    {
      FIXServicesImpl.Instance.Start();
      FIXServicesImpl.Instance.OnQuote += new Layer2.FIXServices.OnQuoteDelegate(host_OnQuote);
    }

    decimal selectedAsk = 0m;
    decimal selectedBid = 0m;
    string selectedTicker = "";

    void host_OnQuote(Counterpart source, string ticker, decimal ask, decimal askSize, decimal bid, decimal bidSize, DateTime datetime)
    {
      this.Dispatcher.Invoke((Action)(() =>
      {
        if (this.SelectedTickerLabel.Text == ticker)
        {
          // usado internamente para generar ordenes
          this.selectedAsk = ask;
          this.selectedBid = bid;
          this.selectedTicker = ticker;
          // solo para visualizacion
          this.SelectedAskAndBidLabel.Text = string.Format("{0}/{1}", ask, bid);
        }
      }));
    }

    private void GetTradeableSymbols_Click(object sender, RoutedEventArgs e)
    {
      this.SymbolsList.ItemsSource = FIXServicesImpl.Instance.TradableSymbols.OrderBy(x => x);
    }

    private void GetNonTradeableSymbols_Click(object sender, RoutedEventArgs e)
    {
      this.SymbolsList.ItemsSource = FIXServicesImpl.Instance.NonTradableSymbols;
    }

    private IEnumerable<string> GetSelectedTickers()
    {
      List<string> tickers = new List<string>();
      foreach (var item in this.SymbolsList.SelectedItems)
      {
        tickers.Add(item.ToString());
      }
      return tickers;
    }

    private void SubscribeSymbol_Click(object sender, RoutedEventArgs e)
    {
      var tickers = this.GetSelectedTickers();
      if (tickers.Count() > 0)
      {
        tickers.ToList().ForEach(ticker =>
        {
          Console.WriteLine("subscribing {0}", ticker);
          FIXServicesImpl.Instance.Subscribe(Counterpart.Dukascopy, ticker);
        });
      }
    }

    private void UnsubscribeSymbol_Click(object sender, RoutedEventArgs e)
    {
      var tickers = this.GetSelectedTickers();
      if (tickers.Count() > 0)
      {
        tickers.ToList().ForEach(ticker =>
        {
          Console.WriteLine("unsubscribing {0}", ticker);
          FIXServicesImpl.Instance.Unsubscribe(Counterpart.Dukascopy, ticker);
        });
      }
    }

    private void SymbolsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count == 1)
      {
        this.SelectedTickerLabel.Text = e.AddedItems[0].ToString();
      }
    }

    private void OrdersIdList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count > 0)
      {
        this.SelectedOrderIdLabel.Text = e.AddedItems[0].ToString();
      }
      else
      {
        this.SelectedOrderIdLabel.Text = "";
      }
    }

    Order lastOrder = null;

    private void ShowOrderOnGrid(Order order)
    {
      this.DatagridOrders.Items.Add(order);
    }

    private void BuyAtMarket_Click(object sender, RoutedEventArgs e)
    {
      this.lastOrder = OrdersManager.Instance.BuyOrder(Counterpart.Dukascopy, Order.GetNewClOrdID(), "", new Instrument(this.SelectedTickerLabel.Text), 1000m);
      this.lastOrder.Send();
      this.ShowOrderOnGrid(this.lastOrder);
    }

    private void SellAtMarket_Click(object sender, RoutedEventArgs e)
    {
      this.lastOrder = OrdersManager.Instance.SellOrder(Counterpart.Dukascopy, Order.GetNewClOrdID(), "", new Instrument(this.SelectedTickerLabel.Text), 1000m);
      this.lastOrder.Send();
      this.ShowOrderOnGrid(this.lastOrder);
    }

    private void StopLimitBuy_Click(object sender, RoutedEventArgs e)
    {
      this.lastOrder = OrdersManager.Instance.BuyLimitOrder(Counterpart.Dukascopy, Order.GetNewClOrdID(), "", new Instrument(this.SelectedTickerLabel.Text), 1000m, this.selectedBid - 0.0002m);
      this.lastOrder.Send();
      this.ShowOrderOnGrid(this.lastOrder);
    }

    private void StopLimitSell_Click(object sender, RoutedEventArgs e)
    {
      this.lastOrder = OrdersManager.Instance.SellLimitOrder(Counterpart.Dukascopy, Order.GetNewClOrdID(), "", new Instrument(this.SelectedTickerLabel.Text), 1000m, this.selectedAsk + 0.0002m);
      this.lastOrder.Send();
      this.ShowOrderOnGrid(this.lastOrder);
    }

    private void CancelOrder_Click(object sender, RoutedEventArgs e)
    {
      OrdersManager.Instance.CancelAll();
    }

    private void ShowPositionsButton_Click(object sender, RoutedEventArgs e)
    {
      this.DatagridPositions.ItemsSource = null;
      this.DatagridPositions.ItemsSource = PositionsManager.Instance.InstrumentsPositions;
    }

    private void Stress1Button_Click(object sender, RoutedEventArgs e)
    {
      Task.Factory.StartNew(() =>
        {
          while (true)
          {
            for (int i = 0; i < 15; i++) OrdersManager.Instance.BuyOrder(Counterpart.Dukascopy, Order.GetNewClOrdID(), "", new Instrument("EUR/USD"), 1000).Send();
            Thread.Sleep(10000);
            for (int i = 0; i < 15; i++) OrdersManager.Instance.SellOrder(Counterpart.Dukascopy, Order.GetNewClOrdID(), "", new Instrument("EUR/USD"), 1000).Send();
            Thread.Sleep(10000);
          }
        });
    }
  }
}
