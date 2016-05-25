using System;
using System.Net;
using System.Threading;
using System.Linq;
using System.Text;

namespace _4_http_git
{
    class Program
    {
        static void Main(string[] args)
        {
            WebServer ws = new WebServer(SendResponse, "http://localhost:8080/test/");
            ws.Run();
            Console.WriteLine("A simple webserver. Press any key to quit.");
            Console.ReadKey();
            ws.Stop();
        }

        public static string SendResponse(HttpListenerRequest request)
        {
            string Response;
            switch (request.Headers[0])
            {
                case "GET":
                    Response = "G";
                    break;
                case "POST":
                    Response = "P";
                    break;
                case "HEAD":
                    Response = "H";
                    break;
                default:

                    Response = "<!DOCTYPE html>" +
                        "<style>input{background-color:blue;color:white;border-radius:5px;width:80px;height:40px;cursor:pointer;font-size:20px;font-family:serif;}</style>" +
                            "</head><body>	" +
                            "<input type=\"button\" id='get' value=\"GET\">	<input type=\"button\" id='post' value=\"POST\"><input type=\"button\" id='head' value=\"HEAD\">" +
                            "<div id='response'>	RESPONSE</div>" +
                            "<script>	get.onclick=function(){	var xhr = new XMLHttpRequest();\r\n	xhr.open('GET','/test',false);\r\n" +
                            "xhr.onreadystatechange=function(){\r\n" +
                            "if(xhr.readyState == 4 && xhr.status==200)" +
                            "response.innerHTML = \"GET\";\r\n" +
                            "};	xhr.setRequestHeader(\"Test\",\"GET\");\r\n" +
                            "xhr.send();};\r\n	post.onclick=function(){	var xhr = new XMLHttpRequest();\r\n" +
                            "xhr.open('POST','/test',false);\r\n" +
                            "xhr.onreadystatechange=function(){" +
                            "if(xhr.readyState == 4 && xhr.status==200)		" +
                            "response.innerHTML = \"POST\";			};\r\n" +
                            "xhr.setRequestHeader(\"Test\",\"POST\");\r\n" +
                            "xhr.send();};head.onclick=function(){" +
                            "var xhr = new XMLHttpRequest();\r\n" +
                            "xhr.open('HEAD','/test',false);\r\n" +
                            "xhr.onreadystatechange=function(){" +
                            "if(xhr.readyState == 4 && xhr.status==200)" +
                            "response.innerHTML = \"HEAD\";			};	xhr.setRequestHeader(\"Test\",\"HEAD\");	xhr.send();}</script></body>\r\n";
                    break;
            }
            return Response;
            //return "<!DOCTYPE html>"+
            //    "<style>input{background-color:blue;color:white;border-radius:5px;width:80px;height:40px;cursor:pointer;font-size:20px;font-family:serif;}</style>" +
            //        "</head><body>	" +
            //        "<input type=\"button\" id='get' value=\"GET\">	<input type=\"button\" id='post' value=\"POST\"><input type=\"button\" id='head' value=\"HEAD\">" +
            //        "<div id='response'>	RESPONSE</div>" +
            //        "<script>get.onclick=function(){" +
            //        "var xhr = new XMLHttpRequest();" +
            //        "xhr.open('GET','/test',false);" +
            //        "xhr.onreadystage=function(){" +
            //        "if(xhr.readyState = 4 && xhr.status==200)response.innerHtml = \"GET\";};" +
            //        "xhr.send();}</script></body>";
        }
    }
}
