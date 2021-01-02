using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickFix;
using System.Threading;
using System.Diagnostics;
using Layer2.FIXServices;

namespace Layer1.QuickFIX
{
  public partial class QuickFixHost : QuickFix.MessageCracker, QuickFix.Application
  {
    #region Components and Start

    readonly string settingsPath = @"C:\hfsystem\Code\ExternalDependencies\quickfix\DukascopyQuickFIXConfiguration.txt";
    SessionSettings settings;
    FileStoreFactory storeFactory;
    FileLogFactory logFactory;
    DefaultMessageFactory messageFactory;
    SocketInitiator initiator;
    IFIXServices fixServices;

    public void Start(IFIXServices fixServices)
    {
      this.fixServices = fixServices;
      this.settings = new SessionSettings(settingsPath);
      this.storeFactory = new FileStoreFactory(this.settings);
      this.logFactory = new FileLogFactory(this.settings);
      this.messageFactory = new DefaultMessageFactory();
      this.initiator = new SocketInitiator(this, this.storeFactory, this.settings, this.messageFactory);
      this.initiator.start();
    }

    #endregion

    public void fromAdmin(Message message, SessionID sessionID)
    {
      Console.WriteLine("fromAdmin: {0}", message.getHeader().getField(MsgType.FIELD));
      this.crack(message, sessionID);
    }

    public void fromApp(Message message, SessionID sessionID)
    {
      //Console.WriteLine("fromApp: {0}", message.getHeader().getField(MsgType.FIELD));

      string msgType = message.getHeader().getField(MsgType.FIELD);
      if (msgType.StartsWith("U"))
      {
        switch (msgType)
        {
          case "U1":
            this.onMessage(new Layer2.FIXServices.BrokerAdapters.Dukascopy.Notification(message), sessionID);
            break;
          case "U2":
            this.onMessage(new Layer2.FIXServices.BrokerAdapters.Dukascopy.AccountInfo(message), sessionID);
            break;
          case "U3":
            this.onMessage(new Layer2.FIXServices.BrokerAdapters.Dukascopy.InstrumentPositionInfo(message), sessionID);
            break;
          case "U6":
            this.onMessage(new Layer2.FIXServices.BrokerAdapters.Dukascopy.ActivationResponse(message), sessionID);
            break;
          default:
            throw new NotImplementedException();
        }
      }
      else
      {
        this.crack(message, sessionID);
      }
    }

    #region Events

    public void onCreate(SessionID sessionID)
    {
      Console.WriteLine("onCreate: {0}", sessionID);
    }

    public void onLogon(SessionID sessionID)
    {
      Console.WriteLine("onLogon: {0}", sessionID);
    }

    public void onLogout(SessionID sessionID)
    {
      Console.WriteLine("onLogout: {0}", sessionID);
    }

    #endregion

    public void toAdmin(Message message, SessionID sessionID)
    {
      if (message.getHeader().getField(MsgType.FIELD) == MsgType.Logon)
      {
        message.setField(Username.FIELD, "YQwAm");
        message.setField(Password.FIELD, "YQwAm");
      }

      //Console.WriteLine("toAdmin: {0}", message.getHeader().getField(MsgType.FIELD));
    }

    public void toApp(Message message, SessionID sessionID)
    {
      //Console.WriteLine("toApp: {0}", message.getHeader().getField(MsgType.FIELD));
    }
  }
}
