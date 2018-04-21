using System.Collections.Specialized;

namespace WitBird.Com.Pay
{
    /// <summary>
    /// Direct payment interface.
    /// </summary>
    public interface IPayService
    {
        /// <summary>
        /// A boolean value that indicates whether the specified payment interface configuration has already been initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// This value indicates which online payment service user use.
        /// </summary>
        string CurrentPayment { get; }

        /// <summary>
        /// Gets the response code if verify request successfully.
        /// </summary>
        /// <returns></returns>
        string ResponseCodeSucceed { get; }

        /// <summary>
        /// Gets the response code if verify request failed.
        /// </summary>
        /// <returns></returns>
        string ResponseCodeFailed { get; }

        /// <summary>
        /// Initializes online payment interface configuration.
        /// </summary>
        /// <param name="merchantId">The unique merchant id in online bank's database which provides the online payment interface.</param>
        /// <param name="key">The key which used to request a trade with the merchant id.</param>
        /// <param name="inputCharset">The encoding charset.</param>
        /// <param name="returnUrl">The sync return url when pay completed. If user close payment page directly, this return page should not be displayed.</param>
        /// <param name="notifyUrl">The async notify url when pay completed. This return page should be called in asynchronous by payment service.</param>
        void Initialize(string merchantId, string key, string inputCharset, string returnUrl, string notifyUrl);

        /// <summary>
        /// Builds the html to request online payment according to trade parameters.
        /// </summary>
        /// <returns>Returns a request html for online payment.</returns>
        string BuildRequest(PayRequestCriteria criteria);

        /// <summary>
        /// Verifies whether the GET or POST request is from a trusted online payment service or not.
        /// </summary>
        /// <param name="requestParamamters">The parameters which includes some required data to verify.</param>
        /// <returns>Returns true indicating the request is valid, returns false is invalid.</returns>
        bool Verify(NameValueCollection requestParamamters);

        /// <summary>
        /// Parses the result from trusted online payment service.
        /// </summary>
        /// <param name="requestParamamters"></param>
        /// <returns></returns>
        PayResult ParseResult(NameValueCollection requestParamamters);
    }
}
