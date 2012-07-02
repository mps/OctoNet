﻿using OctoNet.Models;

namespace OctoNet
{
    public static class Constants
    {
        public const string ApiV3Url = "https://api.github.com";
        public const string GitHubUrl = "https://github.com";

        public const string AuthorizeUrl = "https://github.com/login/oauth";
        public const string AuthenticationUrl = AuthorizeUrl + "/authorize";

        public const SortBy DefaultSortBy = SortBy.Created;
        public const OrderBy DefaultOrderBy = OrderBy.Descending;

        public const string JsonApplicationContent = "application/json";
        public const string JsonTextContent = "text/json";
        public const string XJsonTextContent = "text/x-json";
    }
}