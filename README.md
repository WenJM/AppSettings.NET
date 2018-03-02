### AppSettings.NET

提供对自定义应用程序配置文件的访问（支持深层节点）<br />

1、必须在配置文件Web.config中的节点AppSettings添加自定义配置的物理路径，key="AppSettingsPath"。<br />

2、自定义配置文件必须是以AppSettings节点开始。<br />
例如：
<?xml version="1.0"?><br />
\<appSettings\><br />
&emsp;\<add key="name" value="zhangsan" /><br />
&emsp;\<Person Name="张三" Age="18" Height="185" /><br />
&emsp;\<Orders><br />
&emsp;&emsp;\<Order><br />
&emsp;&emsp;&emsp;\<Amount>15</Amount><br />
&emsp;&emsp;&emsp;\<Name>理财卷</Name><br />
&emsp;&emsp;&emsp;\<Code>SFR324234</Code><br />
&emsp;&emsp;\</Order><br />
&emsp;&emsp;\<Order Amount="17.00" Name="鼠标" Code="SFR544333" /><br />
&emsp;\</Orders><br />
\</appSettings><br />

3、同时确保自定义配置文件设置了对应的读取权限。<br />

使用方法：<br />
1、add键值类型的节点，直接通过Key获取值：AppSettingClient.AppSettings["name"]<br />
2、其他类型节点，可通过节点名称和属性名称获取属性值：AppSettingClient.AttributesValue("Person", "Name")<br />
3、和实体一致的节点，可直接传入实体类型获取对应的节点信息：AppSettingClient\<Person>.Load()<br />

缓存补充说明：<br />
1、如果需要把配置信息加载在缓存中，可以设置AppSettingConfig.IsCacheConfig为true <br />
2、缓存已添加对文件的依赖，配置文件内容更新时会清空缓存，下次读取时自动初始化。<br />
3、可以在程序启动时初始化配置文件到缓存。<br />

2018-03-02：
调整缓存依赖策略，如果是http远程配置文件，默认会启动配置文件的扫描，有变更时，会使缓存失效，下次读取时重新加载。
