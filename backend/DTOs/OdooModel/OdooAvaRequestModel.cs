using PortaCapena.OdooJsonRpcClient.Models;
using PortaCapena.OdooJsonRpcClient.Request;

namespace AiAgentApi.DTOs
{
    using PortaCapena.OdooJsonRpcClient.Models;
    using PortaCapena.OdooJsonRpcClient.Request;
    using System.Collections.Generic;

    public static class OdooAvaRequestModelHelper
    {
        /// <summary>
        /// Construye un OdooRequestModel para invocar un método de un modelo en Odoo (execute_kw)
        /// Ejemplo: action_post, button_cancel, custom_method, etc.
        /// </summary>
        /// <param name="config">Configuración de conexión (OdooConfig)</param>
        /// <param name="uid">User ID autenticado</param>
        /// <param name="tableName">Nombre del modelo en Odoo (por ejemplo: account.move)</param>
        /// <param name="methodName">Nombre del método a invocar (por ejemplo: action_post)</param>
        /// <param name="args">Argumentos del método (por ejemplo: new object[] { new object[] { id } })</param>
        /// <param name="context">Contexto opcional (puedes poner null)</param>
        /// <returns>OdooRequestModel preparado para ejecutar</returns>
        public static OdooRequestModel ExecuteMethod(OdooConfig config, int uid, string tableName, string methodName, object[] args = null, OdooContext context = null)
        {
            return new OdooRequestModel(new OdooRequestParams(
                config.ApiUrlJson,
                "object",
                "execute_kw",
                config.DbName,
                uid,
                config.Password,
                tableName,
                methodName,
                args ?? new object[] { },
                context != null ? context : null
            ));
        }
    } 


}
