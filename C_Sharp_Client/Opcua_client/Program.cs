/****************************************************************************
MIT License
Copyright(c) 2021 Roman Parak
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
File Name: Program.cs
****************************************************************************/

// System Lib.
using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

// OPC UA
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;

/*
    Description:
        Node:
            ns = all variables use a number (6)
            s(Part 1) = Program(Task) Name, 
                Note: If the variable is global, the task name is AsGlobalPV.
            s (Part 2) = Variable(read / write)

            string -> 'ns=6;s=::Program:Test_Variable_REAL_W'
 */

namespace OpcUa_client
{
    public static class OpcUa_Read_Data
    {
        // Network IP Address
        public static string ip_address;
        // Network Port Number
        public const ushort port_number = 4840;
        // Comunication Speed (ms)
        public static int time_step;
        // Node: 'ns=6;s=::Program:Test_Variable_REAL_R'
        public static NodeId node;
        // Result value from the node
        public static float[] value = new float[10];
    }

    public static class OpcUa_Write_Data
    {
        // Network IP Address
        public static string ip_address;
        // Network Port Number
        public const ushort port_number = 4840;
        // Comunication Speed (ms)
        public static int time_step;
        // Node: 'ns=6;s=::Program:Test_Variable_REAL_R[..]'
        public static string[] node = new string[10];
    }

    public static class Program_Data
    {
        // Results matrix from data collection (Server -> Client)
        public static List<List<float>> result = new List<List<float>>();
        // Additional parameters for the SINE function
        public const double SINE_VAR_1 = 1;
        public const double SINE_VAR_2 = 25;
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Variable used  to save data to a file from the OPC UA server (read)
            bool save_data = false;

            // Configuration Client <-> Server
            //   Ip Address (B&R Automation PLC)
            OpcUa_Read_Data.ip_address = OpcUa_Write_Data.ip_address = "127.0.0.1";
            //   Time Step (10 ms)
            OpcUa_Read_Data.time_step = OpcUa_Write_Data.time_step = 10;

            // Initialization of the B&R Automation PLC OPC UA nodes
            //      Type NodeId: (Client: Read, Server: Write)
            OpcUa_Read_Data.node = "ns=6;s=::Program:Test_Variable_REAL_W";
            //      Type String: (Client: Write, Server: Read)
            for(int i = 0; i < OpcUa_Write_Data.node.Length; ++i) 
            {
                OpcUa_Write_Data.node[i] = $"ns=6;s=::Program:Test_Variable_REAL_R[{i}]";
            }

            /*
                Note:
                    Uncomment the OPC UA communication type for Read/Write data.
             */

            // Start Read {OPC UA Communication}
            //OpcUa_Read opcua_read_plc = new OpcUa_Read();
            //opcua_read_plc.Start();

            // Start Read {OPC UA Communication}
            OpcUa_Write opcua_write_plc = new OpcUa_Write();
            opcua_write_plc.Start();

            Console.WriteLine("[INFO] Stop (y):");
            // Stop communication
            string stop_rs = Convert.ToString(Console.ReadLine());

            if (stop_rs == "y")
            {
                if (save_data == true)
                {
                    Write_Data(Program_Data.result, "CSharp_OPC_UA_Data_Evaluation_Sync.txt");
                    Console.WriteLine("File saved successfully!");
                }

                // Destroy B&R OpcUa Client (Read / Write)
                //opcua_read_plc.Destroy();
                opcua_write_plc.Destroy();

                // Application quit
                Environment.Exit(0);
            }
        }

