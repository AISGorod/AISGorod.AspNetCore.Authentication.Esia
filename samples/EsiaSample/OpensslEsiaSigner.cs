using AISGorod.AspNetCore.Authentication.Esia;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace EsiaSample
{
    /// <summary>
    /// Простейшая обёртка подписи запросов над openssl.
    /// </summary>
    public class OpensslEsiaSigner : IEsiaSigner
    {
        private const string KEY_FILE = @"/home/vladdy/test/esia.key";
        private const string CRT_FILE = @"/home/vladdy/test/esia.pem";

        public string Sign(byte[] data)
        {
            Process a = new Process();
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

            StringBuilder resultData = new StringBuilder();
            bool isKeyProcessing = false;
            while (!a.StandardOutput.EndOfStream)
            {
                string line = a.StandardOutput.ReadLine();
                if (line == "-----BEGIN CMS-----")
                {
                    isKeyProcessing = true;
                }
                else if (line == "-----END CMS-----")
                {
                    isKeyProcessing = false;
                }
                else if (isKeyProcessing)
                {
                    resultData.Append(line);
                }
            }
            return resultData.ToString();
        }
    }
}
