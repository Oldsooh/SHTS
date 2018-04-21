namespace WitBird.Com.Pay
{
    /// <summary>
    /// Defines 4 types indicates the trade result. Includes success, failed, invalid and unknow 4 types.
    /// </summary>
    public enum PayStatus
    {
        /// <summary>
        /// This value indicates that the request is from a trusted online payment server and trade successfully.
        /// </summary>
        Success,

        /// <summary>
        /// This value indicates that the request is from a trusted online payment server but trade failed.
        /// </summary>
        Failed,

        /// <summary>
        /// This value indicates that request is from a UNTRUSTED online payment server.
        /// </summary>
        Invalid,

        /// <summary>
        /// Un-known trade status.
        /// </summary>
        UnKnow
    }
}
