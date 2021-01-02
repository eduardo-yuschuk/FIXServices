using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Utils;
using Layer2.FIXServices.BrokerAdapters;

namespace Layer3.ModelServices
{
  public class Position : INotifyPropertyChanged
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

    public Position(InstrumentPositionInfoAdapted instrumentPositionInfoAdapted)
    {
      this.DeliverInfo(instrumentPositionInfoAdapted);
    }

    public string Account { get; set; }
    public string AccountName { get; set; }
    public decimal Amount { get; set; }
    public string Symbol { get; set; }

    public void DeliverInfo(InstrumentPositionInfoAdapted instrumentPositionInfoAdapted)
    {
      this.Account = instrumentPositionInfoAdapted.Account;
      this.AccountName = instrumentPositionInfoAdapted.AccountName;
      this.Amount = instrumentPositionInfoAdapted.Amount;
      this.Symbol = instrumentPositionInfoAdapted.Symbol;
      this.NotifyChanges();
    }
  }
}
