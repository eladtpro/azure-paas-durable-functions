namespace ImageIngest.Functions.Extensions;

public static class BlobClientExtensions
{
    public static async IAsyncEnumerable<BlobTags> QueryAsync(this BlobContainerClient client, string query)
    {
        //List<BlobTags> tags = new List<BlobTags>();
        await foreach (var page in client.FindBlobsByTagsAsync(query).AsPages())
        {
            foreach (var blob in page.Values)
            {
                yield return new BlobTags(blob);
            }
        }
    }

    public static async Task DeleteByQueryAsync(this BlobContainerClient client, string query)
    {
        await foreach (TaggedBlobItem taggedBlobItem in client.FindBlobsByTagsAsync(query))
            await client.DeleteBlobIfExistsAsync(taggedBlobItem.BlobName);
    }

    public static async IAsyncEnumerable<BlobTags> ReadTagsAsync(this BlobClient blobClient)
    {
        if (blobClient.Exists())
        {
            Response<GetBlobTagResult> tags = await blobClient.GetTagsAsync();
            var props = await blobClient.GetPropertiesAsync();
            yield return new BlobTags(tags.Value.Tags);
        }
        yield return new BlobTags();
    }

    public static async Task<Response> WriteTagsAsync(this BlobClient blobClient, BlobTags tags)
    {
        return await blobClient.SetTagsAsync(tags.Tags);
    }
}