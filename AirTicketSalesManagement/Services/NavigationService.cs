using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Services
{
    public static class NavigationService
    {
        private static readonly Stack<object> NavigationStack = new();

        public static Action<Type, object> NavigateToAction { get; set; }
        public static Action NavigateBackAction { get; set; }

        public static void NavigateTo<TViewModel>(object parameter = null)
        {
            NavigationStack.Push(parameter);
            NavigateToAction?.Invoke(typeof(TViewModel), parameter);
        }

        public static void NavigateBack()
        {
            NavigateBackAction?.Invoke();
        }

        public static object GetCurrentParameter()
        {
            return NavigationStack.Count > 0 ? NavigationStack.Pop() : null;
        }
    }
}