        public static void Write_Data(List<List<float>> result, string file_path)
        {
            try
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@file_path, true))
                {
                    int i = 0;
                    foreach (var row in result)
                    {
                        int j = 0;
                        file.Write(i.ToString() + ",");
                        foreach (var element in row)
                        {
                            if (row.Count > j + 1)
                            {
                                file.Write(element.ToString() + ",");
                            }
                            else
                            {
                                file.Write(element.ToString());
                            }

                            j++;
                        }
                        file.WriteLine();
                        i++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Error: ", ex);
            }
        }
    }

    class OpcUa_Read
    {
        // Initialization of Class variables
        //  Thread
        private Thread opcua_thread = null;
        private bool exit_thread = false;
        //  OPCUa Client 
        ApplicationConfiguration app_configuration = new ApplicationConfiguration();

        public void OpcUa_Read_Thread()
        {
            try
            {
                // OPCUa Client configuration
                app_configuration = OpcUa_Client_Configuration();
                // Establishing communication
                EndpointDescription end_point = CoreClientUtils.SelectEndpoint("opc.tcp://" + OpcUa_Read_Data.ip_address + ":" + OpcUa_Read_Data.port_number, useSecurity: false);
                // Create session
                Session client_session = OpcUa_Create_Session(app_configuration, end_point);

                // Initialization timer
                var t = new Stopwatch();

                int counter = 0;
                while (exit_thread == false)
                {
                    // t_{0}: Timer start.
                    t.Start();

                    // Reading actual data from the source(OpcUa Client)
                    OpcUa_Read_Data.value = Array.ConvertAll(client_session.ReadValue(OpcUa_Read_Data.node).ToString().Split(new[] { '{', '}', '|', }, StringSplitOptions.RemoveEmptyEntries), float.Parse);

                    // Data collection for evaluation
                    if (counter < 1000)
                    {
                        Program_Data.result.Add(OpcUa_Read_Data.value.ToList());
                    }

                    // Increase counter variable
                    counter++;

                    // t_{1}: Timer stop.
                    t.Stop();

                    // Recalculate the time: t = t_{1} - t_{0} -> Elapsed Time in milliseconds
                    if (t.ElapsedMilliseconds < OpcUa_Read_Data.time_step)
                    {
                        Thread.Sleep(OpcUa_Read_Data.time_step - (int)t.ElapsedMilliseconds);
                    }

                    // Reset (Restart) timer.
                    t.Restart();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Communication Problem: {0}", e);
            }
        }

        Session OpcUa_Create_Session(ApplicationConfiguration client_configuration, EndpointDescription client_end_point)
        {
            return Session.Create(client_configuration, new ConfiguredEndpoint(null, client_end_point, EndpointConfiguration.Create(client_configuration)), false, "", 10000, null, null).GetAwaiter().GetResult();
        }

        ApplicationConfiguration OpcUa_Client_Configuration()
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

            /*
            var application = new ApplicationInstance
            {
                ApplicationName = "OPCUa_AS",
                ApplicationType = ApplicationType.Client,
                ApplicationConfiguration = config
            };
            
            application.CheckApplicationInstanceCertificate(false, 2048).GetAwaiter().GetResult();
            */

            return config;
        }

        public void Start()
        {
            exit_thread = false;
            // Start a thread to read OPCUA PLC
            opcua_thread = new Thread(new ThreadStart(OpcUa_Read_Thread));
            opcua_thread.IsBackground = true;
            opcua_thread.Start();
        }
        public void Stop()
        {
            exit_thread = true;
            // Stop a thread
            Thread.Sleep(100);
        }
        public void Destroy()
        {
            // Stop a thread (OPCUA communication)
            Stop();
            Thread.Sleep(100);
        }
    }

    class OpcUa_Write
    {
        // Initialization of Class variables
        //  Thread
        private Thread opcua_thread = null;
        private bool exit_thread = false;
        //  OPCUa Client 
        ApplicationConfiguration app_configuration = new ApplicationConfiguration();

        public void OpcUa_Write_Thread()
        {
            try
            {
                // OPCUa Client configuration
                app_configuration = OpcUa_Client_Configuration();
                // Establishing communication
                EndpointDescription end_point = CoreClientUtils.SelectEndpoint("opc.tcp://" + OpcUa_Read_Data.ip_address + ":" + OpcUa_Read_Data.port_number, useSecurity: false);
                // Create session
                Session client_session = OpcUa_Create_Session(app_configuration, end_point);

                // Initialization timer
                var t = new Stopwatch();

                int counter = 0;
                while (exit_thread == false)
                {
                    // t_{0}: Timer start.
                    t.Start();

                    // Writing data to source (OpcUa client) 
                    foreach (string node_id in OpcUa_Write_Data.node)
                    {
                        OpcUa_Write_Value(client_session, node_id, (Program_Data.SINE_VAR_1 * Math.Sin(counter / Program_Data.SINE_VAR_2)).ToString());
                    }

                    // Increase counter variable
                    counter++;

                    // t_{1}: Timer stop.
                    t.Stop();

                    // Recalculate the time: t = t_{1} - t_{0} -> Elapsed Time in milliseconds
                    if (t.ElapsedMilliseconds < OpcUa_Write_Data.time_step)
                    {
                        Thread.Sleep(OpcUa_Write_Data.time_step - (int)t.ElapsedMilliseconds);
                    }

                    // Reset (Restart) timer.
                    t.Restart();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Communication Problem: {0}", e);
            }
        }

        Session OpcUa_Create_Session(ApplicationConfiguration client_configuration, EndpointDescription client_end_point)
        {
            return Session.Create(client_configuration, new ConfiguredEndpoint(null, client_end_point, EndpointConfiguration.Create(client_configuration)), false, "", 10000, null, null).GetAwaiter().GetResult();
        }

        ApplicationConfiguration OpcUa_Client_Configuration()
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
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 10000 },
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

        bool OpcUa_Write_Value(Session client_session, string node_id, string value_write)
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
                return (results[0] == StatusCodes.Good) ? true : false;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                return false;
            }
        }

        public void Start()
        {
            exit_thread = false;
            // Start a thread to read OPCUA PLC
            opcua_thread = new Thread(new ThreadStart(OpcUa_Write_Thread));
            opcua_thread.IsBackground = true;
            opcua_thread.Start();
        }
        public void Stop()
        {
            exit_thread = true;
            // Stop a thread
            Thread.Sleep(100);
        }
        public void Destroy()
        {
            // Stop a thread (OPCUA communication)
            Stop();
            Thread.Sleep(100);
        }
    }
}
