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

The project is focused on a simple demonstration of Client / Server communication via OPC UA. In this case, it is a B&R PLC server and the client is a Python or C# application. An example of an application is reading and writing data of different type (BOOL, REAL / LREAL).

Communication (C# application) can be used in Unity3D for digital twins / augmented reality or in the ROS system (Python or similar approaches as C++).

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
