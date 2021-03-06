# Autumn.DB

> 一个功能强劲的C#数据库操作框架，几分钟即可快速上手。

##快速上手
1.在数据库新建member演示表：
```{go}
CREATE DATABASE IF NOT EXISTS `autumnDemo`
USE `autumnDemo`;

CREATE TABLE `member` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `password` varchar(50) NOT NULL,
  PRIMARY KEY (`id`)
)
```
2.在C#中建立控制台程序作为演示
```{go}
class Program
    {
        public class Member
        {
            public int Id { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        static void Main(string[] args)
        {
            //以mysql演示
            ConfigManager.LoaderConfiguration("mysql", "server=127.0.0.1;uid=root;database=autumnDemo");

            Member member = new Member();

            member.Username = "张三";
            member.Password = "123456";

            bool result = DALFactory<Member>.Insert(member);

            Console.WriteLine(result ? "成功,id:"+member.Id :"失败");
            Console.ReadKey();
        }
    }
```
使用手册：<a href="docs.md">docs.md</a>

## Autumn.DB 是什么

Autumn.DB 是一个功能强大，使用简单的 C# 框架。除了支持sql操作之外，它还提供了经典的面向对象操作方法。Autumn为操作数据库做了大量适配和优化，让你无需配置和繁琐的对应文件就能灵活的操作各种数据库。是Autumn系列框架的成员之一。

## 为什么选择 Autumn.DB

Autumn.DB 作为国内一个完全由底层开发起来的框架，经历过各种复杂项目的考验。Autumn.DB 设计时遵循二八原则，保持最常用 API 的精简子集，可以非常方便开始你的项目。清晰的面向对象功能以及轻松的架构性，更增强了 Autumn.DB 的易用性。仅需要几分钟学习即可快速上手。

## Autumn 历史

Autumn 诞生于2009年10月2日。当时作者正在学习java，为了脱离臃肿繁琐的ssh框架，自行开发了同时集成了mvc和数据库操作的基础类库。由于spring和hibernate和春冬有关，当时正值秋季，故采用Autumn为框架命名。由于框架功能的不断增强，后来将各部分功能分离出来。形成了Autumn.DB,Autumn.QuickSelector,Autumn.Cloud等众多子框架。而进行其它语言学习时，作者同样将易用灵活的Autumn系列框架制作成不同语言下同样好用的不同版本。

## 联系作者
 - Email：<sxningli@163.com>
 - QQ：908031341
