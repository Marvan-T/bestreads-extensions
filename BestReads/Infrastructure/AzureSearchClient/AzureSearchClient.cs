using AutoMapper;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;

namespace BestReads.Infrastructure.AzureSearchClient;

public class AzureSearchClient : IAzureSearchClient
{
    private readonly IMapper _mapper;
    private readonly SearchClient _searchClient;

    public AzureSearchClient(IConfiguration configuration, IMapper mapper)
    {
        _mapper = mapper;
        var serviceEndpoint = configuration["AZURE_SEARCH_SERVICE_ENDPOINT"] ?? string.Empty;
        var indexName = configuration["AZURE_SEARCH_INDEX_NAME"] ?? string.Empty;
        var key = configuration["AZURE_SEARCH_ADMIN_KEY"] ?? string.Empty;

        var searchCredential = new AzureKeyCredential(key);
        var indexClient = new SearchIndexClient(new Uri(serviceEndpoint), searchCredential);

        _searchClient = indexClient.GetSearchClient(indexName);
    }

    public async Task<List<T>> SingleVectorSearch<T>(float[] queryEmbeddings, string vectorField,
        string filter, List<string> selectOptions)
    {
        var searchOptions = new SearchOptions
        {
            VectorQueries =
            {
                new RawVectorQuery { Vector = queryEmbeddings, KNearestNeighborsCount = 8, Fields = { vectorField } }
            },
            Filter = filter
        };

        if (selectOptions != null)
            foreach (var option in selectOptions)
                searchOptions.Select.Add(option);

        SearchResults<SearchDocument> response = await _searchClient.SearchAsync<SearchDocument>(null, searchOptions);

        var results = new List<T>();

        await foreach (var result in response.GetResultsAsync()) results.Add(_mapper.Map<T>(result.Document));

        return results;
    }
}