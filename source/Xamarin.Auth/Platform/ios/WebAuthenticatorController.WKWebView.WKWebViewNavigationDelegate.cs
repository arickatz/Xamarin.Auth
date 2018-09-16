using System;
using System.Threading.Tasks;
using System.Text;
using System.Collections.Generic;
using Foundation;
using UIKit;
using WebKit;

namespace Xamarin.Auth
{
    internal partial class WebAuthenticatorController
    {
        //==============================================================================================================
        #region     WKWebView
        /// <summary>
        /// WKWebView WKWebViewNavigationDelegate, WKWebViewUIDelegate, WKWebViewJacascriptMessageHandler
        /// </summary>
        internal class WKWebViewUIDelegate : WebKit.WKUIDelegate
        {
            WebAuthenticatorController controller = null;

            public WKWebViewUIDelegate(WebAuthenticatorController c)
            {
                controller = c;

                return;
            }

            public override void RunJavaScriptAlertPanel
                                    (
                                        WKWebView webView,
                                        string message,
                                        WKFrameInfo frame,
                                        Action completionHandler
                                    )
            {
                // custom javascript Alert() code possible

                #if DEBUG
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"WKWebViewUIDelegate.RunJavaScriptAlertPanel ");
                sb.AppendLine($"        webView.Url.AbsoluteString = {webView.Url.AbsoluteString}");
                System.Diagnostics.Debug.WriteLine(sb.ToString());
                #endif

                return;
            }
        }

        internal class WKWebViewNavigationDelegate : WebKit.WKNavigationDelegate
        {
            WebAuthenticatorController controller = null;

            public WKWebViewNavigationDelegate(WebAuthenticatorController c)
            {
                controller = c;

                return;
            }

            public override void DecidePolicy
                                    (
                                        WKWebView webView,
                                        WKNavigationAction navigationAction,
                                        Action<WKNavigationActionPolicy> decisionHandler
                                    )
            {
                #if DEBUG
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"WKWebViewNavigationDelegate.DecidePolicy ");
                sb.AppendLine($"        webView.Url.AbsoluteString = {webView.Url.AbsoluteString}");
                System.Diagnostics.Debug.WriteLine(sb.ToString());
                #endif

                Uri uri = new Uri(webView.Url.AbsoluteString);
                string fragment = uri.Fragment;

                if
                    (
                        fragment.Contains("access_token")
                        ||
                        fragment.Contains("state")
                        ||
                        fragment.Contains("expires_in")
                        ||
                        fragment.Contains("error")
                    )
                {
                    IDictionary<string, string> fragments = WebEx.FormDecode(uri.Fragment);

                    Account account = new Account
                                            (
                                                "",
                                                new Dictionary<string, string>(fragments)
                                            );
                    controller.authenticator.OnSucceeded(account);
                }
                else if
                    (
                        fragment.Contains("code")
					)
                {
                    throw new NotImplementedException("code - Explicit/Server");
                }
                // Navigation Allowed?
                // page will not load without this one!
                decisionHandler(WKNavigationActionPolicy.Allow);

                //decisonHandler(WKNavigationActionPolicy.Cancel);    
                //decisonHandler(WKNavigationActionPolicy.Allow);


                return;
            }

            public override void DidFailNavigation
                                    (
                                        WKWebView webView,
                                        WKNavigation navigation,
                                        NSError error
                                    )
            {
                // Navigation Failed

                #if DEBUG
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"WKWebViewNavigationDelegate.DidFailNavigation ");
                sb.AppendLine($"        webView.Url.AbsoluteString = {webView.Url.AbsoluteString}");
                System.Diagnostics.Debug.WriteLine(sb.ToString());
                #endif

                return;
            }

            public override void DidFailProvisionalNavigation
                                    (
                                        WKWebView webView,
                                        WKNavigation navigation,
                                        NSError error
                                    )
            {
                // Provisional Navigation Failed? WAT?

                if  // loading custom url scheme will result in unsupported URL
                    (
                        error.Code == -1002
                        ||
                        error.LocalizedDescription == "unsupported URL"
                    )
                {
                    //custom url schema
                    // NSUrl url_ios = webView.Url; // old URL
                    string url_redirect = error.UserInfo[new NSString("NSErrorFailingURLKey")].ToString();
                    System.Uri uri = new Uri(url_redirect);
                    IDictionary<string, string> fragment = WebEx.FormDecode(uri.Fragment);

                    Account account = new Account
                                            (
                                                "",
                                                new Dictionary<string, string>(fragment)
                                            );
                    controller.authenticator.OnSucceeded(account);
                }

                #if DEBUG
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"WKWebViewNavigationDelegate.DidFailProvisionalNavigation ");
                sb.AppendLine($"        error.LocalizedDescription = {error.LocalizedDescription}");
                sb.AppendLine($"        webView.Url.AbsoluteString = {webView.Url.AbsoluteString}");
                System.Diagnostics.Debug.WriteLine(sb.ToString());
                #endif

                return;
            }

            public override void DidStartProvisionalNavigation
                                    (
                                        WKWebView webView,
                                        WKNavigation navigation
                                    )
            {
                // Provisional Navigation Started

                #if DEBUG
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"WKWebViewNavigationDelegate.DidStartProvisionalNavigation ");
                sb.AppendLine($"        webView.Url.AbsoluteString = {webView.Url.AbsoluteString}");
                System.Diagnostics.Debug.WriteLine(sb.ToString());
                #endif

                return;
            }

            public override void DidFinishNavigation
                                    (
                                        WKWebView webView,
                                        WKNavigation navigation
                                    )
            {
                // Navigation Finished

                #if DEBUG
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"WKWebViewNavigationDelegate.DidFinishNavigation ");
                sb.AppendLine($"        webView.Url.AbsoluteString = {webView.Url.AbsoluteString}");
                System.Diagnostics.Debug.WriteLine(sb.ToString());
                #endif

                return;
            }

            public override void DidCommitNavigation
                                    (
                                        WKWebView webView,
                                        WKNavigation navigation
                                    )
            {
                // mandatory

                #if DEBUG
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"WKWebViewNavigationDelegate.DidCommitNavigation ");
                sb.AppendLine($"        webView.Url.AbsoluteString = {webView.Url.AbsoluteString}");
                System.Diagnostics.Debug.WriteLine(sb.ToString());
                #endif

                return;
            }
        }

        protected class WKWebViewJacascriptMessageHandler : WebKit.WKScriptMessageHandler
        {
            WebAuthenticatorController controller = null;

            public WKWebViewJacascriptMessageHandler(WebAuthenticatorController c)
            {
                controller = c;

                return;
            }

            public override void DidReceiveScriptMessage
                                    (
                                        WKUserContentController userContentController,
                                        WKScriptMessage message
                                    )
            {
                // do whatever you need to do with the message here

                #if DEBUG
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"WKWebViewJacascriptMessageHandler.DecidePolicy ");
                System.Diagnostics.Debug.WriteLine(sb.ToString());
                #endif

                return;
            }
        }
        #endregion  WKWebView
        //==============================================================================================================
    }
}

