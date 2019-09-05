using System;
using Xamarin.Forms;

namespace Stack
{
        public class HybridWebView : View
        {
            Action<object> action;
            public static readonly BindableProperty UriProperty = BindableProperty.Create(
              propertyName: "Uri",
              returnType: typeof(string),
              declaringType: typeof(HybridWebView),
              defaultValue: default(string));

            public string Uri
            {
                get { return (string)GetValue(UriProperty); }
                set { SetValue(UriProperty, value); }
            }

            public static readonly BindableProperty DataProperty = BindableProperty.Create(nameof(Data), typeof(string), typeof(HybridWebView), defaultValue: default(string), defaultBindingMode: BindingMode.TwoWay);

            public string Data
            {
                get { return (string)GetValue(DataProperty); }
                set { SetValue(DataProperty, value); }
            }

            public void RegisterAction(Action<object> callback)
            {
                action = callback;
            }

            public void Cleanup()
            {
                action = null;
            }

            public void InvokeAction(object data)
            {
                if (action == null || data == null)
                {
                    return;
                }
                action.Invoke(data);
            }
        }
    
}
