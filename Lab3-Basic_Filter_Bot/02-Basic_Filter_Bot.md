# 实验室 3：创建基本筛选机器人

## 简介

每一项新技术在带来许多机遇的同时也会带来许多问题，AI 支持的技术有其独特的注意事项。
在设计和实现 AI 工具时，请注意以下 AI 道德准则：

1. 公平性**：在不损害尊严的情况下最大限度地提高效率
1. 责任性**：AI 必须对其算法负责
1. 透明度**：防止偏见和对人类尊严的损害
1. 合乎道德的应用程序**：AI 必须帮助人类，并且针对智能隐私而设计

建议[深入了解](https://ai-ethics.azurewebsites.net/)生成智能应用时的道德考虑因素。

## 先决条件

1. 请按照 [Lab1-Technical_Requirements.md](../Lab1-Technical_Requirements/02-Technical_Requirements.md) 中提供的指示下载 v4 Bot Framework Emulator，以便在本地测试机器人。

## 实验 3.0 创建 Azure Web App Bot

使用 Microsoft Bot Framework 创建的机器人可以托管在任何可公开访问的 URL 上。  出于本实验的目的，我们将使用 [Azure 机器人服务](https://docs.microsoft.com/zh-cn/bot-framework/bot-service-overview-introduction)注册我们的机器人。

1. 导航到 [Azure 门户](https://portal.azure.com)。

1. 在门户中，导航到资源组，然后选择 **“+ 添加”** 并搜索 **“机器人”**。

1. 选择 **“Web 应用机器人”**，然后选择 **“创建”**。

1. 对于名称，必须创建唯一标识符。我们建议使用类似 PictureBot[i][n] 的名称，其中 [i] 是你的姓名首字母缩写，[n] 是一个数字（例如，我的是 PictureBotamt6）。

1. 选择一个区域

1. 对于定价层，请选择 **“F0（10K 高级消息）”**。

1. 选择 Bot 模板区域

1. 选择 **“C#”**，然后选择 **“Echo Bot”**，之后我们将其更新到我们的 PictureBot。

    ![机器人模板区域会突出显示，并选择语言和机器人类型。](../images/lab02-createbot.png 'Select the bot type')

1. 单击 **“确定”**，确保显示 **“Echo Bot”**。

1. 配置新的应用服务计划（将其放在机器人所在的位置）

1. 可以选择打开或关闭 Application Insights。

1. **请勿**更改或选择 **“自动创建应用 ID 和密码”**，我们稍后会对此进行说明。

1. 选择 **“创建”**

1. 部署后，请导航到新的 Azure Web App Bot 资源。

1. 在 **“机器人管理”** 下，选择 **“设置”**

1. 选择 **“Microsoft 应用 ID”** 的 **“管理”** 链接

![选择“管理”链接](../images/ManageBot.png)

1. 选择 **“新建客户端密码”**

1. 对于名称，输入 **PictureBot**

1. 对于过期，请选择 **“从不”**

1. 选择 **“添加”**

1. 将密码记录在记事本或类似应用中，以备日后在实验室中使用。

1. 选择 **“概述”**，将应用程序 ID 记录到记事本或类似应用中，以备日后在实验室中使用。

1. 导航回 **“Web 应用机器人”** 资源，在 **“机器人管理”** 下选择 **“在网上聊天中测试”** 选项卡

1. 启动机器人后，探索它的功能。  如你所见，该机器人只回应你的消息。

![基本版聊天机器人应答](../images/EchoBot.png)

## 实验 3.1：创建一个简单的机器人并运行它

1. 打开 **Visual Studio 2019** 或更高版本

1. 选择 **“创建新的项目”**，搜索 **“机器人”**。

1. 向下滚动，直到看到 **“Echo Bot (Bot Framework v4)”**

>[!警告]
>根据安装的 Visual Studio 版本，以下屏幕截图可能你您自己的不同。  如果你看到 Echo Bot 模板列出了多个版本，请选择 3.1 版而不是 2.1 版。

![选择 Echo Bot 项目模板](../images/NewBotProject.png)

1. 选择 **“下一步”**

> **注意：**如果未看到 Echo Bot 模板，则需要按照必备条件中的步骤安装 Visual Studio 加载项。

1. 对于名称，输入 **PictureBot**，然后选择 **“创建”**

1. 花一些时间来看看通过 Echo Bot 模板所生成的所有不同的内容。我们不会花时间解释每个文件，但是我们**强烈建议 稍后**花一些时间执行并查看这个示例（以及其他 Web 应用机器人示例 - 基础机器人），如果你还没有这么做的话。它包含机器人开发所需的重要、有用的 shell。可以在[此处](https://github.com/Microsoft/BotBuilder-Samples)找到它以及其他有用的 shell 和示例。

1. 首先右键单击该解决方案，并选择 **“生成”**。这将还原 nuget 包。

1. 打开 **appsettings.json** 文件，通过添加在上面记录的机器人服务信息进行更新：

```json
{
    "MicrosoftAppId": "YOURAPPID",
    "MicrosoftAppPassword": "YOURAPPSECRET"
}
```

1. 你可能知道，重命名 Visual Studio 解决方案/项目是一项非常敏感的任务。请**仔细**完成以下任务，使所有名称反映 PictureBot 而不是 EchoBot：

1. 右键单击 **Bots/Echobot.cs** 文件，然后选择 **“重命名”**，将类文件重命名为 **“PictureBot.cs”**

1. 如果系统未提示你，则需要手动重命名该类，然后将对该类的所有引用更改为 **PictureBot**。  这样在尝试生成项目时就可以知道是否缺失该类。

1. 右键单击项目，选择 **“管理 Nuget 包”**

1. 选择 **“浏览”** 选项卡，然后安装以下包，确保使用 **4.6.3** 版本：

* Microsoft.Bot.Builder.Azure
* Microsoft.Bot.Builder.AI.Luis
* Microsoft.Bot.Builder.Dialogs
* Microsoft.Azure.Search（10.1.0 版或更高版本）

1. 生成解决方案。

>**提示**：  如果你只有一台监视器，并且希望在指令和 Visual Studio 之间轻松切换，现在可以通过右键单击“解决方案资源管理器”中的“项目”，并选择 **“添加”>“现有项目”** 来将指令文件添加到 Visual Studio 解决方案。导航到“Lab2”，并添加“MD 文件”类型的所有文件。

### 创建一个 Hello World 机器人

我们已更新基础 shell 以支持命名和将在其余实验室中使用的 NuGet 包，现在已准备好开始添加一些自定义代码。首先，我们将创建一个简单的“Hello world”机器人，它将帮助你熟悉如何使用 V4 SDK 生成机器人。

有一个重要的概念是`回合`，用于说明用户的消息和来自机器人的响应。
例如，如果我说“机器人你好”，并且机器人响应“嗨，你好吗？”，那就是**一个**回合。查看下面的图片，了解一个回合是如何在机器人应用程序的多个层中完成的。

![机器人概念](../images/bots-concepts-middleware.png)

1. 打开 **PictureBot.cs** 文件。  

1. 使用下面的代码查看 `OnMessageActivityAsync` 方法。对话的每个回合都会调用这个方法。稍后你会明白为什么这个很重要，但是现在，请记住每个回合都会调用 OnMessageActivityAsync。

1. 按 **F5** 开始调试。

需要**注意**的一些事项

* default.htm（在 wwwroot 下）页面将在浏览器中显示

* 记下网页的 localhost 端口号。这应该（并且必须）与模拟器中的终结点匹配。

>卡住或中断？到目前为止，可以在 [{GitHubPath}/code/Finished/PictureBot-Part0](./code/Finished/PictureBot-Part0) 下找到适用于本实验的解决方案。解决方案中的自述文件（你打开它后）将告诉你需要添加哪些密钥才能运行解决方案。

#### 使用 Bot Framework Emulator

要与机器人互动，需要执行以下操作：

* 启动 Bot Framework Emulator（注意，我们使用的是 v4 模拟器）。  选择 **“开始”**，然后搜索 **“机器人仿真器”**。

* 在欢迎页面上，选择 **“创建新的机器人配置”**

* 对于名称，输入 **PictureBot**

* 输入机器人网页上显示的 URL

* 输入已输入到 appsettings.json 中的 AppId 和应用机密

> **注意：**如果未在机器人设置中输入 ID 和机密值，则也不需要在机器人仿真器中输入该值

* 选择 **“保存并连接”**，然后将 .bot 文件保存在本地

* 你现在应该能够与机器人对话。

* 键入 **“hello”**。该机器人将通过回应你的消息做出响应，类似于我们之前创建的 Azure 机器人。

> **注意：**可以选择“重启对话”以清除对话历史记录。

![机器人仿真器](../images/botemulator3.png)

在日志中，你应该可以看到如下这类内容：

![ngrok](../images/ngrok.png)

请注意它对我们将跳过 ngrok 以获取本地地址的描述。我们不会在本研讨会中使用 ngrok，但如果我们连接到已发布版本的机器人，我们会通过“生产”终结点来执行这个操作。打开“生产”终结点，观察不同环境中机器人之间的区别。在测试开发机器人并将它与生产机器人进行比较时，这可能是一个有用的功能。

可在[此处](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-service-debug-emulator?view=azure-bot-service-4.0)阅读关于使用模拟器的详细信息。

1. 浏览并检查示例机器人代码。特别之处在于：

* **Startup.cs**：是我们将添加服务/中间件并配置 HTTP 请求管道的位置。其中有很多注释可以帮助你了解正在进行的操作。请花几分钟阅读。

* **PictureBot.cs**：`OnMessageActivityAsync` 方法是等待来自用户的消息的入口点，我们可以在收到消息后在此对消息做出响应并等待进一步的消息。  我们可以使用 `turnContext.SendActivityAsync` 将来自机器人的消息发送回给用户。

## 实验 3.2：  管理状态和服务

1. 再次导航到 **Startup.cs** 文件

1. 通过添加以下内容来更新 `using` 语句列表：

```csharp
using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Bot.Builder.Integration;
using Microsoft.Bot.Configuration;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Microsoft.PictureBot;

using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.Dialogs;
```

我们暂时不会使用上述所有命名空间，但你可以猜一猜我们什么时候会用？

1. 在 **Startup.cs** 类中，重点关注 `ConfigureServices` 方法，该方法用于向机器人添加服务。请仔细查看内容，注意内置内容。

> 有助于加深理解的一些其他注释：
>
> * 如果你不熟悉依赖关系注入，可以[在此阅读更多相关信息](https://docs.microsoft.com/zh-cn/aspnet/web-api/overview/advanced/dependency-injection)。
> * 出于本实验和测试目的，可以使用本地内存。对于生产，必须实现一种[管理状态数据](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-builder-storage-concept?view=azure-bot-service-4.0)的方式。在 `ConfigureServices` 中的大量注释中，有一些相关提示。
> * 在方法的底部，你可能会注意到我们会创建并注册状态访问器。管理状态是创建智能机器人的关键，可以[在此阅读详细信息](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-builder-dialog-state?view=azure-bot-service-4.0)。

幸运的是，这个 shell 非常全面，因此我们只需添加两个项目：

* 中间件
* 自定义状态访问器。

### 中间件

中间件是位于适配器和机器人逻辑之间的一个类或一组类，在初始化期间添加到适配器的中间件集合中。

通过 SDK，可以编写你自己的中间件或添加由其他人创建的中间件的可重用组件。进出机器人的每个活动都会经过中间件。我们稍后会在实验中进行深入了解，但是现在，请务必了解每个活动都会经过中间件，因为它位于运行时调用的 `ConfigureServices` 方法中（该方法在由用户发送的每条消息和 `OnMessageActivityAsync` 之间运行）。

1. 添加名为 **“中间件”** 的新文件夹

1. 右键单击 **“中间件”** 文件夹并选择 **“添加”>“现有项”**。

1. 导航到 **{GitHubDir}\Lab3-Basic_Filter_Bot\code\Middleware**，全选三个文件，然后选择 **“添加”**

1. 将以下变量添加到 **Startup** 类：

```csharp
private ILoggerFactory _loggerFactory;
private bool _isProduction = false;
```

1. 替换 **ConfigureServices** 方法中的以下代码：

```csharp
services.AddTransient<IBot, PictureBot.Bots.PictureBot>();
```

替换为以下代码：

```csharp
services.AddBot<PictureBot.Bots.PictureBot>(options =>
{
    var appId = Configuration.GetSection("MicrosoftAppId")?.Value;
                var appSecret = Configuration.GetSection("MicrosoftAppPassword")?.Value;

                options.CredentialProvider = new SimpleCredentialProvider(appId, appSecret);

    // 创建供应用程序使用的记录程序。
    ILogger logger = _loggerFactory.CreateLogger<PictureBot.Bots.PictureBot>();

    // 捕捉对话回合过程中发生的所有错误并记录下来。
    options.OnTurnError = async (context, exception) =>
    {
        logger.LogError($"捕获到的异常: {exception}");
        await context.SendActivityAsync("抱歉，似乎出现了问题。");
    };

    // 此处使用的内存存储仅用于本地机器人调试。重启机器人后，
    // 内存中存储的所有内容都将消失。
    IStorage dataStore = new MemoryStorage();

    // 对于生产机器人，请使用 Azure Blob 或
    // Azure CosmosDB 存储提供程序。对于基于 Azure
    // 的存储提供程序，请将 Microsoft.Bot.Builder.Azure
    // Nuget 包添加到解决方案中。该包位于：
    // https://www.nuget.org/packages/Microsoft.Bot.Builder.Azure/
    // 取消注释以下行以使用 Azure Blob 存储
    // // .bot 文件中的存储配置名称或 ID。
    // const string StorageConfigurationId = "<STORAGE-NAME-OR-ID-FROM-BOT-FILE>";
    // var blobConfig = botConfig.FindServiceByNameOrId(StorageConfigurationId);
    // if (!(blobConfig is BlobStorageService blobStorageConfig))
    // {
    //    throw new InvalidOperationException($".bot 文件不包含名称为 '{StorageConfigurationId}' 的 blob 存储。");
    // }
    // // 默认容器名称。
    // const string DefaultBotContainer = "botstate";
    // var storageContainer = string.IsNullOrWhiteSpace(blobStorageConfig.Container) ? DefaultBotContainer : blobStorageConfig.Container;
    // IStorage dataStore = new Microsoft.Bot.Builder.Azure.AzureBlobStorage(blobStorageConfig.ConnectionString, storageContainer);

    // 创建对话状态对象。
    // 对话状态对象是我们持久存储会话范围内任何内容的位置。
    var conversationState = new ConversationState(dataStore);

    options.State.Add(conversationState);

    var middleware = options.Middleware;
    // 添加以下包含“middleware.Add(....”的中间件
    // 添加以下 Regex
});
```

1. 将 **Configure**方法替换为以下代码：

```csharp
public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
{
    _loggerFactory = loggerFactory;

    app.UseDefaultFiles()
        .UseStaticFiles()
        .UseBotFramework();

    app.UseMvc();
}
```

#### 自定义状态访问器

在我们讨论需要的自定义状态访问器之前，请务必备份。我们将在下一部分中正式介绍对话框，它们是实现多回合对话逻辑的一种方法，这意味着它们需要依赖持续状态来了解用户在对话中的位置。在基于对话框的机器人中，我们将使用 DialogSet 来保存各种对话框。DialogSet 是使用名为“访问器”的对象的句柄创建的。

在 SDK 中，访问器实现 `IStatePropertyAccessor`接口，这基本上意味着它提供了获取、设置和删除有关状态的信息的功能，因此我们可以跟踪用户在对话中位于哪个步骤。

对于我们创建的每个访问器，我们必须先为它赋予一个属性名称。对于我们的场景，我们想要跟踪一些内容：

1. `PictureState`
    * 我们问候用户了吗？
        * 我们不想多次问候用户，但我们希望确保在对话开始时问候他们。
    * 用户当前是否正在搜索特定的词？如果是，这个词是什么？
        * 我们需要跟踪用户是否已告知我们他们想要搜索的内容，以及搜索内容是什么（如果他们想要搜索）。
2. `DialogState`
    * 用户当前是否正在进行对话？
        * 我们将用它来确定用户在给定对话或对话流中的位置。如果你不熟悉对话框，请不要担心，我们很快就会对此进行介绍。

我们可以使用这些构造函数来跟踪我们称之为 `PictureState` 的内容。

1. 在 **Startup.cs** 文件的 **ConfigureServices** 方法中，在自定义状态访问器列表中添加 `PictureState` 并跟踪对话框，你将使用内置的 `DialogState`：

```csharp
// 创建并注册状态访问器。
// 此处创建的访问器在每个回合中都会传递到 IBot 派生的类中。
services.AddSingleton<PictureBotAccessors>(sp =>
{
    var options = sp.GetRequiredService<IOptions<BotFrameworkOptions>>().Value;
    if (options == null)
    {
        throw new InvalidOperationException("BotFrameworkOptions 必须在设置状态访问器之前进行配置");
    }

    var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
    if (conversationState == null)
    {
        throw new InvalidOperationException("必须先定义和添加 ConversationState，然后才能添加对话范围内的状态访问器。");
    }

    // 创建自定义状态访问器。
    // 状态访问器使其他组件能够读取和写入状态的各属性。
    var accessors = new PictureBotAccessors(conversationState)
    {
        PictureState = conversationState.CreateProperty<PictureState>(PictureBotAccessors.PictureStateName),
        DialogStateAccessor = conversationState.CreateProperty<DialogState>("DialogState"),
    };

    return accessors;
});
```

你应该可以在某些术语下面看到错误（红色波浪线）。但在修复它们之前，你可能想知道为什么我们必须创建两个访问器，为什么一个不够？

* `DialogState` 是来自 `Microsoft.Bot.Builder.Dialogs` 库的特定访问器。发送消息时，对话框子系统将调用 `DialogSet` 上的 `CreateContext`。跟踪这个上下文需要 `DialogState` 访问器来专门获取相应的对话框状态 JSON。
* 另一方面，`PictureState` 将用于跟踪我们在整个对话中指定的特定对话属性（例如，我们是否已问候用户？）

> 现在不要担心对话术语，但这个过程应该有意义。如果你感到疑惑，可以[深入了解状态的运作方式](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-builder-dialog-state?view=azure-bot-service-4.0)。

现在回到你看到的错误。你已经说过要存储这些信息，但是你还未说明存储位置或方式。我们必须更新“PictureState.cs”和“PictureBotAccessor.cs”才能拥有和访问我们想要存储的信息。

1. 右键单击项目，然后选择 **“添加”->“类”**，选择类文件并将其命名为 **PictureState**

1. 将以下代码复制到 **PictureState.cs**。

```csharp
using System.Collections.Generic;

namespace Microsoft.PictureBot
{
    /// <summary>
    /// 存储对话的计数器状态。
    /// 存储在 <see cref="Microsoft.Bot.Builder.ConversationState"/> 中并
    /// 受 <see cref="Microsoft.Bot.Builder.MemoryStorage"/> 支持。
    /// </summary>
    public class PictureState
    {
        /// <summary>
        /// 获取或设置对话中的回合数。
        /// </summary>
        /// <value>对话中的回合数。</value>
        public string Greeted { get; set; } = "not greeted";
        public string Search { get; set; } = "";
        public string Searching { get; set; } = "no";
    }
}
```

1. 查看代码。  我们在这里存储关于活动对话的信息。  可随时添加一些解释字符串用途的注释。正确初始化 PictureState 之后，可以创建 PictureBotAccessor，以删除在 **Startup.cs**中遇到的错误。

1. 右键单击项目，然后选择 **“添加”->“类”**，选择类文件并将其命名为 **PictureBotAccessors**

1. 将以下内容复制到其中：

```csharp
using System;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.PictureBot
{
    /// <summary>
    /// 此类作为 Singleton 创建，并传递到 IBot 派生的构造函数中。
    /// - 请参阅 <see cref="PictureBot"/> 构造函数，了解如何注入。
    /// - 有关如何创建注入到构造函数的 Singleton 的详细信息，
    /// 请参阅 Startup.cs 文件。
    /// </summary>
    public class PictureBotAccessors
    {
        /// <summary>
        /// 初始化 <see cref="PictureBotAccessors"/> 类的新实例。
        /// 包含 <see cref="ConversationState"/> 和相关的 <see cref="IStatePropertyAccessor{T}"/>。
        /// </summary>
        /// <param name="conversationState">存储计数器的状态对象。</param>
        public PictureBotAccessors(ConversationState conversationState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
        }

        /// <summary>
        /// 获取用于 <see cref="CounterState"/> 访问器的 <see cref="IStatePropertyAccessor{T}"/> 名称。
        /// </summary>
        /// <remarks>访问器需要唯一的名称。</remarks>
        /// <value>计数器访问器的访问器名称。</value>
        public static string PictureStateName { get; } = $"{nameof(PictureBotAccessors)}.PictureState";

        /// <summary>
        /// 获取或设置用于 CounterState 的 <see cref="IStatePropertyAccessor{T}"/>。
        /// </summary>
        /// <value>
        /// 访问器存储对话的回合数。
        /// </value>
        public IStatePropertyAccessor<PictureState> PictureState { get; set; }

        /// <summary>
        /// 获取对话的 <see cref="ConversationState"/> 对象。
        /// </summary>
        /// <value> <see cref="ConversationState"/> 对象。</value>
        public ConversationState ConversationState { get; }

        /// <summary> 获取用于 DialogState 访问器的 IStatePropertyAccessor{T} 名称。</summary>
        public static string DialogStateName { get; } = $"{nameof(PictureBotAccessors)}.DialogState";

        /// <summary> 为 DialogState 获取或设置 IStatePropertyAccessor{T}。</summary>
        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }
    }
}
```

1. 查看代码，注意 `PictureStateName` 和 `PictureState` 的实现。

1. 想知道你是否已正确对它进行配置？返回 **Startup.cs** 并确认已解决有关创建自定义状态访问器的错误。

## 实验 3.3：为机器人组织代码

有许多用于开发机器人的不同方法和首选项。通过 SDK，可以采用任何方式组织代码。在这些实验室中，我们将对话整理到不同的对话框中，并且将探索一种围绕对话的组织代码的 [MVVM 样式](https://msdn.microsoft.com/zh-cn/library/hh848246.aspx)。

这个 PictureBot 将按以下方式组织：

* **对话框** - 用于编辑模型的业务逻辑
* **响应** - 定义面向用户的输出的类
* **模型** - 要修改的对象

1. 通过在项目上单击鼠标右键并选择**添加->新建文件夹**，在项目中创建两个新文件夹“**Responses**”和“**Models**”

### 对话框

你可能已经熟悉对话框及其工作方式。如果你不熟悉，请阅读[关于对话框的这个页面](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-builder-dialog-manage-conversation-flow?view=azure-bot-service-4.0&tabs=csharp)，然后再继续。

如果机器人能够执行多个任务，最好有多个对话框或一组对话框，以帮助用户浏览不同的对话流。对于 PictureBot，我们希望用户能够浏览初始菜单流（通常称为主对话框），然后分散到不同的对话框，具体取决于他们尝试进行的操作 - 搜索图片、分享图片、订购图片，或获取帮助。我们可以通过使用对话框容器或这里称为 `DialogSet` 的方式轻松完成此操作。请阅读关于如何[创建模块化机器人逻辑和复杂对话框](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-builder-compositcontrol?view=azure-bot-service-4.0&tabs=csharp)的内容，然后再继续。

对于本实验室，我们将一切从简，但之后，你应该能够创建包含许多对话框的对话框集。对于 PictureBot，我们将有两个主对话框：

* **MainDialog** - 机器人启动时使用的默认对话框。这个对话框将根据用户请求启动其他对话框。作为对话框集的主对话框，这个对话框将负责创建对话框容器，并根据需要将用户重定向到其他对话框。

* **SearchDialog** - 一个对话框，用于管理处理搜索请求并将那些结果返回给用户。 ***注意：**我们将调用这个功能，但不会在本研讨会中实施搜索。*

由于我们只有两个对话框，因此可以保持简单，并将它们放在 PictureBot 类中。但是，在复杂的场景中，可能需要将它们拆分到一个文件夹的不同对话框中（类似于我们分离“响应”和“模型”那样）。

1. 导航回 **PictureBot.cs** 并用以下内容替换 `using` 语句：

```csharp
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Logging;
using System.Linq;
using PictureBot.Models;
using PictureBot.Responses;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.PictureBot;
```

你刚刚添加了对模型/响应以及对服务 LUIS 和 Azure 认知搜索的访问权限。最后，Newtonsoft 引用将帮助你分析来自 LUIS 的响应，我们将在后续实验室中看到它。

接下来，我们需要将 `OnTurnAsync` 方法替换为具有以下功能的方法：处理传入的邮件，然后将邮件路由到各种对话框中。

1. 将 **PictureBot** 类替换为以下代码：

```csharp
/// <summary>
/// 表示处理传入活动的机器人。
/// 对于每次用户交互，会创建这个类的实例并调用 OnTurnAsync 方法。
/// 这是临时生存期服务。  在每次请求临时生存期服务时，
/// 会进行创建。对于收到的每个活动，会创建这个类的
/// 新实例。构造成本昂贵或生存期超过
/// 单个回合的对象应谨慎管理。
/// 例如，<see cref="MemoryStorage"/> 对象和关联的
/// <see cref="IStatePropertyAccessor{T}"/> 对象在单一实例生存期创建。
/// </summary>
/// <seealso cref="https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
/// <summary>包含一组对话框和针对 picture 机器人的提示。</summary>
public class PictureBot : ActivityHandler
{
    private readonly PictureBotAccessors _accessors;
    // 初始化 LUIS 识别器

    private readonly ILogger _logger;
    private DialogSet _dialogs;

    /// <summary>
    /// 我们的 PictureBot 的每个对话回合将调用这个方法。
    /// 这里未使用对话框，因为它是“单回合”处理，表示单个
    /// 请求和响应。稍后，当我们添加对话框时，则必须在这个方法中导航。
    /// </summary>
    /// <param name="turnContext">A <see cref="ITurnContext"/> 包含处理这个对话回合
    /// 所需的所有数据。</param>
    /// <param name="cancellationToken">（可选），<see cref="CancellationToken"/> 可由其他对象
    /// 或线程使用以接收取消通知。</param>
    /// <returns><see cref="Task"/>，表示排队待执行的工作。</returns>
    /// <seealso cref="BotStateSet"/>
    /// <seealso cref="ConversationState"/>
    /// <seealso cref="IMiddleware"/>
    public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
    {
        if (turnContext.Activity.Type is "message")
        {
            // 通过对话状态建立对话框上下文。
            var dc = await _dialogs.CreateContextAsync(turnContext);
            // 继续任何当前对话。
            var results = await dc.ContinueDialogAsync(cancellationToken);

            // 每个回合发送一个响应，因此，如果未发送响应，
            // 则表示当前没有处于活动状态的对话框。
            if (!turnContext.Responded)
            {
                // 启动主对话框
                await dc.BeginDialogAsync("mainDialog", null, cancellationToken);
            }
        }
    }
    /// <summary>
    /// 初始化 <see cref="PictureBot"/> 类的新实例。
    /// </summary>
    /// <param name="accessors">包含用于管理状态的 <see cref="IStatePropertyAccessor{T}"/> 的类。</param>
    /// <param name="loggerFactory"> <see cref="ILoggerFactory"/>，连接到 Azure 应用服务提供程序。</param>
    /// <seealso cref="https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/logging/?view=aspnetcore-2.1#windows-eventlog-provider"/>
    public PictureBot(PictureBotAccessors accessors, ILoggerFactory loggerFactory /*, LuisRecognizer recognizer*/)
    {
        if (loggerFactory == null)
        {
            throw new System.ArgumentNullException(nameof(loggerFactory));
        }

        // 添加 LUIS 识别器的实例

        _logger = loggerFactory.CreateLogger<PictureBot>();
        _logger.LogTrace("PictureBot turn start.");
        _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));

        // DialogSet 需要 DialogState 访问器，它将在具有回合上下文时调用这个访问器。
        _dialogs = new DialogSet(_accessors.DialogStateAccessor);

        // 这个数组定义 Waterfall 的执行方式。
        // 我们可以在此定义不同的对话框及其步骤
        // 根据需要允许重叠。在这种情况下，这十分简单
        // 但在更复杂的情况下，可能需要将不同的
        // 对话框分离到不同的文件中。
        var main_waterfallsteps = new WaterfallStep[]
        {
            GreetingAsync,
            MainMenuAsync,
        };
        var search_waterfallsteps = new WaterfallStep[]
        {
            // 添加 SearchDialog water fall 步骤

        };

        // 将已命名的对话框添加到 DialogSet。这些名称保存在对话框状态中。
        _dialogs.Add(new WaterfallDialog("mainDialog", main_waterfallsteps));
        _dialogs.Add(new WaterfallDialog("searchDialog", search_waterfallsteps));
        // 以下行允许我们在对话框中使用提示
        _dialogs.Add(new TextPrompt("searchPrompt"));
    }
    // 添加 MainDialog 相关的任务

    // 添加 SearchDialog 相关的任务

    // 添加搜索相关的任务

}
```

花一些时间与研讨会参与者一起回顾和讨论这个 shell。在继续之前，你应该了解每一行的用途。

我们稍后会对这个内容进行补充。现可忽略任何错误。

#### 响应

所以在填写对话框之前，我们需要准备一些响应。请记住，我们将把对话框和响应分开，因为这样会产生更简洁的代码，并且更易于遵循对话框的逻辑。如果你现在不同意或不理解，相信你很快就会同意和理解。

1. 在 **“响应”** 文件夹中，创建两个类，分别称为 **MainResponses.cs** 和 **SearchResponses.cs**。你可能已经明白，响应文件将只包含我们想要发送给用户的不同输出，而不是逻辑。

1. 在 **MainResponses.cs** 中将代码替换为以下代码：

```csharp
using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace PictureBot.Responses
{
    public class MainResponses
    {
        public static async Task ReplyWithGreeting(ITurnContext context)
        {
            // 添加问候
        }
        public static async Task ReplyWithHelp(ITurnContext context)
        {
            await context.SendActivityAsync($"I can search for pictures, share pictures and order prints of pictures.");
        }
        public static async Task ReplyWithResumeTopic(ITurnContext context)
        {
            await context.SendActivityAsync($"What can I do for you?");
        }
        public static async Task ReplyWithConfused(ITurnContext context)
        {
            // 如果 Regex 或 LUIS 不知道用户要说什么
            // 请为用户添加回复
        }
        public static async Task ReplyWithLuisScore(ITurnContext context, string key, double score)
        {
            await context.SendActivityAsync($"Intent: {key} ({score}).");
        }
        public static async Task ReplyWithShareConfirmation(ITurnContext context)
        {
            await context.SendActivityAsync($"Posting your picture(s) on twitter...");
        }
        public static async Task ReplyWithOrderConfirmation(ITurnContext context)
        {
            await context.SendActivityAsync($"Ordering standard prints of your picture(s)...");
        }
    }
}
```

请注意，有两个不包含值的响应（ReplyWithGreeting 和 ReplyWithConfused）。根据需要填写这些内容。

1. 在“SearchResponses.cs”中，将代码替换为以下内容：

```csharp
using Microsoft.Bot.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace PictureBot.Responses
{
    public class SearchResponses
    {
        // 添加名为“ReplyWithSearchRequest”的任务
        // 它应该考虑上下文并询问
        // 用户要搜索什么内容
        public static async Task ReplyWithSearchConfirmation(ITurnContext context, string utterance)
        {
            await context.SendActivityAsync($"Ok, searching for pictures of {utterance}");
        }
        public static async Task ReplyWithNoResults(ITurnContext context, string utterance)
        {
            await context.SendActivityAsync("There were no results found for \"" + utterance + "\".");
        }
    }
}
```

1. 请注意，这里缺少一个完整的任务。根据需要填写，但请确保新任务的名称为“ReplyWithSearchRequest”，否则你之后可能会遇到问题。

#### 模型

由于时间限制，我们不会介绍如何创建所有模型。它们很简单，但我们建议你在添加代码后花些时间进行评审。 

1. 右键单击 **“模型”** 文件夹并选择 **“添加”>“现有项目”**。

1. 导航到 **{GitHubDir}\Lab3-Basic_Filter_Bot\code\Models**，全选三个文件，然后选择 **“添加”**。

## 实验 3.4：Regex 和中间件

我们可以通过很多操作来改进机器人。首先，我们可能不想通过调用 LUIS 来获取简单的“搜索图片”消息，机器人将经常从其用户那里获得这个消息。  一个简单的正则表达式可以满足此需求，并节省我们的时间（由于网络延迟）和费用（调用 LUIS 服务产生的费用）。

此外，随着我们机器人复杂程度的加深，我们将接受用户的输入并使用多种服务来解释它，因此，我们需要一个过程来管理此流。  例如，首先尝试使用正则表达式，如果不匹配，则调用 LUIS，我们也可以继续尝试其他服务，例如 [QnA Maker](http://qnamaker.ai) 或 Azure 认知搜索。管理这个流的好方法是借助 [Middleware](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-builder-concept-middleware?view=azure-bot-service-4.0)，SDK 可为这方面提供出色支持。

在继续进行实验之前，请了解有关中间件和 Bot Framework SDK 的详细信息：

1. [概述和架构](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0)

1. [中间件](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-builder-concept-middleware?view=azure-bot-service-4.0)

1. [创建中间件](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-builder-create-middleware?view=azure-bot-service-4.0&tabs=csaddmiddleware%2Ccsetagoverwrite%2Ccsmiddlewareshortcircuit%2Ccsfallback%2Ccsactivityhandler)

最终，我们将使用一些中间件来尝试先理解用户使用正则表达式 (Regex) 所说的内容，如果不能理解，我们将调用 LUIS。如果仍然不能理解，我们将继续并采用通用回复“I'm not sure what you mean”或者你为“ReplyWithConfused”设置的任何内容。

1. 在 **Startup.cs**的 `ConfigureServices` 中的“添加以下 Regex”注释下，添加以下内容：

```csharp
middleware.Add(new RegExpRecognizerMiddleware()
.AddIntent("search", new Regex("search picture(?:s)*(.*)|search pic(?:s)*(.*)", RegexOptions.IgnoreCase))
.AddIntent("share", new Regex("share picture(?:s)*(.*)|share pic(?:s)*(.*)", RegexOptions.IgnoreCase))
.AddIntent("order", new Regex("order picture(?:s)*(.*)|order print(?:s)*(.*)|order pic(?:s)*(.*)", RegexOptions.IgnoreCase))
.AddIntent("help", new Regex("help(.*)", RegexOptions.IgnoreCase)));

```

> 我们实际上只是粗略了解了如何使用正则表达式。如果你有兴趣，可以[在此处了解详细信息](https://docs.microsoft.com/zh-cn/dotnet/standard/base-types/regular-expression-language-quick-reference)。

1. 你可能会注意到 `options.State` 已被弃用。  让我们迁移到最新的方法：

1. 删除以下代码：

```csharp
var conversationState = new ConversationState(dataStore);

options.State.Add(conversationState);
```

1. 将其替换为

```csharp
var userState = new UserState(dataStore);
var conversationState = new ConversationState(dataStore);

// 创建用户状态。
services.AddSingleton<UserState>(userState);

// 创建对话状态。
services.AddSingleton<ConversationState>(conversationState);
```

1. 同时替换 `ConfigureServices` 代码，以从依赖项注入版本中提取：

```csharp
var conversationState = options.State.OfType<ConversationState>().FirstOrDefault();
if (conversationState == null)
{
    throw new InvalidOperationException("必须先定义和添加 ConversationState，然后才能添加对话范围内的状态访问器。");
}
```

1. 将其替换为

```csharp
var conversationState = services.BuildServiceProvider().GetService<ConversationState>();

if (conversationState == null)
{
    throw new InvalidOperationException("必须先定义和添加 ConversationState，然后才能添加对话范围内的状态访问器。");
}
```

如果没有添加 LUIS，我们的机器人实际上只会获取一些变体，但如果用户使用机器人搜索、共享和订购图片，它应该会捕获许多消息。

> 题外话：有人可能会争论说，用户应该不必键入“help”即可获得一个清楚明了的选项菜单，其中包含机器人可以执行的操作；更确切地说，这应该是第一次接触机器人时的默认体验。**可发现性**是机器人面临的最大挑战之一 - 让用户了解机器人能够做什么。  优秀的[机器人设计原则](https://docs.microsoft.com/zh-cn/bot-framework/bot-design-principles)可能有所帮助。

## 实验室 3.5：运行机器人

### 再次介绍 MainDialog

现在开始进入正题。我们需要在 PictureBot.cs 中填写 MainDialog，以便我们的机器人可以对用户所说的要做的事情做出反应。根据 Regex 的结果，我们需要将对话指向正确的方向。仔细阅读代码，确认了解代码的作用。

1. 在 **PictureBot.cs** 中，通过粘贴以下方法代码添加以下代码：

```csharp
// 如果我们还未问候用户，我们需要首先这样做，但是在后面的
// 对话中，我们需要记住我们已经问候过用户了。
private async Task<DialogTurnResult> GreetingAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
{
    // 获取对话中的当前步骤的状态
    var state = await _accessors.PictureState.GetAsync(stepContext.Context, () => new PictureState());

    // 如果我们还未问候用户
    if (state.Greeted == "not greeted")
    {
        // 问候用户
        await MainResponses.ReplyWithGreeting(stepContext.Context);
        // 将 GreetedState 更新为已问候
        state.Greeted = "greeted";
        // 将新的问候状态保存到对话状态
        // 这是为了确保在未来的回合中，我们不会重复问候用户
        await _accessors.ConversationState.SaveChangesAsync(stepContext.Context);
        // 询问用户他们下一步要做什么
        await MainResponses.ReplyWithHelp(stepContext.Context);
        // 由于我们未在这个步骤中明确提示用户，因此我们将结束对话
        // 当用户回复时，因为状态保持不变，因此 else 子句会将它们移动到
        // 下一个 waterfall 步骤
        return await stepContext.EndDialogAsync();
    }
    else // 我们已问候用户
    {
        // 转到下一个 waterfall 步骤，即 MainMenuAsync
        return await stepContext.NextAsync();
    }

}

// 这个步骤将用户路由到其他对话框
// 在这种情况下，只有一个不同的对话框，因此较简单，
// 但在较复杂的情况下，可以转到其他类似的对话框
public async Task<DialogTurnResult> MainMenuAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
{
    // 检查我们当前是否正在处理用户的搜索
    var state = await _accessors.PictureState.GetAsync(stepContext.Context);

    // 如果正则表达式选取任何内容，请存储它
    var recognizedIntents = stepContext.Context.TurnState.Get<IRecognizedIntents>();
    // 根据已识别的意向，引导对话
    switch (recognizedIntents.TopIntent?.Name)
    {
        case "search":
            // 切换到搜索对话框
            return await stepContext.BeginDialogAsync("searchDialog", null, cancellationToken);
        case "share":
            // 回复你正在共享照片
            await MainResponses.ReplyWithShareConfirmation(stepContext.Context);
            return await stepContext.EndDialogAsync();
        case "order":
            // 回复你正在订购
            await MainResponses.ReplyWithOrderConfirmation(stepContext.Context);
            return await stepContext.EndDialogAsync();
        case "help":
            // 显示帮助
            await MainResponses.ReplyWithHelp(stepContext.Context);
            return await stepContext.EndDialogAsync();
        default:
            {
                await MainResponses.ReplyWithConfused(stepContext.Context);
                return await stepContext.EndDialogAsync();
            }
    }
}
```

1. 按 **F5** 运行机器人。

1. 使用机器人仿真器，通过发送一些命令来测试机器人：

* help
* share pics
* order pics
* search pics
  
> **注意：**如果机器人出现 500 错误，则可以在 **Startup.cs**文件的 **OnTurnError** 委托方法中放置一个断点。  最常见的错误是 AppId 和 AppSecret 不匹配。

1. 如果唯一没有提供预期结果的命令是“search pics”，那么一切都是按照你的配置方式运行的。“search pics”失败目前是本实验室的预期行为，但这是为什么呢？请在继续之前提供一个答案！

>提示：从 **PictureBot.cs** 开始，使用断点跟踪与情况“搜索”的匹配。
>卡住或中断？到目前为止，可以在 [resources/code/Finished](./code/Finished) 下找到适用于本实验的解决方案。解决方案中的自述文件（你打开它后）将告诉你需要添加哪些密钥才能运行解决方案。建议使用此代码作为参考，而不是作为解决方案运行，但如果选择运行此代码，请务必为你的环境添加必需的密钥。

## 资源

* [Bot Builder 基础知识](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-builder-basics?view=azure-bot-service-4.0&tabs=cs)
* [.NET Bot Builder SDK 教程](https://docs.microsoft.com/zh-cn/azure/bot-service/dotnet/bot-builder-dotnet-sdk-quickstart?view=azure-bot-service-4.0)
* [机器人服务文档](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-service-overview-introduction?view=azure-bot-service-4.0)
* [部署机器人](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-builder-deploy-az-cli?view=azure-bot-service-4.0&tabs=newrg)

## 后续步骤

* [实验 04-01：记录聊天信息](../Lab4-Log_Chat/01-Introduction.md)
