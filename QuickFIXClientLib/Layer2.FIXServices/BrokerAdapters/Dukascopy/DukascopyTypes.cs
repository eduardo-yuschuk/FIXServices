using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QuickFix;

namespace Layer2.FIXServices.BrokerAdapters.Dukascopy
{
  #region Fields

  public class NotifPriority : QuickFix.IntField
  {
    public const int FIELD = 7003;

    public NotifPriority() : base(FIELD) { }
    public NotifPriority(int data) : base(FIELD, data) { }
  }

  public class AccountName : QuickFix.StringField
  {
    public const int FIELD = 7004;

    public AccountName() : base(FIELD) { }
    public AccountName(string data) : base(FIELD, data) { }
  }

  public class Leverage : QuickFix.DoubleField
  {
    public const int FIELD = 7005;

    public Leverage() : base(FIELD) { }
    public Leverage(double data) : base(FIELD, data) { }
  }

  public class UsableMargin : QuickFix.DoubleField
  {
    public const int FIELD = 7006;

    public UsableMargin() : base(FIELD) { }
    public UsableMargin(double data) : base(FIELD, data) { }
  }

  public class Equity : QuickFix.DoubleField
  {
    public const int FIELD = 7007;

    public Equity() : base(FIELD) { }
    public Equity(double data) : base(FIELD, data) { }
  }

  public class Amount : QuickFix.DoubleField
  {
    public const int FIELD = 7008;

    public Amount() : base(FIELD) { }
    public Amount(double data) : base(FIELD, data) { }
  }

  public class Slippage : QuickFix.DoubleField
  {
    public const int FIELD = 7011;

    public Slippage() : base(FIELD) { }
    public Slippage(double data) : base(FIELD, data) { }
  }

  #endregion

  #region Messages

  public class Notification : Message // U1 <- donde va el binding con esto?
  {
    public Notification() { }
    public Notification(Message message) : base(message.ToString()) { }

    public NotifPriority get(NotifPriority value) { value.setValue(base.getInt(NotifPriority.FIELD)); return value; }
    public Text get(Text value) { value.setValue(base.getString(Text.FIELD)); return value; }
    public AccountName get(AccountName value) { value.setValue(base.getString(AccountName.FIELD)); return value; }
    public Account get(Account value) { value.setValue(base.getString(Account.FIELD)); return value; }

    public NotifPriority getNotifPriority() { return new NotifPriority(base.getInt(NotifPriority.FIELD)); }
    public Text getText() { return new Text(base.getString(Text.FIELD)); }
    public AccountName getAccountName() { return new AccountName(base.getString(AccountName.FIELD)); }
    public Account getAccount() { return new Account(base.getString(Account.FIELD)); }

    public bool isSet(NotifPriority field) { throw new NotImplementedException(); }
    public bool isSet(Text field) { throw new NotImplementedException(); }
    public bool isSet(AccountName field) { throw new NotImplementedException(); }
    public bool isSet(Account field) { throw new NotImplementedException(); }

    public bool isSetNotifPriority() { return base.isSetField(NotifPriority.FIELD); }
    public bool isSetText() { return base.isSetField(Text.FIELD); }
    public bool isSetAccountName() { return base.isSetField(AccountName.FIELD); }
    public bool isSetAccount() { return base.isSetField(Account.FIELD); }

    public void set(NotifPriority value) { throw new NotImplementedException(); }
    public void set(Text value) { throw new NotImplementedException(); }
    public void set(AccountName value) { throw new NotImplementedException(); }
    public void set(Account value) { throw new NotImplementedException(); }
  }

  public class AccountInfo : Message // U2
  {
    public AccountInfo() { }
    public AccountInfo(Message message) : base(message.ToString()) { }

    public Leverage get(Leverage value) { value.setValue(base.getDouble(Leverage.FIELD)); return value; }
    public UsableMargin get(UsableMargin value) { value.setValue(base.getDouble(UsableMargin.FIELD)); return value; }
    public Equity get(Equity value) { value.setValue(base.getDouble(Equity.FIELD)); return value; }
    public Currency get(Currency value) { value.setValue(base.getString(Currency.FIELD)); return value; }
    public AccountName get(AccountName value) { value.setValue(base.getString(AccountName.FIELD)); return value; }

    public Leverage getLeverage() { return new Leverage(base.getDouble(Leverage.FIELD)); }
    public UsableMargin getUsableMargin() { return new UsableMargin(base.getDouble(UsableMargin.FIELD)); }
    public Equity getEquity() { return new Equity(base.getDouble(Equity.FIELD)); }
    public Currency getCurrency() { return new Currency(base.getString(Currency.FIELD)); }
    public AccountName getAccountName() { return new AccountName(base.getString(AccountName.FIELD)); }

