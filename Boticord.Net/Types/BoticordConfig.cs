using System;
using System.Net.Http;

namespace Boticord.Net.Types
{
    public class BoticordConfig
    {
        private const string TokenStartsWith = "SDC ";
        private string _token = "";

        public HttpClient? HttpClient { get; set; } = null;
        
        public string Token
        {
            get => _token;
            set
            {
                value = value.StartsWith(TokenStartsWith) ? value : TokenStartsWith + value;
                
                if (value.Length <= TokenStartsWith.Length + 1)
                    throw new NullReferenceException("Token can not be empty");

                _token = value;
            }
        }
    }
}