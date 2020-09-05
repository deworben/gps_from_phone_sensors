/*
 * Basic data logging for MCEN90032 Sensor Systems.
 * 
 * Author: Ricardo Garcia-Rosas
 */
using System.IO;
using UnityEngine;

// public class DataLogger : MonoBehaviour
public class DataLogger
{
    // Variables we can change externally or in the editor
    [Header("Data Logger configuration")]
    [Tooltip("The filename and extension for the log file. Please note that the log uses strings to log data, so file extensions should be compatible with text data.")]
    public string fileName = "data_1.csv";
    [Tooltip("The format of the data that is being logged. This is used as the header for the log file.")]
    public string dataFormat = "";

    // Private variables
    private StreamWriter fileWriter; // The object that manages writing to a file.
    private string filePath; // Variable to store the file path.

    // The Start routine runs once at the start of the application.
    // It's equivalent to the setup method in Arduino.
    // public void Start()
    // {
    //     InitialiseLogger();
    // }

    // The OnApplicationQuit routine is called whene the application is closed.
    // private void OnApplicationQuit()
    // {
    //     CloseLog();
    // }

    /// <summary>
    /// Initialises the DataLogger GameObject
    /// </summary>
    public void InitialiseLogger()
    {
        // To use data logging we first need to create all the objects that handle files.
        // This is done using the System.IO packages.
        // The data will be stored in the Application's persistent data path. This data path is
        // given by the target device. For more invormation visit: https://docs.unity3d.com/ScriptReference/Application-persistentDataPath.html

        // First check that the file name is not empty
        if (fileName == string.Empty)
            throw new System.Exception("The provided file name is empty");

        // Create the file to hold the data stream for logging
        filePath = Path.Combine(Application.persistentDataPath, fileName);
        FileStream sb = new FileStream(filePath, FileMode.Append); // The selected mode appends data to the end of the file if it already exists.
        // Initialize file StreamWriter with the given file.
        fileWriter = new StreamWriter(sb);
        // Add log format as header
        fileWriter.WriteLine(dataFormat);
        SaveLog();
    }

    /// <summary>
    /// Appends new data to the current Log file.
    /// The log uses strings to log data, so make sure the data provided to this
    /// method is already in the correct format.
    /// </summary>
    /// <param name="data">The string data to be appended.</param>
    public void AppendData(string data)
    {
        try
        {
            fileWriter.WriteLine(data);
        }
        catch (System.Exception e)
        {
            Debug.Log("Couldn't append data to the Log! :'(. Error:" + e.ToString());
        }
    }

    /// <summary>
    /// Creates a new file for logging data.
    /// Useful when you want to log different sessions in different files.
    /// </summary>
    /// <param name="fileName">The new file's name and extension</param>
    /// <param name="dataFormat">Optional data format string. Will replace the data format string if provided.</param>
    public void CreateNewLogFile(string fileName, string dataFormat = "")
    {
        // First check that the file name is not empty
        if (fileName == string.Empty)
            throw new System.Exception("The provided file name is empty");

        // Update variables
        this.fileName = fileName;
        if (dataFormat != string.Empty)
            this.dataFormat = dataFormat;

        // Initialise the logger with the new configuration
        InitialiseLogger();
    }

    /// <summary>
    /// Saves and closes the Log. Must be called on OnApplicationQuit.
    /// </summary>
    public void CloseLog()
    {
        if (fileWriter != null)
        {
            try
            {
                fileWriter.Close();
            }
            catch (System.Exception e)
            {
                Debug.Log("Something went wrong when saving the Log! D:. Error:" + e.ToString());
            }
        }
    }

    /// <summary>
    /// Saves the current log file.
    /// </summary>
    public void SaveLog()
    {
        if (fileWriter != null)
        {
            try
            {
                fileWriter.Flush();
            }
            catch (System.Exception e)
            {
                Debug.Log("Something went wrong when saving the Log! D:. Error:" + e.ToString());
            }
        }
    }
}
