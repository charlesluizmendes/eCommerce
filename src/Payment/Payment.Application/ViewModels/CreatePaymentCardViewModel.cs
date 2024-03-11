namespace Payment.Application.ViewModels
{
    public class CreatePaymentCardViewModel
    {
        public string Number { get; set; }
        public string ClientName { get; set; }
        public string DateValidate { get; set; }
        public int SecurityCode { get; set; }
    }
}
