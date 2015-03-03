
using System.Text;
namespace MobileId
{
    public class ServiceStatus
    {
        /// <summary>
        /// Green, Yellow, Red. See Mobile ID Reference Guide
        /// </summary>
        public ServiceStatusColor Color {  
            get {
                return mapColorFromCode(this.Code);
            }
        }

        /// <summary>
        /// Example: 500
        /// </summary>
        public ServiceStatusCode Code { get; set; }
           

        /// <summary>
        /// Example: SIGNATURE
        /// </summary>
        public string Message { 
            get {
                return this.Code.ToString();
            }
        }

        private static ServiceStatusColor mapColorFromCode(ServiceStatusCode code)
        {
            switch (code)
            {
                // green
                case ServiceStatusCode.SIGNATURE:
                case ServiceStatusCode.VALID_SIGNATURE:
                case ServiceStatusCode.USER_CANCEL:
                case ServiceStatusCode.REQUEST_OK:
                    return ServiceStatusColor.Green;

                // yellow
                case ServiceStatusCode.UNKNOWN_CLIENT:
                case ServiceStatusCode.PIN_NR_BLOCKED:
                case ServiceStatusCode.CARD_BLOCKED:
                case ServiceStatusCode.NO_KEY_FOUND:
                case ServiceStatusCode.NO_CERT_FOUND:
                    return ServiceStatusColor.Yellow;

                 // the rest is red
                default: 
                    return ServiceStatusColor.Red;
            }
        }

        //public static string MapCodeToMessage(ServiceStatusCode code)
        //{
        //    return code.ToString();
        //}

        public ServiceStatus(ServiceStatusCode code, string message)
        {
            this.Code = code;
            if ((message != null) && (message != this.Message))
                throw new System.OverflowException("ServiceStatus.Message does not match the registered Message");

        }

        public ServiceStatus(ServiceStatusCode code)
        {
            this.Code = code;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Code=").Append((int) Code);
            sb.Append(", Reason=").Append(Message);
            sb.Append(", Color=").Append(Color);
            return sb.ToString();
        }
    }
}
