# 实验室 7：将 LUIS 集成到机器人对话中

## 简介

机器人现在能够获取用户输入并根据用户输入做出响应。遗憾的是，机器人的沟通技能薄弱。有一个拼写错误或者换一种表达方式，机器人就无法理解了。这可能会令用户感到失望。使机器人能使用我们在[实验 6](../Lab6-Implement_LUIS/02-Implement_LUIS.md) 中生成的 LUIS 模型理解自然语言，可以大幅提高机器人的会话能力。

必须更新机器人才能使用 LUIS。  修改“Startup.cs”和“PictureBot.cs”就能实现这一点。

> 先决条件：本实验以[实验 3](../Lab3-Basic_Filter_Bot/02-Basic_Filter_Bot.md) 为基础。建议执行该实验，这样才能实现本实验中涵盖的记录。如果没有执行该实验，可以仔细阅读所有练习，并根据需要查看一些代码或者将代码用于自己的应用程序中。

> 注意：如果要使用 Finished 文件夹中的代码，则必须将应用特定的信息替换为自己的应用 ID 和终结点。

## 实验 7.1：添加自然语言理解

### 向 Startup.cs 添加 LUIS

1. 如果尚未打开，请在 Visual Studio 中打开 **“PictureBot”解决方案**

> **注**：如果不是从实验 1 开始，也可以从 **“{GitHubPath}/Lab7-Integrate_LUIS/code/Starter/PictureBot/PictureBot.sln”** 解决方案开始。
> 请务必替换所有应用设置值

1. 打开 **“Startup.cs”** 并找到`ConfigureServices`方法。在此，我们添加 LUIS 的方式是：在创建并注册状态访问器后额外添加一个 LUIS 服务。

如下所示：

```csharp
services.AddSingleton((Func<IServiceProvider, PictureBotAccessors>)(sp =>
{
    .
    .
    .
    return accessors;
});
```

添加：

```csharp
// 创建并注册 LUIS 识别器。
services.AddSingleton(sp =>
{
    var luisAppId = Configuration.GetSection("luisAppId")?.Value;
    var luisAppKey = Configuration.GetSection("luisAppKey")?.Value;
    var luisEndPoint = Configuration.GetSection("luisEndPoint")?.Value;

    // 获取 LUIS 信息
    var luisApp = new LuisApplication(luisAppId, luisAppKey, luisEndPoint);

    // 指定 LUIS 选项。这些选项可能因机器人而异。
    var luisPredictionOptions = new LuisPredictionOptions
    {
        IncludeAllIntents = true,
    };

    // 创建识别器
    var recognizer = new LuisRecognizer(luisApp, luisPredictionOptions, true, null);
    return recognizer;
});
```

1. 对 **“appsettings.json”** 进行修改，使其包括以下属性，请务必使用自己的 LUIS 实例值填写它们：

```json
"luisAppId": "",
"luisAppKey": "",
"luisEndPoint": ""
```

> **注**：.NET SDK 的 Luis 终结点 URL 应该类似于 **“https://{region}.api.cognitive.microsoft.com”**，之后没有 API 或版本。

## 实验 7.2：将 LUIS 添加到 PictureBot 的 MainDialog

1. 打开 **“PictureBot.cs”**。首先需要初始化 LUIS 识别器，具体做法与为 `PictureBotAccessors` 执行初始化的操作相似。在注释行 `// 初始化 LUIS 识别器` 下，添加以下内容：

```csharp
private LuisRecognizer _recognizer { get; } = null;
```

1. 导航到 **“PictureBot”** 构造函数：

```csharp
public PictureBot(PictureBotAccessors accessors, ILoggerFactory loggerFactory /*, LuisRecognizer recognizer*/)
```

现在，可能你已注意到我们在上一个实验中已将此行注释掉了，也可能你并未注意到这一点。现在将其注释掉，因为到目前为止，你未调用 LUIS，所以 LUIS 识别器不必成为对 PictureBot 的输入。现在，我们将使用识别器。

1. 取消注释输入要求（参数 `LuisRecognizer recognizer`），并在 `// 添加 LUIS 识别器实例` 下添加以下代码行：

```csharp
_recognizer = recognizer ?? throw new ArgumentNullException(nameof(recognizer));
```

同样，这和初始化 `_accessors` 实例的方式非常相似。

就更新 `MainDialog` 而言，我们不需要向最初的 `GreetingAsync` 步骤添加任何内容，因为无论用户输入是什么内容，我们都希望在对话开始时问候用户。

1. 在 `MainMenuAsync` 中，我们确实希望首先尝试使用 Regex，所以将保留大部分内容。但是，如果 Regex 没有找到意图，我们希望 `default` 操作有所不同。这时候，需要调用 LUIS。

在 `MainMenuAsync` 切换块中，替换：

```csharp
default:
    {
        await MainResponses.ReplyWithConfused(stepContext.Context);
        return await stepContext.EndDialogAsync();
    }
```

替换为：

