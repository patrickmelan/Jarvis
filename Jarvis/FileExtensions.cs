using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jarvis;

public static class FileExtensions
{
    public static async Task<byte[]> ReadAllBytesAsync(string filePath)
    {
        try
        {
            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Read all bytes from the file asynchronously
                return await File.ReadAllBytesAsync(filePath);
            }
            else
            {
                // Handle the case where the file does not exist
                Console.WriteLine("File not found: " + filePath);
                return null; // or throw an exception based on your requirements
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions, such as file access errors
            Console.WriteLine("Error reading file: " + ex.Message);
            return null; // or throw an exception based on your requirements
        }
    }
}

