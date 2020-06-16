# lab02.3-luis_and_search - 使用 LUIS 和 Azure 认知搜索开发智能应用程序

这个动手实验向你介绍如何使用 Microsoft Bot Framework、Azure 认知搜索和多种认知服务从端到端创建智能机器人。 

在本研讨会中，你将学习以下内容：
- 了解如何将智能服务集成到应用程序中
- 了解如何实现 Azure 认知搜索功能，从而在应用程序内部提供积极的搜索体验
- 配置 Azure 认知搜索服务以扩展数据来启用全文、语言感知搜索
- 生成、训练和发布 LUIS 模型以帮助机器人有效沟通
- 使用利用 LUIS 和 Azure 认知搜索的 Microsoft Bot Framework 生成智能机器人
- 在 .NET 应用程序中调用各种认知服务 API（特别是计算机视觉、人脸、情感和 LUIS）

虽然重点在于 LUIS 和 Azure 认知搜索，但你还将利用以下技术：

- 计算机视觉 API
- 人脸 API
- 情感 API
- Data Science Virtual Machine (DSVM)
- Windows 10 SDK (UWP)
- CosmosDB
- Azure 存储
- Visual Studio

## 先决条件：

本研讨会面向 Azure 的 AI 开发人员。由于这是一个为期半天的研讨会，在开始前需要具备一些条件。

