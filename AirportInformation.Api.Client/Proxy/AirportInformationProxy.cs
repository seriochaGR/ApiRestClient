﻿using Polly;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AirportInformation.Api.Client.Proxy
{
    internal static class AirportInformationProxy
    {
        private const string baseUrl = @"https://raw.githubusercontent.com/jbrooksuk/JSON-Airports/master/airports.json";

        public static async Task<IRestResponse> RequestManager(Method requestMethod)
        {
            var client = new RestClient(baseUrl);
            var request = new RestRequest();
            request.Method = requestMethod;
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/json; charset=utf-8");
            request.RequestFormat = DataFormat.Json;
            request.Parameters.Clear();

            int maxRetryAttempts = 3;
            var policy = Policy
              .Handle<WebException>()
              .Retry(maxRetryAttempts);

            var JsonResponse = await policy.Execute(() =>
            {
                return Task.Run(() => client.Execute(request));
            });

            if (JsonResponse.ResponseStatus == ResponseStatus.Completed)
            {
                return JsonResponse;
            }
            else
                return null;

        }
    }
}
