﻿using System.Web;

namespace BestReads.Infrastructure.ApiClients.NYTimes.Handlers;

public class NyTimesAuthenticationDelegatingHandler(
    IConfiguration configuration)
    : DelegatingHandler
{
    private readonly string _nyTimesApiKey = configuration["NYTimes:ApiKey"]!;

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var query = HttpUtility.ParseQueryString(request.RequestUri.Query);
        query["api-key"] = _nyTimesApiKey;
        request.RequestUri = new UriBuilder(request.RequestUri) { Query = query.ToString() }.Uri;

        //Todo: add logging
        return await base.SendAsync(request, cancellationToken);
    }
}