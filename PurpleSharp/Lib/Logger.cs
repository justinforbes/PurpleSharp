﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurpleSharp.Lib
{
    // Reference https://gist.github.com/heiswayi/69ef5413c0f28b3a58d964447c275058
    public class Logger
    {
    private const string FILE_EXT = ".log";
    private readonly string datetimeFormat;
    private readonly string logFilename;

    public Logger(string logpath)
    {
        datetimeFormat = "MM/dd/yyyy HH:mm:ss";
        if (logpath!= "") logFilename = logpath;
        else logFilename = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + FILE_EXT;


        //string logHeader = logFilename + " is created.";
        /*
        string logHeader = "[*]";
        if (!System.IO.File.Exists(logFilename))
        {
            WriteLine(System.DateTime.Now.ToString(datetimeFormat) + " " + logHeader, false);
        }
        */
    }


    public void Debug(string text)
    {
        WriteFormattedLog(LogLevel.DEBUG, text);
    }

    public void Error(string text)
    {
        WriteFormattedLog(LogLevel.ERROR, text);
    }

    public void Info(string text)
    {
        WriteFormattedLog(LogLevel.INFO, text);
    }


    public void Warning(string text)
    {
        WriteFormattedLog(LogLevel.WARNING, text);
    }

    public void TimestampInfo(string text)
    {
        WriteFormattedLog(LogLevel.TINFO, text);
    }

        private void WriteLine(string text, bool append = true)
    {
        try
        {
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(logFilename, append, System.Text.Encoding.UTF8))
            {
                if (!string.IsNullOrEmpty(text))
                {
                    writer.WriteLine(text);
                }
            }
        }
        catch
        {
            throw;
        }
    }

    private void WriteFormattedLog(LogLevel level, string text)
    {
        string pretext;
        switch (level)
        {

            case LogLevel.INFO:
                    //pretext = System.DateTime.Now.ToString(datetimeFormat) + " [*]    ";
                    pretext = " [*]    ";
                    break;
            case LogLevel.TINFO:
                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [*]    ";
                break;
            case LogLevel.DEBUG:
            pretext = System.DateTime.Now.ToString(datetimeFormat) + " [DEBUG]   ";
                break;
            case LogLevel.WARNING:
                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [!] ";
                break;
            case LogLevel.ERROR:
                pretext = System.DateTime.Now.ToString(datetimeFormat) + " [ERROR]   ";
                break;
            default:
                pretext = "";
                break;
        }
        Console.WriteLine(pretext + text);
        WriteLine(pretext + text);
    }

    [System.Flags]
    private enum LogLevel
    {
        INFO,
        TINFO,
        DEBUG,
        WARNING,
        ERROR,
    }
}
}
