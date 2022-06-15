using System;
using System.Net.Http;

using Boticord.Net.Enums;

namespace Boticord.Net
{
    public class BoticordConfig
    {
        private string _token = "";

        public HttpClient? HttpClient { get; set; } = null;

        public TokenType TokenType { get; set; } = TokenType.Bot;

        public string Token
        {
            get => _token;
            set
            {
                if (TokenType != TokenType.None && string.IsNullOrWhiteSpace(value))
                    throw new NullReferenceException("Token can not be null or empty");

                _token = $"{TokenType} {value}";
            }
        }
    }
}