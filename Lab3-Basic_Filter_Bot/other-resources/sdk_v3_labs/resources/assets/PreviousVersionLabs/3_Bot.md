## 3_Bot：
预计用时：30-40 分钟

## 生成机器人

我们假设你曾接触过 Bot Framework。如果是，那非常好。如果没有，不要太担心，你将在本节中了解到许多相关的内容。我们建议完成[此 Microsoft 虚拟学院课程](https://mva.microsoft.com/zh-cn/training-courses/creating-bots-in-the-microsoft-bot-framework-using-c-17590#!)并查看[文档](https://docs.microsoft.com/zh-cn/bot-framework/)。

### 实验：为机器人开发而设置

我们将使用 C# SDK 来开发机器人。  首先，你需要准备两项内容：
1. Bot Framework 项目模板，可在[此处](http://aka.ms/bf-bc-vstemplate)下载。  此文件名为“Bot Application.zip”，你应将它保存到 \Documents\Visual Studio 2019\Templates\ProjectTemplates\Visual C#\ 目录中。  只需将整个压缩文件放在其中即可；无需解压缩。  
2. 在[此处](https://github.com/Microsoft/BotFramework-Emulator/releases/download/v3.5.33/botframework-emulator-Setup-3.5.33.exe)下载用于本地测试机器人的 Bot Framework Emulator。  此仿真器会安装到 `c:\Users\`_your-username_`\AppData\Local\botframework\app-3.5.33\botframework-emulator.exe`。 

### 实验：创建一个简单的机器人并运行它

在 Visual Studio 中，转到“文件”-->“新建项目”，并创建名为“PictureBot”的机器人应用程序。确保将其命名为“PictureBot”，否则稍后可能会遇到问题。  

![新建机器人应用程序](./resources/assets/NewBotApplication.png) 

>**“创建一个简单的机器人并运行它”** 这一实验剩下的部分是可选进行的。根据前提条件，你应该拥有使用 Bot Framework 的经验。可以按 F5 来确认它的构建是否正确，然后继续下一个实验。

浏览并检查示例机器人代码，这是一个回音机器人，它会重复你的消息及其字符长度。  特别要**注**意的是
+ 在 App_Start 下的 **WebApiConfig.cs** 中，路由模板是 api/{controller}/{id}，其中 id 是可选的。  这就是我们在调用机器人的终结点时始终会在末尾追加 api/messages 的原因。  
+ Controllers 下的 **MessagesController.cs** 是机器人的入口点。请注意，机器人可以响应许多不同的活动类型，并且发送消息将调用 RootDialog。  
+ 在 Dialogs 下的 **RootDialog.cs** 中，“StartAsync”是等待用户消息的入口点，而“MessageReceivedAsync”是处理接收到的消息，然后等待进一步消息的方法。  我们可以使用“context.PostAsync”将来自机器人的消息发回给用户。  

单击 F5 以运行示例代码。  NuGet 应会负责下载相应的依赖项。  

代码将在默认的 Web 浏览器中通过类似于 http://localhost:3979/ 的 URL 启动。  

> 有趣的是：为什么是这个端口号？  这是在你的项目属性中设置的。  在“解决方案资源管理器”中，双击“属性”，然后选择“Web”选项卡。  在“服务器”部分设置项目 URL。  

![机器人项目 URL](./resources/assets/BotProjectUrl.jpg) 

确保你的项目仍在运行（如果你停下来查看过项目属性，请再次按 F5），并启动 Bot Framework Emulator。  （如果你刚安装完毕，它可能没有被编入索引，无法显示在本地计算机上的搜索中，因此，请记住它安装在 c:\Users\your-username\AppData\Local\botframework\app-3.5.27\botframework-emulator.exe 中。）  确保机器人 URL 与上面代码启动的端口号相匹配，并且末尾追加有 api/messages。  你应该能够与机器人对话。  

![机器人仿真器](./resources/assets/BotEmulator.png) 

### 实验：更新机器人以使用 LUIS

我们需要更新我们的机器人才能使用 LUIS。  可以通过使用 [LuisDialog 类](https://docs.botframework.com/zh-cn/csharp/builder/sdkreference/d8/df9/class_microsoft_1_1_bot_1_1_builder_1_1_dialogs_1_1_luis_dialog.html)来进行更新。  

在 **RootDialog.cs** 文件中，添加对以下命名空间的引用：

```csharp

using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

```

然后，将 RootDialog 类更改为从 LuisDialog<object> 派生，而不是从 Idialog<object> 派生。  接下来，使用 LUIS 应用 ID 和 LUIS 密钥为类提供一个 LuisModel 属性。  如果找不到这些值，请返回 http://luis.ai。  单击你的应用程序，然后转到“发布应用”页。可从终结点 URL 获取 LUIS 应用 ID 和 LUIS 密钥（提示：LUIS 应用 ID 中将包含连字符，而 LUIS 密钥则不会）。

```csharp

using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace PictureBot.Dialogs
{
    [LuisModel("96f65e22-7dcc-4f4d-a83a-d2aca5c72b24", "1234bb84eva3481a80c8a2a0fa2122f0")]
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {

```

> 有趣的是：可以使用 [Autofac](https://autofac.org/) 在类上动态加载 LuisModel 属性，而不是对其进行硬编码，以便可以将其正确存储在配置文件中。  [AlarmBot 示例](https://github.com/Microsoft/BotBuilder/blob/master/CSharp/Samples/AlarmBot/Models/AlarmModule.cs#L24)中有相关示例。  

接下来，删除类中的两个现有方法（StartAsync 和 MessageReceivedAsync）。  LuisDialog 已经实现了 StartAsync，它将调用 LUIS 服务并根据响应路由到适当的方法。  

最后，为每个意图添加一个方法。  将针对得分最高的意图调用相应的方法。  我们将从为每个意图显示简单消息开始。  

```csharp

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hmmmm, I didn't understand that.  I'm still learning!");
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Hello!  I am a Photo Organization Bot.  I can search your photos, share your photos on Twitter, and order prints of your photos.  You can ask me things like 'find pictures of food'.");
        }

        [LuisIntent("SearchPics")]
        public async Task SearchPics(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Searching for your pictures...");
        }

        [LuisIntent("OrderPic")]
        public async Task OrderPic(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Ordering your pictures...");
        }

        [LuisIntent("SharePic")]
        public async Task SharePic(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("Posting your pictures to Twitter...");
        } 

```

修改代码后，按 F5 在 Visual Studio 中运行，然后在 Bot Framework Emulator 中启动新对话。  尝试与机器人聊天，并确保获得预期的响应。  如果得到任何意外的结果，请记下它们，我们将修改 LUIS。  

![机器人测试 LUIS](./resources/assets/BotTestLuis.jpg) 

在上面的屏幕截图中，当我向机器人说“订购打印件”时，我希望能得到不同的响应。该言语似乎映射到“SearchPics”意图而不是“OrderPic”意图。  我可以通过返回 http://luis.ai 来更新我的 LUIS 模型。  单击相应的应用程序，然后单击左侧边栏中的“意图”。  我可以手动将其添加为新的言语，或者我可以利用 LUIS 中的“建议的言语”功能来改进我的模型。  单击“SearchPics”意图（或你的言语被错误标记的意图），然后单击“建议的言语”。  单击标记错误的言语的复选框，然后单击“重新分配意图”并选择正确的意图。  

![LUIS 重新分配意图](./resources/assets/LuisReassignIntent.jpg) 

要让你的机器人选取这些更改，必须重新训练并重新发布 LUIS 模型。  单击左侧边栏中的“发布应用”，接着单击“训练”按钮，然后单击靠近底部的“发布”按钮。  然后可以返回到仿真器中的机器人，并再次尝试。  

> 有趣的是：建议的言语非常强大。  LUIS 会明智地决定要显示的言语。  它会选择那些最能帮助它改进的言语，由“人机回圈”手动标记出来。  例如，如果 LUIS 模型预测一个给定的言语以 47% 的置信度映射到 Intent1，并预测它以 48% 的置信度映射到 Intent2，那么这是一个非常适合显示给人类以进行手动映射的候选项，因为模型在两个意图之间非常接近。  

现在我们可以使用 LUIS 模型来了解用户的意图，让我们集成 Azure 认知搜索来查找我们的图片。  

### 实验室: 为 Azure 认知搜索配置机器人

首先，我们需要向机器人提供相关的信息以连接到 Azure 认知搜索索引。  最好将连接信息存储在配置文件中。  

打开 Web.config，然后在 appSettings 部分添加以下内容：

```xml
    <!-- Azure 认知搜索设置 -->
    <add key="SearchDialogsServiceName" value="" />
    <add key="SearchDialogsServiceKey" value="" />
    <add key="SearchDialogsIndexName" value="images" />
```

将 SearchDialogsServiceName 的值设置为之前创建的 Azure 认知搜索服务的名称。  如有需要，请返回 [Azure 门户](https://portal.azure.com)中查找此名称。  

将 SearchDialogsServiceKey 的值设置为此服务的密钥。  该密钥可在 [Azure 门户](https://portal.azure.com)中 Azure 认知搜索的“密钥”部分下找到。  在下面的屏幕截图中，SearchDialogsServiceName 将为“aiimmersionsearch”，SearchDialogsServiceKey 将为“375…”。  

![Azure 认知搜索设置](./resources/assets/AzureSearchSettings.jpg) 

### 实验室: 更新机器人以使用 Azure 认知搜索

接下来，我们将更新机器人以调用 Azure 认知搜索。  首先，打开“工具”-->“NuGet 包管理器”-->“管理用于解决方案的 NuGet 包”。  在搜索框中，键入“Microsoft.Azure.Search”。  选择相应的库，选中指示项目的框，然后进行安装。  可能还会安装其他依赖项。在已安装的包下，你可能还需要更新“Newtonsoft.Json”包。

![Azure 认知搜索 NuGet](./resources/assets/AzureSearchNuGet.jpg) 

在 Visual Studio 的“解决方案资源管理器”中右键单击项目，然后选择“添加”-->“新建文件夹”。  创建一个名为“Models”的文件夹。  然后右键单击 Models 文件夹，并选择“添加”-->“现有项”。  执行此操作两次，将这两个文件添加到 Models 文件夹下（确保在必要时调整名称空间）：
1. [ImageMapper.cs](./resources/code/Models/ImageMapper.cs)
2. [SearchHit.cs](./resources/code/Models/SearchHit.cs)

>可在 [resources/code/Models](./resources/code/Models) 下的此存储库中找到这些文件

接下来，右键单击 Visual Studio 的“解决方案资源管理器”中的 Dialogs 文件夹，然后选择“添加”-->“类”。  将类命名为“SearchDialog.cs”。在[此处](./resources/code/SearchDialog.cs)添加内容。

最后我们还需要更新你的 RootDialog 以调用 SearchDialog。  在 Dialogs 文件夹的 RootDialog.cs 中，更新 SearchPics 方法并添加以下“ResumeAfter”方法：

```csharp

        [LuisIntent("SearchPics")]
        public async Task SearchPics(IDialogContext context, LuisResult result)
        {
            // 检查 LUIS 是否识别出了我们应该查找的搜索词。  
            string facet = null;
            EntityRecommendation rec;
            if (result.TryFindEntity("facet", out rec)) facet = rec.Entity;

            // 如果我们不知道搜索什么（例如，用户说
            // “查找图片”或“搜索”而不是“查找 x 的图片”)，
            // 则会提示搜索词。  
            if (string.IsNullOrEmpty(facet))
            {
                PromptDialog.Text(context, ResumeAfterSearchTopicClarification,
                    "What kind of picture do you want to search for?");
            }
            else
            {
                await context.PostAsync("Searching pictures...");
                context.Call(new SearchDialog(facet), ResumeAfterSearchDialog);
            }
        }

        private async Task ResumeAfterSearchTopicClarification(IDialogContext context, IAwaitable<string> result)
        {
            string searchTerm = await result;
            context.Call(new SearchDialog(searchTerm), ResumeAfterSearchDialog);
        }

        private async Task ResumeAfterSearchDialog(IDialogContext context, IAwaitable<object> result)
        {
            await context.PostAsync("Done searching pictures");
        }

```

按 F5 再次运行你的机器人。  在机器人仿真器中，尝试使用“查找狗的照片”或“搜索幸福的照片”进行搜索。  确保在请求图片中的标记时你能看到结果。  


### 继续 [4_Bot_Enhancements](./4_Bot_Enhancements.md)

返回 [README](./0_README.md)