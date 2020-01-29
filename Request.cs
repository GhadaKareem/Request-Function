using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace HTTPServer
{
    public enum RequestMethod
    {
        GET,
        POST,
        HEAD
    }

    public enum HTTPVersion
    {
        HTTP10,
        HTTP11,
        HTTP09
    }

    class Request
    {
        string[] requestLines;
        RequestMethod method;
        public string relativeURI;
        Dictionary<string, string> headerLines;

        public Dictionary<string, string> HeaderLines
        {
            get { return headerLines; }
        }

        HTTPVersion httpVersion;
        string requestString;
        string[] contentLines;
        string[] request;

        public Request(string requestString)
        {
            this.requestString = requestString;
            headerLines = new Dictionary<string, string>();
        }
        /// <summary>
        /// Parses the request string and loads the request line, header lines and content, returns false if there is a parsing error
        /// </summary>
        /// <returns>True if parsing succeeds, false otherwise.</returns>
        public bool ParseRequest()
        {
            //throw new NotImplementedException();

            //TODO: parse the receivedRequest using the \r\n delimeter   

            this.requestLines = this.requestString.Split(new[] { "\r\n" }, StringSplitOptions.None);
            // check that there is atleast 3 lines: Request line, Host Header, Blank line (usually 4 lines with the last empty line for empty content)

            if (requestLines.Length < 3)
            {
                return false;
            }

            // Parse Request line
            if (!ParseRequestLine())
            {
                return false;
            }

            // Validate blank line exists
            if (!ValidateBlankLine())
            {
                return false;
            }

            // Load header lines into HeaderLines dictionary
            if (!LoadHeaderLines())
            {
                return false;
            }

            return true;
        }

        private bool ParseRequestLine()
        {
            // throw new NotImplementedException();

            string[] reqline = this.requestLines[0].Split(' '); //// check single space between parameters
            int len = reqline.Length;

            if (len < 3) //Check at least request line and host header and blank lines exist.
            {
                return false;
            }
            this.relativeURI = reqline[1].Substring(1);     /// get Relative url

            switch (reqline[2]) // check httpVersion
            {
                case "HTTP/0.9":
                    this.httpVersion = HTTPVersion.HTTP09;
                    break;

                case "HTTP/1.0":
                    this.httpVersion = HTTPVersion.HTTP10;
                    break;

                case "HTTP/1.1":
                    this.httpVersion = HTTPVersion.HTTP11;
                    break;
            }

            if (!ValidateIsURI(relativeURI)) // Validate Url
            {
                return false;
            }


            return true;
        }

        private bool ValidateIsURI(string uri)
        {
            return Uri.IsWellFormedUriString(uri, UriKind.RelativeOrAbsolute);
        }

        private bool LoadHeaderLines()
        {
            for (int i = 1; i < this.requestLines.Length -2; i++)
            {
                string[] lst = requestLines[i].Split(new[] { ": " }, StringSplitOptions.None);
                if (lst.Length < 2)
                {
                    return false;
                }

                this.HeaderLines.Add(lst[0], lst[1]);
            }
            return true;

            //throw new NotImplementedException();
        }

        private bool ValidateBlankLine()
        {

            if (this.requestLines[requestLines.Length - 2] == "")
            {
                return true;
            }

            return false;

            //  throw new NotImplementedException();
        }

    }
}
