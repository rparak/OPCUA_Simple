/****************************************************************************
MIT License
Copyright(c) 2020 Roman Parak
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*****************************************************************************
Author   : Roman Parak
Email    : Roman.Parak @outlook.com
Github   : https://github.com/rparak
File Name: ar_object_control.cs
****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

namespace Opcua_client
{
    class Program
    {
        // -------------------- ApplicationConfiguration -------------------- //
        static ApplicationConfiguration client_configuration_r = new ApplicationConfiguration();
        static ApplicationConfiguration client_configuration_w = new ApplicationConfiguration();
        // -------------------- EndpointDescription -------------------- //
        static EndpointDescription client_end_point_r, client_end_point_w;
        // -------------------- Session -------------------- //
        static Session client_session_r, client_session_w;
        // -------------------- Thread -------------------- //
        static Thread opcua_client_r_Thread, opcua_client_w_Thread;
        // -------------------- NodeId -------------------- //
        static NodeId node_read_start_v, node_read_move_v;
        // -------------------- String -------------------- //
        static string node_write_start_v;
        static string program_name;
        // -------------------- Bool -------------------- //
        static bool read_start_v;
        static bool opcua_c_r_while, opcua_c_w_while;
        static bool start_move_m, is_moving_m;
        // -------------------- Float -------------------- //
        static float read_move_v;

        // ------------------------------------------------------------------------------------------------------------------------//
        // ------------------------------------------------ MAIN FUNCTION {Cyclic} ------------------------------------------------//
        // ------------------------------------------------------------------------------------------------------------------------//
        static void Main(string[] args)
        {
            // ------------------------ Initialization { OPCUa Config.} ------------------------//
            // Robot IP Address
            string ip_adr_robot = "127.0.0.1";
            // Robot Port
            string port_adr_robot = "4840";

            // ------------------------ Threading Block { OPCUa Read Data } ------------------------//
            opcua_c_r_while = true;
            opcua_client_r_Thread = new Thread(() => OPCUa_r_thread_function(ip_adr_robot, port_adr_robot));
            opcua_client_r_Thread.IsBackground = true;
            opcua_client_r_Thread.Start();

            // ------------------------ Threading Block { OPCUa Write Data } ------------------------//
            opcua_c_w_while = true;
            opcua_client_w_Thread = new Thread(() => OPCUa_w_thread_function(ip_adr_robot, port_adr_robot));
            opcua_client_w_Thread.IsBackground = true;
            opcua_client_w_Thread.Start();

            // ------------------------ Main Block { Control of the PLC (B&R) } ------------------------//
            try
            {
                // Program name {Task}
                program_name = "Server_t";
                // Node {Read}
                node_read_start_v = "ns=6;s=::"+ program_name + ":start_var";
                node_read_move_v  = "ns=6;s=::" + program_name + ":move_var";
                // Node {Write}
                node_write_start_v = "ns=6;s=::" + program_name + ":start_var";

                // -------------------- Main Cycle {While} -------------------- //
                while (true)
                {

                    // -------------------- Move Instruction {R/W} -------------------- //
                    if (read_start_v == false)
                    {
                        Console.WriteLine("Start command: {0}", read_start_v);
                        // Start moving {Start - ON} -> Edge variable
                        start_move_m = true;
                    }
                    else
                    {
                        Console.WriteLine("Start command: {0} || Move variable: {1}", read_start_v, read_move_v);
                        // Is moving variable
                        is_moving_m = true;
                    }

                    if(is_moving_m == true)
                    {
                        // Start moving {Start - OFF} -> Edge variable
                        start_move_m = false;
                    }
                    // Thread Sleep {100 ms}
                    Thread.Sleep(100);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Application_Quit();
            }

        }

        // ------------------------------------------------------------------------------------------------------------------------//
        // -------------------------------------------------------- FUNCTIONS -----------------------------------------------------//
        // ------------------------------------------------------------------------------------------------------------------------//

        // -------------------- Abort Threading Blocks -------------------- //
        static void Application_Quit()
        {
            opcua_c_r_while = false;
            opcua_client_r_Thread.Abort();

            opcua_client_w_Thread.Abort();
            opcua_c_w_while = false;

        }

        // ------------------------ Threading Block { OPCUa Read Data } ------------------------//
        static void OPCUa_r_thread_function(string ip_adr, string port_adr)
        {
            // OPCUa client configuration
            client_configuration_r = opcua_client_configuration();
            // Establishing communication
            client_end_point_r = CoreClientUtils.SelectEndpoint("opc.tcp://" + ip_adr + ":" + port_adr, useSecurity: false, operationTimeout: 10000);
            // Create session
            client_session_r = opcua_create_session(client_configuration_r, client_end_point_r);

            // Threading while {read data}
            while (opcua_c_r_while)
            {
                // Read Data - BOOL
                read_start_v = bool.Parse(client_session_r.ReadValue(node_read_start_v).ToString());

                if (read_start_v == true)
                {
                    // Read Data - Float (Automation Studio B&R -> REAL/LREAL)
                    read_move_v = float.Parse(client_session_r.ReadValue(node_read_move_v).ToString());
                }
            }
        }

        // ------------------------ Threading Block { OPCUa Write Data } ------------------------//
        static void OPCUa_w_thread_function(string ip_adr, string port_adr)
        {
            // OPCUa client configuration
            client_configuration_w = opcua_client_configuration();
            // Establishing communication
            client_end_point_w = CoreClientUtils.SelectEndpoint("opc.tcp://" + ip_adr + ":" + port_adr, useSecurity: false, operationTimeout: 10000);
            // Create session
            client_session_w = opcua_create_session(client_configuration_w, client_end_point_w);

            // Threading while {write data}
            while (opcua_c_w_while)
            {
                if (start_move_m == true)
                {
                    // Write Data - BOOL
                    opcua_write_value(client_session_w, node_write_start_v, "True");
                }
            }
        }

        // ------------------------ OPCUa Client {Application -> Configuration (STEP 1)} ------------------------//
        static ApplicationConfiguration opcua_client_configuration()
        {
            // Configuration OPCUa Client {W/R -> Data}
            var config = new ApplicationConfiguration()
            {
                // Initialization (Name, Uri, etc.)
                ApplicationName = "OPCUa_AS", // OPCUa AS (Automation Studio B&R)
                ApplicationUri = Utils.Format(@"urn:{0}:OPCUa_AS", System.Net.Dns.GetHostName()),
                // Type -> Client
                ApplicationType = ApplicationType.Client,
                SecurityConfiguration = new SecurityConfiguration
                {
                    // Security Configuration - Certificate
                    ApplicationCertificate = new CertificateIdentifier { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault", SubjectName = Utils.Format(@"CN={0}, DC={1}", "OPCUa_AS", System.Net.Dns.GetHostName()) },
                    TrustedIssuerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities" },
                    TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications" },
                    RejectedCertificateStore = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates" },
                    AutoAcceptUntrustedCertificates = true,
                    AddAppCertToTrustedStore = true
                },
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas { OperationTimeout = 10000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 50000 },
                TraceConfiguration = new TraceConfiguration()
            };
            config.Validate(ApplicationType.Client).GetAwaiter().GetResult();
            if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
            {
                config.CertificateValidator.CertificateValidation += (s, e) => { e.Accept = (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted); };
            }

            var application = new ApplicationInstance
            {
                ApplicationName = "OPCUa_AS",
                ApplicationType = ApplicationType.Client,
                ApplicationConfiguration = config
            };
            application.CheckApplicationInstanceCertificate(false, 2048).GetAwaiter().GetResult();

            return config;
        }

        // ------------------------ OPCUa Client {Application -> Create Session (STEP 2)} ------------------------//
        static Session opcua_create_session(ApplicationConfiguration client_configuration, EndpointDescription client_end_point)
        {
            return Session.Create(client_configuration, new ConfiguredEndpoint(null, client_end_point, EndpointConfiguration.Create(client_configuration)), false, "", 10000, null, null).GetAwaiter().GetResult();
        }

        // ------------------------ OPCUa Client {Write Value (Define - Node)} ------------------------//
        static bool opcua_write_value(Session client_session, string node_id, string value_write)
        {
            // Initialization
            NodeId init_node = NodeId.Parse(node_id);

            try
            {
                // Find Node (OPCUa Client)
                Node node = client_session.NodeCache.Find(init_node) as Node;
                DataValue init_data_value = client_session.ReadValue(node.NodeId);

                // Preparation data for writing
                WriteValue value = new WriteValue()
                {
                    NodeId = init_node,
                    AttributeId = Attributes.Value,
                    Value = new DataValue(new Variant(Convert.ChangeType(value_write, init_data_value.Value.GetType()))),
                };

                // Initialization (Write)
                WriteValueCollection init_write = new WriteValueCollection();
                // Append variable
                init_write.Add(value);

                StatusCodeCollection results = null;
                DiagnosticInfoCollection diagnosticInfos = null;

                // Wriate data
                client_session.Write(null, init_write, out results, out diagnosticInfos);

                // Check Result (Status)
                if (results[0] == StatusCodes.Good)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
        }
    }
}
