Running C programs with ARMSim#
===============================

Instructions for Windows Systems

0.  Prerequisite: some components of the Mentor Graphics Sourcery CodeBench
    are needed if access to the standard C library is required. Unfortunately
    Mentor Graphics has stopped distributing the free Lite Edition of the
    CodeBench for the ARM EABI (the platform we need). Only a professional
    version is currently available. The professional edition may be obtained
    from this URL:
       http://www.mentor.com/embedded-software/sourcery-tools/sourcery-codebench/editions/lite-edition/
    Look for the download named "Sourcery CodeBench for ARM Embedded".

    The particular library archive files we need for running C programs are,
    however, included with this ARMSim# distribution. (Redistributing the files
    does not infringe any laws because these files were extracted from a
    distribution to which the GNU General Public License applies.)

1.  Compile the C source files to generate ARM binary files.
    The gcc compiler distributed by Mentor Graphics (formerly Code Sourcery)
    must be used.

    Example: if there is one file named factorial.c then the command is
       arm-none-eabi-gcc -c factorial.c

2.  Start ARMSim#

    Check File/Preferences to make sure that the Angel SWI instruction set
    is enabled as a plugin. (It should be enabled by default.)

    Open File/Load Multiple and select the files and libraries listed below.

    Begin with all .o files produced in step 1 above

    Then add these four:

       start.o
       libc.a
       libcs3hosted.a
       libgcc.a

    If you installed the Sourcery CodeBench, the last three of these files
    may be obtained from these locations:
       PATH_TO_CODEBENCH\arm-none-eabi\lib\libc.a
       PATH_TO_CODEBENCH\arm-none-eabi\lib\libcs3hosted.a
       PATH_TO_CODEBENCH\lib\gcc\arm-none-eabi\4.8.3\libgcc.a
    where PATH_TO_CODEBENCH is the location of the folder where the Mentor
    Graphics Sourcery Code bench was installed on your computer.
    The full path would likely be similar to this if the package was installed
    in a user's Documents folder:
       C:\Users\USERNAME\Documents\MentorGraphics\Sourcery_CodeBench_Lite_for_ARM_EABI

    The start.o file is provided separately, as is its source code start.s.

3.  If there are no linking errors reported in the Console window, then it's
    ready to run.


---------------------


Running C programs with Code Sourcery's ARM simulator
=====================================================

1.  Sample compile ccommand is

      arm-none-eabi-gcc -c factorial.c -o factorial -T generic-hosted.ld

2.  Run command is

      arm-none-eabi-run factorial