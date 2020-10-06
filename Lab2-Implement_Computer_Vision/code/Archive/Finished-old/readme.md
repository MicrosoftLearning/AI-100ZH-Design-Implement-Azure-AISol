Note! If you want to use the finished solution, you must fill in the following with keys in `settings.json` under TestCLI

```json
{
  "CognitiveServicesKeys": {
    "Vision": "VisionKeyHere"
  },
  "AzureStorage": {
    "ConnectionString": "ConnectionStringHere",
    "BlobContainer": "images"
  },
  "CosmosDB": {
    "EndpointURI": "CosmosURIHere",
    "Key": "CosmosKeyHere",
    "DatabaseName": "images",
    "CollectionName": "metadata"
  }
}
```