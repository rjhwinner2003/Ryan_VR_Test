# VR-Novel-Balance-Test
Unity VR Novel Balance Test
# Quick Start Guide
Unity Version 2019.4.29f1
Designed for HTC Vive
After Cloning repo, make sure Unity Version is correct and add the project file.
# Data Location
Data is located in a CSV file labeled "ResearchData_subjectID" under the folder labeled "Novel Balance Test Software_Data" in builds.
# Test Builds
Builds are located under Builds/Windows/x86_64
# Functional Requirements
This software includes 6 VR test scenes. Each test is conducted 3 times with a duration of 20 seconds each. The goal of the subject in each test is to remain as still as possible for the duration of the tests. The first Scene is a fixed environment with the lights on. The Second Scene is a fixed environment with the lights off. The third scene is a non-fixed environment that moves with the subjects head position and rotation with the lights on. Scenes 4 - 6 are repeats of the first three scenes with the difference being the subject will stand on a foam mat instead of a stable floor.
# Non_Functional Requirements
In each Scene there is a calibrated Focus Object that the subject will focus on to keep the data as consistent as possible.
There are Two additional scenes, the first of which allows the Test administrator to enter the subjectId, calibrate the focus object height and begin the tests.
The second pauses testing until the foam mat has been placed under the subject at which point the test administrator can resume testing.
