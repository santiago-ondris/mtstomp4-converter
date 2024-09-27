using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static async Task Main(string[] args)
    {
        // Definir el directorio de entrada donde se encuentran los archivos .mts
        string inputDir = @"C:\Users\Administrador\Downloads\VIDEOSPRODUDELACAMARA";
        // Crear un subdirectorio llamado "Converted" para los archivos convertidos
        string outputDir = Path.Combine(inputDir, "Converted-2");

        // Crear el directorio de salida si no existe
        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        // Obtener todos los archivos .mts del directorio de entrada
        var mtsFiles = Directory.GetFiles(inputDir, "*.mts");
        // Iterar sobre cada archivo .mts encontrado
        foreach (var file in mtsFiles)
        {
            // Generar el nombre del archivo de salida con extensión .mp4
            string outputFile = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(file) + ".mp4");
            // Llamar al método de conversión para cada archivo
            await ConvertMtsToMp4(file, outputFile);
        }

        Console.WriteLine("Conversión completa.");
    }

    static async Task ConvertMtsToMp4(string inputFile, string outputFile)
    {
        // Ruta del ejecutable ffmpeg (asumiendo que está en el PATH del sistema)
        string ffmpegPath = "ffmpeg";

        // Configurar el proceso para ejecutar ffmpeg
        var startInfo = new ProcessStartInfo
        {
            FileName = ffmpegPath,
            // Argumentos para la conversión: entrada, códec de video, preajuste, calidad, códec de audio, tasa de bits de audio, salida
            Arguments = $"-i \"{inputFile}\" -c:v libx264 -preset medium -crf 23 -c:a aac -b:a 128k \"{outputFile}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        // Iniciar el proceso de conversión
        using (var process = new Process { StartInfo = startInfo })
        {
            process.Start();

            // Leer la salida estándar y de error de manera asíncrona
            var outputTask = process.StandardOutput.ReadToEndAsync();
            var errorTask = process.StandardError.ReadToEndAsync();

            // Esperar a que se complete la lectura de la salida y los errores
            await Task.WhenAll(outputTask, errorTask);

            string output = await outputTask;
            string error = await errorTask;

            // Esperar a que el proceso termine
            await process.WaitForExitAsync();

            // Verificar si la conversión fue exitosa
            if (process.ExitCode != 0)
            {
                Console.WriteLine($"Error al convertir {inputFile}: {error}");
            }
            else
            {
                Console.WriteLine($"Convertido: {outputFile}");
            }
        }
    }
}




