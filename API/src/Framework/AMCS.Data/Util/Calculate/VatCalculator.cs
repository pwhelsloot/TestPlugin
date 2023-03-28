namespace AMCS.Data.Util.Calculate
{
  using System;

  public class VatCalculationExeption : Exception
  {
    public VatCalculationExeption() : base() { }
    public VatCalculationExeption(string message) : base(message) { }
  }

  public class VATCalculator
  {
    private decimal _vatRate;

    public VATCalculator(decimal vatRate)
    {
      //  people may mistakenly submit something like 0.175 instead of 17.5
      //  don't try and be clever and account for this as it will cause confusion if instances of this class are created
      //  through the same constructor but with different looking values for vatRate.
      if (vatRate > 0 && vatRate < 1)
        throw new VatCalculationExeption("'vatRate' must not be between 0 and 1");
      
      _vatRate = vatRate;

    }

    public decimal GetNetFromGross(decimal gross)
    {
      return gross / ((_vatRate / 100) + 1);
    }

    public decimal GetVATFromGross(decimal gross)
    {
      return gross - GetNetFromGross(gross);
    }

    public decimal GetGrossFromNet(decimal net)
    {
      return net + GetVATFromNet(net);
    }

    public decimal GetVATFromNet(decimal net)
    {
      return (net / 100) * _vatRate;
    }

    public decimal RoundVATTotalForInvoice(decimal gross)
    {
      return PriceRoundCalculator.RoundPriceForInvoice(gross);
    }
  }
}
