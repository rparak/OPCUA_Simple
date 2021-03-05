"""
## =========================================================================== ## 

MIT License

Copyright (c) 2020 Roman Parak

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
File Name: client_opcua_simple.py

## =========================================================================== ## 
"""

# System
import sys

# Opc UA
from opcua import Client
from opcua import ua

# Time
import time

# Configuration Client <-> Server
IP_ADDRESS = "127.0.0.1"
PORT = "4840"
# Connect
client = Client("opc.tcp://"+ IP_ADDRESS+":"+ PORT)
client.connect()

# << Input >> 
# Read Data:
# start -> t/f {Bool}
node_read_start_v   = client.get_node("ns=6;s=::Server_t:start_var")
# move -> increase value {Float (Automation Studio B&R -> REAL/LREAL)}
node_read_move_v    = client.get_node("ns=6;s=::Server_t:move_var")
# << Output >> 
# Write data: start -> t/f {Bool}
node_write_start_v  = client.get_node("ns=6;s=::Server_t:start_var")

def main():
    # Initialization variables
    # Bool
    read_start_v = False
    is_moving    = False
    # Float
    read_move_v  = 0.0

    while True:
        # ----- READ/WRITE init. param ----- #
        # get infor start t/f {input}
        read_start_v = node_read_start_v.get_value()

        if read_start_v == True:
            read_move_v = node_read_move_v.get_value()

            # init. format string {actual move}
            format_string_moving = "Start command: {start_fv} || Move variable: {move_fv}".format(start_fv = read_start_v, move_fv = read_move_v)
            # print result
            print(format_string_moving)

            # Is moving variable
            is_moving = True
        else:
            # init. format string {start command}
            format_string_moving = "Start command: {start_fv}".format(start_fv = read_start_v)
            # print result
            print(format_string_moving)

            # Is moving variable
            is_moving = False

        if is_moving == False:
            # write data
            # Start moving {Start - ON} -> Edge variable
            node_write_start_v.set_value(ua.DataValue(ua.Variant(True, ua.VariantType.Boolean)))
            
        # Sleep 500ms
        time.sleep(0.5)

    # ----- DISCONNECT CLIENT ----- #
    client.disconnect()
    
if __name__ == '__main__':
    sys.exit(main())
