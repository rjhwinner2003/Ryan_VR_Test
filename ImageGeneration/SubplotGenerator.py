import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
import csv
import re
import os
import datetime
import sys

class SubgraphGenerator:

    def __init__(self, recentVariables, longTermVariables, subjectName, directoryPath):
        #self.recentVariables = recentVariables
        #self.longTermVariables = longTermVariables
        self.subjectName = subjectName
        self.path = os.path.abspath(os.path.join(os.getcwd(), os.pardir)) + '\\VREyeTesting - Education\\PatientResults\\' + subjectName + '\\'
        print(self.path);
        self.collect_data()

    subjectName = ''
    
    """
     A dictionary where the keys are variables to be displayed from the most recent tests.
     The pair to the key is a list with all of that variables values from each test.
     KEY MUST MATCH VARIABLE HEADER IN CSV
    """
    recentVariables =  {
            "Left Eye Precision (Test 2)"     : [],
            "Right Eye Precision (Test 2)"    : [],
            "Combined Eye Precision (Test 2)" : [],
            "Left Eye Precision (Test 3)"     : [],
            "Right Eye Precision (Test 3)"    : [],
            "Combined Eye Precision (Test 3)" : [],
            "Frequency"                       : [],
            "Latency"                         : [],
            "Speed"                           : []
       }
    
    """
     A dictionary where the keys are variables to calculate and display averages for each 
     test in record over time.
     The pair to the key is a list with all of that variables values from each test.
     KEY MUST MATCH VARIABLE HEADER IN CSV
    """
    longTermVariables = {
            "Frequency"                       : [],
            "Latency"                         : [],
            "Combined Eye Precision (Test 2)" : [],
            "Combined Eye Precision (Test 3)" : []
        }   
    
    variablePairs = {
        "Left Eye Precision (Test 2)" : "Right Eye Precision (Test 2)",
        "Left Eye Precision (Test 3)" : "Right Eye Precision (Test 3)"}
    
    """
    This variable is initialized in the collect_data function. It contains
    filenames and dates in the format {"Filename" : datetime obj}
    """
    files = {}
    
    """
    The path to the patients directory
    """
    path = ''

    """
     create_subgraph
     yVar: A list of values, in string format
     yName: The name of the variable recorded
     dates: A list of the dates for each test. Defaults to none. If no variable
     overrides the default, it is assumed this data is for one test only.
    """
    fileCounter = 0
    pairPlots = []
    def create_subgraph(self, yVar, yName, dates = None, yVar2 = None):
        imgPath = os.path.abspath(os.path.join(os.getcwd(), os.pardir)) + '\\VREyeTesting - Education\\ImageGeneration\\'
        plt.rcParams["figure.figsize"] = [7.50, 3.50]
        plt.rcParams["figure.autolayout"] = True
        

        y = yVar
        x = np.array(range(0, yVar.size))
        ax = plt.subplot()
        
        
        if(dates is not None):
            ax.set_xlabel("Date", fontsize=12)
        else:
            if(yVar2 is not None):
                ax.plot(x, y, label = "Left Eye")
            else:
                ax.plot(x, y)
                ax.set_ylabel("Average " + yName, fontsize=12)
                ax.set_title("Average " + yName + " Over Time", fontsize = 18)
            ax.set_xlabel("CentiSeconds", fontsize=12)
        
        if(yVar2 is not None):
            y = yVar2
            x = np.array(range(0, yVar.size))
            ax.plot(x,y, label = "Right Eye")
            ax.set_ylabel("Average Eye Precision", fontsize=12)
            ax.set_title("Average " + re.sub("Left", "Left/Right", yName) + "Over Time", fontsize = 18)
            plt.legend()
        
        if(dates is not None):
            dateStrings = []
            for i in dates:
                dateStrings.append(i[:4] + "-" + i[4:6] + "-" + i[6:])
            plt.bar(y,y)
            plt.title("Average " + yName + " Over Time")
            plt.savefig(imgPath + "longterm" + str(self.fileCounter) + ".png")
            self.fileCounter += 1
        else:
            plt.savefig(imgPath + "recent" + str(self.fileCounter) + ".png")
            self.fileCounter += 1
            
        plt.close()
   
    """
    parse_date
    dateString: a string representing a date in the format MM-DD-YYYY
    Uses Regex to interpret string and returns an int array.
    Int array is formatted for datetime python objects, where:
        index 0 = year
        index 1 = month
        index 2 = day
    (Unity is saving these strings in MM-DD-YYYY format, if this is changed
     this function will need to change as well)
    """
    def parse_date(self, dateString):
        searchDate = re.search('[0-9][0-9]-[0-9][0-9]-[0-9][0-9][0-9][0-9]', dateString, 1)
        if(searchDate != ''):
            dateArray = re.split('-', searchDate.group())
            if(len(dateArray) == 3):
                temp = dateArray [0]
                dateArray[0] = int(dateArray[2])
                dateArray[2] = int(dateArray[1])
                dateArray[1] = int(temp)
                return dateArray
          
    """
    collect_data
    This function loops through the files in the patients directory, storing
    date information for each.
    """
    def collect_data(self):
        for file in os.listdir(self.path):
            date = self.parse_date(file) #201
            dateObj = datetime.datetime(date[0], date[1], date[2])
            self.files[file] = dateObj
    
    """
    read_files
    iterates through all files and calls create_subgraph with relevant information
    for each variable contained in recentVariables and longTermVariables
    """
    def read_files(self):
        print("Reading Files")
        recent = ""
        datesAndLongTermVar = {}
        datesStrings = ""
        for file in self.files:
            #Find the most recent directory
            if(recent == ""):
                filesCopy = dict(self.files)
                dateStrings = [date_obj.strftime('%Y%m%d') for date_obj in list(filesCopy.values())]
                recent = max(dateStrings)
                
            filename = self.path + file + "/"
            for testFile in os.listdir(filename):
                
                with open(filename + testFile) as csvfile:
                    df = pd.read_csv(csvfile)
                    fileString = str(self.files[file].strftime('%Y%m%d'))
                    if(fileString == recent):
                        for key in self.recentVariables:
                            if(key in df.columns):
                                self.recentVariables[key].append(df[key])
                    for key in self.longTermVariables:
                        if(key in df.columns):
                            dateKey = key + fileString
                            datesAndLongTermVar[dateKey] = np.mean(df[key])
                        
                    """
                    reader = csv.DictReader(csvfile)
                    for row in reader:
                        for key in self.recentVariables:
                            self.recentVariables[key].append(row[key])
                            
                        for key in self.longTermVariables:
                            self.longTermVariables[key].append(row[key])
                    """
                
        for key in self.recentVariables:
            if(self.recentVariables[key] != []):
                if(not(key in self.variablePairs or key in self.variablePairs.values())):
                    yArray = np.array(self.recentVariables[key])
                    self.create_subgraph(yArray[0], key)
        
        for key in self.longTermVariables:
            averageOrdList = np.array([])
            dates = np.array([])
            for dateKey in sorted(datesAndLongTermVar.keys()):
                if(key in dateKey):
                    averageOrdList = np.append(averageOrdList, datesAndLongTermVar[dateKey])
                    dates = np.append(dates, dateKey)
            self.create_subgraph(averageOrdList, key, dateStrings)
            
        for key in self.variablePairs.keys():
            if(self.recentVariables[key] != []):
                yArray = np.array(self.recentVariables[key])
                yArray2 = np.array(self.recentVariables[self.variablePairs[key]])
                self.create_subgraph(yArray[0], key, None, yArray2[0])
                
