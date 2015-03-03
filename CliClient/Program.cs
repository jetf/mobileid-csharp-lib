using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Specialized;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MobileId
{
    public class Program
    {
        static TraceSource logger = new TraceSource("MobileId.WebClient");
        static WebClientConfig cfg;

        static void usage() {
            Console.WriteLine(
@"Usage:
  CliClient.exe <phoneNumber> <message> [<userLanguage>]

Examples:
*  CliClient.exe +41791234567 ""Good Morning!""
*  CliClient.exe +41791234567 ""Hello Mobile ID user!"" en

");
        }

        public static void Main(string[] args)
        {
            if (args.Length != 2 && args.Length != 3)
            {
                usage();
                return;
            }
            // this would use app.config, but it doesn't work.
            // cfg = new WebClientConfig((NameValueCollection) ConfigurationManager.GetSection("MobileIdClient"));

            // We use a dedicated config file instead of app.config 
            string cfgFile = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase),
                    "MobileIdClient.xml");
            Console.WriteLine("Step 0: read application configuration " + cfgFile);
            cfg = WebClientConfig.CreateConfigFromFile(cfgFile);
            logger.TraceEvent(TraceEventType.Verbose, (int) EventId.Config, cfg.ToString());

            Console.WriteLine("Step 1: prepare request input");
            AuthRequestDto req = new AuthRequestDto();
            req.PhoneNumber = args[0];
            req.DataToBeSigned = args[1];
            if (args.Length == 3) req.UserLanguage = Util.ParseUserLanguage(args[2]);

            Console.WriteLine("Step 2: call service");
            IAuthentication webClient = new WebClientImpl(cfg);

            // for synchrnous call, uncomment next line
            // syncReqSign(req, webClient);

            // for asynchrous call, uncomment next line
            asyncReqSign(req, webClient);

            // This is experiment 3.
            //AuthResponseDto rsp, rsp2;
            //Console.WriteLine(rsp = webClient.RequestSignature(req, true));
            //string apTransId1 = req.TransId;
            //Thread.Sleep(1000);
            //req.TransId = null;
            //Console.WriteLine(rsp2 = webClient.RequestSignature(req, false));
            //Console.WriteLine();
            //Console.WriteLine("AP_TransID_1=" + apTransId1 + ", MSS_Code=" + rsp.Status.Code + ", MSS_TransID_1=" + rsp.MsspTransId);
            //Console.WriteLine("AP_TransID_2=" + req.TransId + ", MSS_Code=" + rsp2.Status.Code + ", MSS_TransID_2=" + rsp2.MsspTransId);

        }

        // Experiment 1: poll signature status with a random MSSP_TransId
        // Result:
        //   reason=WRONG_PARAM, detail="There is no such transaction."
        // Code:
            //Console.WriteLine(webClient.PollSignature(req, "hbc99"));

        // Experiment 2: send 2 async MSS_Signature with 0-delay for the SAME AP_TransID to the same MSISDN
        // Result: 
        //      1st rsp is REQUEST_OK with an non-empty MsspTransid, 
        //      2nd is code=PB_SIGNATURE_PROCESS, detail="Signature request already in progress."
        //      Exactly one SMS is sent to MSISDN.
        // Code:
            //Console.WriteLine(webClient.RequestSignature(req,true));
            //Console.WriteLine(webClient.RequestSignature(req,true)); // reuse AP_TransId

        // Experiment 3: send 2 async MSS_Signature with 0-delay for the DIFFERENT AP_TransID to the same MSISDN
        // Result: 
        //      1st rsp is REQUEST_OK with an non-empty MsspTransid, 
        //      2nd is code=PB_SIGNATURE_PROCESS, detail="Signature request already in progress."
        //      Exactly one SMS is sent to MSISDN. 
        // Remark: If the experiment is run multiple times in short delay, mobile phone may display after PIN entry "Sorry, this phone does not support Mobile ID (AA)"
        //      Guess the phoen got 2 SMS.
        // Code:
            //AuthResponseDto rsp,rsp2;
            //Console.WriteLine(rsp = webClient.RequestSignature(req, true));
            //string apTransId1 = req.TransId;
            //req.TransId = null;
            //Console.WriteLine(rsp2 = webClient.RequestSignature(req, true));
            //Console.WriteLine();
            //Console.WriteLine("AP_TransID_1=" + apTransId1 + ", MSS_Code=" + rsp.Status.Code + ", MSS_TransID_1=" + rsp.MsspTransId);
            //Console.WriteLine("AP_TransID_2=" + req.TransId + ", MSS_Code=" + rsp2.Status.Code + ", MSS_TransID_2=" + rsp2.MsspTransId);

        // Experiment 4: remove AP_PWD
        // Result: While AP_PWD="" is accepted, removing the attribute AP_PWD completely results in 
        //      (code=102, reason=MISSING_PARAM, detail="... AP_Info is missing required attribute: AP_PWD".
        // Code: need temporarily modify WebClientImpl.cs

        // Experiment 5: mixing async & synchrous calls
        // Code:


        // This example demonstrate the synchronous MSS_SignatureReq
        private static void syncReqSign(AuthRequestDto req, IAuthentication webClient)
        {
            AuthResponseDto rsp = webClient.RequestSignature(req, false);
            Console.WriteLine("Request Signature:" + rsp);
        }
        
        // This example demonstrates the asynchronous MSS_SignatureReq
        private static void asyncReqSign(AuthRequestDto req, IAuthentication webClient)
        {
            AuthResponseDto rsp, rsp2;

            rsp = webClient.RequestSignature(req, true); 
            // expect an immediate REQUEST_OK here

            if (rsp.Status.Code == ServiceStatusCode.REQUEST_OK)
            {
                Thread.Sleep(15000); // 15 sec wait time for user action
                for (int i = 15; i <= 80; i++)
                {
                    rsp2 = webClient.PollSignature(req, rsp.MsspTransId);
                    switch (rsp2.Status.Code) 
                    {
                        case ServiceStatusCode.OUSTANDING_TRANSACTION:
                            Console.Write(" [" + i + "] ");
                            Thread.Sleep(1000);
                            continue;
                        case ServiceStatusCode.SIGNATURE:
                            Console.WriteLine("SIGNATURE:" + rsp2);
                            return;
                        default:
                            Console.WriteLine("UNKNOWN RESULT:" + rsp2);
                            return;
                    }
                }
                Console.WriteLine("NO RESULT AFTER 80 sec");
            }
            else
            {
                Console.WriteLine("PREMATURE EXIT:\r\n" + rsp);
            }
        }
    }
}
