﻿# AI-100-DesignImplementAzureAISol
AI100T01A ILT 课程的实验文件


## 实验 1：满足技术要求

在本实验中，我们将介绍我们的研讨会案例研究以及你的本地工作站和 Azure 实例中的设置工具，以便你可以在 Microsoft 认知服务套件中生成工具。

## 实验 2：实现机器人的计算机视觉功能

本动手实验将指导你使用认知服务（尤其是计算机视觉 API）从端到端创建智能控制台应用程序。我们使用 ImageProcessing 可移植类库 (PCL)，讨论其内容以及如何在你自己的应用程序中使用它。

## 实验 3：基本筛选机器人

在本实验中，我们将设置一个可响应用户聊天窗口文本提示的端到端智能机器人。我们将根据已了解到的关于在 Azure 中生成机器人的内容进行生成，同时会添加一层自定义逻辑以赋予机器人更多定制的功能。

这个机器人将在 Microsoft Bot Framework 中生成。我们将调用使机器人接口能够接收和响应文本消息的体系结构，并生成使机器人能够响应包含特定文本的查询的逻辑。

此外，我们还将在机器人模拟器中测试机器人，并处理中间件，此中间件使我们能够针对机器人所接收的用户消息来执行特定任务。

我们将调用涉及 Azure 搜索和 Microsoft 语言理解智能服务 (LUIS) 的部分概念，但不会在本实验中实现这些概念。

## 实验 4：记录机器人聊天信息

在上一实验中，我们从 echo 机器人项目开始，并修改了代码，以适应我们的需求。  现在，我们希望记录与机器人的聊天信息，以使我们的客户服务团队能够跟进查询，确定机器人是否以预期方式执行操作，以及分析客户数据。

此动手实验向你介绍如何为机器人解决方案启用各种日志记录方案。

在高级分析领域，存储日志对话有多种用途。拥有聊天对话语料库，开发人员便可执行以下操作：

1. 生成特定于域的问题和答案引擎。
2. 确定机器人是否以预期方式响应。
3. 对特定主题或产品进行分析以确定趋势。

在下面的实验课程中，我们将介绍如何启用聊天日志记录和拦截消息。虽然数据解决方案不在本研讨会的范围内，但我们将介绍可以存储数据的各种方式。

## 实验 5：QnA Maker

在本实验中，你将使用 Microsoft QnA Maker 应用程序创建和发布知识库，然后在机器人中使用它。

## 实验 6：实现 LUIS 模型

我们将生成一个端到端的方案，它允许你引入自己的图片，使用认知服务查找图像中的对象和人物，以及获取说明和标签。我们稍后将使用 LUIS 生成一个 Bot Framework 机器人，以便轻松且有针对性地进行查询。

## 实验 7：使用对话将 LUIS 集成到机器人中

现在，我们的机器人能够获取用户输入并做出相应的响应，我们将使机器人能够使用在[实验 6](/Lab6-Implement_LUIS/02-Implement_LUIS.md) 中生成的 LUIS 模型来理解自然语言。

## 实验 8：检测用户语言

在本实验中，我们将为机器人添加从用户输入中检测语言的功能。

如果你已训练机器人或将机器人与 QnA Maker 集成，但在此过程中仅使用一种特定语言，则应告知用户这一点。  

## 实验 9：在 Direct Line 中测试机器人

此动手实验引导你完成机器人测试的一些基本要点。本研讨会演示如何（使用 Direct Line）执行功能测试。
