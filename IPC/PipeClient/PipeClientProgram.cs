﻿using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Security.Principal;
using System.Diagnostics;
using System.Threading;

public class PipeClient
{
    private static int numClients = 4;

    public static void Main(string[] Args)
    {
        if (Args.Length > 0)
        {
            if (Args[0] == "spawnclient")
            {
                /**
                 * Robust Programming
                 * The client and server processes in this example are intended to run on the same computer, 
                 * so the server name provided to the NamedPipeClientStream object is ".". 
                 * If the client and server processes were on separate computers, 
                 * "." would be replaced with the network name of the computer that runs the server process.
                 */
                NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "testpipe", PipeDirection.InOut, PipeOptions.None, TokenImpersonationLevel.Impersonation);

                Console.WriteLine("Connecting to server...\n");
                pipeClient.Connect();

                StreamString ss = new StreamString(pipeClient);
                // Validate the server's signature string
                if (ss.ReadString() == "I am the one true server!")
                {
                    // The client security token is sent with the first write.
                    // Send the name of the file whose contents are returned
                    // by the server.
                    ss.WriteString("c:\\textfile.txt");

                    // Print the file to the screen.
                    Console.Write(ss.ReadString());
                }
                else
                {
                    Console.WriteLine("Server could not be verified.");
                }
                pipeClient.Close();
                // Give the client process some time to display results before exiting.
                Thread.Sleep(4000);
            }
        }
        else
        {
            Console.WriteLine("\n*** Named pipe client stream with impersonation example ***\n");
            StartClients();
        }
    }

    // Helper function to create pipe client processes
    private static void StartClients()
    {
        int i;
        string currentProcessName = Environment.CommandLine;
        Process[] plist = new Process[numClients];

        Console.WriteLine("Spawning client processes...\n");

        if (currentProcessName.Contains(Environment.CurrentDirectory))
        {
            currentProcessName = currentProcessName.Replace(Environment.CurrentDirectory, String.Empty);
        }

        // Remove extra characters when launched from Visual Studio
        currentProcessName = currentProcessName.Replace("\\", String.Empty);
        currentProcessName = currentProcessName.Replace("\"", String.Empty);

        for (i = 0; i < numClients; i++)
        {
            // Start 'this' program but spawn a named pipe client.
            plist[i] = Process.Start(currentProcessName, "spawnclient");
        }
        while (i > 0)
        {
            for (int j = 0; j < numClients; j++)
            {
                if (plist[j] != null)
                {
                    if (plist[j].HasExited)
                    {
                        Console.WriteLine("Client process[{0}] has exited.",
                            plist[j].Id);
                        plist[j] = null;
                        i--;    // decrement the process watch count
                    }
                    else
                    {
                        Thread.Sleep(250);
                    }
                }
            }
        }
        Console.WriteLine("\nClient processes finished, exiting.");
    }
}

// Defines the data protocol for reading and writing strings on our stream
public class StreamString
{
    private Stream ioStream;
    private UnicodeEncoding streamEncoding;

    public StreamString(Stream ioStream)
    {
        this.ioStream = ioStream;
        streamEncoding = new UnicodeEncoding();
    }

    public string ReadString()
    {
        int len;
        len = ioStream.ReadByte() * 256;
        len += ioStream.ReadByte();
        byte[] inBuffer = new byte[len];
        ioStream.Read(inBuffer, 0, len);

        return streamEncoding.GetString(inBuffer);
    }

    public int WriteString(string outString)
    {
        byte[] outBuffer = streamEncoding.GetBytes(outString);
        int len = outBuffer.Length;
        if (len > UInt16.MaxValue)
        {
            len = (int)UInt16.MaxValue;
        }
        ioStream.WriteByte((byte)(len / 256));
        ioStream.WriteByte((byte)(len & 255));
        ioStream.Write(outBuffer, 0, len);
        ioStream.Flush();

        return outBuffer.Length + 2;
    }
}