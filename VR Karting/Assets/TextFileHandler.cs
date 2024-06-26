using System.IO;
using UnityEngine;

public class TextFileHandler
{
    private StreamWriter _streamWriter;
    private string _filePath;
    private readonly string _directoryPath;

    public TextFileHandler(string directoryPath, string fileName)
    {
        _directoryPath = directoryPath;
        _filePath = Path.Combine(directoryPath, fileName);
        OpenFile();
    }

    private void OpenFile()
    {
        if (!Directory.Exists(_directoryPath)) Directory.CreateDirectory(_directoryPath);

        if (File.Exists(_filePath))
        {
            var version = 1;
            string newFilePath;
            do
            {
                newFilePath = Path.Combine(_directoryPath,
                    $"{Path.GetFileNameWithoutExtension(_filePath)}_{version}{Path.GetExtension(_filePath)}");
                version++;
            } while (File.Exists(newFilePath));

            _filePath = newFilePath;
        }

        _streamWriter = new StreamWriter(_filePath, true);
        _streamWriter.AutoFlush = true; // Ensure that every line is saved immediately
    }

    public void WriteLine(string line)
    {
        _streamWriter.WriteLine(line);
    }

    public void CloseFile()
    {
        _streamWriter.Close();

        Debug.Log($"Saved to {_filePath}");

        // open the file
        // System.Diagnostics.Process.Start(_filePath);
    }
}