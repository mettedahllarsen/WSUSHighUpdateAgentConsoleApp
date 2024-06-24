using Newtonsoft.Json;
using System.Diagnostics;
using WSUSHighUpdateAgentConsoleApp.Models;
using File = System.IO.File;

class Program
{
	static async Task Main(string[] args)
	{
		string folderPath = @"C:\WSUSUpdates";

		MonitorForNewTarGZFiles(folderPath);
		CopyAndDeleteTarFile(folderPath);
		ExtractTarFileAndReadMetadata();
	}

	private static void MonitorForNewTarGZFiles(string folderPath)
	{
		string[] filesInFolder = Directory.GetFiles(folderPath);

		bool hasTarFile = CheckForTarFile(filesInFolder);

		if (hasTarFile)
		{
			Console.WriteLine("Der findes en.tar-fil i mappen.");
		}
		else
		{
			Console.WriteLine("Der findes ingen.tar-fil i mappen.");
		}
	}

	private static bool CheckForTarFile(string[] filesInFolder)
	{
		return filesInFolder.Any(file => Path.GetExtension(file).Equals(".tar", StringComparison.OrdinalIgnoreCase));
	}

	private static void CopyAndDeleteTarFile(string folderPath)
	{
		string[] tarFiles = Directory.GetFiles(folderPath, "*.tar");
		if (tarFiles.Length > 0)
		{
			string tarFile = tarFiles[0];
			string destinationFolderPath = @"C:\AvailableUpdates";
			string destinationFile = Path.Combine(destinationFolderPath, Path.GetFileName(tarFile));

			// Kopierer filen til den nye mappe
			File.Copy(tarFile, destinationFile, false); 

			Console.WriteLine($"Filen '{Path.GetFileName(tarFile)}' er blevet kopieret til '{destinationFile}'.");

			// Sletter filen fra den originale mappe
			File.Delete(tarFile);
			Console.WriteLine($"Filen '{Path.GetFileName(tarFile)}' er blevet slettet fra den originale mappe.");
		}
	}

	private static void ExtractTarFileAndReadMetadata()
	{
		// Sti til mappen, der indeholder .tar-filen og destination mappe
		string tarDirectoryPath = @"C:\AvailableUpdates";
		string extractPath = Path.Combine(tarDirectoryPath, "extracted");

		// Find .tar-filen dynamisk
		string tarFilePath = Directory.GetFiles(tarDirectoryPath, "*.tar").FirstOrDefault();

		if (tarFilePath != null)
		{

			// Ekstraher .tar-filen ved hjælp af en ekstern proces
			Process tarProcess = new Process();
			tarProcess.StartInfo.FileName = "tar";
			tarProcess.StartInfo.Arguments = $"-xf \"{tarFilePath}\" -C \"{extractPath}\"";
			tarProcess.StartInfo.UseShellExecute = false;
			tarProcess.StartInfo.RedirectStandardOutput = true;
			tarProcess.StartInfo.RedirectStandardError = true;
			tarProcess.Start();
			tarProcess.WaitForExit();

			// Tjek om der var nogen fejl under ekstraktionen
			string stderr = tarProcess.StandardError.ReadToEnd();
			if (!string.IsNullOrEmpty(stderr))
			{
				Console.WriteLine($"Fejl under ekstraktion: {stderr}");
				return;
			}

			// Læs JSON-filen for at hente metadata
			// Find JSON-filen dynamisk
			string jsonFilePath = Directory.GetFiles(extractPath, "*.json").FirstOrDefault();

			if (jsonFilePath != null)
			{
				// Læs JSON-filen for at hente metadata
				string jsonContent = File.ReadAllText(jsonFilePath);
				UpdateInfo updateInfo = JsonConvert.DeserializeObject<UpdateInfo>(jsonContent);

				// Gem metadata i en database eller skriv til konsollen
				Console.WriteLine($"Titel: {updateInfo.Title}, ID: {updateInfo.Id}");
			}
			else
			{
				Console.WriteLine("JSON-filen findes ikke.");
			}
		}
		else
		{
			Console.WriteLine("Ingen .tar-fil blev fundet i mappen.");
		}
	}
}
