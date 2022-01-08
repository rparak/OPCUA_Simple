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
File Name: example_write_opcua_sync.py
## =========================================================================== ## 
"""

# System (Default)
import sys
# Opc UA (Free OPC-UA Library) [pip3 install opcua]
from opcua import Client
from opcua import ua
# Numpy (Array computing) [pip3 install numpy]
import numpy as np
# Time (Time access and conversions)
import time

# Constants (Macros): Additional parameters for the SINE function
CONST_SINE_VAR_1 = 1
CONST_SINE_VAR_2 = 25

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

        # Calculate the value of the SINE function with additional parameters
        Time_Variable = np.arange(0.0, 1000.0, 1.0)
        Test_Variable_REAL_W = CONST_SINE_VAR_1 * np.sin(Time_Variable / CONST_SINE_VAR_2)

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
            test_var_node = ua_client.get_node('ns=6;s=::Program:Test_Variable_REAL_R')
            # Get the value(s) from the node.
            test_var_value = test_var_node.get_value()
            # Set the value(s) to the node.
            test_var_node.set_value(ua.DataValue(ua.Variant([Test_Variable_REAL_W[counter]]*len(test_var_value), ua.VariantType.Float)))

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
        ua_client.disconnect()

if __name__ == '__main__':
    sys.exit(main())
