
            messages.Add(new ChatMessage(ChatRole.System, systemPrompt));

            messages.AddRange(request.Messages);

            var response = await chatClient.GetResponseAsync(messages);


 var products = await db.Product.ToListAsync();
 var productJson = JsonSerializer.Serialize(products, ProductSerializerContext.Default.ListProduct);
 systemPrompt = @"You are a TinyShop assistant helping customers find outdoor products.
 Use emojis and HTML with Bootstrap classes in your responses. Please convert any Markdown to HTML. Include product
 images using full URLs like
 https://raw.githubusercontent.com/MicrosoftDocs/mslearn-dotnet-cloudnative/main/dotnet-docker/Products/wwwroot/images/product1.png
 and keep them small. Use media cards where possible.
 Products: " + productJson;