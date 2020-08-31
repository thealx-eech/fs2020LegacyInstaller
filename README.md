# fs2020LegacyInstaller
NSIS installer script for MSFS2020 legacy add-ons

This simple script can be used for making Microsoft Flight Simulator 2020 add-on installer that require files from the past FS games - like Flight Simulator X.

Requirements:

  NSIS 3.0 or newer
  
  Crypto plugin for NSIS

  ReplaceInFile plugin for NSIS

  StrRep plugin for NSIS
  
  

You can generate JSON files for your airplane in "planeconverter" (available on github).

Required config values you need to change in case of FSX:

  AIRPLANEID "ms-aircreation-582sl" ; aircraft directory name in FS2020

  FSXAIRPLANEID "Aircreation_582SL" ; aircraft directory name in original game

  VERSION "0.1" ; on your choice, can't be empty

  BLDDIR "J:\MSFS2020\MSFS MODS\MSFSLegacy\" ; installer parent directory

  SHDDIR "${BLDDIR}setup" ; files from this folder will be unpacked into FS2020 Community\aircraft_name\ directory


Feel free to use this installer for free or paid add-ons without limitations.

Maybe some design improvements will be made in future.