using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System;
using System.Windows.Forms;
using Microsoft.VisualBasic;

public class NetworkUtils
{
    private static ProtoRandom.ProtoRandom _random = new ProtoRandom.ProtoRandom(10);

    private static Tuple<string, string> GetControlCode()
    {
        ServicePointManager.DefaultConnectionLimit = int.MaxValue;
        ServicePointManager.MaxServicePoints = int.MaxValue;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"https://codicefiscale.it/controllo/");

        request.Proxy = null;
        request.UseDefaultCredentials = false;
        request.AllowAutoRedirect = false;
        request.Timeout = 70000;

        FieldInfo field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);
        request.Method = "POST";
        string content = "";

        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            int generatedNumber = _random.GetRandomInt32(5, 95);
            content = "{\"key\":" + generatedNumber.ToString() + "}";
            streamWriter.Write(content);
            streamWriter.Close();
            streamWriter.Dispose();
        }

        var headers1 = new CustomWebHeaderCollection(new Dictionary<string, string>
        {
            ["Host"] = "codicefiscale.it",
            ["User-Agent"] = "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:136.0) Gecko/20100101 Firefox/136.0",
            ["Accept"] = "*/*",
            ["Accept-Language"] = "it-IT,it;q=0.8,en-US;q=0.5,en;q=0.3",
            ["Accept-Encoding"] = "gzip, deflate, br, zstd",
            ["Referer"] = "https://codicefiscale.it/inverso/",
            ["Content-Type"] = "application/json",
            ["X-Requested-With"] = "XMLHttpRequest",
            ["Content-Length"] = content.Length.ToString(),
            ["Origin"] = "https://codicefiscale.it",
            ["DNT"] = "1",
            ["Sec-GPC"] = "1",
            ["Connection"] = "keep-alive",
            ["Sec-Fetch-Dest"] = "empty",
            ["Sec-Fetch-Mode"] = "cors",
            ["Sec-Fetch-Site"] = "same-origin",
            ["Priority"] = "u=4",
            ["Cache-Control"] = "max-age=0",
            ["TE"] = "trailers",
        });

        field.SetValue(request, headers1);
        var response = request.GetResponse();
        string responseContent = Encoding.UTF8.GetString(DecompressGzip(ReadStream(response.GetResponseStream())));
        string cookie = response.Headers.Get("Set-Cookie").Split(';')[0];

        response.Close();
        response.Dispose();

        return new Tuple<string, string>(responseContent.Replace("\"", "").Replace("{", "").Replace("}", "").Replace("codice:", "").Replace(" ", ""), cookie);
    }

    public static Tuple<string, string> GetTaxCodeInfo(string taxCode)
    {
        Tuple<string, string> controlCode = GetControlCode();

        var request = (HttpWebRequest)WebRequest.Create($"https://codicefiscale.it/inverso/");

        request.Proxy = null;
        request.UseDefaultCredentials = false;
        request.AllowAutoRedirect = false;
        request.Timeout = 70000;

        FieldInfo field = typeof(HttpWebRequest).GetField("_HttpRequestHeaders", BindingFlags.Instance | BindingFlags.NonPublic);
        request.Method = "POST";
        string content = "";

        using (StreamWriter streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            content = $"codice={taxCode}&controllo={controlCode.Item1}";
            streamWriter.Write(content);
            streamWriter.Close();
            streamWriter.Dispose();
        }

        CustomWebHeaderCollection headers = new CustomWebHeaderCollection(new Dictionary<string, string>
        {
            ["Host"] = "codicefiscale.it",
            ["User-Agent"] = "User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:136.0) Gecko/20100101 Firefox/136.0",
            ["Accept-Language"] = "it-IT,it;q=0.8,en-US;q=0.5,en;q=0.3",
            ["Accept-Encoding"] = "gzip, deflate, br, zstd",
            ["Referer"] = "https://codicefiscale.it/inverso/",
            ["Content-Type"] = "application/x-www-form-urlencoded",
            ["Content-Length"] = content.Length.ToString(),
            ["DNT"] = "1",
            ["Sec-GPC"] = "1",
            ["Connection"] = "keep-alive",
            ["Cookie"] = controlCode.Item2,
            ["Upgrade-Insecure-Requests"] = "1",
            ["Sec-Fetch-Dest"] = "document",
            ["Sec-Fetch-Mode"] = "navigate",
            ["Sec-Fetch-Site"] = "same-origin",
            ["Sec-Fetch-User"] = "?1",
            ["Priority"] = "u=0, i",
            ["TE"] = "trailers",
        });

        field.SetValue(request, headers);
        WebResponse response = request.GetResponse();
        string responseContent = Encoding.UTF8.GetString(DecompressGzip(ReadStream(response.GetResponseStream())));
       
        string[] splitted = Strings.Split(responseContent, "x-ref=\"cognomi\">");
        string surname = Strings.Split(splitted[1], "</div>")[0].Trim();

        splitted = Strings.Split(responseContent, "x-ref=\"nomi\">");
        string name = Strings.Split(splitted[1], "</div>")[0].Trim();

        response.Close();
        response.Dispose();

        return new Tuple<string, string>(name, surname);
    }

    private static byte[] ReadStream(Stream input)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            input.CopyTo(ms);
            return ms.ToArray();
        }
    }

    private static byte[] DecompressGzip(byte[] gzipData)
    {
        using (MemoryStream inputStream = new MemoryStream(gzipData))
        using (GZipStream gzipStream = new GZipStream(inputStream, CompressionMode.Decompress))
        using (MemoryStream outputStream = new MemoryStream())
        {
            gzipStream.CopyTo(outputStream);
            return outputStream.ToArray();
        }
    }
}