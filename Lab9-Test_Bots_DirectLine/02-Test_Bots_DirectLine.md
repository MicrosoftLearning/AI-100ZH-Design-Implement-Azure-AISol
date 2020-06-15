# 实验室 9：在 Direct Line 中测试机器人

## 简介

在某些情况下，可能需要直接与机器人通信。例如，你可能希望使用托管机器人执行功能测试。可以使用 [Direct Line API](https://docs.microsoft.com/zh-cn/bot-framework/rest-api/bot-framework-rest-direct-line-3-0-concepts) 来执行机器人与你自己的客户端应用程序之间的通信。

此外，有时你可能希望使用其他通道或进一步自定义机器人。在这种情况下，你可以使用 Direct Line 与自定义机器人进行通信。

Microsoft Bot Framework Direct Line 机器人是可以使用自己设计的自定义客户端运行的机器人。Direct Line 机器人类似于普通机器人。它们只是不需要使用提供的通道。可以按你的期望编写 Direct Line 客户端。你可以编写 Android 客户端、iOS 客户端，甚至是基于控制台的客户端应用程序。

此动手实验引入了与 Direct Line API 相关的主要概念。

## 实验 9.0：先决条件：

本实验假定你已在[实验 3](../Lab3-Basic_Filter_Bot/02-Basic_Filter_Bot.md) 中生成并发布了机器人。建议执行该实验，这样才能成功完成其后续实验。如果没有执行该实验，可以仔细阅读所有练习，并根据需要查看一些代码或者将代码用于自己的应用程序中。

我们还假设你已完成了[实验 4](../Lab4-Log_Chat/02-Logging_Chat.md)，不过，即使你未完成记录实验，也能完成本实验。

### 收集密钥

在前几个试验课程中，我们已收集各种密钥。如果将初学者项目作为起点，则将需要其中的大多数密钥。

>_密钥_
>
>- 认知服务 API URL：
>- 认知服务 API 密钥：
>- LUIS API 终结点：
>- LUIS API 密钥：
>- LUIS API 应用 ID：
>- 机器人应用名称：
>- 机器人应用 ID：
>- 机器人应用密码：
>- Azure 存储连接字符串：
>- Cosmos DB URL：
>- Cosmos DB 密钥：
>- DirectLine 密钥：

请确保使用所有必要值更新 **“appsettings.json”** 文件。

## 实验 9.1：发布机器人

1. 打开 **“PictureBot”** 解决方案

1. 右键单击项目并选择 **“发布”**。

1. 在发布对话框中，选择 **“选择现有”**，然后选择 **“发布”**

1. 如果出现提示，请使用在实验中使用的帐户登录

1. 选择一直在使用的订阅

1. 展开资源组，然后选择我们在实验 3 中创建的 Picture Bot 应用服务

1. 选择 **“确定”**

> **注**：根据进入此实验时采用的路径，在进行以下测试时可能需要第二次发布，以获取 Echo Bot 服务。  重新发布机器人，仅在这次更改发布设置以删除现有文件。  

## 实验 9.2：设置 Direct Line 通道

1. 在门户中，找到已发布的 PictureBot **Web 应用机器人**，并导航到 **“通道”** 选项卡。

1. 选择“Direct Line”图标（看起来像地球仪）。你将看到显示 **“默认网站”**。

1. 在 **“机密密钥”** 处选择 **“显示”**，并将其中某个密钥存储在记事本中或者记录密钥的任何位置。

你可以阅读有关[启用 Direct Line 通道](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-service-channel-connect-directline?view=azure-bot-service-4.0)以及[机密和令牌](https://docs.microsoft.com/zh-cn/azure/bot-service/rest-api/bot-framework-rest-direct-line-3-0-authentication?view=azure-bot-service-3.0#secrets-and-tokens)的详细说明。

## 实验 9.3：创建控制台应用程序

我们将创建一个控制台应用程序，帮助我们了解如何使用 Direct Line 直接连接到机器人。我们将创建的控制台客户端应用程序会在两个会话中运行。主会话接受用户输入并将消息发送给机器人。辅助会话每秒轮询一次机器人以检索来自机器人的任何消息，然后显示收到的消息。

> **注**：已按[文档中的最佳做法](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-builder-howto-direct-line?view=azure-bot-service-4.0&tabs=cscreatebot%2Ccsclientapp%2Ccsrunclient)修改了此处的说明和代码。

1. 如果尚未打开，请在 Visual Studio 中打开 **“PictureBot”** 解决方案

1. 在解决方案资源管理器中右键单击解决方案，然后选择 **“添加”>“新项目”**。

1. 搜索 **“控制台应用 (.NET Core)”**，选择它并选择 **“下一步”**

1. 在名称处输入 **“PictureBotDL”**

1. 选择 **“创建”**。

### 将 NuGet 包添加到 PictureBotDL

1. 右键单击 PictureBotDL 项目并选择 **“管理 NuGet 包”**。

1. 在 **“浏览”** 选项卡中，搜索并安装/更新以下内容：

- Microsoft.Bot.Connector.DirectLine
- Microsoft.Rest.ClientRuntime

1. 打开 **“Program.cs”**

1. 将 **“Program.cs”** （位于 PictureBotDL 中）的内容替换为以下内容：

```csharp
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using Activity = Microsoft.Bot.Connector.DirectLine.Activity;

namespace PictureBotDL
{
    class Program
    {
        // ************
        // 将以下值替换为 Direct Line 机密和机器人资源 ID 的名称。
        //*************
        private static string directLineSecret = "YourDLSecret";
        private static string botId = "YourBotServiceName";

        // 此操作给机器人用户命名。
        private static string fromUser = "PictureBotSampleUser";

        static void Main(string[] args)
        {
            StartBotConversation().Wait();
        }


        /// <summary>
        /// 促使用户与机器人进行对话。
        /// </summary>
        /// <returns></returns>
        private static async Task StartBotConversation()
        {
            // 新建一个 Direct Line 客户端。
            DirectLineClient client = new DirectLineClient(directLineSecret);

            // 开始对话。
            var conversation = await client.Conversations.StartConversationAsync();

            // 在单独的会话中启动机器人消息阅读器。
            new System.Threading.Thread(async () => await ReadBotMessagesAsync(client, conversation.ConversationId)).Start();

            // 提示用户开始与机器人对话。
            Console.Write("Conversation ID: " + conversation.ConversationId + Environment.NewLine);
            Console.Write("Type your message (or \"exit\" to end): ");

            // 循环，直到用户选择退出此循环。
            while (true)
            {
                // 接受用户的输入。
                string input = Console.ReadLine().Trim();

                // 查看用户是否要退出。
                if (input.ToLower() == "exit")
                {
                    // 如果用户请求退出，则退出应用。
                    break;
                }
                else
                {
                    if (input.Length > 0)
                    {
                        // 使用用户输入的文本创建消息活动。
                        Activity userMessage = new Activity
                        {
                            From = new ChannelAccount(fromUser),
                            Text = input,
                            Type = ActivityTypes.Message
                        };

                        // 将消息活动发送给机器人。
                        await client.Conversations.PostActivityAsync(conversation.ConversationId, userMessage);
                    }
                }
            }
        }


        /// <summary>
        /// 连续轮询机器人并检索机器人发送给客户端的消息。
        /// </summary>
        /// <param name="client">Direct Line 客户端。</param>
        /// <param name="conversationId">对话 ID。</param>
        /// <returns></returns>
        private static async Task ReadBotMessagesAsync(DirectLineClient client, string conversationId)
        {
            string watermark = null;

            // 每秒轮询一次机器人，看看是否有回答。
            while (true)
            {
                // 检索来自机器人的活动集。
                var activitySet = await client.Conversations.GetActivitiesAsync(conversationId, watermark);
                watermark = activitySet?.Watermark;

                // 提取机器人发送的活动。
                var activities = from x in activitySet.Activities
                                 where x.From.Id == botId
                                 select x;

                // 分析活动集中的每个活动。
                foreach (Activity activity in activities)
                {
                    // 显示活动的文本。
                    Console.WriteLine(activity.Text);

                    // 是否有附件？
                    if (activity.Attachments != null)
                    {
                        // 提取活动中的每个附件。
                        foreach (Attachment attachment in activity.Attachments)
                        {
                            switch (attachment.ContentType)
                            {
                                // 显示一张英雄卡。
                                case "application/vnd.microsoft.card.hero":
                                    RenderHeroCard(attachment);
                                    break;

                                // 在浏览器中显示图像。
                                case "image/png":
                                    Console.WriteLine($"Opening the requested image '{attachment.ContentUrl}'");
                                    Process.Start(attachment.ContentUrl);
                                    break;
                            }
                        }
                    }

                }

                // 等待一秒钟，然后再次轮询机器人。
                await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
            }
        }


        /// <summary>
        /// 在控制台上显示英雄卡。
        /// </summary>
        /// <param name="attachment">包含英雄卡的附件。</param>
        private static void RenderHeroCard(Attachment attachment)
        {
            const int Width = 70;
            // 用于使字符串在星号之间居中的函数。
            Func<string, string> contentLine = (content) => string.Format($"{{0, -{Width}}}", string.Format("{0," + ((Width + content.Length) / 2).ToString() + "}", content));

            // 提取英雄卡数据。
            var heroCard = JsonConvert.DeserializeObject<HeroCard>(attachment.Content.ToString());

            // 显示英雄卡。
            if (heroCard != null)
            {
                Console.WriteLine("/{0}", new string('*', Width + 1));
                Console.WriteLine("*{0}*", contentLine(heroCard.Title));
                Console.WriteLine("*{0}*", new string(' ', Width));
                Console.WriteLine("*{0}*", contentLine(heroCard.Text));
                Console.WriteLine("{0}/", new string('*', Width + 1));
            }
        }
    }
}
```

> **注**：此代码根据[文档](https://docs.microsoft.com/zh-cn/azure/bot-service/bot-builder-howto-direct-line?view=azure-bot-service-4.0&tabs=cscreatebot%2Ccsclientapp%2Ccsrunclient#create-the-console-client-app)稍作了修改，以便包含我们在接下来的实验部分中将使用的一些内容。

1. 在 **“Program.cs”** 中，使用自己的特定值更新 Direct Line 机密和机器人 ID。

花些时间查看这段示例代码。这对确保了解如何连接 PictureBot 并获得响应非常有益。

### 运行应用

1. 右键单击 **“PictureBotDL”** 项目并选择 **“设置为启动项目”**。

1. 接着按 **F5**以运行应用。

1. 使用命令行应用程序与机器人进行对话

![控制台应用](../images//consoleapp.png)

> **注**：如果没有得到回应，则机器人可能出错。使用 Bot Emulator 在本地测试机器人，修复所有问题，然后再次发布。

急智测验 - 如何显示对话 ID？我们将在后面的部分中了解需要这样做的原因。

## 实验 9.4：使用 HTTP Get 检索消息

我们拥有对话 ID，所以可以使用 HTTP Get 检索用户和机器人的消息。如果熟练 Rest 客户端且经验丰富，请随意使用自己偏好的工具。

在本实验中，我们将演练如何使用 Postman（基于 Web 的客户端）来检索消息。

### 下载 Postman

1. [下载适用于自己的平台的本机应用](https://www.getpostman.com/apps)。你可能需要创建一个简单的帐户。

### 测试应用

使用 Direct Line API，客户端可以通过发出 HTTP Post 请求向机器人发送消息。还可以通过 WebSocket 流或发出 HTTP GET 请求来接收来自机器人的消息。在本实验中，我们将了解如何使用 HTTP Get 选项接收消息。

我们发出 GET 请求。还需要确保标头包含标头名称（**授权**）和标头值（持有者 **YourDirectLineSecret**）。另外，我们将通过在请求中将 {conversationId} 替换为当前对话 ID，调用控制台应用中的现有对话：`https://directline.botframework.com/api/conversations/{conversationId}/messages`。

使用 Postman，配置相当轻松：

- 使用下拉列表将请求更改为“GET”类型。
- 在请求 URL 中输入对话 ID。
- 将授权更改为“持有者令牌”类型并在“令牌”框中输入 Direct Line 机密。

![持有者令牌](../images//bearer.png)

1. 打开 **“Postman”**

1. 请务必在类型处选择 **“GET”**

1. 在 URL 处输入 **“https://directline.botframework.com/api/conversations/{conversationId}/messages”**。  请务必将其中的 converstationId 替换为你的特定对话 ID

1. 选择 **“授权”**，在类型处选择 **“持有者令牌”**

1. 将值设置为 **“{你的 Direct Line 机密}”**

1. 最后，选择 **“发送”**。

1. 检查结果。

1. 新建与控制台应用的对话，务必搜索图片。

1. 使用新的对话 ID 通过 Postman 创建新请求。

1. 检查返回的响应。  应该能看到图像 URL 显示在响应的图像数组中。

![图像数组示例](../images//imagesarray.png)

## 延伸阅读

有额外的时间吗？能否从终端利用 curl（下载链接：https://curl.haxx.se/download.html）来检索对话（就像对 Postman 执行的操作那样）？

> 提示：命令可能类似于 `curl -H "Authorization:Bearer {SecretKey}" https://directline.botframework.com/api/conversations/{conversationId}/messages -XGET`

## 资源

- [Direct Line API](https://docs.microsoft.com/zh-cn/bot-framework/rest-api/bot-framework-rest-direct-line-3-0-concepts)