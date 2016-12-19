using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using vFlash.Controls;
using vFlash.Models;
using Windows.Storage;

namespace vFlash.Utils
{

    /// <summary>
    /// Handles authentication for a Microsoft Azure account.
    /// </summary>
    class AuthHandler : DelegatingHandler
    {
        /// <summary>
        /// Client used for logging in.
        /// </summary>
        public IMobileServiceClient Client { get; set; }

        /// <summary>
        /// Delegate to save the user into storage.
        /// </summary>
        private Action<MobileServiceUser> saveUserDelegate;

        /// <summary>
        /// Method to save the user into storage.
        /// </summary>
        /// <param name="saveUserDelegate"></param>
        public AuthHandler(Action<MobileServiceUser> saveUserDelegate)
        {
            this.saveUserDelegate = saveUserDelegate;
        }

        /// <summary>
        /// Try to log the user into their Microsoft account.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (this.Client == null)
            {
                Client = new MobileServiceClient(App.MobileService.MobileAppUri);
            }

            // Cloning the request, in case we need to send it again
            var clonedRequest = await CloneRequest(request);
            var response = await base.SendAsync(clonedRequest, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                // Oh noes, user is not logged in – we got a 401
                // Log them in, this time hardcoded with Facebook but you would
                // trigger the login presentation in your application
                try
                {
                    var user = await this.Client.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
                    // we're now logged in again.
                    

                    // Clone the request
                    clonedRequest = await CloneRequest(request);

                    // Save the user to the app settings
                    this.saveUserDelegate(user);

                    clonedRequest.Headers.Remove("X-ZUMO-AUTH");
                    // Set the authentication header
                    clonedRequest.Headers.Add("X-ZUMO-AUTH", user.MobileServiceAuthenticationToken);

                    // Resend the request
                    response = await base.SendAsync(clonedRequest, cancellationToken);
                }
                catch (InvalidOperationException)
                {
                    // user cancelled auth, so lets return the original response
                    return response;
                }
            }

            return response;
        }

        /// <summary>
        /// Clones the request incase it's needed again.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<HttpRequestMessage> CloneRequest(HttpRequestMessage request)
        {
            var result = new HttpRequestMessage(request.Method, request.RequestUri);
            foreach (var header in request.Headers)
            {
                result.Headers.Add(header.Key, header.Value);
            }

            if (request.Content != null && request.Content.Headers.ContentType != null)
            {
                var requestBody = await request.Content.ReadAsStringAsync();
                var mediaType = request.Content.Headers.ContentType.MediaType;
                result.Content = new StringContent(requestBody, Encoding.UTF8, mediaType);
                foreach (var header in request.Content.Headers)
                {
                    if (!header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        result.Content.Headers.Add(header.Key, header.Value);
                    }
                }
            }

            return result;
        }



    }
}
