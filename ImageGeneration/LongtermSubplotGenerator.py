import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
import csv
import re
import os
import datetime
import sys
import statistics

class LongtermSubplotGenerator:
    
    def __init__(self, subjectName, directoryPath):
        #self.recentVariables = recentVariables
        #self.longTermVariables = longTermVariables
        self.subjectName = subjectName
        self.path = os.path.abspath(os.path.join(os.getcwd(), os.pardir)) + '\\VREyeTesting - Education\\PatientResults\\' + subjectName + '\\'
        print(self.path)
        self.collect_data()
        
    """
     A dictionary where the keys are variables to calculate and display averages for each 
     test in record over time.
     The pair to the key is dictionary, mapping a string representing date to a list. This represents a list of data points for a single test at the date/time.
     KEY MUST MATCH VARIABLE HEADER IN CSV
    """
    longTermVariables = {
            "Frequency"                       : { "" : []},
            "Latency"                         : { "" : []},
            "Speed"                           : { "" : []},
            "Combined Eye Precision (Test 2)" : { "" : []},
            "Combined Eye Precision (Test 3)" : { "" : []},
        }   
    
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
            date = self.parse_date(file) 
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
        
        fileList = list(reversed(sorted(self.files, key = self.files.get)))
        
        #LOAD DATA INTO longtermVariables
        for file in fileList:
            filename = self.path + file + "/"
            for testFile in os.listdir(filename):
                with open(filename + testFile) as csvfile:
                    df = pd.read_csv(csvfile)
                    for key in self.longTermVariables:
                        if(key in df.columns):
                            dateString = self.files[file].strftime("%m/%d/%Y")
                            self.longTermVariables[key][dateString] = df[key]
    """END OF read_files"""
    
    def create_graph(self, name, valueList):
        imgPath = os.path.abspath(os.path.join(os.getcwd(), os.pardir)) + '\\VREyeTesting - Education\\ImageGeneration\\'
        plt.rcParams["figure.figsize"] = [7.50, 3.50]
        plt.rcParams["figure.autolayout"] = True
        
        dates = list(valueList.keys())
        averages = list(valueList.values())
        
        plt.bar(dates, averages)
        
        plt.xlabel("Test Dates", fontsize=12)
        plt.ylabel("Average " + name + " per Test")
        plt.title(name + " results per test")
        
        plt.savefig(imgPath + "longterm" + str(self.fileCounter) + ".png")
        self.fileCounter += 1
        plt.close()
    """END OF create_graph"""
        
    
    def create_subgraphs(self):
        for key, value in self.longTermVariables.items():
            valueList = {}
            for k, v in value.items():
                average = 0
                if(len(v) > 0):
                    average = sum(v) / len(v)
                    valueList[k] = average
            self.create_graph(key, valueList)
                
                
    """END OF create_subgraphs"""
    
    def get_average(self, listAvg):
        listSum = 0
        for i in listAvg:
            listSum += i
        if(len(listAvg) > 0):
            return listSum / len(listAvg)
        else:
            return 0
                    
    #END OF CLASS
            
#DRIVER CODE
patientName = ""
filePath = ""


if __name__ == "__main__":
    patientName = sys.argv[1]
    filePath = sys.argv[2]
        
generator = LongtermSubplotGenerator(patientName, filePath)
generator.read_files()
generator.create_subgraphs()
print("Successfully Generated Subplots")
        