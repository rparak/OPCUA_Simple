# OPC UA Communication between B&R Automation PLC (Server) and simple Client (C#, Python)

## Requirements:

**Software:**
```bash
B&R Automation Studio, Visual Studio (or something similar)
```
B&R Automation: https://www.br-automation.com/en/downloads/#categories=Software-1344987434933

Visual Studio: https://visualstudio.microsoft.com/downloads/

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

The project is focused on a simple demonstration of Client / Server communication via OPC UA. In this case, it is a B&R Automation PLC (server), which communicates with the client via the C# or Python application. An example of an application is reading and writing data of different types (BOOL, REAL / LREAL).

In PLC configuration packages, it is mandatory to activate OPC UA communication with port 4840 and enable the structure of variables for reading and writing.

The application uses performance optimization using multi-threaded programming. Communication (C# application) can be used in Unity3D for digital twin / augmented reality or in the ROS system (Python or similar approaches as C++).

**Node Identification (NodeId):**

```bash
ns = all variables use a number (6)
s (Part 1) = Task/Program name
s (Part 2) = Variable (read / write)

string -> "ns=6;s=::Task:Var"
```

<p align="center">
<img src=https://github.com/rparak/OPCUA_Simple/blob/main/images/communication_scheme.png width="650" height="350">
</p>

## Project Hierarchy:

**Server (B&R PLC) - Repositary [/BaR_PLC_Server/OPCUa_Client_simple/Logical/]:**

```bash
[ AS Main Program ] /Server_t/Main.c/
```

**Client (C#) - Repositary [/C_Sharp_Client/Opcua_client/Opcua_client/]:**

```bash
[ Main Program (Application) ] /Program.cs/
```

**Client (Python) - Repositary [/Python_Client/]:**

```bash
[ Main Program (Script) ] /client_opcua_simple.py/
```

## Contact Info:
Roman.Parak@outlook.com

## License
[MIT](https://choosealicense.com/licenses/mit/)
