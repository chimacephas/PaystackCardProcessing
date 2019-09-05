using System;
using System.Text;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Webkit;
using Java.Interop;
using Stack;
using Stack.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HybridWebView), typeof(HybridWebViewRenderer))]
namespace Stack.Droid
{
    public class HybridWebViewRenderer : ViewRenderer<HybridWebView, Android.Webkit.WebView>
    {

        private const string CallBackJavaScriptFunction = "function invokeCSharpAction(data){jsBridge.invokeCallbackAction(data);}";
        private const string CloseJavaScriptFunction = "function invokeCSharpCloseAction(){jsBridge.invokeCloseAction();}";
        private Context _context;

        public HybridWebViewRenderer(Context context) : base(context)
        {
            _context = context;
        }
        protected override void OnElementChanged(ElementChangedEventArgs<HybridWebView> e)
        {
            base.OnElementChanged(e);
            if (Control == null)
            {
                var webview = new Android.Webkit.WebView(_context);
                webview.Settings.JavaScriptEnabled = true;
                webview.LayoutParameters = new Android.Widget.RelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                SetNativeControl(webview);
            }
            if (e.OldElement != null)
            {
                //unsubscribe from events
                Control.RemoveJavascriptInterface("jsBridge");
                var hybridWebView = e.OldElement as HybridWebView;
                hybridWebView.Cleanup();
            }
            if (e.NewElement != null)
            {
                //subscribe to Events
                var webviewElement = Element;
                Control.AddJavascriptInterface(new JSBridge(this, _context), "jsBridge");
                string content = LoadHtmlString();
                Control.SetWebViewClient(new CustomWebViewClient(webviewElement.Data));
                // Control.LoadUrl("file:///android_asset/paystack.html");


                Control.LoadDataWithBaseURL("", content, "text/html", "UTF-8", null);
                //InjectJS(CallBackJavaScriptFunction);
                //InjectJS(CloseJavaScriptFunction);
            }
        }

        private void InjectJS(string script)
        {
            if (Control != null)
            {
                Control.LoadUrl(string.Format("javascript: {0}", script));
            }
        }

        internal string LoadHtmlString()
        {

            var html = new StringBuilder();
            html.Append("<html>");
            html.AppendLine();
            html.Append("<head>");
            html.AppendLine();
            html.Append("<script src=\"https://js.paystack.co/v2/paystack.js\" ></script>");
            html.AppendLine();
            html.Append("</head>");
            html.AppendLine();
            html.Append("<body><h4>Processing Payment...</h4>");
            html.AppendLine();
            html.Append("<script>");
            html.AppendLine();
            html.Append("var submitFunction = async function(jobj) {");
            html.AppendLine();
            html.Append("var transaction = await Paystack.Transaction.request(jobj.Data);");
            html.AppendLine();
            html.Append("var validation = Paystack.Card.validate(jobj.Info);");
            html.AppendLine();
            html.Append("if (validation.isValid) {");
            html.AppendLine();
            html.Append("await transaction.setCard(jobj.Info);");
            html.AppendLine();
            html.Append("var chargeResponse = await transaction.chargeCard();");
            html.AppendLine();
            html.Append("if (chargeResponse.status === \"success\") {");
            html.AppendLine();
            html.Append("invokeCSharpAction(chargeResponse)");
            html.AppendLine();
            html.Append("}");
            html.AppendLine();
            html.Append("if (chargeResponse.status === \"auth\") {");
            html.AppendLine();
            html.Append("invokeCSharpAction(chargeResponse)");
            html.AppendLine();
            html.Append("}");
            html.AppendLine();
            html.Append("}");
            html.AppendLine();
            html.Append("");
            html.Append("}");
            html.AppendLine();
            html.Append("</script>");
            html.AppendLine();
            html.Append("</body>");
            html.AppendLine();
            html.Append("</html>");

            System.Diagnostics.Debug.WriteLine(html);
            return html.ToString();
        }



        internal class CustomWebViewClient : WebViewClient
        {
            private string Record = "";
            public CustomWebViewClient(string record)
            {
                Record = record;
            }

            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                base.OnPageFinished(view, url);

                view.EvaluateJavascript(string.Format("javascript: {0}", CallBackJavaScriptFunction), null);
                view.EvaluateJavascript(string.Format("javascript: {0}", CloseJavaScriptFunction), null);

                view.LoadUrl(string.Format("javascript:submitFunction({0})", Record));
            }

            public override void OnPageStarted(Android.Webkit.WebView view, string url, Bitmap favicon)
            {
                base.OnPageStarted(view, url, favicon);
            }
            public override void OnReceivedError(Android.Webkit.WebView view, IWebResourceRequest request, WebResourceError error)
            {
                base.OnReceivedError(view, request, error);
            }

            public override void OnReceivedError(Android.Webkit.WebView view, [GeneratedEnum] ClientError errorCode, string description, string failingUrl)
            {
                base.OnReceivedError(view, errorCode, description, failingUrl);
            }

            public override void OnReceivedHttpError(Android.Webkit.WebView view, IWebResourceRequest request, WebResourceResponse errorResponse)
            {
                base.OnReceivedHttpError(view, request, errorResponse);
            }
        }
    }



    internal class JSBridge : Java.Lang.Object
    {
        private readonly WeakReference<HybridWebViewRenderer> hybridWebViewRenderer;
        private Context _context;
        public JSBridge(HybridWebViewRenderer hybridRenderer, Context context)
        {
            hybridWebViewRenderer = new WeakReference<HybridWebViewRenderer>(hybridRenderer);
            _context = context;
        }

        [JavascriptInterface]
        [Export("invokeCallbackAction")]
        public void InvokeAction(string data)
        {
            HybridWebViewRenderer hybridRenderer;

            if (hybridWebViewRenderer != null && hybridWebViewRenderer.TryGetTarget(out hybridRenderer))
            {
                hybridRenderer.Element.InvokeAction(data);
            }
        }
        //[JavascriptInterface]
        //[Export("invokeCloseAction")]
        //public void InvokeCloseAction()
        //{
        //    HybridWebViewRenderer hybridRenderer;

        //    if (hybridWebViewRenderer != null && hybridWebViewRenderer.TryGetTarget(out hybridRenderer))
        //    {
        //        hybridRenderer.Element.InvokeCloseAction();
        //    }
        //}
    }
}