```csharp
default:
{
    // 调用 LUIS 识别器
    var result = await _recognizer.RecognizeAsync(stepContext.Context, cancellationToken);
    // 获取结果中排在最前面的意图
    var topIntent = result?.GetTopScoringIntent();
    // 根据意图切换对话，概念与上面的 Regex 类似
    switch ((topIntent != null) ? topIntent.Value.intent : null)
    {
        case null:
            // 无结果时，添加应用逻辑。
            await MainResponses.ReplyWithConfused(stepContext.Context);
            break;
        case "None":
            await MainResponses.ReplyWithConfused(stepContext.Context);
            // 我们将为每个语句添加 LuisScore，这只是为了测试，以便我们知道是否调用了 LUIS
            await MainResponses.ReplyWithLuisScore(stepContext.Context, topIntent.Value.intent, topIntent.Value.score);
            break;
        case "Greeting":
            await MainResponses.ReplyWithGreeting(stepContext.Context);
            await MainResponses.ReplyWithHelp(stepContext.Context);
            await MainResponses.ReplyWithLuisScore(stepContext.Context, topIntent.Value.intent, topIntent.Value.score);
            break;
        case "OrderPic":
            await MainResponses.ReplyWithOrderConfirmation(stepContext.Context);
            await MainResponses.ReplyWithLuisScore(stepContext.Context, topIntent.Value.intent, topIntent.Value.score);
            break;
        case "SharePic":
            await MainResponses.ReplyWithShareConfirmation(stepContext.Context);
            await MainResponses.ReplyWithLuisScore(stepContext.Context, topIntent.Value.intent, topIntent.Value.score);
            break;
        case "SearchPic":
            await MainResponses.ReplyWithSearchConfirmation(stepContext.Context);
            await MainResponses.ReplyWithLuisScore(stepContext.Context, topIntent.Value.intent, topIntent.Value.score);
            break;
        default:
            await MainResponses.ReplyWithConfused(stepContext.Context);
            break;
    }
    return await stepContext.EndDialogAsync();
}
```

简述一下我们在添加新代码过程中所做的工作。首先，我们将调用 LUIS，而不是回答我们不明白。于是，我们使用 LUIS 识别器调用 LUIS，并将排在最前面的意图存储在变量中。然后我们使用 `switch` 以不同的方式响应，具体取决于选取的意图。这几乎与使用 Regex 执行的操作相同。

> **注意**：如果在 LUIS 中未按[实验室 6](../Lab6-Implement_LUIS/02-Implement_LUIS.md) 随附的代码中的说明为意图命名，需要对 `case` 语句做出相应修改。

另外需要注意的是，我们会在调用 LUIS 的每个回答之后添加 LUIS 意图值和分数。这样做只是为了向你展示调用 LUIS 而不是 Regex 的时间（你可以从最终产品中删除这些回答，但这些回答在测试机器人时是非常棒的指示）。

## 实验 7.3：测试自然语音短语

1. 按 **F5**以运行应用。

1. 切换到 Bot Emulator。尝试向机器人发送不同的图片搜索方式。当你说“给我发水的图片”或“给我看狗的图片”时会发生什么？尝试使用一些其他方式来索要、分享和订购图片。

如果有额外的时间，请看看是否有你希望 LUIS 选取，但它未选取的内容。现在可能非常适合前往 luis.ai，[查看终结点言语](https://docs.microsoft.com/zh-cn/azure/cognitive-services/LUIS/label-suggested-utterances)，并重新训练/重新发布模型。

> **有趣的是**：查看终结点言语功能极其强大。  LUIS 会明智地决定要显示的言语。  它会选择那些最能帮助它改进的言语，由“人机回圈”手动标记出来。  例如，如果 LUIS 模型预测一个给定的言语以 47% 的置信度映射到 Intent1，并预测它以 48% 的置信度映射到 Intent2，那么这是一个非常适合显示给人类以进行手动映射的候选项，因为模型在两个意图之间非常接近。

## 延伸阅读

如果在自定义 LUIS 实现方面存在问题，请查看[此处](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-builder-howto-v4-luis?view=azure-bot-service-4.0&tabs=cs)关于向机器人添加 LUIS 的文档指南。

>卡住或中断？可以在 [code/Finished](./code/Finished) 下找到适用于到目前为止的实验的解决方案。你需要在 `appsettings.json` 文件中插入 Azure 机器人服务的密钥。建议使用此代码作为参考，而不是作为运行的解决方案，但如果选择运行此代码，请务必添加必需的密钥（在本节中，不应添加密钥）。

## **友情提示**

如果希望在先前补充的 LUIS 模型与搜索 [培训](https://github.com/Azure/LearnAI-Bootcamp/tree/master/lab01.5-luis) 的基础上，尝试集成 LUIS 机器人（包括 Azure 认知搜索），请按以下培训资料操作：[Azure 认知搜索](https://github.com/Azure/LearnAI-Bootcamp/tree/master/lab02.1-azure_search)和 [Azure 认知搜索机器人](https://github.com/Azure/LearnAI-Bootcamp/blob/master/lab02.2-building_bots/2_Azure_Search.md)。

## 后续步骤

- [实验 08-01：检测语言](../Lab8-Detect_Language/01-Introduction.md)
