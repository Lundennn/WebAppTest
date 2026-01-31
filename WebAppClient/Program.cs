using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using Microsoft.VisualBasic;
using WebAppClient;

namespace WebAppClient
{
    partial class Program
    {
        static public void Main(string[] args)
        {
            //App.StartTest();
            App app = new App();
            app.Start();
        }
}
}