    public bool isSet(Leverage field) { throw new NotImplementedException(); }
    public bool isSet(UsableMargin field) { throw new NotImplementedException(); }
    public bool isSet(Equity field) { throw new NotImplementedException(); }
    public bool isSet(Currency field) { throw new NotImplementedException(); }
    public bool isSet(AccountName field) { throw new NotImplementedException(); }

    public bool isSetLeverage() { return base.isSetField(Leverage.FIELD); }
    public bool isSetUsableMargin() { return base.isSetField(UsableMargin.FIELD); }
    public bool isSetEquity() { return base.isSetField(Equity.FIELD); }
    public bool isSetCurrency() { return base.isSetField(Currency.FIELD); }
    public bool isSetAccountName() { return base.isSetField(AccountName.FIELD); }

    public void set(Leverage value) { throw new NotImplementedException(); }
    public void set(UsableMargin value) { throw new NotImplementedException(); }
    public void set(Equity value) { throw new NotImplementedException(); }
    public void set(Currency value) { throw new NotImplementedException(); }
    public void set(AccountName value) { throw new NotImplementedException(); }
  }

  public class InstrumentPositionInfo : Message // U3
  {
    public InstrumentPositionInfo() { }
    public InstrumentPositionInfo(Message message) : base(message.ToString()) { }

    public Symbol get(Symbol value) { value.setValue(base.getString(Symbol.FIELD)); return value; }
    public Amount get(Amount value) { value.setValue(base.getDouble(Amount.FIELD)); return value; }
    public AccountName get(AccountName value) { value.setValue(base.getString(AccountName.FIELD)); return value; }
    public Account get(Account value) { value.setValue(base.getString(Account.FIELD)); return value; }

    public Symbol getSymbol() { return new Symbol(base.getString(Symbol.FIELD)); }
    public Amount getAmount() { return new Amount(base.getDouble(Amount.FIELD)); }
    public AccountName getAccountName() { return new AccountName(base.getString(AccountName.FIELD)); }
    public Account getAccount() { return new Account(base.getString(Account.FIELD)); }

    public bool isSet(Symbol field) { throw new NotImplementedException(); }
    public bool isSet(Amount field) { throw new NotImplementedException(); }
    public bool isSet(AccountName field) { throw new NotImplementedException(); }
    public bool isSet(Account field) { throw new NotImplementedException(); }

    public bool isSetSymbol() { return base.isSetField(Symbol.FIELD); }
    public bool isSetAmount() { return base.isSetField(Amount.FIELD); }
    public bool isSetAccountName() { return base.isSetField(AccountName.FIELD); }
    public bool isSetAccount() { return base.isSetField(Account.FIELD); }

    public void set(Symbol value) { throw new NotImplementedException(); }
    public void set(Amount value) { throw new NotImplementedException(); }
    public void set(AccountName value) { throw new NotImplementedException(); }
    public void set(Account value) { throw new NotImplementedException(); }
  }

  public class ActivationResponse : Message // U6
  {
    public ActivationResponse() { }
    public ActivationResponse(Message message) : base(message.ToString()) { }

    public Username get(Username value) { value.setValue(base.getString(Username.FIELD)); return value; }
    public Account get(Account value) { value.setValue(base.getString(Account.FIELD)); return value; }

    public Username getUsername() { return new Username(base.getString(Username.FIELD)); }
    public Account getAccount() { return new Account(base.getString(Account.FIELD)); }

    public bool isSet(Username field) { throw new NotImplementedException(); }
    public bool isSet(Account field) { throw new NotImplementedException(); }

    public bool isSetUsername() { return base.isSetField(Username.FIELD); }
    public bool isSetAccount() { return base.isSetField(Account.FIELD); }

    public void set(Username value) { throw new NotImplementedException(); }
    public void set(Account value) { throw new NotImplementedException(); }
  }

  public class AccountInfoRequest : Message // U7
  {
    public AccountInfoRequest() : base(new BeginString("FIX.4.4"), new MsgType("U7"))
    {
    }

    public Account get(Account value) { value.setValue(base.getString(Account.FIELD)); return value; }

    public Account getAccount() { return new Account(base.getString(Account.FIELD)); }

    public bool isSet(Account field) { throw new NotImplementedException(); }

    public bool isSetAccount() { return base.isSetField(Account.FIELD); }

    public void set(Account value) { base.setField(value); }
  }

  #endregion
}
