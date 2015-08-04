using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Witbird.SHTS.Web.Models
{
    /// <summary>
    /// Ajax result object.
    /// </summary>
    public class AjaxResult
    {
        /// <summary>
        /// Gets or sets the exception information.
        /// </summary>
        /// <value>The exception information.</value>
        public String ExceptionInfo { get; set; }

        /// <summary>
        /// Gets or sets the errorcode.
        /// </summary>
        /// <value>The ErrorCode.</value>
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is success.
        /// </summary>
        /// <value><c>true</c> if this instance is success; otherwise, <c>false</c>.</value>
        public bool IsSuccess
        {
            get
            {
                return this.ExceptionInfo == null;
            }
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public object Data { get; set; }
    }
}
