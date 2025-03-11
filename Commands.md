            //var products = await db.Product.ToListAsync();
            //var productJson = JsonSerializer.Serialize(products, ProductSerializerContext.Default.ListProduct);

            var systemPrompt = @"You are a TinyShop assistant helping customers find outdoor products.
            Use emojis and HTML with Bootstrap classes in your responses. Please convert any Markdown to HTML. Include product
            images using full URLs like
            https://raw.githubusercontent.com/MicrosoftDocs/mslearn-dotnet-cloudnative/main/dotnet-docker/Products/wwwroot/images/product1.png
            and keep them small. Use media cards where possible."
            //systemPrompt += " Products: " + productJson;

            var messages = new List<ChatMessage> { new ChatMessage(ChatRole.System, systemPrompt) };
            messages.AddRange(request.Messages);

            var response = await chatClient.GetResponseAsync(messages);

            return response?.Message != null
                ? Results.Ok(new ChatResponse { Message = response.Message.Text })
                : Results.BadRequest("Failed to generate response");
