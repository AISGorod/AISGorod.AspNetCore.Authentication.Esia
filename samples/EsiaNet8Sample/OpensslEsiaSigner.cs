using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using AISGorod.AspNetCore.Authentication.Esia;

namespace EsiaNet8Sample;

/// <summary>
/// Простейшая обёртка подписи запросов над openssl.
/// </summary>
public class OpensslEsiaSigner : IEsiaSigner
{
    private const string KEY_FILE = "/home/vv_tokarev/esia.key";
    private const string CRT_FILE = "/home/vv_tokarev/esia.pem";

    public string Sign(byte[] data)
    {
        var a = new Process();
        a.StartInfo.FileName = "openssl";
        a.StartInfo.Arguments = $"cms -sign -binary -stream -engine gost -inkey {KEY_FILE} -signer {CRT_FILE} -nodetach -outform pem";

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            a.StartInfo.FileName = "wsl";
            a.StartInfo.Arguments = "openssl " + a.StartInfo.Arguments;
        }

        a.StartInfo.RedirectStandardInput = true;
        a.StartInfo.RedirectStandardOutput = true;
        a.StartInfo.UseShellExecute = false;

        a.Start();
        a.StandardInput.Write(Encoding.UTF8.GetString(data)); // просто передавать массив байтов не получается - ломает подпись
        a.StandardInput.Close();

        var resultData = new StringBuilder();
        var isKeyProcessing = false;
        while (!a.StandardOutput.EndOfStream)
        {
            var line = a.StandardOutput.ReadLine();
            switch (line)
            {
                case "-----BEGIN CMS-----":
                    isKeyProcessing = true;
                    break;
                case "-----END CMS-----":
                    isKeyProcessing = false;
                    break;
                default:
                {
                    if (isKeyProcessing)
                    {
                        resultData.Append(line);
                    }

                    break;
                }
            }
        }
        return resultData.ToString();
    }
}