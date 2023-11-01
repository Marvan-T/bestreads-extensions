namespace BestReads.Infrastructure.AzureSearchClient;

public interface IAzureSearchClient
{
    Task<List<T>> SingleVectorSearch<T>(float[] queryEmbeddings, string vectorField, string filter,
        List<string> selectOptions);
}