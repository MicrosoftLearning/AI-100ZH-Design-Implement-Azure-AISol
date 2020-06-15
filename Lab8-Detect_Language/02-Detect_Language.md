# 实验室 8 - 检测语言

在本实验中，我们将把认知服务的语言检测功能集成到机器人中。

## 实验 8.1：检索你的认知服务 URL 和密钥

1. 打开 [Azure 门户](https://portal.azure.com)

1. 导航到你的资源组，选择通用的认知服务资源（即包含所有终结点）。

1. 在 **“资源管理”** 下，选择 **“快速入门”** 选项卡，并记下认知服务资源的 URL 和密钥

## 实验 8.2：向机器人添加语言支持

1. 如果尚未打开，请打开 **“PictureBot”** 解决方案

1. 右键单击项目，选择 **“管理 NuGet 包”**

1. 选择 **“浏览”**。

1. 搜索 **“Microsoft.Azure.CognitiveServices.Language.TextAnalytics”**，选择它并选择 **“安装”**，然后选择 **“我接受”**

1. 打开 **“Startup.cs”** 文件，添加以下 using 语句：

```csharp
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
using Microsoft.Azure.CognitiveServices.Language.LUIS.Runtime;
```

1. 将以下代码添加到 **“ConfigureServices”** 方法：

```csharp
services.AddSingleton(sp =>
{
    string cogsBaseUrl = Configuration.GetSection("cogsBaseUrl")?.Value;
    string cogsKey = Configuration.GetSection("cogsKey")?.Value;

    var credentials = new ApiKeyServiceClientCredentials(cogsKey);
    TextAnalyticsClient client = new TextAnalyticsClient(credentials)
    {
        Endpoint = cogsBaseUrl
    };

    return client;
});
```

1. 打开 **“PictureBot.cs”** 文件，添加以下 using 语句：

```csharp
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics;
using Microsoft.Azure.CognitiveServices.Language.TextAnalytics.Models;
```

1. 添加以下类变量：

```csharp
private TextAnalyticsClient _textAnalyticsClient;
```

1. 对构造函数进行修改，以包含新的 TextAnalyticsClient：

```csharp
public PictureBot(PictureBotAccessors accessors, ILoggerFactory loggerFactory,LuisRecognizer recognizer, TextAnalyticsClient analyticsClient)
```

1. 在构造函数中，初始化类变量：

```csharp
_textAnalyticsClient = analyticsClient;
```

1. 导航到 **“OnTurnAsync”** 方法并找到以下代码行：

```csharp
var utterance = turnContext.Activity.Text;
var state = await _accessors.PictureState.GetAsync(turnContext, () => new PictureState());
state.UtteranceList.Add(utterance);
await _accessors.ConversationState.SaveChangesAsync(turnContext);
```

1. 在其后添加以下代码行

```csharp
//Check the language
var result = _textAnalyticsClient.DetectLanguage(turnContext.Activity.Text, "us");

switch (result.DetectedLanguages[0].Name)
{
    case "English":
        break;
    default:
        //throw error
        await turnContext.SendActivityAsync($"I'm sorry, I can only understand English. [{result.DetectedLanguages[0].Name}]");
        return;
        break;
}
```

1. 打开 **“appsettings.json”** 文件，并确保输入你的认知服务设置：

```csharp
"cogsBaseUrl": "",
"cogsKey" :  ""
```

1. 按 **F5**启动机器人

1. 使用 Bot Emulator，输入一些短语，看看会发生什么：

- Como Estes?
- Bon Jour!
- Привет
- Hello

## 延伸阅读

我们已在先前的实验中介绍 LUIS，所以请考虑一下可能需要进行哪些更改才能使用 LUIS 支持多种语言。  一些对你有帮助的文章：

- [LUIS 的语言和区域支持](https://docs.microsoft.com/zh-cn/azure/cognitive-services/luis/luis-language-support)

## 资源

- [示例：使用文本分析检测语言](https://docs.microsoft.com/zh-cn/azure/cognitive-services/text-analytics/how-tos/text-analytics-how-to-language-detection)
- [快速入门：.NET 的文本分析客户端库](https://docs.microsoft.com/zh-cn/azure/cognitive-services/text-analytics/quickstarts/csharp)

## 后续步骤

- [实验 09-01：测试机器人 DirectLine](../Lab9-Test_Bots_DirectLine/01-Introduction.md)