using AirTicketSalesManagement.ViewModel.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirTicketSalesManagement.Services
{
    public static class NavigationService
    {
        // Stack lưu trữ các màn hình đã điều hướng qua
        private static readonly Stack<Type> ViewModelStack = new();

        // Stack lưu trữ các tham số đi kèm
        private static readonly Stack<object> ParameterStack = new();

        public static Action<Type, object> NavigateToAction { get; set; }
        public static Action<Type, object> NavigateBackAction { get; set; }

        public static void NavigateTo<TViewModel>(object parameter = null)
        {
            // Lưu lại loại ViewModel hiện tại vào stack trước khi chuyển sang màn hình mới
            ViewModelStack.Push(typeof(TViewModel));
            ParameterStack.Push(parameter);
            NavigateToAction?.Invoke(typeof(TViewModel), parameter);
        }

        public static void NavigateBack()
        {
            if (ViewModelStack.Count > 0)
            {
                // Lấy màn hình trước đó và tham số của nó
                Type previousViewModel = ViewModelStack.Count > 0 ? ViewModelStack.Pop() : typeof(HomePageViewModel);
                object previousParameter = ParameterStack.Count > 0 ? ParameterStack.Pop() : null;

                // Điều hướng về màn hình trước đó
                NavigateBackAction?.Invoke(previousViewModel, previousParameter);
            }
        }

        public static object GetCurrentParameter()
        {
            return ParameterStack.Count > 0 ? ParameterStack.Peek() : null;
        }
    }
}
