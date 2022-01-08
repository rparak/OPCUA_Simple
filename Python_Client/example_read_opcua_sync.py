"""
## =========================================================================== ## 
MIT License
Copyright (c) 2021 Roman Parak
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
## =========================================================================== ## 
Author   : Roman Parak
Email    : Roman.Parak@outlook.com
Github   : https://github.com/rparak
File Name: example_read_opcua_sync.py
## =========================================================================== ## 
"""

# System (Default)
import sys
# Opc UA (Free OPC-UA Library) [pip3 install opcua]
from opcua import Client
from opcua import ua
# Pandas (Data analysis and manipulation) [pip3 install pandas]
import pandas as pd
# Time (Time access and conversions)
import time

def main():
    # Configuration Client <-> Server
    #    Address and Port (B&R Automation PLC)
    IP_ADDRESS = "127.0.0.1"
    PORT       = "4840"
    #   Time Step (10 ms)
    OPC_UA_DT = 0.01

    try:
        # Initialization OPC UA Client
        ua_client = Client("opc.tcp://"+ IP_ADDRESS + ":" + PORT, timeout=OPC_UA_DT)
        ua_client.connect()

        result = []
        counter = 0
        while counter < 1000:
            # t_{0}: time start
            t_0 = time.time()

            """
            Description:
                Get Node:
                    ns = all variables use a number (6)
                    s (Part 1) = Program (Task) Name, 
                        Note: If the variable is global, the task name is AsGlobalPV.
                    s (Part 2) = Variable (read / write)

                    string -> 'ns=6;s=::Program:Test_Variable_REAL_W'
            """
            test_var_node = ua_client.get_node('ns=6;s=::Program:Test_Variable_REAL_W')
            # Get the value(s) from the node.
            test_var_value = test_var_node.get_value()

            # Save the value(s) in the array.
            result.append(test_var_value)

            # Increase counter variable
            counter += 1

            # t_{1}: time stop
            #   t = t_{1} - t_{0}
            t = time.time() - t_0

            # Recalculate the time
            if t < OPC_UA_DT:
                time.sleep(OPC_UA_DT - t)

    except KeyboardInterrupt:
        sys.exit(1)
    finally:
        data_collection = pd.DataFrame(data = result)
        data_collection.to_csv('OPC_UA_Data_Evaluation_Sync.txt')
        print('[INFO] The data has been successfully saved.')

        ua_client.disconnect()

if __name__ == '__main__':
    sys.exit(main())
