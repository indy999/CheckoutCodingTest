namespace Checkout.PaymentGateway.Models
{
    using System.Text.RegularExpressions;

    public class PaymentRequest
    {
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string CVV { get; set; }
        public string ExpiryDate { get; set; }

        public string MaskCardNumber()
        {
            if (CardNumber == null)
            {
                return null;
            }

            if (CardNumber.Length <= 4)
            {
                return Regex.Replace(CardNumber, "\\d", "*");
            }

            return Regex.Replace(
                       CardNumber.Substring(0, CardNumber.Length - 4),
                       "\\d",
                       "*") +
                   CardNumber.Substring(CardNumber.Length - 4, 4);
        }
    }
}