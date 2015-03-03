
using System.Text;

namespace MobileId
{
    /// <summary>
    /// Output of RequestSignature(...) service call
    /// </summary>
    public class AuthResponseDto
    {
        public ServiceStatus Status { get; set; }

        /// <summary>
        /// optional details. Content depends on Status
        /// </summary>
        public object Detail { get; set;  }

        /// <summary>
        /// MSSP_TransID
        /// </summary>
        public string MsspTransId { get; set; }

        public byte[] Signature { get; set; }

        /// <summary>
        /// Mainly used to construct error response.
        /// </summary>
        /// <param name="StatusCode"></param>
        /// <param name="payload"></param>
        public AuthResponseDto(ServiceStatusCode statusCode, string payload)
        {
            this.Status = new ServiceStatus(statusCode);
            this.Detail = payload;
        }

        public AuthResponseDto(ServiceStatusCode statusCode)
        {
            this.Status = new ServiceStatus(statusCode);
        }

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("MsspTransid=").Append(MsspTransId);
            sb.Append(", Status: {").Append(Status);
            sb.Append("}, Detail: ").Append(Detail);
            return sb.ToString();
        }

    }
}
