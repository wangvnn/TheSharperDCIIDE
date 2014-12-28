using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KimHaiQuang.TheDCIBabyIDE.Usecases
{
    class USECASE1_Open_CONTEXT_from_FILE
    {
        /* USE CASE 1: Open CONTEXT from FILE
        # Primary Actor: USER
        # Precondition: The DCI EDITOR is loaded and running
        # Postcondition: The DCI EDITOR opens the Context in the DCI EDITOR
        # Trigger: USER selects a CONTEXT FILE to be opened in the DCI EDITOR
        # Main Success Scenario:
        # 1. USER selects a CONTEXT FILE
        #   2. SYSTEM tracks selected CONTEXT FILE
        # 3. USER wants to open selected CONTEXT FILE in the DCI EDITOR
        #   4. SYSTEM parses the CONTEXT FILE to find the CONTEXT INFO (Parse CONTEXT FILE)
        #   5. SYSTEM display CONTEXT INFO in the DCI EDITOR
        */
    }

    class USECASE2_Parse_CONTEXT_FILE //Habit 
    {
        /* USE CASE 2: Parse CONTEXT FILE
        # Primary Actor: SYSTEM
        # Precondition: CONTEXT FILE is valid
        # Postcondition: DCI CONTEXT PARSER will return CONTEX INFO
        # Trigger: SYSTEM asks the PARSER to parse the FILE
        # Main Success Scenario:
        # 1. SYSTEM asks the PARSER to parse the FILE
        #   2. PARSER parses the FILE
        #   3. PARSER return INFO
        */
    }
}