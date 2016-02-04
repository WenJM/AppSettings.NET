# AppSettings.NET

/// 提供对自定义应用程序配置文件的访问（支持深层节点）
/// 1、必须在配置文件Web.config中的节点AppSettings添加自定义配置的物理路径，key="AppSettingsPath"。
/// 2、自定义配置文件必须是以AppSettings节点开始。
/// 例如：
/// ***|<?xml version="1.0"?>
/// ***|<appSettings>
/// ***|     <add key="name" value="zhangsan" />
/// ***|     <Person Name="张三" Age="18" Height="185" />
/// ***|</appSettings>
/// 3、同时确保自定义配置文件设置了对应的读取权限。
/// 
/// 使用方法
/// 1、add键值类型的节点，直接通过Key获取值：SettingsManager.AppSettings["name"]
/// 2、其他类型节点，可通过节点名称和属性名称获取属性值：SettingsManager.GetAttributesValue("Person", "Name")
/// 3、和实体一致的节点，可直接传入实体类型获取对应的节点信息：SettingsManager.GetEntity<Person>()
/// 
/// 缓存补充说明：
/// 1、如果需要把配置信息加载在缓存中，在AppSettings节点添加属性Cache="true"。
/// 2、缓存已添加对文件的依赖，配置文件内容更新时会清空缓存，下次读取时自动初始化。
/// 3、可以在程序启动时初始化配置文件到缓存。
