public class BuyDataModel
{
    public BuyDataModel(string email, long creditCard, string expirationDate)
    {
        this.email = email;
        this.creditCard = creditCard;
        this.expirationDate = expirationDate;
    }

    public string email;
    public long creditCard;
    public string expirationDate;
}
