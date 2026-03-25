public class HarvestedItem
{
    public ConstructionType Type;
    public int SellPrice;
    public int Amount;

    public HarvestedItem(ConstructionType type, int sellPrice, int amount)
    {
        Type = type;
        SellPrice = sellPrice;
        Amount = amount;
    }
}