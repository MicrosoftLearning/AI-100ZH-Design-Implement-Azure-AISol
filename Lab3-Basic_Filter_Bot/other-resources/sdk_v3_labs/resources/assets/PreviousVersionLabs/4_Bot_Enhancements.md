## 4_Bot_Enhancements：
预计用时：20-30 分钟

### 实验室: 正则表达式和可得分组

我们可以进行许多操作来改进机器人。首先，我们可能不想只为了实现简单的问候“hi”而调用 LUIS，机器人将经常从它的用户那里收到此问候语。  一个简单的正则表达式可以满足此需求，并节省我们的时间（由于网络延迟）和费用（调用 LUIS 服务产生的费用）。  

此外，随着我们机器人复杂程度的加深，我们将接受用户的输入并使用多种服务来解释它，因此，我们需要一个过程来管理此流。  例如，首先尝试使用正则表达式，如果不匹配，则调用 LUIS，随后我们也可以继续尝试其他服务，例如 [QnA Maker](http://qnamaker.ai) 和 Azure 认知搜索。  管理此流的一个好方法是 [ScorableGroups](https://blog.botframework.com/2017/07/06/Scorables/)。  ScorableGroups 为你提供了一个属性，用于对这些服务调用强制执行排序操作。  在我们的代码中，让我们首先对正则表达式强制执行匹配顺序，然后调用 LUIS 来解释言语，最终，最低优先级为使用通用响应“I'm not sure what you mean”。    

若要使用 ScorableGroups，你的 RootDialog 需要从 DispatchDialog 而不是 LuisDialog 继承（但仍然可以在类上保留 LuisModel 属性）。  还需要一个对 Microsoft.Bot.Builder.Scorables（以及其他）的引用。  因此，请在 RootDialog.cs 文件中添加：

```csharp

using Microsoft.Bot.Builder.Scorables;
using System.Collections.Generic;

```

并将类派生更改为：

```csharp

    public class RootDialog : DispatchDialog<object>

```

然后，让我们在 ScorableGroup 0 中添加一些匹配正则表达式的新方法作为第一优先级。  在 RootDialog 类的开头添加以下内容：

```csharp

        [RegexPattern("^hello")]
        [RegexPattern("^hi")]
        [ScorableGroup(0)]
        public async Task Hello(IDialogContext context, IActivity activity)
        {
            await context.PostAsync("Hello from RegEx!  I am a Photo Organization Bot.  I can search your photos, share your photos on Twitter, and order prints of your photos.  You can ask me things like 'find pictures of food'.");
        }

        [RegexPattern("^help")]
        [ScorableGroup(0)]
        public async Task Help(IDialogContext context, IActivity activity)
        {
            // 通过按钮菜单启动帮助对话  
            List<string> choices = new List<string>(new string[] { "Search Pictures", "Share Picture", "Order Prints" });
            PromptDialog.Choice<string>(context, ResumeAfterChoice, 
                new PromptOptions<string>("How can I help you?", options:choices));
        }

        private async Task ResumeAfterChoice(IDialogContext context, IAwaitable<string> result)
        {
            string choice = await result;
            
            switch (choice)
            {
                case "Search Pictures":
                    PromptDialog.Text(context, ResumeAfterSearchTopicClarification,
                        "What kind of picture do you want to search for?");
                    break;
                case "Share Picture":
                    await SharePic(context, null);
                    break;
                case "Order Prints":
                    await OrderPic(context, null);
                    break;
                default:
                    await context.PostAsync("I'm sorry. I didn't understand you.");
                    break;
            }
        }

```

这段代码将匹配用户以“hi”、“hello”和“help”开头的表达。  请注意，当用户请求帮助时，我们会向他/她提供一个简单的按钮菜单，其中包括我们的机器人可以执行的三项核心操作：搜索图片、共享图片和订购打印件。  

> 有趣的是：有人可能会争论说，用户应该不必键入“help”即可获得一个清楚明了的选项菜单，其中包含机器人可以执行的操作；更确切地说，这应该是第一次接触机器人时的默认体验。**可发现性**是机器人面临的最大挑战之一 - 让用户了解机器人能够做什么。  优秀的[机器人设计原则](https://docs.microsoft.com/zh-cn/bot-framework/bot-design-principles)可能有所帮助。   

在 Scorable Group 1 中，如果没有匹配的正则表达式，这个设置使我们调用 LUIS 作为第二次尝试。  

LUIS 中的“None”意图意味着言语没有映射到任何意图。  在这种情况下，我们则需要使优先级下降到 ScorableGroup 的下一个级别。  修改 RootDialog 类中的“None”方法，如下所示：

```csharp

        [LuisIntent("")]
        [LuisIntent("None")]
        [ScorableGroup(1)]
        public async Task None(IDialogContext context, LuisResult result)
        {
            // Luis 返回“None”作为入选的意图，
            // 因此使优先级下降为 ScorableGroups 的下一级。  
            ContinueWithNextGroup();
        }

```

在“Greeting”方法中，添加 ScorableGroup 属性并添加“来自 LUIS”以区分。  运行代码时，试着说“hi”和“hello”（应该由 RegEx 匹配捕获），然后说“greetings”或“hey there”（可能会由 LUIS 捕获，这取决于你的训练方式）。  注意哪种方法会响应。  

```csharp

        [LuisIntent("Greeting")]
        [ScorableGroup(1)]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            // 重复逻辑，用于针对 Scorable 进行的教学。  
            await context.PostAsync("Hello from LUIS!  I am a Photo Organization Bot.  I can search your photos, share your photos on Twitter, and order prints of your photos.  You can ask me things like 'find pictures of food'.");
        }

```

然后，将 ScorableGroup 属性添加到“SearchPics”方法和“OrderPic”方法中。  

```csharp

        [LuisIntent("SearchPics")]
        [ScorableGroup(1)]
        public async Task SearchPics(IDialogContext context, LuisResult result)
        {
            ...
        }

        [LuisIntent("OrderPic")]
        [ScorableGroup(1)]
        public async Task OrderPic(IDialogContext context, LuisResult result)
        {
            ...
        }

```

> 额外加分练习（稍后完成）：在“Dialogs”文件夹中创建一个 OrderDialog 类。  通过使用 [FormFlow](https://docs.botframework.com/zh-cn/csharp/builder/sdkreference/forms.html) 的机器人创建下令打印的过程。  你的机器人需要收集以下信息：照片尺寸（8x10、5x7、皮夹大小等）、打印数量、亮面或亚光面处理，用户电话号码和用户电子邮件。

也可以更新“SharePic”方法。  该方法包含一些代码，用于显示如何提示是/否确认以及设置 ScorableGroup。  这段代码实际上没有发布 tweet，因为我们不想花时间让每个人都设置 Twitter 开发人员帐户之类的，但是如果你愿意，可以尝试实现此操作。  

```csharp

        [LuisIntent("SharePic")]
        [ScorableGroup(1)]
        public async Task SharePic(IDialogContext context, LuisResult result)
        {
            PromptDialog.Confirm(context, AfterShareAsync,
                "Are you sure you want to tweet this picture?");            
        }

        private async Task AfterShareAsync(IDialogContext context, IAwaitable<bool> result)
        {
            if (result.GetAwaiter().GetResult() == true)
            {
                // 是，共享照片。
                await context.PostAsync("Posting tweet.");
            }
            else
            {
                // 否，不共享照片。  
                await context.PostAsync("OK, I won't share it.");
            }
        }

```

最后，如果上述服务都无法理解，请添加默认处理程序。  此 ScorableGroup 需要一个显式的 MethodBind，因为它没有被 LuisIntent 或 RegexPattern 属性（包括 MethodBind）修饰。

```csharp

        // 由于前一组中没有可得分项入选，对话将发送一条帮助消息。
        [MethodBind]
        [ScorableGroup(2)]
        public async Task Default(IDialogContext context, IActivity activity)
        {
            await context.PostAsync("I'm sorry. I didn't understand you.");
            await context.PostAsync("You can tell me to find photos, tweet them, and order prints.  Here is an example: \"find pictures of food\".");
        }

```

按 F5 在机器人仿真器中运行你的机器人并进行测试。  

### 实验：发布机器人

使用 Microsoft Bot 创建的机器人可以托管在任何可公开访问的 URL 上。  出于本实验的目的，我们将在 Azure 网站/应用服务中托管我们的机器人。  

在 Visual Studio 的“解决方案资源管理器”中，右键单击“机器人应用程序”项目并选择“发布”。  这将启动一个向导，可帮助你将机器人发布到 Azure。  

选择“Microsoft Azure 应用服务”的发布目标。  

![将机器人发布到 Azure 应用服务](./resources/assets/PublishBotAzureAppService.png) 

在“应用服务”屏幕上，选择相应的订阅，然后单击“新建”。然后输入 API 应用名称、订阅、到目前为止使用的相同资源组以及应用服务计划。  

![创建应用服务](./resources/assets/CreateAppService.jpg) 

最后，你将看到 Web 部署设置，并可单击“发布”。  Visual Studio 中的输出窗口将显示部署过程。  然后，机器人将托管在一个类似于 http://testpicturebot.azurewebsites.net/ 的 URL 中，其中“testpicturebot”是应用服务 API 应用名称。  

### 实验：使用机器人连接器注册机器人

请转到 Web 浏览器并导航到 [http://dev.botframework.com](http://dev.botframework.com)。  单击[注册机器人](https://dev.botframework.com/bots/new)。填写机器人的名称、句柄和描述。  消息终结点将为 Azure 网站 URL，其末尾附加了“api/messages”，如 https://testpicturebot.azurewebsites.net/api/messages。  

![机器人注册](./resources/assets/BotRegistration.jpg) 

然后单击按钮以创建 Microsoft 应用 ID 和密码。  这是你在 Web.config 中所需的机器人应用 ID 和密码。  将机器人应用名称、应用 ID 和应用密码存储在安全的地方！  设置密码时，一旦单击“确定”，将不能恢复。  然后单击“完成并返回 Bot Framework”。  

![机器人生成应用名称、ID 和密码](./resources/assets/BotGenerateAppInfo.jpg) 

在机器人注册页面上，你的应用 ID 应该已自动填写。你可以选择添加 AppInsights 检测密钥，以便从机器人进行日志记录。如果你同意服务条款，请选中此框，然后单击“注册”。  

然后，你将进入机器人的仪表板页，其 URL 类似于 https://dev.botframework.com/bots?id=TestPictureBot，但使用你自己的机器人名称。在这里，我们可以启用各种通道。  Skype 和 Web 聊天两个通道将自动启用。  

最后，你需要使用其注册信息更新你的机器人。  返回 Visual Studio 并打开 Web.config。  使用应用名称更新 BotId，使用应用 ID 更新 MicrosoftAppId，使用从机器人注册站点获得的应用密码更新 MicrosoftAppPassword。  

```xml

    <add key="BotId" value="TestPictureBot" />
    <add key="MicrosoftAppId" value="95b76ae6-8643-4d94-b8a1-916d9f753a30" />
    <add key="MicrosoftAppPassword" value="kC200000000000000000000" />

```

重新构建项目，然后在“解决方案资源管理器”中右键单击该项目，再次选择“发布”。  上次的设置应该已有记录，因此只需点击“发布”即可。 

> 出现了指向 MicrosoftAppPassword 的错误？由于该文件为 XML 格式，因此，如果密钥包含“&”、“<”、“>”、“'”或“"”，则需要将它们替换为各自的[转义字符](https://en.wikipedia.org/wiki/XML#Characters_and_escaping)：“&amp;”、“&lt”、“&gt;”、“&apos;”、“&quot;”。 

导航回机器人的仪表板（类似于 https://dev.botframework.com/bots?id=TestPictureBot）。  尝试在聊天窗口中与它交谈。  Web 聊天中的传送可能与仿真器看起来不同。  https://docs.botframework.com/en- us/channelinspector/channels/skype/# navtitle 中有一个很好用的工具，名为 Channel Inspector，可以使用它来查看不同通道中不同控件的用户体验。  
你可以在机器人的仪表板中可以添加其他通道，并且可在 Skype、Facebook Messenger 或 Slack 中试用你的机器人。  只需单击机器人仪表板上频道名称右侧的“添加”按钮，然后按照说明操作即可。


### 继续 [5_Challenge_and_Closing](./5_Challenge_and_Closing.md)

返回 [README](./0_README.md)