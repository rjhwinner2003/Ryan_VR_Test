import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
import csv
import re
import os
import datetime
import sys

class RecentSubgraphGenerator:

    def __init__(self, subjectName, directoryPath):
        #self.recentVariables = recentVariables
        #self.longTermVariables = longTermVariables
        self.subjectName = subjectName
        self.path = os.path.abspath(os.path.join(os.getcwd(), os.pardir)) + '\\VREyeTesting - Education\\PatientResults\\' + subjectName + '\\'
        print(self.path);
        self.collect_data()
    
    """
    A dictionary where the keys are variables to be displayed from the most recent tests.
    The value is a list with of all that datapoint values for the key.
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
    
    fileCounter = 0
    
    """
    parse_date
    dateString: a string representing a date in the format MM-DD-YYYY
    Uses Regex to interpret string and returns an int array.
    Returns: Int array formatted for datetime python objects, where:
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
    """END OF parse_date"""
            
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
    """END OF collect_data"""
    
    """
    read_files
    iterates through all files and calls create_subgraph with relevant information
    for each variable contained in recentVariables and longTermVariables
    """
    def read_files(self):
        print("Reading Files")
        
        filename = ""
        
        recentList = sorted(self.files, key = self.files.get)
        filename = self.path + recentList[len(recentList)-1] + "/"
        
        #LOAD DATA INTO recentVariables
        print(filename)
        for testFile in os.listdir(filename):
            with open(filename + testFile) as csvfile:
                df = pd.read_csv(csvfile)
                for key in self.recentVariables:
                    if(key in df.columns):
                        self.recentVariables[key].append(df[key])
    """END OF read_files"""
    
    def create_graph(self, name, listOne, listTwo = None):
        imgPath = os.path.abspath(os.path.join(os.getcwd(), os.pardir)) + '\\VREyeTesting - Education\\ImageGeneration\\'
        plt.rcParams["figure.figsize"] = [7.50, 3.50]
        plt.rcParams["figure.autolayout"] = True
        twoYAxes = False
        
        yAxis = np.array(listOne)
        if(listTwo is not None):
            yAxisTwo = np.array(listTwo)
            twoYAxes = True
        xAxis = np.array(range(0, yAxis.size))
        
        ax = plt.subplot()
        
        if(twoYAxes):
            ax.plot(xAxis, yAxis, label = "Left Eye")
            ax.plot(xAxis, yAxisTwo, label = "Right Eye")
            ax.set_ylabel("Average Eye Precision", fontsize=12)
            ax.set_title("Average " + re.sub("Left", "Left/Right", name) + "Over Time", fontsize = 18)
            plt.legend()
        else:
            ax.plot(xAxis, yAxis)
            ax.set_ylabel("Average " + name, fontsize=12)
            ax.set_title("Average " + name + " Over Time", fontsize = 18)
        
        ax.set_xlabel("CentiSeconds", fontsize=12)
        
        plt.savefig(imgPath + "recent" + str(self.fileCounter) + ".png")
        self.fileCounter += 1
        plt.close()
    """END OF create_graph"""
        
    
    def create_subgraphs(self):
        for key in self.recentVariables:
            if(self.recentVariables[key] != []):
                if(not(key in self.variablePairs or key in self.variablePairs.values())):
                    thisList = self.recentVariables[key]
                    self.create_graph(key, thisList[0])
                    
        for key in self.variablePairs.keys():
            if(self.recentVariables[key] != []):
                leftList = self.recentVariables[key]
                rightList = self.recentVariables[self.variablePairs[key]]
                name = ""
                if('2' in key):
                    name = "Test 2 Precision "
                else:
                    name = "Test 3 Precision "
                self.create_graph(name, leftList[0], rightList[0])
    """END OF create_subgraphs"""
        
                
#END OF CLASS
        
#DRIVER CODE
patientName = ""
filePath = ""


if __name__ == "__main__":
    patientName = sys.argv[1]
    filePath = sys.argv[2]
        
generator = RecentSubgraphGenerator(patientName, filePath)
generator.read_files()
generator.create_subgraphs()
print("Successfully Generated Subplots")