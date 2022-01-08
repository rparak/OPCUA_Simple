# OPC UA Communication between B&R Automation PLC (Server) and simple Client (C#, Python)

## Requirements:

**Software:**
```bash
B&R Automation Studio, Visual Studio (or something similar)
```

| Software/Package      | Link                                                                                  |
| --------------------- | ------------------------------------------------------------------------------------- |
| B&R Automation        | https://www.br-automation.com/en/downloads/#categories=Software-1344987434933         |
| Visual Studio         | https://visualstudio.microsoft.com/downloads/                                         |

**Programming Language:**
```bash
C#, Python
```

**Packages:**
```bash
C# (OPCFoundation), Python (FreeOpcUa)
```
OPCFoundation: https://www.nuget.org/packages/OPCFoundation.NetStandard.Opc.Ua/

FreeOpcUa: https://github.com/FreeOpcUa/python-opcua

## Project Description:

The project is focused on a simple demonstration of client-server communication via OPC UA. In this case, it is a B&R Automation PLC (server), which communicates with the client via the C# or Python application. An example application is reading and writing multiple data of the same type (REAL/Float).

In PLC configuration packages, it is mandatory to activate OPC UA communication with port 4840 and enable the structure of variables for reading and writing.

The application uses performance optimization using multi-threaded programming. Communication (C# application) can be used in Unity3D for digital twin / augmented reality or in the ROS system (Python or similar approaches as C++).

The project was realized at the Institute of Automation and Computer Science, Brno University of Technology, Faculty of Mechanical Engineering (NETME Centre - Cybernetics and Robotics Division).

**Node Identification (NodeId):**

```bash
ns = all variables use a number (6)
s (Part 1) = Program (Task) Name, 
  Note: If the variable is global, the task name is AsGlobalPV.
s (Part 2) = Variable (read / write)

string -> "ns=6;s=::Task:Var"
```

<p align="center">
<img src=https://github.com/rparak/OPCUA_Simple/blob/main/images/communication_scheme.png width="650" height="350">
</p>

## Project Hierarchy:

**Server (B&R PLC) - Repositary [/BaR_PLC_Server/OPCUa_Client_simple/test/Logical/]:**

```bash
Server (B&R PLC) - Repositary [/BaR_PLC_Server/OPCUa_Client_simple/test/Logical/]
[ AS Main Program ] /Program/Main.c/

Client (C#) - Repositary [/C_Sharp_Client/Opcua_client/]
[ Main Program (Application) ] /Program.cs/

Client (Python) - Repositary [/Python_Client/]:
[ Main Program (Sync. Write)  ] /example_write_opcua_sync.py/
[ Main Program (Sync. Read)   ] /example_read_opcua_sync.py/
[ Main Program (ASync. Write) ] /example_write_opcua_async.py/
[ Main Program (ASync. Read)  ] /example_read_opcua_async.py/
[ Data Evalation              ] /opcua_evaluation.py/

Data - Repositary [/Python_Client/Data/]:
[ Collected data (Python, C#) ] /CSharp_OPC_UA_Data_Evaluation_Sync.txt/ 
                                 /Python_OPC_UA_Data_Evaluation_ASync.txt/ 
                                 /Python_OPC_UA_Data_Evaluation_Sync.txt/
```

## Evaluation: Client -> Server:

**Python Sync.:**
<p align="center">
 <img src="https://github.com/rparak/OPCUA_Simple/blob/main/images/Python_OPCUA_Client_Server_Sync.PNG" width="800" height="450">
</p>

**Python ASync.:**
<p align="center">
 <img src="https://github.com/rparak/OPCUA_Simple/blob/main/images/Python_OPCUA_Client_Server_ASync.PNG" width="800" height="450">
</p>

**CSharp:**
<p align="center">
 <img src="https://github.com/rparak/OPCUA_Simple/blob/main/images/CSharp_OPCUA_Client_Server.PNG" width="800" height="450">
</p>

## Evaluation: Server -> Client:
**Python Sync.:**
<p align="center">
 <img src="https://github.com/rparak/OPCUA_Simple/blob/main/images/Python_OPCUA_Server_Client_Sync.png" width="800" height="450">
</p>

**Python ASync.:**
<p align="center">
 <img src="https://github.com/rparak/OPCUA_Simple/blob/main/images/Python_OPCUA_Server_Client_ASync.png" width="800" height="450">
</p>

**CSharp Sync.:**
<p align="center">
 <img src="https://github.com/rparak/OPCUA_Simple/blob/main/images/CSharp_OPCUA_Server_Client_Sync.png" width="800" height="450">
</p>

## Contact Info:
Roman.Parak@outlook.com

## Citation (BibTex)
```bash
@misc{RomanParak_DT_BaR,
  author = {Roman Parak},
  title = {Data collection from the B&R Automation control system using the OPC UA communication protocol},
  year = {2020-2021},
  publisher = {GitHub},
  journal = {GitHub repository},
  howpublished = {\url{https://github.com/rparak/OPCUA_Simple/}}
}
```

## License
[MIT](https://choosealicense.com/licenses/mit/)
