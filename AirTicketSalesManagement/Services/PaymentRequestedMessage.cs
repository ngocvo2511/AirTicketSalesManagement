using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AirTicketSalesManagement.Services
{
    public class PaymentRequestedMessage : ValueChangedMessage<string>
    {
        public PaymentRequestedMessage(string paymentUrl) : base(paymentUrl) { }
    }
}
