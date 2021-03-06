﻿using System;
using System.Net;
using System.Runtime.CompilerServices;
using NetTelegraph.Result;
using NetTelegraph.Type;
using Newtonsoft.Json;
using RestSharp;
using Node = NetTelegraph.Interface.Node;

#if DEBUG
[assembly: InternalsVisibleTo("NetTelegraph.Test")]
#endif

namespace NetTelegraph
{
    /// <summary>
    /// 
    /// </summary>
    public class TelegraphBot
    {

        /// <summary>
        /// 
        /// </summary>
        public TelegraphBot()
        {
            mRestClient = new RestClient("https://api.telegra.ph");
        }

        internal RestClient mRestClient { private get; set; }

        private const string mCreateAccountUri = "/createAccount";
        private const string mCreatePagetUri = "/createPage";

        private static RestRequest NewRestRequest(string uri)
        {
            RestRequest request = new RestRequest(string.Format(uri), Method.POST);

            return request;
        }

        /// <summary>
        /// Use this method to create a new Telegraph account.
        /// Most users only need one account, but this can be useful for channel administrators
        /// who would like to keep individual author names and profile links for each of their channels.
        /// </summary>
        /// <param name="shortName">Required. Account name, helps users with several accounts remember which they are currently using. Displayed to the user above the "Edit/Publish" button on Telegra.ph, other users don't see this name.</param>
        /// <param name="authorName">Default author name used when creating new articles.</param>
        /// <param name="authorUrl">Default profile link, opened when users click on the author's name below the title. Can be any link, not necessarily to a Telegram profile or channel.</param>
        /// <returns>
        /// On success, returns an Account object with the regular fields and an additional access_token field.
        /// </returns>
        public AccountResult CreateAccount(string shortName, string authorName = null, string authorUrl = null)
        {
            //todo exp with seralize object
            RestRequest request = NewRestRequest(mCreateAccountUri);

            request.AddParameter("short_name", shortName);

            if (authorName != null)
                request.AddParameter("author_name", authorName);
            if (authorUrl != null)
                request.AddParameter("author_url", authorUrl);

            return ExecuteRequest<AccountResult>(request) as AccountResult;
        }

        /// <summary>
        /// Use this method to create a new Telegraph page. On success, returns a Page object.
        /// </summary>
        /// <param name="accessToken">Required. Access token of the Telegraph account.</param>
        /// <param name="title">Required. Page title.</param>
        /// <param name="content">Required. Content of the page. </param>
        /// <param name="authorName">Author name, displayed below the article's title.</param>
        /// <param name="authorUrl">Profile link, opened when users click on the author's name below the title. 
        /// Can be any link, not necessarily to a Telegram profile or channel.</param>
        /// <param name="returnContent">If true, a content field will be returned in the Page object (see: Content format).</param>
        /// <returns></returns>
        public PageResult CreatePage(string accessToken, string title, Node[] content, string authorName = null,
            string authorUrl = null, bool returnContent = false)
        {
            RestRequest request = NewRestRequest(mCreatePagetUri);

            request.AddParameter("access_token", accessToken);
            request.AddParameter("title", title);
            request.AddParameter("content", content);

            if(authorName != null)
                request.AddParameter("author_name", authorName);
            if (authorUrl != null)
                request.AddParameter("author_url", authorUrl);
            if (returnContent)
                request.AddParameter("return_content", true);

            return ExecuteRequest<PageResult>(request) as PageResult;
        }

        private object ExecuteRequest<T>(IRestRequest request) where T : class
        {
            IRestResponse response = mRestClient.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                if (typeof (T) == typeof (AccountResult))
                    return JsonConvert.DeserializeObject<AccountResult>(response.Content);
                if (typeof (T) == typeof (PageResult))
                    return JsonConvert.DeserializeObject<PageResult>(response.Content);
            }

            throw new Exception(response.StatusDescription);
        }
    }
}