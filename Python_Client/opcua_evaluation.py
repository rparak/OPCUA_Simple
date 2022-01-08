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
File Name: opcua_evaluation.py
## =========================================================================== ## 
"""

# System (Default)
import sys
# Pandas (Data analysis and manipulation) [pip3 install pandas]
import pandas as pd
# Matplotlib (Visualization) [pip3 install matplotlib]
import matplotlib.pyplot as plt

def main():
    # Input Data:
    #   PY: Python_OPC_UA_Data_Evaluation_Sync, Python_OPC_UA_Data_Evaluation_ASync
    #   C#: CSharp_OPC_UA_Data_Evaluation_Sync
    FILE_NAME = 'CSharp_OPC_UA_Data_Evaluation_Sync'

    # Read Data from the File
    ua_client_data = pd.read_csv(f'Data//{FILE_NAME}.txt')
    print('[INFO] The data is successfully read from the file.')

    # Assign data to variables
    #   Time [ms]
    time = ua_client_data[ua_client_data.columns[0]]
    print(f'[INFO] Number of input data: {len(time)}')

    # Create figure
    fig, ax = plt.subplots()

    fig.suptitle(f'File name: {FILE_NAME}.txt', fontsize = 20)

    for i, col in enumerate(ua_client_data.columns):
        if i > 0:
            # Raw Data
            plt.plot(time, ua_client_data[col], label=f'OPCUA Variable {i}')

    # Axis Parameters:
    #   Label
    ax.set_xlabel(r't (ms)')
    ax.set_ylabel(f'Sine (t)')
    #   Other dependencies
    ax.grid(linewidth = 0.75, linestyle = '--')
    ax.legend(fontsize=10.0)

    print('[INFO] Display the result.')
    # Display the result
    plt.show()

if __name__ == '__main__':
    sys.exit(main())
