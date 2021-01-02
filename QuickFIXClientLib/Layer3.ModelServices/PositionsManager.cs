using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Layer2.FIXServices.BrokerAdapters;
using System.ComponentModel;
using Utils;
using System.Collections.ObjectModel;

namespace Layer3.ModelServices
{
  public class PositionsManager
  {
    private PositionsManager() { }
    private static PositionsManager _instance = null;
    public static PositionsManager Instance
    {
      get
      {
        if (_instance == null) _instance = new PositionsManager();
        return _instance;
      }
    }

    public List<Position> InstrumentsPositions = new List<Position>();

    public void DeliverInstrumentPositionInfo(InstrumentPositionInfoAdapted instrumentPositionInfoAdapted)
    {
      var item = this.InstrumentsPositions.FirstOrDefault(pos => pos.Symbol == instrumentPositionInfoAdapted.Symbol);
      if (item != null)
      {
        item.DeliverInfo(instrumentPositionInfoAdapted);
      }
      else
      {
        this.InstrumentsPositions.Add(new Position(instrumentPositionInfoAdapted));
      }
    }
  }
}
