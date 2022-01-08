/**
 * MIT License
 * Copyright(c) 2021 Roman Parak
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

/**
 * Author   : Roman Parak
 * Email    : Roman.Parak@outlook.com
 * File Name: Main.c
 * Github   : https://github.com/rparak
 */

/** < Include B&R Automation libraries (declarations for B&R ANSI C extensions) */
#include <bur/plctypes.h>

/** < Include other libraries (mathematical functions, character string handling, etc.) */
#include <math.h>

#ifdef _DEFAULT_INCLUDES
	#include <AsDefault.h>
#endif

/**
* Array (REAL) type test variable for OPC UA communication.
*
*	W (Write): B&R Automation PLC (OPC UA Server) -> Client (Python / C#)
*	R (Read): Client (Python / C#) -> B&R Automation PLC (OPC UA Server)
*/
_LOCAL REAL Test_Variable_REAL_W[10];
_LOCAL REAL Test_Variable_REAL_R[10];
/// The time variable equals the cycle time
_LOCAL REAL Time_Variable;

/// Constants (Macros): Additional parameters for the SINE function
#define CONST_SINE_VAR_1 1
#define CONST_SINE_VAR_2 25

/**
 * Program Intitialization
 */
void _INIT ProgramInit(void)
{
	/// Reset time variable
	Time_Variable = 0.0;
}

/**
 * Program Cyclic 
 * 
 * Duration (Cycle Time): 10000 [µs] 
 * Tolerance            : 10000 [µs]
 */
void _CYCLIC ProgramCyclic(void)
{
	int i;
	for(i = 0; i < (int)(sizeof(Test_Variable_REAL_W)/sizeof(Test_Variable_REAL_W[0])); i++){
		/// Calculate the value of the SINE function with additional parameters
		Test_Variable_REAL_W[i] = CONST_SINE_VAR_1 * sin(Time_Variable / CONST_SINE_VAR_2);
	}
	
	/// Increase time variable
	Time_Variable++;
}

