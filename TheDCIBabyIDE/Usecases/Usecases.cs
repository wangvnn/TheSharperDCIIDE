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
        #   4. SYSTEM parses the CONTEXT FILE to find the CONTEXT INFO (Habit 2)
        #   5. SYSTEM display CONTEXT INFO in the DCI EDITOR
        */
    }

    class HABIT2_Parse_CONTEXT_FILE 
    {
        /* USE CASE 2: Parse CONTEXT FILE
        # Primary Actor: SYSTEM
        # Precondition: CONTEXT FILE is valid
        # Postcondition: DCI CONTEXT PARSER will return CONTEX INFO
        # Trigger: SYSTEM asks the PARSER to parse the FILE
        # Main Success Scenario:
        # 1. READER FACTORY reads IDE SETTINGS to create READER
        #   2. READER reads and returns INFO (Habit 3)
        */
    }

    class HABIT3_Read_INJECTIONLESS_CONTEXT_FILE 
    {
        /* USE CASE 2: Read INJECTIONLESS CONTEXT FILE
        # Primary Actor: SYSTEM
        # Precondition: CONTEXT FILE is valid
        # Postcondition: DCI CONTEXT READER will return CONTEX INFO
        # Trigger: SYSTEM asks the CONTEXT READER to read CONTEXT FILE
        # Main Success Scenario:
        # 1. REGION READER reads all the REGIONS
        # 2. CONTEXT READER reads CONTEXT INFO
        # 3. USECASE READER reads USECASE INFO
        # 4. ROLE READER reads ROLE INFO
        # 5. CONTEXT READER returns CONTEXT MODEL
        */
    }

    class HABIT4_Read_MARVIN_CONTEXT_FILE //TODO: implement marvin context reader
    {
        /* USE CASE 2: Read MARVIN CONTEXT FILE
        # Primary Actor: SYSTEM
        # Precondition: CONTEXT FILE is valid
        # Postcondition: DCI CONTEXT READER will return CONTEX INFO
        # Trigger: SYSTEM asks the CONTEXT READER to read CONTEXT FILE
        # Main Success Scenario:
        */
    }
}