using CommunityToolkit.Mvvm.Messaging.Messages;

namespace AirTicketSalesManagement.Messages
{
    public class WebViewClearCacheMessage : ValueChangedMessage<bool>
    {
        public WebViewClearCacheMessage() : base(true) { }
    }
} 