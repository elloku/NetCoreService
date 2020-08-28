namespace Mediinfo.Enterprise
{
    /// <summary>
    /// 通用返回值定义
    /// </summary>
    public enum ReturnCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        SUCCESS = 0,

        /// <summary>
        /// 通用错误代码
        /// </summary>
        ERROR = -1,

        /// <summary>
        /// 未授权
        /// </summary>
        UNAUTHORIZED = -5,

        /// <summary>
        /// 底层错误代码
        /// </summary>
        INFRASTRUCTUREERROR = -10,

        /// <summary>
        /// 插件错误代码
        /// </summary>
        PLUGINSERROR = -20,

        /// <summary>
        /// DB层错误代码（业务错误)
        /// </summary>
        DBERROR = -100,

        /// <summary>
        /// 仓储错误代码
        /// </summary>
        REPOSITORYERROR = -101,

        /// <summary>
        /// Domain层错误代码（业务错误）
        /// </summary>
        DOMAINERROR = -200,

        /// <summary>
        /// DTO层错误代码（业务错误）
        /// </summary>
        DTOERROR = -300,

        /// <summary>
        /// 服务层错误代码（业务错误）
        /// </summary>
        SERVICEERROR = -400,

        /// <summary>
        /// 客户端异常代码（业务错误）
        /// </summary>
        CLIENTERROR = -500,

        /// <summary>
        /// 登陆用户连接错误（业务错误）
        /// </summary>
        LOGINDBCONNECTERROR = -600,

        /// <summary>
        /// 主数据库连接错误（指xt_connect表配置不正确，业务错误）
        /// </summary>
        MAINDBCONNECTERROR = -601,

        /// <summary>
        /// Domain Service错误（业务错误）
        /// </summary>
        DOMAINSERVICEERROR = -700,

        /// <summary>
        /// 平台错误代码
        /// </summary>
        CLOUDERROR = -800
    }
}
