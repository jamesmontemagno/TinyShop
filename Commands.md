var credential = new ApiKeyCredential(builder.Configuration["GitHubModels:Token"]!);
var openAIOptions = new OpenAIClientOptions()
{
    Endpoint = new Uri("https://models.inference.ai.azure.com")
};

var ghModelsClient = new OpenAIClient(credential, openAIOptions);
var chatClient = ghModelsClient.AsChatClient("gpt-4o-mini");
builder.Services.AddSingleton(chatClient);

---

var ollama = builder.AddOllama("ollama")
    .WithGPUSupport()
    .WithDataVolume();

var chat = ollama.AddModel("chat", "phi3");

---

builder.AddOllamaApiClient("chat")
    .AddChatClient();

