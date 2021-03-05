/**
 * MIT License
 * Copyright(c) 2020 Roman Parak
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
 */


/** < Include B&R Automation libraries (declarations for B&R ANSI C extensions) */
#include <bur/plctypes.h>

#ifdef _DEFAULT_INCLUDES
	#include <AsDefault.h>
#endif


/// Variable for the motion start process.
_LOCAL BOOL start_var;
/// Actual moving position.
_LOCAL REAL move_var;

/// DEFINE (Maximum Interation)
#define MAX_VALUE 100;


/**
 * Program Intitialization
 */
void _INIT ProgramInit(void)
{
	/// initial start variable for motion -> read from OPCUA client
	start_var = 0;
	/// motion variable -> write to OPCUA client (information about actual position)
	move_var = 0;
}

/**
 * Program Cyclic 
 * 
 * Duration (Cycle Time): 500000  [µs] 
 * Tolerance            : 10000000[µs]
 */
void _CYCLIC ProgramCyclic(void)
{
	/**
     * The basic condition for a simple demonstration of OPCUA Client <-> Server Communication.
	 */
	if(start_var == 1 && move_var != MAX_VALUE){
		move_var = move_var + 1;
	}else{
		/// null variables -> the motion variable is equal to the maximum value
		move_var  = 0;
		start_var = 0;
	}
}