#end of class

patientName = ""
filePath = ""


if __name__ == "__main__":
    patientName = sys.argv[1]
    filePath = sys.argv[2]
        
generator = SubgraphGenerator({}, {}, patientName, filePath)
generator.read_files()
print("Successfully Generated Subplots")

###
# Old test code
###
    
"""
name = input("Enter name exactly as recorded in Unity: ")
files = {
    }
path = os.getcwd()
path = os.path.abspath(os.path.join(path, os.pardir))
path = path + '/PatientResults/' + name + '/'
for file in os.listdir(path):
    date = re.search('[0-9][0-9]-[0-9][0-9]-[0-9][0-9]', file, 1)
    if(date != ''):
        dateArr = re.split('-', date.group())
        if(len(dateArr) == 3):
            dateObj = datetime.datetime(int(dateArr[2]), int(dateArr[0]), int(dateArr[1]))
            files[file] = dateObj
            print(dateObj.strftime("%x"))
            
print(files)
            

for file in files:
    filename = path + file + "/"
    for testFile in os.listdir(filename):
        
        with open(filename + testFile) as csvfile:
            reader = csv.DictReader(csvfile)
            for row in reader:
                for key in recentVariables:
                    recentVariables[key].append(row[key])
                    
                for key in longTermVariables:
                    longTermVariables[key].append(row[key])
    
        for key in recentVariables:
            y = np.array(recentVariables[key])
            create_subgraph(y, key)
            
        for key in longTermVariables:
            y = np.array(longTermVariables[key])
            create_subgraph(y, key)"""

