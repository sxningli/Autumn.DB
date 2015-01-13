---
文档: Autumn.DB 使用指南
---
目录
----
<a name="Directory-Bin"></a>
- [引用框架](#Bin)
<a name="Directory-LoaderConfiguration"></a>
- [全局配置](#LoaderConfiguration)
  <a name="Directory-LoaderConfiguration-BS"></a>
	- [B/S模式下的自动加载](#LoaderConfiguration-BS)
	<a name="Directory-LoaderConfiguration-XmlFileName"></a>
	- [从xml文件加载配置](#LoaderConfiguration-XmlFileName)
	<a name="Directory-LoaderConfiguration-ConnectionString"></a>
	- [直接写connectionString](#LoaderConfiguration-ConnectionString)
	<a name="Directory-LoaderConfiguration-XmlContent"></a>
	- [传入xml配置内容](#LoaderConfiguration-XmlContent)
<a name="Directory-IDBHelper"></a>
- [sql命令执行接口](#IDBHelper)
  <a name="Directory-IDBHelper-ExecuteNonQuery"></a>
	- [执行sql命令并返回受影响行数](#IDBHelper-ExecuteNonQuery)
	<a name="Directory-IDBHelper-ExecuteNumber"></a>
	- [执行sql命令并返回数字结果](#IDBHelper-ExecuteNumber)
	<a name="Directory-IDBHelper-ExecuteScalar"></a>
	- [执行sql命令并检索单个返回值](#IDBHelper-ExecuteScalar)
	<a name="Directory-IDBHelper-ExecuteTable"></a>
	- [执行sql命令并返回DataTable](#IDBHelper-ExecuteTable)
<a name="Directory-DBOO"></a>
- [面向对象的数据库操作](#DBOO)
  <a name="Directory-Insert"></a>
  - [插入数据](#Insert)
  <a name="Directory-Delete"></a>
  - [删除数据](#Delete)
  <a name="Directory-Update"></a>
  - [修改数据](#Update)
  <a name="Directory-Select"></a>
  - [查询数据](#Select)
    <a name="Directory-SelectAll"></a>
    - [所有数据](#SelectAll)
    <a name="Directory-SelectAll-MultiPage"></a>
    - [所有数据<带分页>](#SelectAll-MultiPage)
      <a name="Directory-SelectAll-MultiPage-General"></a>
      - [普通分页](#SelectAll-MultiPage-General)
      <a name="Directory-SelectAll-MultiPage-Condition"></a>
      - [条件筛选](#SelectAll-MultiPage-Condition)
      <a name="Directory-SelectAll-MultiPage-Condition-Fields"></a>
      - [条件筛选<仅获取部分字段数据>](#SelectAll-MultiPage-Condition-Fields)
    <a name="Directory-SelectAllCount"></a>
    - [总数据条数](#SelectAllCount)
    <a name="Directory-SelectByDynamic"></a>
    - [按条件筛选数据](#SelectByDynamic)
    <a name="Directory-SelectAllCount-Condition"></a>
    - [按条件筛选的数据条数](#SelectAllCount-Condition)
    <a name="Directory-SelectByField"></a>
    - [按字段值查询数据](#SelectByField)
    <a name="Directory-SelectById"></a>
    - [通过id查询](#SelectById)
    <a name="Directory-SelectByTSQL"></a>
    - [通过sql命令查询](#SelectByTSQL)
    <a name="Directory-SelectExistByDynamic"></a>
    - [判定是否包含按条件筛选的信息](#SelectExistByDynamic)
    <a name="Directory-SelectExistByTSQL"></a>
    - [判定是否包含按指定sql命令查询的信息](#SelectExistByTSQL)
    <a name="Directory-SelectOneByDynamic"></a>
    - [返回筛选后的唯一一条数据](#SelectOneByDynamic)
    <a name="Directory-SelectOneByTSQL"></a>
    - [返回通过sql命令查询的唯一一条数据](#SelectOneByTSQL)

----
<a name="Bin"></a>
##1. 引用框架【<a href="#Directory-Bin">返回目录</a>】
进入 bin 目录，将`Autumn.DB.dll`类库引入项目即可。另外可将`autumn-config.xml`配置信息文件复制到项目中，并修改其中相关配置信息。以便使用xml配置文件方式装载全局配置。

<a name="LoaderConfiguration"></a>
##2. 全局配置【<a href="#Directory-LoaderConfiguration">返回目录</a>】
要使用框架进行相关操作，需要先装载全局配置信息（手动创建IDBHelper类使用除外）。

<a name="LoaderConfiguration-BS"></a>
###2.1 B/S模式下的自动加载【<a href="#Directory-LoaderConfiguration-BS">返回目录</a>】
在B/S项目中，可将`autumn-config.xml`文件复制到`Bin`目录中，以便直接直接使用不带参数的`LoaderConfiguration`方法即可自动载入配置信息：
```{go}
Autumn.DB.Config.ConfigManager.LoaderConfiguration();
```

<a name="LoaderConfiguration-XmlFileName"></a>
###2.2 从xml文件加载配置【<a href="#Directory-LoaderConfiguration-XmlFileName">返回目录</a>】
传入 xml 配置文件的物理路径，即可从指定配置文件中加载相应的配置信息：
```{go}
Autumn.DB.Config.ConfigManager.LoaderConfiguration(“E:\yourpath\yourConfigFileName.xml”);
```

<a name="LoaderConfiguration-ConnectionString"></a>
###2.3 直接写connectionString【<a href="#Directory-LoaderConfiguration-ConnectionString">返回目录</a>】
若不想使用配置文件，可以直接在代码中指定数据库类型以及连接字符串：
```{go}
Autumn.DB.Config.ConfigManager.LoaderConfiguration(
  "mysql",
  "server=127.0.0.1;uid=root;pwd=123456;database=test"
);
```

<a name="LoaderConfiguration-XmlContent"></a>
###2.4 传入xml配置内容【<a href="#Directory-LoaderConfiguration-XmlContent">返回目录</a>】
在一些高级应用中，已经有一个配置文件。可将`autumn-config.xml`配置信息直接写入其它配置文件中，使用时读出配置信息并交由`LoaderConfigurationXml`函数来处理：
```{go}
Autumn.DB.Config.ConfigManager.LoaderConfigurationXml("<xml>...")
```
<a name="IDBHelper"></a>
##3. sql命令执行接口【<a href="#Directory-IDBHelper">返回目录</a>】
`IDBHelper`接口为底层sql命令执行接口。可使用`Autumn.DB.Factory.DBHelperFactory`工厂类来创建。一般情况下直接使用`CreateCommonDBHelper`函数创建：
```{go}
Autumn.DB.Data.IDBHelper helper = Autumn.DB.Factory.DBHelperFactory.CreateCommonDBHelper();
```
在某些特殊情况下，要使用默认配置以外的连接信息创建`IDBHelper`时，可使用`CreateDBHelper`函数：
```{go}
Autumn.DB.Data.IDBHelper helper = Autumn.DB.Factory.DBHelperFactory.CreateDBHelper(
  Autumn.DB.Data.DBType.MySQL,
  "server=127.0.0.1;uid=root;pwd=123456;database=test"
);
```

<a name="IDBHelper-ExecuteNonQuery"></a>
###3.1 执行sql命令并返回受影响行数【<a href="#Directory-IDBHelper-ExecuteNonQuery">返回目录</a>】
该函数一般用于执行 Insert,Delete,Update 等 sql 命令。并且根据返回受影响行数来进行其它判断或其它操作：
```{go}
Autumn.DB.Data.IDBHelper helper = Autumn.DB.Factory.DBHelperFactory.CreateCommonDBHelper();

int insertNonQuery = helper.ExecuteNonQuery("insert into member values('张三','abcdef')");
int deleteNonQuery = helper.ExecuteNonQuery("delete from member where id = 10");
```

<a name="IDBHelper-ExecuteNumber"></a>
###3.2 执行sql命令并返回数字结果【<a href="#Directory-IDBHelper-ExecuteNumber">返回目录</a>】
该函数是`ExecuteScalar`命令的一个扩展函数，用于将单一结果中的整数值直接返回对应的 int 类型结果：
```{go}
Autumn.DB.Data.IDBHelper helper = Autumn.DB.Factory.DBHelperFactory.CreateCommonDBHelper();

int count = helper.ExecuteNumber("select count(*) from member");
```

<a name="IDBHelper-ExecuteScalar"></a>
###3.3 执行sql命令并检索单个返回值【<a href="#Directory-IDBHelper-ExecuteScalar">返回目录</a>】
该函数多用于单一结果返回时使用，如聚合函数的返回：
```{go}
Autumn.DB.Data.IDBHelper helper = Autumn.DB.Factory.DBHelperFactory.CreateCommonDBHelper();

object count = helper.ExecuteNumber("select count(*) from member");
object maxid = helper.ExecuteScalar("select max(id) from member");
```

<a name="IDBHelper-ExecuteTable"></a>
###3.4 执行sql命令并返回DataTable【<a href="#Directory-IDBHelper-ExecuteTable">返回目录</a>】
该函数一般适用于 select 等查询返回结果集的使用中：
```{go}
Autumn.DB.Data.IDBHelper helper = Autumn.DB.Factory.DBHelperFactory.CreateCommonDBHelper();

System.Data.DataTable table = helper.ExecuteTable("select * from `user`");

Console.Write(table.Rows.Count);
```

<a name="DBOO"></a>
##4. 面向对象的数据库操作【<a href="#Directory-DBOO">返回目录</a>】
在实际项目中，建议采用面向对象的数据库管理方式而非`sql命令执行接口`中提供的sql语句执行方案。

1.在设计数据库表时，建议每个表中增加名为`id`的数字类型列，并设置自动增长。原因是`Autumn.DB`设计时，考虑到二八原则，默认的删除和修改方法建议基于id进行。这样的好处就是无需任何配置即可快速操作实体对象。
例：
```{go}
CREATE TABLE `member` (
  `id` int(10) NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `password` varchar(50) NOT NULL,
  PRIMARY KEY (`id`)
)
```

2.在项目开发中，要建立与数据库表象一至的类，并且有与数据库中字段对应的属性。当实体类中包含了数据库表字段之外的其它辅助设计属性时，需要添加`Autumn.DB.Data.CustomType`属性标注。
例：
```{go}
namespace Model
{
  public class Member
  {
    public int Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    [Autumn.DB.Data.CustomType]
    public string ExtInfo { get; set; }
  }
}
```

<a name="Insert"></a>
###4.1 插入数据【<a href="#Directory-Insert">返回目录</a>】
将一个模型对象插入到数据库中：
```{go}
Model.Member member = new Model.Member();

member.Username = "张三";
member.Password = "123456";

DALFactory<Model.Member>.Insert(member);
```

<a name="Delete"></a>
###4.2 删除数据【<a href="#Directory-Delete">返回目录</a>】
可通过多种不同方式删除数据：
```{go}
DALFactory<Model.Member>.Delete(1); //删除id为1的member表记录

DALFactory<Model.Member>.DeleteByDynamic("username='张三' and password='123456'");  //删除指定条件的member表记录

DALFactory<Model.Member>.DeleteByField("username", "张三"); //删除username字段值为“张三”的member表记录

DALFactory<Model.Member>.DeleteByIds(1, 2, 3);  //删除id为1,2,3的所有member表记录

Model.Member member = new Model.Member();

member.Username = "张三";
member.Password = "123456";

DALFactory<Model.Member>.DeleteByTemplate(member);  //通过传入模板删除指定信息（可指定删除逻辑）
```

<a name="Update"></a>
###4.3 修改数据【<a href="#Directory-Update">返回目录</a>】
可通过id或其它字段值为条件来修改指定数据：
```{go}
Model.Member member = new Model.Member();

member.Id = 1;
member.Password = "654321";

DALFactory<Model.Member>.Update(member);    //默认以id为条件，即将id为1的member记录的password修改为"654321"

member = new Model.Member();
member.Username = "王五";
member.Password = "1";

DALFactory<Model.Member>.UpdateByField(member, "username", "李四");   //将username为"李四"的username修改为"王五"，并将password修改为"1"
```

<a name="Select"></a>
###4.4 查询数据【<a href="#Directory-Select">返回目录</a>】

<a name="SelectAll"></a>
####4.4.1 所有数据【<a href="#Directory-SelectAll">返回目录</a>】
得到某类数据的所有实例的集合：
```{go}
IList<Model.Member> members = DALFactory<Model.Member>.SelectAll();
```

<a name="SelectAll-MultiPage"></a>
####4.4.2 所有数据<带分页>【<a href="#Directory-SelectAll-MultiPage">返回目录</a>】
在某些情况下，数据量可能过大，需要采用分页。下列函数满足了不同情况下的各种分页需求：

<a name="SelectAll-MultiPage-General"></a>
#####4.4.2.1 普通分页【<a href="#Directory-SelectAll-MultiPage-General">返回目录</a>】
```{go}
IList<Model.Member> members = DALFactory<Model.Member>.SelectAll(10,1); //每页10条，查询第1页
```

<a name="SelectAll-MultiPage-Condition"></a>
#####4.4.2.2 条件筛选【<a href="#Directory-SelectAll-MultiPage-Condition">返回目录</a>】
```{go}
//筛选password为"123456"的结果，并以每页10条分页，显示第1页
IList<Model.Member> members = DALFactory<Model.Member>.SelectAll(10,1,"password='123456'");
```

<a name="SelectAll-MultiPage-Condition-Fields"></a>
#####4.4.2.3 条件筛选<仅获取部分字段数据>【<a href="#Directory-SelectAll-MultiPage-Condition-Fields">返回目录</a>】
```{go}
//筛选password为"123456"的结果，并以每页10条分页，显示第1页
IList<Model.Member> members = DALFactory<Model.Member>.SelectAll(10,1,"password='123456'");
```

<a name="SelectAllCount"></a>
####4.4.3 总数据条数【<a href="#Directory-SelectAllCount">返回目录</a>】
```{go}
int count = DALFactory<Model.Member>.SelectAllCount();
```

<a name="SelectByDynamic"></a>
####4.4.4 按条件筛选数据【<a href="#Directory-SelectByDynamic">返回目录</a>】
```{go}
IList<Model.Member> members = DALFactory<Model.Member>.SelectByDynamic("password='123456'");
```

<a name="SelectAllCount-Condition"></a>
####4.4.5 按条件筛选的数据条数【<a href="#Directory-SelectAllCount-Condition">返回目录</a>】
```{go}
int count = DALFactory<Model.Member>.SelectAllCount("password='123456'");
```

<a name="SelectByField"></a>
####4.4.6 按字段值查询数据【<a href="#Directory-SelectByField">返回目录</a>】
```{go}
IList<Model.Member> members = DALFactory<Model.Member>.SelectByField("password", "123456");
```

<a name="SelectById"></a>
####4.4.7 通过id查询【<a href="#Directory-SelectById">返回目录</a>】
```{go}
Model.Member member = DALFactory<Model.Member>.SelectById(1);
```

<a name="SelectByTSQL"></a>
####4.4.8 通过sql命令查询【<a href="#Directory-SelectByTSQL">返回目录</a>】
```{go}
IList<Model.Member> members = DALFactory<Model.Member>.SelectByTSQL("select * from member");
```

<a name="SelectExistByDynamic"></a>
####4.4.9 判定是否包含按条件筛选的信息【<a href="#Directory-SelectExistByDynamic">返回目录</a>】
```{go}
bool existZhangSan = DALFactory<Model.Member>.SelectExistByDynamic("username='张三'");
```

<a name="SelectExistByTSQL"></a>
####4.4.10 判定是否包含按指定sql命令查询的信息【<a href="#Directory-SelectExistByTSQL">返回目录</a>】
```{go}
bool existZhangSan = DALFactory<Model.Member>.SelectExistByTSQL("select * from member where username='张三'");
```
<a name="SelectOneByDynamic"></a>
####4.4.11 返回筛选后的唯一一条数据【<a href="#Directory-SelectOneByDynamic">返回目录</a>】
```{go}
Model.Member member = DALFactory<Model.Member>.SelectOneByDynamic("username='张三'");
```

<a name="SelectOneByTSQL"></a>
####4.4.12 返回通过sql命令查询的唯一一条数据【<a href="#Directory-SelectOneByTSQL">返回目录</a>】
```{go}
Model.Member member = DALFactory<Model.Member>.SelectOneByTSQL("select * from member where username='张三'");
```