第一，你应该拥有使用 Visual Studio 的经验。我们会将它用于我们在研讨会中构建的所有内容，因此你应熟悉[如何使用它](https://docs.microsoft.com/zh-cn/visualstudio/ide/visual-studio-ide)来创建应用程序。此外，这不是一门教授如何编写代码或开发应用程序的课程。我们假设你知道如何使用 C# 编写代码（可以在[此处](https://mva.microsoft.com/zh-cn/training-courses/c-fundamentals-for-absolute-beginners-16169?l=Lvld4EQIC_2706218949)学习如何编写代码），但是不知道如何实现高级搜索和 NLP（自然语言处理）解决方案。 

第二，你应该拥有一些使用 Microsoft Bot Framework 开发机器人的经验。我们不会花太多时间讨论如何设计它们或对话的工作原理。如果你不熟悉 Bot Framework，则应该在参与研讨会之前参加[此 Microsoft 虚拟学院课程](https://mva.microsoft.com/zh-cn/training-courses/creating-bots-in-the-microsoft-bot-framework-using-c-17590#!)。

第三，你应该拥有使用门户的经验，并且能够在 Azure 上创建资源（和花钱）。我们不会为本研讨会提供 Azure Pass。

## 简介

我们将构建一个端到端的场景，使你能够引入自己的图片、使用认知服务查找图像中的对象和人物、了解那些人的感情以及将所有数据存储到 NoSQL 存储 (DocumentDB)。我们将使用该 NoSQL 存储来填充 Azure 认知搜索索引，然后使用 LUIS 生成一个 Bot Framework 机器人以进行简单、有针对性的查询。

## 体系结构

我们构建了一个简单的 C# 应用程序，使你能够从本地驱动器中引入图片，然后调用数种不同认知服务来收集这些图片上的数据：

- [计算机视觉](https://www.microsoft.com/cognitive-services/zh-cn/computer-vision-api)：用于获取标签和描述
- [人脸](https://www.microsoft.com/cognitive-services/zh-cn/face-api)：用于从每个图像中抓取人脸及其细节
- [情感](https://www.microsoft.com/cognitive-services/zh-cn/emotion-api)：用于提取图像中每张人脸的情绪分数

获得这些数据后，我们会对其进行处理，以提取所需详细信息，并将其存储到 [DocumentDB](https://azure.microsoft.com/zh-cn/services/documentdb/)（我们的 [NoSQL](https://en.wikipedia.org/wiki/NoSQL) [PaaS](https://azure.microsoft.com/zh-cn/overview/what-is-paas/) 产品/服务）中。

数据存储在 DocumentDB 中后，我们将在其基础上构建一个 [Azure 认知搜索](https://azure.microsoft.com/zh-cn/services/search/)索引（Azure 认知搜索是我们用于分面、容错搜索的 PaaS 产品/服务，请考虑没有管理开销的弹性搜索）。我们将向你演示如何查询数据，然后构建一个 [Bot Framework](https://dev.botframework.com/) 机器人来查询数据。最后，我们将通过 [LUIS](https://www.microsoft.com/cognitive-services/zh-cn/language-understanding-intelligent-service-luis) 扩展这个机器人，以自动从查询中获取意图，并使用这些信息智能地指导搜索。 

![体系结构图](./resources/assets/AI_Immersion_Arch.png)


## 导航 GitHub ##

[资源](./resources)文件夹中包含几个目录：

- **assets**：这包含实验手册的所有图像。可以忽略此文件夹。
- **code**：其中包含我们将使用的几个目录：
	- **ImageProcessing**：这一解决方案 (.sln) 包含研讨会第一部分的多个不同项目，让我们总体来看一下：
		- **ImageProcessingLibrary**：这是一个可移植类库 (PCL)，包含用于访问与视觉相关的各种认知服务的帮助程序类，以及一些用于封装结果的“Insights”类。
		- **ImageStorageLibrary**：由于 Cosmos Db（目前）不支持 UWP，因此这是一个用于访问 Blob 存储和 Cosmos DB 的非可移植库。
		- **TestApp**：一种 UWP 应用程序，使你能够加载图像并调用各种认知服务对其进行处理，然后研究结果。适用于图像的实验和探索。
		- **TestCLI**：一种控制台应用程序，使你能够调用各种认知服务，然后将图像和数据上传到 Azure。图像会上传到 Blob 存储，而各种元数据（标签、标题、人脸）则上传到 Cosmos DB。

		TestApp 和 TestCLI 都包含一个 `settings.json` 文件，其中包含访问认知服务和 Azure 所需的各种密钥和终结点____。它们一开始为空白，因此在预配资源后，我们将获取你的服务密钥并设置你的存储帐户和 Cosmos DB 实例。
		
	- **LUIS**：在这里，你将找到 PictureBot 的 LUIS 模型。你将创建自己的模型，但如果未跟上进度或希望测试另外的 LUIS 模型，则可以使用 .json 文件导入此 LUIS 应用。
	- **模型**：在我们将搜索添加到 PictureBot 时会用到这些类。
	- **PictureBot**：此处有 PictureBot.sln，用于研讨会后面的部分，到时我们会将 LUIS 和搜索索引集成到 Bot Framework 中


### 实验：设置 Azure 帐户

你可以在以下位置激活 Azure 免费试用版：[https://azure.microsoft.com/zh-cn/free/](https://azure.microsoft.com/zh-cn/free/)。  

如果你已获得用于完成这个实验的 Azure Pass，则可访问 [http://www.microsoftazurepass.com/](http://www.microsoftazurepass.com/) 激活它。  请按照 [https://www.microsoftazurepass.com/howto](https://www.microsoftazurepass.com/howto) 上的说明进行操作，其中记录了激活过程。  Microsoft 帐户可能具有 Azure 的**一个免费试**用版以及与之关联的 Azure Pass，因此如果你已在 Microsoft 帐户上激活 Azure Pass，则需要使用免费试用版或使用其他 Microsoft 帐户。

### 实验：设置 Data Science Virtual Machine

创建 Azure 帐户后，你可以访问 [Azure 门户](https://portal.azure.com)。在门户中为此实验创建资源组。有关 Data Science Virtual Machine 的详细信息可[在线查找](https://docs.microsoft.com/zh-cn/azure/machine-learning/data-science-virtual-machine/overview)，不过我们将只讨论本次研讨会所需的内容。在资源组中，部署并连接到 Data Science Virtual Machine for Windows (2016)，大小为 DS3_V2（所有其他默认值均可保留）。 

连接后，你需要执行几项操作来为研讨会设置 DSVM：

1. 在 Firefox 中导航到此存储库，并以 zip 文件的形式下载。提取所有文件，然后将此实验的文件夹移动到桌面。
2. 打开“资源”>“代码”>“ImageProcessing”下的“ImageProcessing.sln”。首次打开 Visual Studio 可能需要一段时间，并且必须登录。
3. 打开后，系统将提示你安装适用于 Windows 10 应用开发 (UWP) 的 SDK。请按照提示进行安装。如果未收到提示，请右键单击 TestApp 并选择“重新加载项目”，然后将出现提示。
4. 在安装过程中，可以完成以下几个任务： 
	- 在 Cortana 搜索栏中输入“针对开发人员设置”，并将设置更改为“开发人员模式”。
	- 在 Cortana 搜索栏键入“gpedit.msc”并按 Enter。启用以下策略：“计算机配置”>“Windows 设置”>“安全设置”>“本地策略”>“安全选项”>“用户帐户控制”：内置管理员帐户的管理员批准模式
	- 开始“收集密钥”实验。 
5. 安装完成并已更改开发人员设置和用户帐户控制策略后，请重启 DSVM。 
> **注意：** 请务必在研讨会结束后关闭 DSVM，以免收取费用。


### 实验：收集密钥

在进行这个实验的过程中，我们将收集认知服务密钥和存储密钥。你应将所有内容保存在文本文件中，以便在将来的实验中轻松访问它们。

>_认知服务密钥_
>- 计算机视觉 API：
>- 人脸 API：
>- 情感 API：
>- LUIS API：

>_存储密钥_
>- Azure Blob 存储连接字符串：
>- Cosmos DB URI：
>- Cosmos DB 密钥：

**获取认知服务 API 密钥**

在门户中，我们将首先为要使用的认知服务创建密钥。我们主要在[计算机视觉](https://www.microsoft.com/cognitive-services/zh-cn/computer-vision-api)认知服务下使用不同 API，因此我们先为此创建一个 API 密钥。

在门户中，点击 **“新建”**，然后在搜索框中输入 **“认知”** 并选择 **“认知服务”**：

![创建认知服务密钥](./resources/assets/new-cognitive-services.PNG)

这将引导你填写将要创建的 API 终结点的一些详细信息，选择感兴趣的 API 和希望终结点驻留的位置，以及想要的定价方案。我们将使用 S1，这样我们就可以获得这个教程所需的吞吐量，同时还将创建一个新的资源组__。我们将在下面为 Blob 存储和 Cosmos DB 使用相同的资源组。固定到仪表板__，这样你就可以轻松找到它。由于计算机视觉 API 在 Microsoft 内部存储图像（以一种安全的方式存储），为了帮助改进未来的认知服务视觉产品，需要启用帐户创建__。对于企业环境中的用户而言，这可能是一个绊脚石，因为只有订阅管理员才有权启用此功能，但对于 Azure Pass 用户来说，这不是问题。

![选择认知服务详细信息](./resources/assets/cognitive-account-creation.PNG) 

创建新的 API 订阅后，可以从边栏选项卡的相应部分获取密钥并将其添加到 TestApp 和 TestCLI 的 `settings.json` 文件中____。

![认知 API 密钥](./resources/assets/cognitive-keys.PNG)

我们还将在计算机视觉系列中使用其他 API，请利用这个机会为情感 API 和人脸 API 创建 API 密钥____。它们的创建方式与上面的相同，并且应该重复使用你创建的同一资源组。固定到仪表板__，然后将这些密钥添加到 `settings.json` 文件中。

由于我们将在本教程的后面使用 [LUIS](https://www.microsoft.com/cognitive-services/zh-cn/language-understanding-intelligent-service-luis)，因此，我们也利用这个机会在此创建我们的 LUIS 订阅。其创建方式与上面的方式完全相同，但从 API 下拉列表中选择语言理解智能服务，并重复使用你在前面创建的同一资源组。再一次固定到仪表板__，这样在我们进入教程的那一阶段后，你会发现很轻松就可以获得访问权限。  

**设置存储**

我们将在 Azure 中为此项目使用两个不同的存储，一个用于存储原始图像，另一个用于存储认知服务调用的结果。Azure Blob 存储用于以类似于文件系统的格式存储大量数据，是存储图像等数据的不错之选。Azure Cosmos DB 是我们的可复原 NoSQL PaaS 解决方案，对于存储结构松散的数据（如我们的图像元数据结果中含有的数据）非常有用。还有其他选择（Azure 表存储、SQL Server），但 Cosmos DB 使我们可以灵活地自由发展架构（比如为新服务添加数据）、轻松查询，并且可以快速集成到 Azure 认知搜索。

_Azure Blob 存储_

详细的“入门”说明可以[在线查找](https://docs.microsoft.com/zh-cn/azure/storage/storage-dotnet-how-to-use-blobs)，不过我们仅仅探讨这个实验需要什么。

在 Azure 门户中，单击 **“新建”->“存储”->“存储帐户”**

![新建 Azure 存储](./resources/assets/create-blob-storage.PNG)

单击后，你将看到上面需要填写的字段。选择存储帐户名称（小写字母和数字），将“帐户种类”设置为“Blob 存储”，将“复制”设置为“本地冗余存储(LRS)”（这只是为了节省资金），使用与上面相同的资源组，并将“位置”设置为“美国西部”____________。  （各区域中可用的 Azure 服务列表位于 https://azure.microsoft.com/zh-cn/regions/services/）。固定到仪表板__，这样你就可以轻松找到它。

现在，你已具有 Azure 存储帐户，让我们获取“连接字符串”__并将其添加到 TestCLI__和 TestApp__ `settings.json` 中。

![Azure Blob 密钥](./resources/assets/blob-storage-keys.PNG)

_Cosmos DB_

详细的“入门”说明可以[在线查找](https://docs.microsoft.com/zh-cn/azure/documentdb/documentdb-get-started)，不过我们将在此处介绍此项目所需的内容。

在 Azure 门户中，单击 **“新建”->“数据库”->“Azure Cosmos DB”**。

![新建 Cosmos DB](./resources/assets/create-cosmosdb-portal.png)

单击后，必须根据需要填写几个字段。 

![Cosmos DB 创建表单](./resources/assets/create-cosmosdb-formfill.png)

在我们的示例中，请按照限制来选择需要的 ID（需要使用小写字母、数字或破折号）。我们将使用文档 DB SDK 而不是 Mongo，因此请选择文档 DB 作为 NoSQL API。让我们使用与前面步骤相同的资源组和相同的位置，选择“固定到仪表板”__，确保我们可进行跟踪并能够轻松返回，然后单击“创建”。

创建完成后，打开新数据库的面板，然后选择“密钥”__子面板。

![Cosmos DB 的密钥子面板](./resources/assets/docdb-keys.png)

你需要为 _TestCLI_ 的 `settings.json` 文件提供 **URI** 和**主键**，将其复制到该处，而你随即便可以将图像和数据存储到云中。


## 认知服务

本研讨会的重点不是所有认知服务 API。我们将专注于 Azure 认知搜索和 LUIS。但是，如果有兴趣在认知服务方面进行更多实践，建议查看 -TODO：添加 reference lab link-Lab 1，其中你将使用多种认知服务构建一个智能 Kiosk。 

出于本次研讨会的目的，我们将使用计算机视觉 API 来理解图像（通过标签和描述），使用人脸 API 跟踪我们上传的图像中的所有人脸，以及使用情感 API 获取图像中人们所呈现出情感的分数。我们将仅需调用 API 来获取该信息。我们不需要进行任何训练或测试。 

结果信息将包括标签、描述和性别/年龄/情感（如果是人的话）。完成以下实验后，请查看创建的 ImageInsights.json 文件，了解此信息的结构。

稍后，我们还会花一些时间开发 LUIS 模型，以便我们的机器人能够更好地理解语言。对于此服务，我们将在训练和发布模型前向其添加意图、实体、言语等。只有这样我们才能从 API 调用中对其进行访问。

### 实验：探索认知服务和图像处理库

当你打开 `ImageProcessing.sln` 解决方案时，你会发现一个 UWP 应用程序 (`TestApp`)，使你能够加载图像并调用各种认知服务对其进行处理，然后研究结果。适用于图像的实验和探索。此应用在 `ImageProcessingLibrary` 项目的基础上生成，TestCLI 项目也使用它来分析图像。 

你需要“构建”解决方案（右键单击 `ImageProcessing.sln` 并选择“构建”）。你也可能需要重新加载 TestApp 项目，方法是右键单击它并选择“重新加载项目”。 

在运行应用前，请确保在 `TestApp` 项目下的 `settings.json` 文件中输入认知服务 API 密钥。执行该操作后，请运行应用，将其指向含有图像的任何文件夹（需要先解压 `sample_images`）（通过`选择文件夹`按钮），该应用应生成如下结果，显示其处理的所有图像，以及分解出的一些独特人脸、情绪和标签，它们还会用作图像集合的筛选条件。

![UWP 测试应用](./resources/assets/UWPTestApp.JPG)

应用处理指定目录后，就会将结果缓存在同一文件夹中的 `ImageInsights.json` 文件中，这样你无需调用各种 API 就可以再次查看该文件夹结果。 

## 浏览 Cosmos DB

Cosmos DB 不是本次研讨会的重点，但如果你对将要进行的操作感兴趣，这里有一些我们将使用的代码的重点：
- 导航到 `ImageStorageLibrary` 中的 `DocumentDBHelper.cs` 类。我们要使用的许多实现都可在[入门指南](https://docs.microsoft.com/zh-cn/azure/documentdb/documentdb-get-started)中找到。
- 转到 `TestCLI` 的 `Util.cs` 并查看 `ImageMetadata` 类。在这里，我们会将从认知服务中检索的 `ImageInsights` 转换为要存储到 Cosmos DB 的相应元数据。
- 最后，查看 `Program.cs` 并注意 `ProcessDirectoryAsync`。首先，我们要检查是否已上传图像和元数据 - 可以使用 `DocumentDBHelper` 按 ID 查找文档，如果文档不存在，则返回 `null`。接下来，如果我们设置了 `forceUpdate` 或之前未处理图像，则将使用 `ImageProcessingLibrary` 中的 `ImageProcessor` 调用认知服务，并检索添加到当前 `ImageMetadata` 的 `ImageInsights`。 
- 完成所有这些操作后，我们首先可以使用 `BlobStorageHelper` 实例将实际图像存储到 Blob 存储中，然后使用 `DocumentDBHelper` 实例将 `ImageMetadata` 存储到 Cosmos DB 中。如果文档已经存在（基于我们上一次检查），则应更新现有文档。如果不存在，我们应该创建一个新文档。

### 实验：使用 TestCLI 加载图像

我们将实现主要的处理和存储代码作为命令行/控制台应用程序，因为这使你可以专注于处理代码，而无需担心事件循环、窗体或任何其他用户体验相关干扰。之后可随意添加自己的用户体验。

设置认知服务 API 密钥、Azure Blob 存储连接字符串以及 Cosmos DB 终结点 URI 和 TestCLI__ 的 `settings.json` 中的密钥后，即可运行 TestCLI__。

运行 TestCLI__，然后打开命令提示符并导航到 ImageProcessing\TestCLI 文件夹（提示：使用“cd”命令更改目录）。然后输入 `.\bin\Debug\TestCLI.exe`。应得到以下结果：

    > .\bin\Debug\TestCLI.exe

    Usage:  [options]

    Options:
    -force            Use to force update even if file has already been added.
    -settings         The settings file (optional, will use embedded resource settings.json if not set)
    -process          The directory to process
    -query            The query to run
    -? | -h | --help  Show help information

默认情况下，它会从 `settings.json` 中加载你的设置（它会将其构建到 `.exe`），但你可以使用 `-settings` 标记提供你自己的设置。要将图像（及其来自认知服务的元数据）加载到你的云存储中，你只需要求 TestCLI__ `-process` 图像目录，如下所示：

    > .\bin\Debug\TestCLI.exe -process c:\my\image\directory

完成处理后，可直接使用 TestCLI__ 查询 Cosmos DB，如下所示：

    > .\bin\Debug\TestCLI.exe -query "select * from images"

## Azure 认知搜索

[Azure 认知搜索](https://docs.microsoft.com/zh-cn/azure/search/search-what-is-azure-search)是一种“搜索即服务”的解决方案，开发人员通过该方案可将出色的搜索体验集成到应用程序中，而无需管理基础设施或成为搜索专家。

开发人员在 Azure 中寻找 PaaS 服务，以便在其应用中以更快的速度获得更佳结果。搜索是许多应用程序类别的关键。Web 搜索引擎为搜索设置了很高的门槛；用户期望的是：即时结果，在键入时自动完成，突出显示搜索结果中的热门内容，良好的排名，并且即使他们拼写错误或包含额外字词，也要能够理解他们所寻找的内容。

搜索是一大困难而又很少属于核心专业知识的领域。从基础结构的角度来看，它需要具有高可用性、持久性、规模和操作。从功能角度来看，它需要具有排名、语言支持和地理空间功能。

![搜索要求示例](./resources/assets/AzureSearch-Example.png) 

上面的示例说明了用户在搜索体验中期望的一些组件。[Azure 认知搜索](https://docs.microsoft.com/zh-cn/azure/search/search-what-is-azure-search)可以实现这些用户体验功能，并提供[监视和报告](https://docs.microsoft.com/zh-cn/azure/search/search-traffic-analytics)、[简单评分](https://docs.microsoft.com/zh-cn/rest/api/searchservice/add-scoring-profiles-to-a-search-index)以及用于[原型](https://docs.microsoft.com/zh-cn/azure/search/search-import-data-portal)和[检验](https://docs.microsoft.com/zh-cn/azure/search/search-explorer)的工具。

常见工作流：
1. 预配服务
	- 你可以通过[门户](https://docs.microsoft.com/zh-cn/azure/search/search-create-service-portal)或 [PowerShell](https://docs.microsoft.com/zh-cn/azure/search/search-manage-powershell) 创建或预配 Azure 认知搜索服务。
2. 创建索引
	- [索引](https://docs.microsoft.com/zh-cn/azure/search/search-what-is-an-index)是数据的容器，比如“表”。它具有架构、[CORS 选项](https://docs.microsoft.com/zh-cn/aspnet/core/security/cors)、搜索选项。你可以在[门户](https://docs.microsoft.com/zh-cn/azure/search/search-create-index-portal)中或在[应用初始化](https://docs.microsoft.com/zh-cn/azure/search/search-create-index-dotnet)期间创建索引。 
3. 索引数据
	- 有两种方法可以[使用数据填充索引](https://docs.microsoft.com/zh-cn/azure/search/search-what-is-data-import)。第一个选项是使用 Azure 认知搜索 [REST API](https://docs.microsoft.com/zh-cn/azure/search/search-import-data-rest-api) 或 [.NET SDK](https://docs.microsoft.com/zh-cn/azure/search/search-import-data-dotnet) 手动将数据填充到索引中。第二个选项是将一个[支持的数据源](https://docs.microsoft.com/zh-cn/azure/search/search-indexer-overview)指向索引，并让 Azure 认知搜索自动按计划引入数据。
4. 搜索索引
	- 向 Azure 认知搜索提交搜索请求时，可以使用简单搜索选项，并可以对结果进行[筛选](https://docs.microsoft.com/zh-cn/azure/search/search-filters)、[排序](https://docs.microsoft.com/zh-cn/rest/api/searchservice/add-scoring-profiles-to-a-search-index)、[设计](https://docs.microsoft.com/zh-cn/azure/search/search-faceted-navigation)和[分页](https://docs.microsoft.com/zh-cn/azure/search/search-pagination-page-layout)。你能够解决拼写错误、语音和正则表达式的问题，并且可以选择使用搜索和[建议](https://docs.microsoft.com/zh-cn/rest/api/searchservice/suggesters)。通过这些查询参数，你能够对[全文搜索体验](https://docs.microsoft.com/zh-cn/azure/search/search-query-overview)实现更深层次的控制


### 实验：创建 Azure 认知搜索服务

在 Azure 门户中，单击 **“新建”>“Web + 移动”>“Azure 认知搜索”**。

单击后，必须根据需要填写几个字段。对于本实验，“免费”层就以足够。

![创建新的 Azure 认知搜索服务](./resources/assets/AzureSearch-CreateSearchService.png)

创建完成后，打开新搜索服务的面板。

### 实验：创建 Azure 认知搜索索引

索引是数据的容器，与 SQL Server 表的概念类似。  正如表含有行，索引也有文档。  正如表含有字段，索引也有字段。  这些字段具有一些属性，这些属性告诉我们这是否为全文可搜索，或者是否可进行筛选。  可以通过编程方式[推送内容](https://docs.microsoft.com/zh-cn/rest/api/searchservice/addupdate-or-delete-documents)或使用 [Azure 认知搜索索引器](https://docs.microsoft.com/zh-cn/azure/search/search-indexer-overview)（可抓取常见数据存储）将内容填充到 Azure 认知搜索中。

对于本实验，我们将使用[用于 Cosmos DB 的 Azure 认知搜索索引器](https://docs.microsoft.com/zh-cn/azure/search/search-howto-index-documentdb)来抓取 Cosmos DB 容器中的数据。 

![导入向导](./resources/assets/AzureSearch-ImportData.png) 

在刚刚创建的 Azure 认知搜索边栏选项卡中，单击 **“导入数据”>“数据源”>“文档 DB”**。

![DocDB 导入向导](./resources/assets/AzureSearch-DataSource.png) 

单击后，为 Cosmos DB 数据源选择名称，然后选择数据所在的 Cosmos DB 帐户以及相应的容器和集合。  

单击 **“确定”**。

此时，Azure 认知搜索将连接到你的 Cosmos DB 容器并分析一些文档，以确定 Azure 认知搜索索引的默认架构。  完成此操作后，你可以根据应用程序的需要设置字段的属性。

将索引名称更新为：**图像**

将密钥更新为：**id**（唯一标识每个文档）

将所有字段设置为 **“可检索”** （以便客户端使在搜索时检索这些字段）

将 **“标签”、“NumFaces”和“人脸”** 字段设置为 **“可筛选”** （以便使客户端根据这些值筛选结果）

将 **“NumFaces”字段** 设置为 **“可排序”**（以便使客户端根据图像中的人脸数对结果进行排序）

将 **“标签”、“NumFaces”和“人脸”** 字段设置为 **“可分组”** （以便使客户端根据计数对结果进行分组，例如，在搜索结果中，有 5 张带有“沙滩”标签的图片）

将 **“标题”、“标签”和“人脸”字段** 设置为 **“可搜索”**（以便使客户端对这些字段中的文本进行全文搜索）

![配置 Azure 认知搜索索引](./resources/assets/AzureSearch-ConfigureIndex.png) 

此时，我们将配置 Azure 认知搜索分析器。  在较高的层次上，你可以将分析器视为采用用户输入的术语并在索引中查找最佳匹配术语的工具。  Azure 认知搜索包括在必应和 Office 等技术中使用的分析器，这些技术对 56 种语言有深入的了解。  

单击 **“分析器”** 选项卡并设置 **“标题”、“标签”和“人脸”** 字段，以使用 **“英语 - Microsoft”** 分析器

![语言分析器](./resources/assets/AzureSearch-Analyzer.png) 

对于索引配置的最后一步，我们将设置要用于提前键入的字段，使用户可键入单词的某些部分，Azure 认知搜索将在这些字段中查找最佳匹配

单击 **“建议器”** 选项卡，输入一个建议器名称：**sg** 并选择 **“标签”和“人脸”** 作为字段来查找术语建议

![搜索建议](./resources/assets/AzureSearch-Suggester.png) 

单击 **“确定”** 可完成索引器的配置。  可以按计划设置索引器检查更改的频率，但对于本实验，我们仅运行一次。  

单击 **“高级选项”** 并选择 **“Base 64 编码键”**，确保 ID 字段只使用 Azure 认知搜索密钥字段中支持的字符。

单击 **“确定”三次** 可启动索引器作业，该作业将开始从 Cosmos DB 数据库导入数据。

![配置索引器](./resources/assets/AzureSearch-ConfigureIndexer.png) 

**查询搜索索引**

你应该会看到一条消息，指示已开始编制索引。  如果要检查索引的状态，可以在主 Azure 认知搜索边栏选项卡中选择“索引”选项。

此时我们可以尝试搜索索引。  

单击 **“搜索资源管理器”** 并在生成的边栏选项卡中选择你的索引（如果尚未选择）。

单击 **“搜索”** 搜索所有文件。

![搜索资源管理器](./resources/assets/AzureSearch-SearchExplorer.png)

**提早完成？请尝试这个额外加分练习的实验：**

[Postman](https://www.getpostman.com/) 是一款不错的工具，可让你轻松执行 Azure 认知搜索 REST API 调用，并且它还是一款不错的调试工具。  可从 Azure 认知搜索资源管理器中获取任何查询，同时获取执行 Azure 认知搜索 API 密钥，以在 Postman 中执行。

下载 [Postman](https://www.getpostman.com/) 工具并安装。 

安装后，从 Azure 认知搜索资源管理器中获取查询并将其粘贴到 Postman 中，选择 GET 作为请求类型。  

单击“标头”并输入以下参数：

+ 内容类型：application/json
+ api-key：[在 Azure 认知搜索门户中的“密钥”部分下输入 API 密钥]

选择“发送”，你应该会看到数据的格式设置为 JSON 格式。

尝试使用[以下示例](https://docs.microsoft.com/zh-cn/rest/api/searchservice/search-documents#a-namebkmkexamplesa-examples)执行其他搜索。


## LUIS

首先，让我们[了解语言理解智能服务 (LUIS)](https://docs.microsoft.com/zh-cn/azure/cognitive-services/LUIS/Home)。

我们现已了解 LUIS 的含义，接着就要规划 LUIS 应用。我们将创建一个基于搜索返回图像的机器人，然后我们可以共享或订购这些图像。我们需要创建可触发机器人能够执行的不同操作的意图，然后创建实体来对执行该操作所需的一些参数建模。例如，PictureBot 的一个意图可能是“SearchPics”，该意图触发搜索服务来查找照片，这需要一个“facet”实体才能了解要搜索的内容。可在[此处](https://docs.microsoft.com/zh-cn/azure/cognitive-services/LUIS/plan-your-app)查看更多规划应用的示例。

构思好应用后，我们就可以[生成并对其进行训练](https://docs.microsoft.com/zh-cn/azure/cognitive-services/LUIS/luis-get-started-create-app)。以下是创建 LUIS 应用程序时通常要采取的步骤：
  1. [添加意图](https://docs.microsoft.com/zh-cn/azure/cognitive-services/LUIS/add-intents) 
  2. [添加言语](https://docs.microsoft.com/zh-cn/azure/cognitive-services/LUIS/add-example-utterances)
  3. [添加实体](https://docs.microsoft.com/zh-cn/azure/cognitive-services/LUIS/add-entities)
  4. [使用功能改进性能](https://docs.microsoft.com/zh-cn/azure/cognitive-services/LUIS/add-features)
  5. [训练和测试](https://docs.microsoft.com/zh-cn/azure/cognitive-services/LUIS/train-test)
  6. [使用主动学习](https://docs.microsoft.com/zh-cn/azure/cognitive-services/LUIS/label-suggested-utterances)
  7. [发布](https://docs.microsoft.com/zh-cn/azure/cognitive-services/LUIS/publishapp)



### 实验：使用 LUIS 为应用程序添加智能功能

在下一个实验中，我们将创建 PictureBot。首先，我们来了解一下如何使用 LUIS 添加一些自然语言功能。通过 LUIS，你可以将自然语言的言语映射到意图中。  就我们的应用程序而言，可能有几个意图，例如：查找图片、共享图片和订购图片打印件。  我们可以举几个示例言语作为询问这些内容的方法，并且 LUIS 将根据其学到的内容将其他新的言语映射到每个意图。  

导航到 [https://www.luis.ai](https://www.luis.ai) 并使用 Microsoft 帐户登录。  （此账户应与你在实验开始时用于创建认知服务密钥的帐户相同。）  你应该重定向到 [https://www.luis.ai/applications](https://www.luis.ai/applications) 上 LUIS 应用程序的列表。  我们将创建一个新 LUIS 应用来支持机器人。  

> 有趣的是：请注意，[当前页面](https://www.luis.ai/applications)上的“新应用”按钮旁边也有一个“导入应用”。  创建 LUIS 应用程序后，可以将整个应用导出为 JSON 并将其签入源代码管理。  这是建议的最佳做法，因此你可以在编写代码时对 LUIS 模型进行版本控制。  可以使用“导入应用”按钮重新导入导出的 LUIS 应用。  如果跟不上实验的进度并想走捷径，则可以单击“导入应用”按钮并导入 [LUIS 模型](./resources/code/LUIS/PictureBotLuisModel.json)。  

在 [https://www.luis.ai/applications](https://www.luis.ai/applications) 中单击“新建应用”按钮。  为其命名（我选择“PictureBotLuisModel”）并将区域性设置为“英语”。  你可以选择提供说明。  单击下拉菜单，选择要使用的终结点密钥，如果研讨会开始时在 Azure 门户上创建的 LUIS 密钥在菜单上，请选择该密钥（发布应用之前，此选项可能不会显示，因此如果没有，也不要担心）。  然后单击“创建”。  

![LUIS 新应用](./resources/assets/LuisNewApp.jpg) 

你将进入新应用的“仪表板”。  应用 ID 会显示；请记录下来，之后将作为 **LUIS 应用 ID**。  然后单击“创建意图”。  

![LUIS 仪表板](./resources/assets/LuisDashboard.jpg) 

我们希望机器人能够执行以下操作：
+ 搜索/查找图片
+ 在社交媒体上分享图片
+ 订购图片打印件
+ 问候用户（不过这也可以通过其他方式完成，我们稍后会了解这一点）

让我们为请求实现其中每一项的用户创建意图。  单击“添加意图”按钮。  

请将第一个意图命名为“问候语”，然后单击“保存”。  然后，提供几个用户在问候机器人时可能回应的言语示例，并在提供每个示例后按 Enter。  输入一些言语后，单击“保存”。  

![LUIS 问候语意图](./resources/assets/LuisGreetingIntent.jpg) 

现在我们来了解一下如何创建实体。  用户请求搜索图片时，他们可以指定要寻找的内容。  让我们在实体中捕获该内容。  

单击左侧列中的“实体”，然后单击“添加自定义实体”。  请将实体命名为“facet”并将实体类型命名为“简单”。  然后单击“保存”。  

![添加 Facet 实体](./resources/assets/AddFacetEntity.jpg) 

现在，单击左侧边栏中的“意图”，然后单击黄色的“添加意图”按钮。  请将意图命名为“SearchPics”，然后单击“保存”。  

现在，让我们添加一些示例言语（用户在与机器人交谈时可能会用到的字词/短语/句子）。  人们可能会通过多种方式搜索图片。  请随意使用下面的一些言语，并添加自己要求机器人搜索图片会使用的措辞。 

+ 查找户外照片
+ 是否有火车的图片？
+ 查找食物的图片。
+ 搜索一名 6 个月大的男孩的照片
+ 请提供 20 岁女性的图片
+ 请显示海滩的图片
+ 我想查找狗的照片
+ 搜索室内女性的照片
+ 显示看起来很开心的女孩的照片
+ 我想查找伤心女孩的照片
+ 请提供快乐的婴儿的照片

现在，我们必须让 LUIS 了解如何挑选搜索主题作为“facet”实体。  请将鼠标悬停在词上方并单击该词（或拖动选择一组字词），然后选择“facet”实体。  

![标注实体](./resources/assets/LabellingEntity.jpg) 

以下列出的言语…

![添加 Facet 实体](./resources/assets/SearchPicsIntentBefore.jpg) 

…在标记为 facet 时，可能会变成以下内容。  

![添加 Facet 实体](./resources/assets/SearchPicsIntentAfter.jpg) 

完成后，请不要忘记单击“保存”！  

最后，单击左侧边栏中的“意图”，并添加另外两个意图：
+ 将其中一个意图命名为 **“SharePic”**。  这可以通过诸如“分享这张照片”、“你能为此发推文吗？”或“发布到 Twitter”之类的言语来识别。  
+ 创建另一个名为 **“OrderPic”** 的意图。  这可以与“打印这张照片”、“我想订购打印件”，“我可以获得大小为 8x10 的照片吗？”以及“订购钱包”等言语进行沟通。  
选择言语时，使用问题、命令和“我想…”格式的组合会很有帮助。  

另请注意，有一种意图名为“None”。  不映射到任何意图的随机言语可能会映射到“None”。  欢迎你随意发挥，比如“你喜欢花生酱和果冻么？”

现在我们已准备好训练我们的模型。  单击左侧边栏中的“训练和测试”。  然后单击训练按钮。  这会生成用于执行言语的模型 --> 使用提供的训练数据映射意图。  

然后单击左侧边栏中的“发布应用”。  如果尚未执行此操作，请选择先前设置的终结点密钥，或者单击链接从 Azure 帐户创建一个新密钥。  可以将终结点槽保留为“生产”。  然后单击“发布”。  

![发布 LUIS 应用](./resources/assets/PublishLuisApp.jpg) 

发布会创建终结点，以调用 LUIS 模型。  URL 将会显示。  

单击左侧边栏中的“训练和测试”。  选中“启用已发布模型”框以使调用通过已发布的终结点完成，而不是直接调用模型。  尝试输入一些言语，并查看返回的意图。  

![测试 LUIS](./resources/assets/TestLuis.jpg) 

**提早完成？请尝试以下额外额度的任务：**


创建可由“SearchPics”意图利用的其他实体。例如，我们知道应用可确定年龄，请尝试为年龄创建预生成实体。 

使用实体类型为“列表”的自定义实体进行探索以捕获情感和性别。请参阅下面的情感示例。 

![带列表的自定义情感实体](./resources/assets/CustomEmotionEntityWithList.jpg) 

> **注意：** 添加更多实体或功能时，请不要忘记转到`"意图”>“言语”`，并使用添加的实体来确认/添加更多的言语。此外，还需要重新训练和发布模型。

## 生成机器人

我们假设你曾接触过 Bot Framework。如果是，那非常好。如果没有，不要太担心，你将在本节中了解到许多相关的内容。我们建议完成[此 Microsoft 虚拟学院课程](https://mva.microsoft.com/zh-cn/training-courses/creating-bots-in-the-microsoft-bot-framework-using-c-17590#!)并查看[文档](https://docs.microsoft.com/zh-cn/bot-framework/)。

### 实验：为机器人开发而设置

我们将使用 C# SDK 来开发机器人。  首先，你需要准备两项内容：
1. Bot Framework 项目模板，可在[此处](https://aka.ms/bf-bc-vstemplate)下载。  此文件名为“Bot Application.zip”，你应将它保存到 \Documents\Visual Studio 2019\Templates\ProjectTemplates\Visual C#\ 目录中。  只需将整个压缩文件放在其中即可；无需解压缩。  
2. 在[此处](https://emulator.botframework.com/)下载用于本地测试机器人的 Bot Framework Emulator。  此仿真器会安装到 `c:\Users\`_your-username_`\AppData\Local\botframework\app-3.5.27\botframework-emulator.exe`。 

### 实验：创建一个简单的机器人并运行它

在 Visual Studio 中，转到“文件”-->“新建项目”，并创建名为“PictureBot”的机器人应用程序。  

![新建机器人应用程序](./resources/assets/NewBotApplication.jpg) 

浏览并检查示例机器人代码，这是一个回音机器人，它会重复你的消息及其字符长度。  特别要**注意**的是
+ 在 App_Start 下的 **WebApiConfig.cs** 中，路由模板是 api/{controller}/{id}，其中 id 是可选的。  这就是我们在调用机器人的终结点时始终会在末尾追加 api/messages 的原因。  
+ Controllers 下的 **MessagesController.cs** 是机器人的入口点。请注意，机器人可以响应许多不同的活动类型，并且发送消息将调用 RootDialog。  
+ 在 Dialogs 下的 **RootDialog.cs** 中，“StartAsync”是等待用户消息的入口点，而“MessageReceivedAsync”是处理接收到的消息，然后等待进一步消息的方法。  我们可以使用“context.PostAsync”将来自机器人的消息发回给用户。  

右键单击解决方案，并选择 **“管理此解决方案的 NuGet 包”**。在已安装项下，搜索 Microsoft.Bot.Builder，并更新为最新版本。

单击 F5 以运行示例代码。  NuGet 应会负责下载相应的依赖项。  

代码将在默认的 Web 浏览器中通过类似于 http://localhost:3979/ 的 URL 启动。  

> 有趣的是：为什么是这个端口号？  这是在你的项目属性中设置的。  在“解决方案资源管理器”中，双击“属性”，然后选择“Web”选项卡。  在“服务器”部分设置项目 URL。  

![机器人项目 URL](./resources/assets/BotProjectUrl.jpg) 

确保你的项目仍在运行（如果你停下来查看过项目属性，请再次按 F5），并启动 Bot Framework Emulator。  （如果你刚安装完毕，它可能没有被编入索引，无法显示在本地计算机上的搜索中，因此，请记住它安装在 c:\Users\your-username\AppData\Local\botframework\app-3.5.27\botframework-emulator.exe 中。）  确保机器人 URL 与上面代码启动的端口号相匹配，并且末尾追加有 api/messages。  现在你应该可以和机器人交谈了。  

![机器人仿真器](./resources/assets/BotEmulator.png) 

### 实验：更新机器人以使用 LUIS

现在我们需要更新机器人才能使用 LUIS。  可以通过使用 [LuisDialog 类](https://docs.botframework.com/zh-cn/csharp/builder/sdkreference/d8/df9/class_microsoft_1_1_bot_1_1_builder_1_1_dialogs_1_1_luis_dialog.html)来进行更新。  

在 **RootDialog.cs** 文件中，添加对以下命名空间的引用：

```csharp

using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

```

然后，将 RootDialog 类更改为从 LuisDialog<object> 派生，而不是从 Idialog<object> 派生。  然后，使用 LUIS 应用 ID 和 LUIS 密钥向该类提供 LuisModel 属性。  （提示：LUIS 应用 ID 中将包含连字符，而 LUIS 密钥则不会。  如果找不到这些值，请返回 http://luis.ai。  单击你的应用程序，应用 ID 将在仪表板页面和 URL 中显示。  然后单击[顶部边栏中的“我的密钥”](https://www.luis.ai/home/keys)，在列表中找到终结点密钥。）  

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

现在，让我们运行代码。  点击 F5 在 Visual Studio 中运行，然后在 Bot Framework Emulator 中启动新对话。  尝试与机器人聊天，并确保获得预期的响应。  如果得到任何意外的结果，请记下它们，我们将修改 LUIS。  

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

现在，我们将更新机器人来调用 Azure 认知搜索。  首先，打开“工具”-->“NuGet 包管理器”-->“管理用于解决方案的 NuGet 包”。  在搜索框中，键入“Microsoft.Azure.Search”。  选择相应的库，选中指示项目的框，然后进行安装。  可能还会安装其他依赖项。在已安装的包下，你可能还需要更新“Newtonsoft.Json”包。

![Azure 认知搜索 NuGet](./resources/assets/AzureSearchNuGet.jpg) 

在 Visual Studio 的“解决方案资源管理器”中右键单击项目，然后选择“添加”-->“新建文件夹”。  创建一个名为“Models”的文件夹。  然后右键单击 Models 文件夹，并选择“添加”-->“现有项”。  执行此操作两次，将这两个文件添加到 Models 文件夹下（确保在必要时调整名称空间）：
1. [ImageMapper.cs](./resources/code/Models/ImageMapper.cs)
2. [SearchHit.cs](./resources/code/Models/SearchHit.cs)

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

### 实验：正则表达式和可得分组

我们可以进行许多操作来改进机器人。首先，我们可能不想只为了实现简单的问候“hi”而调用 LUIS，机器人将经常从它的用户那里收到此问候语。  一个简单的正则表达式可以满足此需求，并节省我们的时间（由于网络延迟）和费用（调用 LUIS 服务产生的费用）。  

此外，随着我们机器人复杂程度的加深，我们将接受用户的输入并使用多种服务来解释它，因此，我们需要一个过程来管理此流。  例如，首先尝试使用正则表达式，如果不匹配，则调用 LUIS，随后我们也可以继续尝试其他服务，例如 [QnA Maker](http://qnamaker.ai) 和 Azure 认知搜索。  管理此流的一个好方法是 [ScorableGroups](https://blog.botframework.com/2017/07/06/Scorables/)。  ScorableGroups 为你提供了一个属性，用于对这些服务调用强制执行排序操作。  在我们的代码中，让我们首先对正则表达式强制执行匹配顺序，然后调用 LUIS 来解释言语，最终，最低优先级为使用通用响应“I'm not sure what you mean”。    

若要使用 ScorableGroups，你的 RootDialog 需要从 DispatchDialog 而不是 LuisDialog 继承（但仍然可以在类上保留 LuisModel 属性）。  还需要一个对 Microsoft.Bot.Builder.Scorables（以及其他）的引用。  因此，请在 RootDialog.cs 文件中添加：

```csharp

using Microsoft.Bot.Builder.Scorables;
using System.Collections.Generic;

```

并将类派生更改为：

```csharp

    public class RootDialog : DispatchDialog

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

现在在 Scorable Group 1 中，如果没有正则表达式匹配，我们将调用 LUIS 作为第二次尝试。  

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

![将机器人发布到 Azure 应用服务](./resources/assets/PublishBotAzureAppService.jpg) 

在“应用服务”屏幕上，选择相应的订阅，然后单击“新建”。然后输入 API 应用名称、订阅、到目前为止使用的相同资源组以及应用服务计划。  

![创建应用服务](./resources/assets/CreateAppService.jpg) 

最后，你将看到 Web 部署设置，并可单击“发布”。  Visual Studio 中的输出窗口将显示部署过程。  然后，机器人将托管在一个类似于 http://testpicturebot.azurewebsites.net/ 的 URL 中，其中“testpicturebot”是应用服务 API 应用名称。  

### 实验：使用机器人连接器注册机器人

现在，请转到 Web 浏览器并导航到 [http://dev.botframework.com](http://dev.botframework.com)。  单击[注册机器人](https://dev.botframework.com/bots/new)。填写机器人的名称、句柄和描述。  消息终结点将为 Azure 网站 URL，其末尾附加了“api/messages”，如 https://testpicturebot.azurewebsites.net/api/messages。  

![机器人注册](./resources/assets/BotRegistration.jpg) 

然后单击按钮以创建 Microsoft 应用 ID 和密码。  这是你在 Web.config 中所需的机器人应用 ID 和密码。  将机器人应用名称、应用 ID 和应用密码存储在安全的地方！  设置密码时，一旦单击“确定”，将不能恢复。  然后单击“完成并返回 Bot Framework”。  

![机器人生成应用名称、ID 和密码](./resources/assets/BotGenerateAppInfo.jpg) 

在机器人注册页面上，你的应用 ID 应该已自动填写。你可以选择添加 AppInsights 检测密钥，以便从机器人进行日志记录。如果你同意服务条款，请选中此框，然后单击“注册”。  

然后，你将进入机器人的仪表板页，其 URL 类似于 https://dev.botframework.com/bots?id=TestPictureBot，但使用你自己的机器人名称。在这里，我们可以启用各种通道。  Skype 和 Web 聊天两个通道将自动启用。  

最后，你需要使用其注册信息更新你的机器人。  返回 Visual Studio 并打开 Web.config。  使用应用名称更新 BotId，使用应用 ID 更新 MicrosoftAppId，使用从机器人注册站点获得的应用密码更新 MicrosoftAppPassword。  

```xml

    <add key="BotId" value="TestPictureBot" />
    <add key="MicrosoftAppId" value="95b76ae6-8643-4d94-b8a1-916d9f753ab0" />
    <add key="MicrosoftAppPassword" value="kC200000000000000000000" />

```

重新构建项目，然后在“解决方案资源管理器”中右键单击该项目，再次选择“发布”。  上次的设置应该已有记录，因此只需点击“发布”即可。 

> 出现了指向 MicrosoftAppPassword 的错误？由于该文件为 XML 格式，因此，如果密钥包含“&”、“<”、“>”、“'”或“"”，则需要将它们替换为各自的[转义字符](https://en.wikipedia.org/wiki/XML#Characters_and_escaping)：“&amp;”、“&lt”、“&gt;”、“&apos;”、“&quot;”。 

现可导航回机器人的仪表板（类似于 https://dev.botframework.com/bots?id=TestPictureBot）。  尝试在聊天窗口中与它交谈。  Web 聊天中的传送可能与仿真器看起来不同。  https://docs.botframework.com/en- us/channelinspector/channels/skype/# navtitle 中有一个很好用的工具，名为 Channel Inspector，可以使用它来查看不同通道中不同控件的用户体验。  
你可以在机器人的仪表板中可以添加其他通道，并且可在 Skype、Facebook Messenger 或 Slack 中试用你的机器人。  只需单击机器人仪表板上频道名称右侧的“添加”按钮，然后按照说明操作即可。

**提早完成？请尝试这个额外加分练习的实验：**

尝试使用更高级的 Azure 认知搜索查询。通过扩展 LUIS 模型来识别“查找快乐的人”__之类的实体，将“快乐的”映射到“快乐”（认知服务返回的情感），并使用[术语提升](https://docs.microsoft.com/zh-cn/rest/api/searchservice/Lucene-query-syntax-in-Azure-Search#bkmk_termboost)将这些实体转换为提升的查询，从而添加术语提升。 

## 实验完成

在本实验室中，我们介绍了如何使用 Microsoft Bot Framework、Azure 认知搜索和多种认知服务从端到端创建智能机器人。

你应该已了解以下内容：
- 如何将智能服务集成到应用程序中
- 如何实现 Azure 认知搜索功能以在应用程序内提供积极的搜索体验
- 如何配置 Azure 认知搜索服务以扩展数据来启用全文、语言感知搜索
- 如何生成、训练和发布 LUIS 模型以帮助机器人有效沟通
- 如何通过利用 LUIS 和 Azure 认知搜索的 Microsoft Bot Framework 构建智能机器人
- 如何在 .NET 应用程序中调用各种认知服务 API（特别是计算机视觉、人脸、情感和 LUIS）

未来项目/学习的资源：
- [Azure 机器人服务文档](https://docs.microsoft.com/zh-cn/bot-framework/)
- [Azure 认知搜索文档](https://docs.microsoft.com/zh-cn/azure/search/search-what-is-azure-search)
- [Azure Bot Builder 示例](https://github.com/Microsoft/BotBuilder-Samples)
- [Azure 认知搜索示例](https://github.com/Azure-Samples/search-dotnet-getting-started)
- [LUIS 文档](https://docs.microsoft.com/zh-cn/azure/cognitive-services/LUIS/Home)
- [LUIS 示例](https://github.com/Microsoft/BotBuilder-Samples/blob/master/CSharp/intelligence-LUIS/README.md)