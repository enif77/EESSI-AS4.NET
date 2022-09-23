﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
//using System.Management.Automation;


namespace Eu.EDelivery.AS4.UnitTests.Utilities
{
    public static class Computer
    {
        public static void RunPowershellScript(string command)
        {
            //using (PowerShell instance = PowerShell.Create())
            //{
            //    instance.AddScript(command);
            //    instance.Invoke();
            //}

            using (var process = Process.Start(
                       new ProcessStartInfo
                       {
                           FileName = "powershell.exe",
                           UseShellExecute = false,
                           Arguments = command,
                           CreateNoWindow = true
                       }))
            {
                process?.WaitForExit();
            }
        }

        public static IEnumerable<string> GetFilesInDirectory(string directoryName, string pattern, bool recursive)
        {
            if (Directory.Exists(directoryName) == false)
            {                
                throw new DirectoryNotFoundException($"Directory {directoryName} does not exist. (Current directory = {Environment.CurrentDirectory} )");
            }

            List<string> matchingFiles = new List<string>();

            var subDirectories = Directory.GetDirectories(directoryName);

            foreach (var subDirectory in subDirectories)
            {
                matchingFiles.AddRange(GetFilesInDirectory(subDirectory, pattern, recursive));
            }

            matchingFiles.AddRange(Directory.GetFiles(directoryName, pattern));

            return matchingFiles.ToArray();
        }
    }
}
